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
using System.Text;
using System.Threading;
using Windows.Devices.I2c;
using nanoFramework.Companion.Drivers.Sensors;

namespace nanoFramework.Companion.Samples.Sensors
{
    /// <summary>
    /// This sample is to demonstrate how to use the HTU21D sensor driver
    /// </summary>
    public class HTU21D_Sample : ISample
    {
        /// <summary>
        /// The run method
        /// </summary>
        public void RunSample()
        {            

            HTU21D htu = new HTU21D("I2C1",/*For STM32F401RE Nucleo board, I2C1 bus is being configured in nanoFramework*/ 
                                    0x40, /*The sensor slave address*/ 
                                    I2cBusSpeed.StandardMode, /*We want to use the 100KHz speed to communicate with sensor*/ 
                                    3 /*3 decimal places is enough for us while reading temperature and humidity changes*/);

            /*
             * NOTE: Limiting to the decimal places will not work when you do a "ToString" on floats.
             * The limit to decimal places is only for comparison. For exmaple, if last measured temperature value
             * was 25.3343567 and the next value is 25.3343667, then the difference between the two is about 0.00001.
             * If we limit to 3 decimla places, then the values are read as 25.334 and 25.334, resulting in a difference 
             * of zero. This is used to compute if sensors changed or not...more the number of decimal places, higher
             * is the change event possibility (because even a very small change will be registered)
             */

            int loopCount = 3; //used later to limit test duration

            /*********************************************************************************************************
             * This driver supports, one-shot , single read mode, poll mode (meaning,you check the sensor for changes 
             * in temperature/humidity values) and event mode (meaning, the driver will alert you when 
             * temperature/humidity changes)
             *********************************************************************************************************/
            //One-Shot-mode example...
            htu.Initialize(); //Initialize sensor
            htu.PrepareToRead();
            htu.Read();            
            Console.WriteLine("HTU21D Sensor reading in One-Shot-mode; T=" + htu.TemperatureInCelcius.ToString() + " C, RH%=" + htu.RelativeHumidity.ToString());

            //Poll-mode example...

            htu.Reset(); //reset to start from scratch
            htu.Initialize();
            htu.PrepareToRead();
            
            while(loopCount > 0)
            {
                //This is how you can read the sensor on changes
                if (htu.HasSensorValueChanged())
                {
                    htu.Read();//Must not call read until "HasSensorValueChanged" method is called...
                    Console.WriteLine("HTU21D Sensor reading in Poll-mode; T=" + htu.TemperatureInCelcius.ToString() + " C, RH%=" + htu.RelativeHumidity.ToString());
                }
                loopCount--;//we want at-least 3-instances of change...be careful with "number of decimal places to register change
                Thread.Sleep(3000);
            }
            loopCount = 3;
            //Event mode....option 3
            htu.Reset();
            htu.Initialize();
            if (htu.CanTrackChanges())
            {
                htu.SensorValueChanged += () => {
                    htu.Read();
                    Console.WriteLine("HTU21D Sensor reading in event-mode; T=" + htu.TemperatureInCelcius.ToString() + " C, RH%=" + htu.RelativeHumidity.ToString());
                    loopCount--;//we want at-least 3-instances of change...be careful with "number of decimal places to register change
                };
                htu.BeginTrackChanges(2000 /*track changes every 2 seconds*/);
                while(loopCount > 0)
                {
                    Thread.Sleep(3000);//Just a wait in the sample to end sample gracefully
                    loopCount--;
                }
                htu.EndTrackChanges();
            }
            htu.Dispose();
        }
    }
}
