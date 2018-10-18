﻿/**
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
    /// This sample demonstrates how to the the display driver for OLED displays that
    /// use the SSD1306 controller chip
    /// </summary>
    public class OLEDSSD1306_SPI_Sample : ISample
    {
        /// <summary>
        /// Run the sample
        /// </summary>
        public void RunSample()
        {
            /*
             * On STM32F401RE Nucleo board, SPI2 and SPI3 are enabled in nanoFramework. This example
             * uses SPI2. See datasheet for STM32F401. It provides the following pins for SPI2:
             * a. PC2/PB14 = SPI2_MISO
             * b. PC3/PB15 = SPI2_MOSI
             * c. PB9/PB12 = SPI2_NSS
             * d. PB10/PB13= SPI2_SCK
             * This example was tested with the pins PC2,PC3,PB12 and PB13 for SPI
             */
            GpioPin csPin = OpenPin('B', 12, GpioSharingMode.Exclusive); //SPI2_NSS pin is PB12
            GpioPin rstPin = OpenPin('A', 2, GpioSharingMode.Exclusive); //PA2 used for reset pin
            GpioPin dcPin = OpenPin('A', 3, GpioSharingMode.Exclusive);//PA3 used for data/command pin
            OLEDSSD1306_SPI oled = new OLEDSSD1306_SPI("SPI2", /*On STM32F401RE Nucleo board, SPI2 is configured for use*/
                                                       csPin, /*Chip select pin*/
                                                       dcPin, /*Data/Command pin*/
                                                       rstPin, /*Reset pin*/
                                                       128, /*Width in pixel*/
                                                       64 /*height in pixel*/);
            /*
             * This companion library comes with a few default fonts. These fonts are pixel based fonts (7X9 means 7 pixel wide and 9 pixel high).
             * Commonly used symbols are supplied as pixel font too. If you want o build your own font to use with this library, 
             * use the font generator tool to design your own fonts.
             */ 
            oled.SetFont(new PixelFont7X9());

            oled.Initialize();
            oled.ClearScreen();
            oled.PrepareToWrite();
            oled.DrawRectangle(0, 0, 128, 64);//draw a rectangle border
            oled.DrawText(4, 4, "Hello nanoFramework"); //There are 19 characters in the text, meaning total pixel width of 19 x 7 = 143 + 19 = 152...
                                                        //so wrapping will happen...for each character, there is 1-pixel gutter provided. Also, when line
                                                        //wraps, another 1-pixel gutter is provided in y-direction
            oled.Write();//Write buffer contents to display...
            Thread.Sleep(3000);

            oled.SetFont(new Symbols7X9());//We want to use symbols...each ASCII code (32-126) represents a symbol
            oled.ClearScreen();
            oled.PrepareToWrite();
            oled.DrawText(1,1,"ABCDEFGHIJKLMNOPQRSTUVWXYZ"); //Draw 26 symbols corresponding to ascii codes from A-Z
            oled.Write(); //Write buffer contents to display

            oled.Dispose();
        }
        /// <summary>
        /// Open the physical pin
        /// </summary>
        /// <param name="port">Port name</param>
        /// <param name="pin">Port's pin number</param>
        /// <param name="sharingMode">The Gpio sharing mode</param>
        /// <returns>The GpioPin object</returns>
        private GpioPin OpenPin(char port,int pin, GpioSharingMode sharingMode)
        {
            if ((port < 'A' || port > 'J') ||
                (pin > 15))
            {
                throw new ArgumentException();
            }

            return GpioController.GetDefault().OpenPin(((port - 'A') * 16) + pin, sharingMode);
        }
    }
}
