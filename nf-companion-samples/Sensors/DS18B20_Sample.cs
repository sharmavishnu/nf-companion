/**
Copyright 2019 Vishnu Sharma , (Twitter : @getvishnu)

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
using nanoFramework.Devices.OneWire;

namespace nanoFramework.Companion.Samples.Sensors
{
    /// <summary>
    /// This sample is to demonstrate how to use the DS18B20 sensor driver
    /// </summary>
    public class DS18B20_Sample : ISample
    {
        /// <summary>
        /// The run method
        /// </summary>
        public void RunSample()
        {
            OneWireController oneWire = new OneWireController();
            DS18B20 ds18b20 = new DS18B20(oneWire,/* The 1-wire bus*/ 
                                    null, /*Let this driver find out a DS18B20 on the bus*/ 
                                    3 /*3 decimal places is enough for us while reading temperature changes*/);

            /*
             * NOTE: Limiting to the decimal places will not work when you do a "ToString" on floats.
             * The limit to decimal places is only for comparison. For exmaple, if last measured temperature value
             * was 25.3343567 and the next value is 25.3343667, then the difference between the two is about 0.00001.
             * If we limit to 3 decimla places, then the values are read as 25.334 and 25.334, resulting in a difference 
             * of zero. This is used to compute if sensors changed or not...more the number of decimal places, higher
             * is the change event possibility (because even a very small change will be registered)
             */

            int loopCount = 3; //used later to limit test duration
            string devAddrStr = "";//store the device address as string...

            /*********************************************************************************************************
             * This driver supports, one-shot , poll mode (meaning,you check the sensor for changes 
             * in temperature values) and event mode (meaning, the driver will alert you when 
             * temperature changes)
             *********************************************************************************************************/
            //One-Shot-mode example...
            ds18b20.Initialize(); //Initialize sensor
            /*After device gets initialized and if initialization is successful, the class DS18B20 should have an address*/
            if(ds18b20.Address != null && ds18b20.Address.Length == 8 && ds18b20.Address[0] == DS18B20.FAMILY_CODE)
            {
                //Initialization successful...let's try to read the address
                /*
                 * Since this class was initialized without an address, the Initialize() method will search for valid 
                 * devices on the bus, and select the first device of type DS18B20 on the bus. If you have multiple devices,
                 * You can use the OneWireController class's "Find" methods to first search for devices, and then initialize
                 * the class with an address.
                 */
                foreach (var addrByte in ds18b20.Address) devAddrStr += addrByte.ToString("X2");
                
                ds18b20.PrepareToRead();
                ds18b20.Read();
                Console.WriteLine("DS18B20["+ devAddrStr +"] Sensor reading in One-Shot-mode; T=" + ds18b20.TemperatureInCelcius.ToString() + " C");
            }

            /*Polled example*/
            loopCount = 3;
            ds18b20.Reset();
            ds18b20.Initialize();//after this device should have valid address...see above on how to check

            while (loopCount > 0)
            {
                if (ds18b20.HasSensorValueChanged())
                {
                    //no need to read again (like HTU21D)
                    Console.WriteLine("DS18B20[" + devAddrStr + "] in Poll-mode;T=" + ds18b20.TemperatureInCelcius.ToString());
                }
                loopCount--;
            }

            /*Event mode...*/
            loopCount = 3;
            ds18b20.Reset();
            ds18b20.Initialize(); //again, if initialization is successful, object will have valid address (see above)
            if (ds18b20.CanTrackChanges())
            {
                ds18b20.SensorValueChanged += () => {
                    //no need to read again (like HTU21D)
                    Console.WriteLine("DS18B20 (" + devAddrStr + ") in Event-mode;T=" + ds18b20.TemperatureInCelcius.ToString());
                };
                ds18b20.BeginTrackChanges(2000/*track changes every 2 seconds*/);
                while (loopCount > 0)
                {
                    Thread.Sleep(3000);//Wait for a change...
                    loopCount--;
                }
                ds18b20.EndTrackChanges();
            }
            ds18b20.Dispose();
        }
    }
}
