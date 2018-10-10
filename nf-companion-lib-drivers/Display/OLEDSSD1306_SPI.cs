/**
Copyright 2018 Vishnu Sharma , (Twitter : @getvishnu)

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION 
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System;
using System.Threading;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;
using Windows.Devices.Spi;

namespace nanoFramework.Companion.Drivers.Display
{
    /// <summary>
    /// OLED display driver that uses SSD1306 controller. Currently, this is designed to support 128x32 and 128x64
    /// display driver(s). Tested on 128x64 only. 
    /// This driver uses SPI interface.
    /// </summary>
    public class OLEDSSD1306_SPI : AbstractActuator
    {
        #region Implementation
        /// <summary>
        /// Display width
        /// </summary>
        private readonly int _width;
        /// <summary>
        /// Display height
        /// </summary>
        private readonly int _height;
        /// <summary>
        /// Display memory pages
        /// </summary>
        private readonly int _pages;
        /// <summary>
        /// Display buffer
        /// </summary>
        private byte[] _buffer;
        
        /// <summary>
        /// Holds an instance of the I2C device
        /// </summary>
        private SpiDevice _spiDevice;
        /// <summary>
        /// Pin to reset the OLED
        /// </summary>
        private GpioPin _rstPin;
        /// <summary>
        /// Pin to control data/command signals
        /// </summary>
        private GpioPin _dcPin;
        /// <summary>
        /// The font to use (current font)
        /// </summary>
        private IPixelFont _font = null;
        #endregion

        #region Properties
        /// <summary>
        /// Accessor for OLED display width
        /// </summary>
        public int Width { get {return _width;}}
        /// <summary>
        /// Accessor for OLED display height
        /// </summary>
        public int Height { get {return _height;}}
        /// <summary>
        /// Accessor for font width
        /// </summary>
        public int FontWidth { get { return _font.Width; } }
        /// <summary>
        /// Accessor for font height
        /// </summary>
        public int FontHeight { get { return _font.Height; } }
        #endregion

        #region Constructor/Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="busSelector">The SPI bus selector</param>
        /// <param name="dcPin">Gpio pin to use for data/command indication</param>
        /// <param name="rstPin">Gpio pin to use for reset control</param>
        /// <param name="csPin">Gpio pin to use for chip select signal</param>
        /// <param name="width">Width of OLED display (in pixel)</param>
        /// <param name="height">Height of OLED display (in pixel)</param>
        public OLEDSSD1306_SPI(string busSelector,GpioPin dcPin, GpioPin rstPin, GpioPin csPin,  int width = 128, int height = 32)
        {
            csPin.SetDriveMode(GpioPinDriveMode.Output);

            _rstPin = rstPin;
            _rstPin.SetDriveMode(GpioPinDriveMode.Output);
            _rstPin.Write(GpioPinValue.High);

            _dcPin = dcPin;
            _dcPin.SetDriveMode(GpioPinDriveMode.Output);

            this._spiDevice = SpiDevice.FromId(busSelector, 
                new SpiConnectionSettings(csPin.PinNumber) {
                    BitOrder = DataBitOrder.MSB,
                    ChipSelectLine = csPin.PinNumber,
                    ClockFrequency = 8000000,
                    DataBitLength = 8,
                    Mode = SpiMode.Mode0,
                    SharingMode = SpiSharingMode.Shared
                });
            
            this._width = width;
            this._height = height;
            this._pages = _height / 8;
            _buffer = new byte[width * _pages];
            _font = new PixelFont7X9();//default font            
        }

        /// <summary>
        /// Dispose the object
        /// </summary>
        protected override void DisposeActuator()
        {
            if (_buffer != null) _buffer = null;
            if (_dcPin != null) _dcPin.Dispose();
            if (_rstPin != null) _rstPin.Dispose();
            if (_spiDevice != null) _spiDevice.Dispose();
        }
        #endregion        

        #region SPI interface methods
        /// <summary>
        /// Send a single byte to the display controller
        /// </summary>
        /// <param name="data">Data to send</param>
        private void Send(byte data)
        {
            _spiDevice.Write(new byte[] { data });
        }
        /// <summary>
        /// Send an array of bytes to the display controller
        /// </summary>
        /// <param name="data">Data array to send</param>
        private void Send(byte[] data)
        {
            _spiDevice.Write(data);
        }
        /// <summary>
        /// Send a command to the display controller
        /// </summary>
        /// <param name="cmd">Command to send</param>
        private void SendCommand(byte cmd)
        {
            if (_dcPin.Read() != GpioPinValue.Low)
            {
                _dcPin.Write(GpioPinValue.Low);
                Thread.Sleep(10);
            }
            Send(new byte[] { 0x00, cmd });            
        }
        /// <summary>
        /// Send data to the display controller
        /// </summary>
        /// <param name="data">The single-byte data to send</param>
        private void SendData(byte data)
        {
            if (_dcPin.Read() != GpioPinValue.High)
            {
                _dcPin.Write(GpioPinValue.High);
                Thread.Sleep(10);
            } 
            Send(new byte[] { 0x40, data });
        }
        /// <summary>
        /// Send data as byte array to the display controller
        /// </summary>
        /// <param name="data">Data array to send</param>
        /// <param name="start">Index of starting element to send from</param>
        /// <param name="len">Number of elements to send</param>
        private void SendData(byte[] data, int start, int len)
        {
            if(_dcPin.Read() != GpioPinValue.High)
            {
                _dcPin.Write(GpioPinValue.High);
                Thread.Sleep(10);
            }
            byte[] buf = new byte[len + 1];
            Array.Copy(data, start, buf, 1, len);
            buf[0] = 0x40;
            Send(buf);
        }

        #endregion

        #region OLED Control Methods
        /// <summary>
        /// Initialize the display
        /// </summary>
        public override void Initialize()
        {
            //NOTE: We do not need constants for these commands...they are not to be reused by either other classes or by SDK user

            //Ensure display is switched off
            SendCommand(0xAE);
            //Set mux ratio...this tells the controller to use how many row numbers in the display
            SendCommand(0xA8); SendCommand((byte)(_height - 1));
            //Set display offset...we want to start from 0 (the shift by COM)
            SendCommand(0xD3); SendCommand(0x00);
            //Set display line address in display RAM ...we want to start from 0
            SendCommand(0x40);
            //Set the segment re-map...(Segment 0 is mapped to column 0)
            SendCommand(0xA0 | 0x01);//::TODO...Change this as per your hardware design
            //Set scan direction...we want it to scan from left to right (COM0 - COM[N-1])
            SendCommand(0xC8);//::TODO...Change this as per your hardware design
            //Set COM pins hardware configuration...this is hardware dependent...see page 40 in ssd1306 datasheet
            SendCommand(0xDA); SendCommand(0x02);
            //Set the contrast...just in the middle
            SendCommand(0x81); SendCommand(0x7F);
            //Set to resume from RAM content display
            SendCommand(0xA4);
            //Set normal display
            SendCommand(0xA6);
            //Set oscillator frequency
            SendCommand(0xD5); SendCommand(0x80);
            //Enable charge pump regulator
            SendCommand(0x8D); SendCommand(0x14);//TODO::...Change this as per your hardware design
            //Set memory addressing mode to page address mode, horizontal addressing
            SendCommand(0x20); SendCommand(0x00);                        
            //Finally set the display ON
            SendCommand(0xAF);
        }
        /// <summary>
        /// Prepare display to write data..basically resets the internal buffer
        /// </summary>
        public override void PrepareToWrite()
        {
            if (_buffer != null && _buffer.Length > 0)
                Array.Clear(_buffer, 0, _buffer.Length);
        }
        /// <summary>
        /// Write buffer to display
        /// </summary>
        public override void Write()
        {
            SendCommand(0x21);
            SendCommand(0);
            SendCommand((byte)(_width - 1));

            //set page address
            SendCommand(0x22);
            SendCommand(0);
            SendCommand((byte)(_pages - 1));

            for (ushort i = 0; i < _buffer.Length; i = (ushort)(i + 16))
            {
                SendCommand(0x40);
                //SendArray(buffer, i, (ushort)(i + 16));
                SendData(_buffer, i, 16);
            }
        }
        /// <summary>
        /// Reset the display...it will also initialize the display
        /// </summary>
        public override void Reset()
        {
            _rstPin.Write(GpioPinValue.Low);
            Thread.Sleep(50);
            _rstPin.Write(GpioPinValue.High);
            Thread.Sleep(10);
            /**
             * Now re-initializing the display--soft reset
             */            
            Initialize();
            SetFont(new PixelFont7X9());
            SetContrast();
            SetInverseDisplay(false);
            StopScrolling();
            PrepareToWrite();
            Write();
        }
        /// <summary>
        /// Set the font object to use to render text elements
        /// </summary>
        /// <param name="font">ILCDFont instance</param>
        public void SetFont(IPixelFont font)
        {
            _font = font ?? throw new ArgumentNullException("font");
        }
        /// <summary>
        /// Set contrast
        /// </summary>
        /// <param name="value">0x00 to 0xFF</param>
        public void SetContrast(byte value = 0xFF)
        {
            //sendCommand(new byte[]{0x81,value});
            SendCommand(0x81);
            SendCommand(value);
        }
        /// <summary>
        /// Inverse the display (painted pixels become off and vice versa)
        /// </summary>
        /// <param name="inverse">If true,display is inversed</param>
        public void SetInverseDisplay(bool inverse)
        {
            if (inverse) SendCommand(0xA7);
            else SendCommand(0xA6);
        }
        /// <summary>
        /// Switch on the whole display
        /// </summary>
        /// <param name="setOn">if true, display switches on</param>
        public void SetEntireDisplayON(bool setOn)
        {
            if (setOn) SendCommand(0xA5);
            else SendCommand(0xA4);
        }
        
        /// <summary>
        /// Start horizontall scrolling
        /// </summary>
        /// <param name="left">true = scrolling to left, false = scroll to right</param>
        public void StartHorizontalScroll(bool left)
        {
            StopScrolling();

            if (left) SendCommand(0x27);
            else SendCommand(0x26);

            SendCommand(0x00);
            SendCommand(0); //start page index
            SendCommand(0x00);  //scroll interval in frames
            SendCommand((byte)(this._pages - 1));  //end page index
            SendCommand(0x00);
            SendCommand(0xFF);

            SendCommand(0x2F);  //start scroll
        }
        
        /// <summary>
        /// Turn off scrolling
        /// </summary>
        public void StopScrolling()
        {
            SendCommand(0x2E);
        }

        #endregion

        #region OLED Drawing/Painting methods
        /// <summary>
        /// Clear the OLED screen
        /// </summary>
        public void ClearScreen()
        {
            PrepareToWrite();
            Write();
        }
        /// <summary>
        /// Set the pixel at a given position
        /// </summary>
        /// <param name="xPos">The x-position</param>
        /// <param name="yPos">The y-position</param>
        /// <param name="on">If true, the pixel is turned on, else its turned off</param>
        public void SetPixel(byte xPos, byte yPos, bool on = true)
        {
            if (xPos < 0 || xPos > _width) return;
            if (yPos < 0 || yPos > _height) return;

            int index = (yPos / 8) * _width + xPos;

            if (on)
                _buffer[index] = (byte)(_buffer[index] | (byte)(1 << (yPos % 8)));
            else
                _buffer[index] = (byte)(_buffer[index] & ~(byte)(1 << (yPos % 8)));
        }

        /// <summary>
        /// Draw a line
        /// </summary>
        /// <param name="startX">Starting x-pos</param>        
        /// <param name="startY">Starting y-pos</param>
        /// <param name="endX">Ending x-pos</param>
        /// <param name="endY">Ending y-pos</param>
        /// <param name="on">If true, the line pixels are turned on, else its turned off</param>
        public void DrawLine(byte startX, byte startY, byte endX, byte endY, bool on = true)
        {
            if (startX < 0 || startX > _width) throw new ArgumentException("Invalid start x-pos");
            if (endX < 0 || endX > _width || endX < startX) throw new ArgumentException("Invalid end x-pos");
            if (startY < 0 || startY > _height) throw new ArgumentException("Invalid start y-pos");
            if (endY < 0 || endY > _height || endY < startY) throw new ArgumentException("Invalid end y-pos");

            if (endX == _width) endX -= 1;
            if (startX == _width) startX -= 1;
            if (endY == _height) endY -= 1;
            if (startY == _height) startY -= 1;

            for (byte xpos = startX; xpos <= endX; xpos++)
            {
                for (byte ypos = startY; ypos <= endY; ypos++)
                    SetPixel(xpos, ypos, on);
            }
        }
        /// <summary>
        /// Draw a rectangle
        /// </summary>
        /// <param name="x">The starting x-pos</param>
        /// <param name="y">The starting y-pos</param>
        /// <param name="width">The rectangle width</param>
        /// <param name="height">The rectangle height</param>
        /// <param name="on">If true, the line pixels are turned on, else its turned off</param>
        public void DrawRectangle(byte x, byte y, byte width, byte height, bool on = true)
        {
            if (x < 0 || width <= 0 || width > _width) throw new ArgumentOutOfRangeException("x,width");
            if (y < 0 || height <= 0 || height > _height) throw new ArgumentOutOfRangeException("y,height");

            if (x == width) x -= 1;
            if (y == height) y -= 1;

            //Draw top horizontal
            DrawLine(x, y, (byte)(x + width - 1), y, on);
            //draw right vertical
            DrawLine((byte)(x + width - 1), y, (byte)(x + width - 1), (byte)(y + height - 1), on);
            //draw bottom horizontal
            DrawLine(x, (byte)(height - 1), (byte)(x + width - 1), (byte)(height - 1), on);
            //draw left vertical
            DrawLine(x, y, x, (byte)(y + height - 1));
        }
        /// <summary>
        /// Draw the given character
        /// </summary>
        /// <param name="x">The character x position</param>
        /// <param name="y">The character y position</param>
        /// <param name="c">The character to print</param>
        public void DrawChar(byte x, byte y, char c)
        {
            if (((int)c - 32) >= _font.CharMap.Length) c = '?';

            UInt32 cmp = 0;
            UInt32 pixMapHorizontal = 0;

            for (int yPos = 0; yPos < _font.Height; yPos++)
            {
                pixMapHorizontal = _font.CharMap[(c - 32) * _font.Height + yPos];
                //left to right.                
                cmp = (UInt32)0x80000000;
                for (int xPos = 0; xPos < _font.Width; xPos++)
                {
                    if ((pixMapHorizontal & cmp) == cmp)
                        SetPixel((byte)(xPos + x), (byte)(yPos + y), true);
                    cmp = (UInt32)(cmp >> 1);
                }
            }
        }
        /// <summary>
        /// Draw the given text
        /// </summary>
        /// <param name="x">Starting x position</param>
        /// <param name="y">Starting y position</param>
        /// <param name="text">Text to draw</param>
        public void DrawText(byte x, byte y, string text)
        {
            char[] chars = text.ToCharArray();
            foreach (char c in chars)
            {
                if (c == '\r') continue;
                if (c == '\n') { y += (byte)(_font.Height + 2); continue; }//TODO...next line

                if ((x + _font.Width) > _width)//wrap
                {
                    x = 0;
                    y += (byte)(_font.Height + 2);
                }
                if ((y + _font.Height) > _height) break;//nothing more to draw

                DrawChar(x, y, c);
                x += (byte)(_font.Width + 2);
            }
        }
        #endregion
    }
}
