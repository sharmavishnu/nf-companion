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
using System.Collections;
using System.Threading;
using nanoFramework.Devices.OneWire;

namespace nanoFramework.Companion.Drivers.Sensors
{
    /// <summary>
    /// Driver for DS18B20 temperature sensor. At the time of writing this code, more details about
    /// this sensor can be found at https://datasheets.maximintegrated.com/en/ds/DS18B20.pdf
    /// </summary>
    public class DS18B20 : AbstractSensor
    {
        #region Implementation
        /// <summary>
        /// The underlying I2C device
        /// </summary>
        private OneWireController _oneWire = null;
        /// <summary>
        /// How many decimal places to account in temperature measurements
        /// </summary>
        private uint _scale = 2;
        #endregion

        #region Constants
        /// <summary>
        /// Command to soft reset the HTU21D sensor
        /// </summary>
        public static readonly byte FAMILY_CODE = 0x28;
        /// <summary>
        /// Command to trigger a temperature conversion
        /// </summary>
        private readonly byte CONVERT_TEMPERATURE = 0x44;
        /// <summary>
        /// Command to copy the scratchpad register
        /// </summary>
        private readonly byte COPY_SCRATCHPAD = 0x48;
        /// <summary>
        /// Command to write to scratchpad register
        /// </summary>
        private readonly byte WRITE_SCRATCHPAD = 0x4E;
        /// <summary>
        /// Command to read scratchpad register
        /// </summary>
        private readonly byte READ_SCRATCHPAD = 0xBE;
        /// <summary>
        /// Error value of temperature
        /// </summary>
        private const float ERROR_TEMPERATURE = -999.99F;
        #endregion

        #region Properties
        /// <summary>
        /// The 8-byte address of this device (since there could be more than one such devices on the bus)
        /// </summary>
        public byte[] Address { get; private set; }
        /// <summary>
        /// Accessor/Mutator for temperature in celcius
        /// </summary>
        public float TemperatureInCelcius { get; private set; }
        /// <summary>
        /// Sensor resolution
        /// R1=0,R0=0=>0 -> 9bit 
        /// R1=0,R0=1=>1 -> 10bit 
        /// R1=1,R0=0=>2 -> 11bit 
        /// R1=1,R0=1=>3 -> 12bit (default on power up) 
        /// </summary>
        public byte Resolution { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="owBus">Which one wire controller (logical bus) to use</param>
        /// <param name="deviceAddr">The device address (if null, then this device will search for one on the bus and latch on to the first one found)</param>
        /// <param name="scale">How many decimal places to look for in the temperature and humidity values</param>
        public DS18B20(OneWireController owBus,byte[] deviceAddr = null,uint scale=2)
        {
            _oneWire = owBus;
            if (deviceAddr != null) {
                if (deviceAddr.Length != 8) throw new ArgumentException();//must be 8 bytes
                if (deviceAddr[0] != FAMILY_CODE) throw new ArgumentException();//invalid family code
                Address = deviceAddr;
            }
            
            TemperatureInCelcius = ERROR_TEMPERATURE;
            Resolution = 3;
            _scale = scale;
        }
        #endregion

        #region IDisposable Support

        /// <summary>
        /// Dispose this object
        /// </summary>
        protected override void DisposeSensor()
        {
            Address = null;
        }

        #endregion

        #region Core Methods
        /// <summary>
        /// Initialize the sensor. This step will perform a reset of the 1-wire bus.
        /// It will check for existence of a 1-wire device. If no address was provided, then the
        /// 1-wire bus will be searched and the first device that matches the family code will be latched on to.
        /// Developer should check for successful initialization by checking the Address property. 
        /// It should have valid 64-bit value
        /// </summary>
        public override void Initialize()
        {
            bool found = true;
            if (Address == null) //search for a device with the required family code
            {
                found = false;
                if (_oneWire.FindFirstDevice(true, false)) //Current nF firmware works if reset if performed before a find operation
                {
                    do
                    {
                        if (_oneWire.SerialNumber[0] == FAMILY_CODE)
                        {
                            //found the device
                            Address = new byte[_oneWire.SerialNumber.Length];
                            Array.Copy(_oneWire.SerialNumber, Address, _oneWire.SerialNumber.Length);
                            found = true;
                            break;
                        } 
                    } while (_oneWire.FindNextDevice(true, false));//keep searching until we get one                    
                }
            }
            if (!found) throw new Exception();
        }
        /// <summary>
        /// Prepare sensor to read the data
        /// </summary>
        public override void PrepareToRead()
        {
            if (Address != null && Address.Length == 8 && Address[0] == FAMILY_CODE)
            {
                _oneWire.TouchReset();
                //first address all devices
                _oneWire.WriteByte(0xCC);//Skip ROM command
                _oneWire.WriteByte(CONVERT_TEMPERATURE);//convert temperature
                Thread.Sleep(1000);//Wait for conversion (in default 12-bit resolution mode)                                            
            }
        }
        /// <summary>
        /// Read sensor data
        /// </summary>
        /// <returns>true on success, else false</returns>
        public override bool Read()
        {
            if (Address != null && Address.Length == 8 && Address[0] == FAMILY_CODE)
            {
                //now write command and ROM at once
                byte[] cmdAndData = new byte[9] {
                    0x55, //match ROM command
                    Address[0],Address[1],Address[2],Address[3],Address[4],Address[5],Address[6],Address[7] //do not convert to a for..loop
                };

                _oneWire.TouchReset();
                foreach (var b in cmdAndData) _oneWire.WriteByte(b);

                //now read the scratchpad
                var verify = _oneWire.WriteByte(READ_SCRATCHPAD);

                //Now read the temperature
                var tempLo = _oneWire.ReadByte();
                var tempHi = _oneWire.ReadByte();

                if (_oneWire.TouchReset())
                {
                    float currentTemperature = ((tempHi << 8) | tempLo) / 16;
                    TemperatureInCelcius = (float)(Math.Floor(currentTemperature * Math.Pow(10, _scale)) / Math.Pow(10, _scale));
                }
                else
                    TemperatureInCelcius = ERROR_TEMPERATURE;
            }
            return (TemperatureInCelcius != ERROR_TEMPERATURE);
        }
        /// <summary>
        /// Reset the sensor...this performs a soft reset. To perform a hard reset, the system must be 
        /// power cycled
        /// </summary>
        public override void Reset()
        {
            _oneWire.TouchReset();
            TemperatureInCelcius = ERROR_TEMPERATURE;
        }

        #endregion

        #region Change tracking
        /// <summary>
        /// This sensor suports change tracking
        /// </summary>
        /// <returns>bool</returns>
        public override bool CanTrackChanges()
        {
            return true;
        }
        /// <summary>
        /// Let the world know whether the sensor value has changed or not
        /// </summary>
        /// <returns>bool</returns>
        public override bool HasSensorValueChanged()
        {
            float previousTemperature = TemperatureInCelcius;

            PrepareToRead();
            Read();

            float currentTemperature = (float)(Math.Floor(TemperatureInCelcius * Math.Pow(10, _scale)) / Math.Pow(10, _scale));
            
            bool valuesChanged = (previousTemperature != currentTemperature);

            return valuesChanged;
        }
        #endregion        
    }
}
