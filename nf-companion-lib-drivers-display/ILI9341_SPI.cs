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
using Windows.Devices.Spi;

namespace nanoFramework.Companion.Drivers.Display
{
    /// <summary>
    /// Defines a set of commands required by this driver to work with the
    /// controller
    /// </summary>
    internal enum ILI9341Commands : byte
    {
        Nop = 0x00, //no operation
        SoftwareReset = 0x01,
        SleepMode=0x10,
        SleepOut=0x11,
        NormalDisplayMode=0x13,
        DisplayInversionOff=0x20,
        DisplayInversionOn=0x21,
        DisplayOff=0x28,
        DisplayOn=0x29,
        ColumnAddressSet=0x2A,
        PageAddressSet=0x2B,
        MemoryWrite=0x2C,
        ColorSet=0x2D,
        PixelFormatSet=0x3A,
        MemoryAccessControl=0x36,
        WriteDisplayBrightness=0x51,
        ReadDisplayBrightness=0x52,
        PowerControlA=0xCB,
        PowerControlB=0xCF,
        TimingControlA=0xCB,
        TimingControlB=0xCF,
        PowerOnSequenceControl=0xED,
        PumpRatioControl=0xF7,
        PowerControl1=0xC0,
        PowerControl2=0xC1,
        VComControl1=0xC5,
        VComControl2=0xC7,
        FrameRateControl=0xB1,
        DisplayFunctionControl=0xB6,
        Enable3G=0xF2,
        EnableGammaSet=0x26,
        PositiveGammaCorrection=0xE0,
        NegativeGammaCorrection=0xE1
    }
    /// <summary>
    /// Supported rotations in ILI9341
    /// </summary>
    public enum ILI9341Rotations
    {
        ZeroDegrees=0,
        NinetyDegrees=1,
        OneEightyDegrees=2,
        TwoSeventyDegrees=3
    }
    /// <summary>
    /// The IL9341 display controller driver
    /// See https://cdn-shop.adafruit.com/datasheets/ILI9341.pdf for a datasheet of this controller
    /// </summary>
    public class ILI9341_SPI : AbstractActuator
    {
        #region Implementation
        /// <summary>
        /// The underlying SPI device
        /// </summary>
        private SpiDevice _spiDevice = null;
        /// <summary>
        /// Width of display in pixels
        /// </summary>
        private uint _coreWidth = 320;
        /// <summary>
        /// Height of display in pixels
        /// </summary>
        private uint _coreHeight = 240;
        /// <summary>
        /// Gpio pin to control D/C line (data or command)
        /// </summary>
        private GpioPin _dcPin = null;
        /// <summary>
        /// GpioPin to control hardware reset of the display
        /// </summary>
        private GpioPin _rstPin = null;
        /// <summary>
        /// The Gpio pin to use for chip select
        /// </summary>
        private GpioPin _csPin = null;
        /// <summary>
        /// The internal display buffer.
        /// </summary>
        private byte[] _buffer = null;
        /// <summary>
        /// Holds the current font in use
        /// </summary>
        private IPixelFont _font = null;
        
        #endregion

        #region Attributes
        /// <summary>
        /// Accessor for width of the display
        /// </summary>
        public uint Width { get; set; }
        /// <summary>
        /// Accessor for height of the display
        /// </summary>
        public uint Height { get; set; }
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
        /// Construct the driver using the specific SPI port.
        /// </summary>
        /// <param name="busSelector">Which SPI bus to use</param>
        /// <param name="width">Width of display in pixels</param>
        /// <param name="height">Height of display in pixels</param>
        /// <param name="dcPin">The Gpio pin to use for D/C (Data or Command) signal</param>
        /// <param name="rstPin">The Gpio pin to use for hardware reset of the display</param>
        /// <param name="csPin">The Cpio pin to use for Chip Select signal for SPI communication</param>
        /// <param name="bufSize">The size of the internal display buffer</param>
        public ILI9341_SPI(string busSelector, uint width, uint height, GpioPin dcPin, GpioPin rstPin, GpioPin csPin, uint bufSize=256)
        {
            if (dcPin == null || rstPin == null || csPin == null) throw new ArgumentNullException();
            _dcPin = dcPin;
            _rstPin = rstPin;
            _csPin = csPin;

            _dcPin.SetDriveMode(GpioPinDriveMode.Output);
            _rstPin.SetDriveMode(GpioPinDriveMode.Output);
            _csPin.SetDriveMode(GpioPinDriveMode.Output);

            SpiBusInfo busInfo = SpiDevice.GetBusInfo(busSelector);

            var spiConnSettings = new SpiConnectionSettings(csPin.PinNumber)
            {
                ClockFrequency = 10000000, //10 MHz SPI clock,max period for read is 150ns, write is 100ns
                DataBitLength = 8, //8-bit data length
                Mode = SpiMode.Mode3 //mode 3, data read on the rising edge - idle high
            };

            _coreWidth = Width = width;
            _coreHeight= Height = height;
            _buffer = new byte[bufSize];

            _font = new PixelFont7X9();//default font

            _spiDevice = SpiDevice.FromId(busSelector, spiConnSettings);                       
        }
        /// <summary>
        /// Dispose the driver
        /// </summary>
        protected override void DisposeActuator()
        {
            if (_csPin != null) _csPin.Dispose();
            if (_dcPin != null) _dcPin.Dispose();
            if (_rstPin != null) _rstPin.Dispose();
            if (_spiDevice != null) _spiDevice.Dispose();
            _buffer = null;
        }
        #endregion

        #region Core IActuator Implementation
        /// <summary>
        /// Initialize the driver
        /// </summary>
        public override void Initialize()
        {
            HardReset();
            Thread.Sleep(10);
            Reset();
            Thread.Sleep(10);
            Wakeup();
        }
        /// <summary>
        /// Prepare the driver subsystem to begin writing data/command to the display
        /// </summary>
        public override void PrepareToWrite()
        {
            if(_buffer != null)
                Array.Clear(_buffer, 0, _buffer.Length);
        }
        /// <summary>
        /// Perform a software reset of the display. For a hard reset, call HardReset method
        /// </summary>
        public override void Reset()
        {
            SendCommand((byte)ILI9341Commands.SoftwareReset);
        }
                
        /// <summary>
        /// All data is now written to the internal buffer.Write to the display. 
        /// </summary>
        public override void Write()
        {
            SendData(_buffer);
        }
        #endregion

        #region Drawing Operations
        /// <summary>
        /// A simple method to test the display
        /// </summary>
        public void TestDisplay()
        {
            Sleep();
            Wakeup();
            SetRotation(ILI9341Rotations.NinetyDegrees);
            uint w = (uint)(Width / 10);
            uint x = 0;
            ushort color = RGB888ToRGB565(255, 255, 255);
            for(int l=0; l < 10; l++)
            {                
                FillRect(x, 0, w-1, Height, (ushort)(color * 10 * l));
                color -= (ushort)(color / w);
                x += w;
            }
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
        /// Set a pixel (1x1) with the specific color
        /// </summary>
        /// <param name="x0">The x-pos</param>
        /// <param name="y0">The y-pos</param>
        /// <param name="color">The pixel color</param>
        public void SetPixel(uint x0, uint y0, ushort color)
        {
            SetCanvas(x0, y0, x0 + 1, y0 + 1);

            _buffer[0] = (byte)(color >> 8);
            _buffer[1] = (byte)color;

            SendData(_buffer, 2);
            SendCommand((byte)ILI9341Commands.Nop);
        }
        /// <summary>
        /// Draw a horizontal line (1 pixel high)
        /// </summary>
        /// <param name="x0">Line starting x-post</param>
        /// <param name="y0">Line starting y-pos</param>
        /// <param name="lineWidth">The line Width</param>
        /// <param name="color">The color of the line</param>
        public void DrawHLine(uint x0, uint y0, uint lineWidth, ushort color)
        {
            if (x0 > Width || y0 > Height) throw new ArgumentOutOfRangeException();
            if (lineWidth == 0 || lineWidth > Width) throw new ArgumentOutOfRangeException();

            SetCanvas(x0, y0, lineWidth, 1);
            PrepareBufferAndSendData((int)lineWidth * 2, color);
            SendCommand((byte)ILI9341Commands.Nop);
        }
        /// <summary>
        /// Draw a vertical line (1 pixel thick)
        /// </summary>
        /// <param name="x0">Line starting x-post</param>
        /// <param name="y0">Line starting y-pos</param>
        /// <param name="lineHeight">The line Height</param>
        /// <param name="color">The color of the line</param>
        public void DrawVLine(uint x0, uint y0, uint lineHeight, ushort color)
        {
            if (x0 > Width || y0 > Height) throw new ArgumentOutOfRangeException();
            if (lineHeight == 0 || lineHeight > Height) throw new ArgumentOutOfRangeException();

            SetCanvas(x0, y0, 1, lineHeight);
            PrepareBufferAndSendData((int)lineHeight * 2, color);
            SendCommand((byte)ILI9341Commands.Nop);
        }
        /// <summary>
        /// Draw a rectangle
        /// </summary>
        /// <param name="x0">Top-left x-pos</param>
        /// <param name="y0">Top-left y-pos</param>
        /// <param name="w">Rectangle width</param>
        /// <param name="h">Rectangle height</param>
        /// <param name="color">Line color</param>
        public void DrawRect(uint x0, uint y0, uint w, uint h, ushort color)
        {
            DrawHLine(x0, y0, w, color);//L-R ----
            DrawVLine(x0 + w - 1, y0, h, color);//T-B |
            DrawHLine(x0, y0 + h - 1, w, color);//R-L ---
            DrawVLine(x0, y0, h, color);//B-T |
            SendCommand((byte)ILI9341Commands.Nop);
        }
        /// <summary>
        /// Draw a rectangle and fill it with the given color
        /// </summary>
        /// <param name="x0">Top-left x-pos</param>
        /// <param name="y0">Top-left y-pos</param>
        /// <param name="w">Rectangle width</param>
        /// <param name="h">Rectangle height</param>
        /// <param name="color">Fill color</param>
        public void FillRect(uint x0, uint y0, uint w, uint h, ushort color)
        {
            SetCanvas(x0, y0, w, h);

            byte vh = (byte)(color >> 8);
            byte vl = (byte)color;
            uint totalPixels = w * h * 2;

            PrepareBufferAndSendData((int)totalPixels, color);
            SendCommand((byte)ILI9341Commands.Nop);
        }
        /// <summary>
        /// Draw the given character
        /// </summary>
        /// <param name="x">The character x position</param>
        /// <param name="y">The character y position</param>
        /// <param name="c">The character to print</param>
        /// <param name="color">The color of the character</param>
        public void DrawChar(uint x, uint y, char c, ushort color)
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
                        SetPixel((byte)(xPos + x), (byte)(yPos + y),color);
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
        /// <param name="color">The color of the text</param>
        public void DrawText(uint x, uint y, string text, ushort color)
        {
            char[] chars = text.ToCharArray();
            uint originX = x;
            foreach (char c in chars)
            {
                if (c == '\r') continue;
                if (c == '\n') { y += (uint)(_font.Height + 1); continue; }//TODO...next line

                if ((x + _font.Width) > Width)//wrap
                {
                    x = originX;
                    y += (uint)(_font.Height + 1);
                }
                if ((y + _font.Height) > Height) break;//nothing more to draw

                DrawChar(x, y, c,color);
                x += (uint)(_font.Width + 1);
            }
        }
        /// <summary>
        /// Sets entire screen to black
        /// </summary>
        public void ClearScreen()
        {            
            FillRect(0, 0, Width, Height, RGB888ToRGB565(0, 0, 0));
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Perform a hard reset
        /// </summary>
        public void HardReset()
        {
            _rstPin.Write(GpioPinValue.Low);
            Thread.Sleep(5);
            _rstPin.Write(GpioPinValue.High);
            Thread.Sleep(20);
        }
        /// <summary>
        /// Bring display out from sleep mode
        /// </summary>
        public void Wakeup()
        {
            SendCommand((byte)ILI9341Commands.SleepOut);
            Thread.Sleep(60);//some displays take long time

            /*
             * Now we send a series of initialization commands to the display.
             * Unfortunately, some older datasheets do not list all commands, however,
             * found an application note, that lists down the initialization sequence.
             * See it here http://aitendo3.sakura.ne.jp/aitendo_data/product_img/lcd/tft2015/M032/ILI9341_AN_V0.4_20101227.pdf
             */

            SendCommand((byte)ILI9341Commands.PowerControlA);
            SendData(new byte[] { 0x39, 0x2C, 0x00, 0x34, 0x02 });

            SendCommand((byte)ILI9341Commands.PowerControlB);
            SendData(new byte[] { 0x00, 0xC1, 0x30 });

            SendCommand((byte)ILI9341Commands.TimingControlA);
            SendData(new byte[] { 0x85, 0x00, 0x78 });

            SendCommand((byte)ILI9341Commands.TimingControlB);
            SendData(new byte[] { 0x00, 0x00 });

            SendCommand((byte)ILI9341Commands.PowerOnSequenceControl);
            SendData(new byte[] { 0x64, 0x03, 0x12, 0x81 });

            SendCommand((byte)ILI9341Commands.PumpRatioControl);
            SendData(new byte[] { 0x20 });

            SendCommand((byte)ILI9341Commands.PowerControl1);
            SendData(new byte[] { 0x23 });

            SendCommand((byte)ILI9341Commands.PowerControl2);
            SendData(new byte[] { 0x10 });

            SendCommand((byte)ILI9341Commands.VComControl1);
            SendData(new byte[] { 0x3e, 0x28 });

            SendCommand((byte)ILI9341Commands.VComControl2);
            SendData(new byte[] { 0x86 });

            SendCommand((byte)ILI9341Commands.MemoryAccessControl);
            SendData(new byte[] { 0x48 });

            SendCommand((byte)ILI9341Commands.PixelFormatSet);
            SendData(new byte[] { 0x55 });

            SendCommand((byte)ILI9341Commands.FrameRateControl);
            SendData(new byte[] { 0x00, 0x18 });

            SendCommand((byte)ILI9341Commands.DisplayFunctionControl);
            SendData(new byte[] { 0x08, 0x82, 0x27 });

            SendCommand((byte)ILI9341Commands.Enable3G);
            SendData(new byte[] { 0x00 });

            SendCommand((byte)ILI9341Commands.EnableGammaSet);
            SendData(new byte[] { 0x01 });

            SendCommand((byte)ILI9341Commands.PositiveGammaCorrection);
            SendData(new byte[] { 0x0F, 0x31, 0x2B, 0x0C, 0x0E, 0x08, 0x4E, 0xF1, 0x37, 0x07, 0x10, 0x03, 0x0E, 0x09, 0x00 });

            SendCommand((byte)ILI9341Commands.NegativeGammaCorrection);
            SendData(new byte[] { 0x00, 0x0E, 0x14, 0x03, 0x11, 0x07, 0x31, 0xC1, 0x48, 0x08, 0x0F, 0x0C, 0x31, 0x36, 0x0F });

            SetRotation(ILI9341Rotations.ZeroDegrees);

            SendCommand((byte)ILI9341Commands.SleepOut);
            Thread.Sleep(200);
            SendCommand((byte)ILI9341Commands.DisplayOn);
            Thread.Sleep(100);
        }
        /// <summary>
        /// Switch off display
        /// </summary>
        public void DisplayOff()
        {
            SendCommand((byte)ILI9341Commands.DisplayOff);
            Thread.Sleep(100);
        }
        /// <summary>
        /// Switch on display
        /// </summary>
        public void DisplayOn()
        {
            SendCommand((byte)ILI9341Commands.DisplayOn);
            Thread.Sleep(100);
        }
        /// <summary>
        /// Set the screen rotation
        /// </summary>
        /// <param name="rotation">Possible rotation values</param>
        public void SetRotation(ILI9341Rotations rotation)
        {            
            SendCommand((byte)ILI9341Commands.MemoryAccessControl);
            switch (rotation)
            {
                case ILI9341Rotations.ZeroDegrees:
                    SendData(0x40 | 0x08);
                    Width = _coreWidth;
                    Height = _coreHeight;
                    break;
                case ILI9341Rotations.NinetyDegrees:
                    SendData(0x20 | 0x08);
                    Height = _coreWidth;
                    Width = _coreHeight;
                    break;
                case ILI9341Rotations.OneEightyDegrees:
                    SendData(0x80 | 0x08);
                    Width = _coreWidth;
                    Height = _coreHeight;
                    break;
                case ILI9341Rotations.TwoSeventyDegrees:
                    SendData(0x40 | 0x20 | 0x80 | 0x08);
                    Height = _coreWidth;
                    Width = _coreHeight;
                    break;
            }
        }
        /// <summary>
        /// Put the display in sleep mode
        /// </summary>
        public void Sleep()
        {
            DisplayOff();
            SendCommand((byte)ILI9341Commands.SleepMode);
            Thread.Sleep(200);
        }
        /// <summary>
        /// Set the drawing canvas
        /// </summary>
        /// <param name="x0">Top-x pixel position</param>
        /// <param name="y0">Top-y pixel position</param>
        /// <param name="width">Canvas width</param>
        /// <param name="height">Canvas height</param>
        public void SetCanvas(uint x0,uint y0,uint width,uint height)
        {
            if (width > Width) width = Width;
            if (height > Height) height = Height;

            uint x1 = x0 + width - 1;
            uint y1 = y0 + height - 1;

            SendCommand((byte)ILI9341Commands.ColumnAddressSet);
            SendData(new byte[] { (byte)(x0 >> 8), (byte)(x0), (byte)(x1 >> 8), (byte)(x1) });
            SendCommand((byte)ILI9341Commands.PageAddressSet);
            SendData(new byte[] { (byte)(y0 >> 8), (byte)(y0), (byte)(y1 >> 8), (byte)(y1) });
            SendCommand((byte)ILI9341Commands.MemoryWrite);
        }
        /// <summary>
        /// We are using 16-bit colors. R=5 bits, G=6 bits, B=5 bits.
        /// Convert conventional 8-bit RGB codes to 565 bit RGB codes
        /// </summary>
        /// <param name="r8">Red</param>
        /// <param name="g8">Green</param>
        /// <param name="b8">Blue</param>
        /// <returns>16-bit 565 RGB color value</returns>
        public ushort RGB888ToRGB565(byte r8, byte g8, byte b8)
        {
            ushort r5 = (ushort)((r8 * 249 + 1014) >> 11);
            ushort g6 = (ushort)((g8 * 253 + 505) >> 10);
            ushort b5 = (ushort)((b8 * 249 + 1014) >> 11);
            return (ushort)(r5 << 11 | g6 << 5 | b5);
        }
        
        /// <summary>
        /// Prepare the internal buffer and send it to the controller memory
        /// </summary>
        /// <param name="totalPixels">Total pixels to send</param>
        /// <param name="color">Color of all the pixels (single color)</param>
        protected void PrepareBufferAndSendData(int totalPixels,uint color)
        {
            byte vh = (byte)(color >> 8);
            byte vl = (byte)color;

            if (totalPixels < _buffer.Length)
            {
                PrepareToWrite();//reset buffer
                for (int loop = 0; loop < totalPixels; loop += 2)
                {
                    _buffer[loop] = vh;
                    _buffer[loop + 1] = vl;
                }
                SendData(_buffer, totalPixels);
            }
            else
            {
                int fullIterations = totalPixels / _buffer.Length;
                int partialIteration = (totalPixels % _buffer.Length != 0) ? 1 : 0;
                int pixelsToWrite = totalPixels;

                for (int loop = 0; loop < fullIterations; loop++)
                {
                    PrepareToWrite();//reset buffer
                    for (int idx = 0; idx < _buffer.Length; idx += 2)
                    {
                        _buffer[idx] = vh;
                        _buffer[idx + 1] = vl;                        
                    }
                    SendData(_buffer, _buffer.Length);
                    pixelsToWrite -= _buffer.Length;
                }
                if (partialIteration != 0)
                {
                    PrepareToWrite();//reset buffer
                    for (int idx = 0; idx < pixelsToWrite; idx += 2)
                    {
                        _buffer[idx] = vh;
                        _buffer[idx + 1] = vl;
                    }
                    SendData(_buffer, pixelsToWrite);
                }
            }
        }
        #endregion

        #region Driver Read/Write        
        /// <summary>
        /// Send a command to the controller
        /// </summary>
        /// <param name="cmd">The command to send</param>
        protected void SendCommand(byte cmd)
        {
            byte[] cmdBuf = { cmd };
            _dcPin.Write(GpioPinValue.Low);
            _csPin.Write(GpioPinValue.Low);
            _spiDevice.Write(cmdBuf);
            _dcPin.Write(GpioPinValue.High);
            _csPin.Write(GpioPinValue.High);
            cmdBuf = null;
        }
        /// <summary>
        /// Send the data as byte array 
        /// </summary>
        /// <param name="data">Byte array</param>
        protected void SendData(byte[] data)
        {
            _dcPin.Write(GpioPinValue.High);
            _csPin.Write(GpioPinValue.Low);
            _spiDevice.Write(data);
            _csPin.Write(GpioPinValue.High);
        }
        /// <summary>
        /// Send the data as byte array 
        /// </summary>
        /// <param name="data">Byte array</param>
        /// <param name="len">Number of items to send from the buffer</param>
        protected void SendData(byte[] data,int len)
        {
            byte[] dataToSend = new byte[len];
            Array.Copy(data, 0, dataToSend, 0, len);
            SendData(dataToSend);
            dataToSend = null;
        }
        /// <summary>
        /// Send data, single-byte
        /// </summary>
        /// <param name="data">Single byte data</param>
        protected void SendData(byte data)
        {
            byte[] dataBuf = { data };
            SendData(dataBuf);
            dataBuf = null;
        }
        
        #endregion
    }
}
