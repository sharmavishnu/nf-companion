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
using nanoFramework.Companion.Drivers.Display;
using Windows.Devices.Gpio;

namespace nanoFramework.Companion.Samples.Display
{
    /// <summary>
    /// This sample demonstrates how to work with the the display driver for TFT displays that
    /// use the ILI9341 controller chip
    /// </summary>
    public class ILI9341_SPI_Sample : ISample
    {
        /// <summary>
        /// Run the sample
        /// </summary>
        public void RunSample()
        {
            GpioPin dcPin = GpioController.GetDefault().OpenPin(PinNumber('B', 6)); //PB6...D/C pin
            GpioPin rstPin = GpioController.GetDefault().OpenPin(PinNumber('C', 7)); //PC1...Reset pin
            GpioPin csPin = GpioController.GetDefault().OpenPin(PinNumber('A', 9)); //PA9...ChipSelect pin

            /*
            * This companion library comes with a few default fonts. These fonts are pixel based fonts (7X9 means 7 pixel wide and 9 pixel high).
            * Commonly used symbols are supplied as pixel font too. If you want o build your own font to use with this library, 
            * use the font generator tool to design your own fonts.
            */

            //TODO::Change the SPI bus as per your board
            ILI9341_SPI display = new ILI9341_SPI("SPI2", 240, 320, dcPin, rstPin, csPin);
            display.SetFont(new PixelFont7X9());//Want a new font? Use the font designer tool
            display.Initialize();

            display.SetRotation(ILI9341Rotations.ZeroDegrees); //This makes it potrait...
            display.ClearScreen();

            ushort redColor = display.RGB888ToRGB565(255, 0, 0);
            ushort greenColor = display.RGB888ToRGB565(0, 255, 0);
            ushort blueColor = display.RGB888ToRGB565(0, 0,255);

            //Write some text
            display.DrawText(1, 1, @"Hello Coder !",redColor);
            //Draw a rectangle frame
            display.DrawRect(1, 20, display.Width - 20, 20, greenColor);
            //Draw a fill rectangle
            display.DrawRect(1, 40, display.Width - 20, 20, blueColor);
            Thread.Sleep(2000);
            //Now fill the screen with test pattern
            display.TestDisplay();
            Thread.Sleep(3000);

            display.Dispose();
        }
        /// <summary>
        /// Just a helper to get underlying pin number
        /// </summary>
        /// <param name="port">The port identifier</param>
        /// <param name="pin">The pin number on that port</param>
        /// <returns>int</returns>
        private int PinNumber(char port, byte pin)
        {
            if (port < 'A' || port > 'J')
                throw new ArgumentException();

            return ((port - 'A') * 16) + pin;
        }
    }
}
