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
using nanoFramework.Companion.Samples.Display;
using nanoFramework.Companion.Samples.Memory;
using nanoFramework.Companion.Samples.Sensors;


namespace nanoFramework.Companion.Samples
{
    /// <summary>
    /// Main entry point for various samples that demonstrate how to work with various library elements
    /// provided by the nanoFramework companion library
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method
        /// </summary>
        public static void Main()
        {
            /*
             * Uncomment the driver you want to test
             */
            //(new EEPROM_24LC256_Sample()).RunSample(); //EEPROM, 24LC256 based test
            //(new HTU21D_Sample()).RunSample(); //Test HTU21D sensor driver
            (new OLEDSSD1306_I2C_Sample()).RunSample(); //Test for 128 x 32 OLED I2C display
            //(new OLEDSSD1306_SPI_Sample()).RunSample(); //Test for 128 x 32 OLED SPI display

            //Alternative method to run all the tests
            //ISample[] drivers = { new EEPROM_24LC256_Sample(),  new HTU21D_Sample(), new OLEDSSD1306_I2C_Sample() };
            //foreach (ISample driverSample in drivers)
            //{
            //    driverSample.RunSample();
            //}
        }
    }
}
