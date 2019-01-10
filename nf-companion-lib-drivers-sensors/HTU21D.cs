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
using Windows.Devices.I2c;

namespace nanoFramework.Companion.Drivers.Sensors
{
    /// <summary>
    /// Driver for HTU21D temperature sensor. At the time of writing this code, more details about
    /// this sensor can be found at http://www.te.com/usa-en/product-CAT-HSC0004.html
    /// </summary>
    public class HTU21D : AbstractSensor
    {
        #region Implementation
        /// <summary>
        /// The underlying I2C device
        /// </summary>
        private I2cDevice _i2CDevice = null;
        /// <summary>
        /// How many decimal places to account in temperature and humidity measurements
        /// </summary>
        private uint _scale = 2;
        #endregion

        #region Constants
        /// <summary>
        /// Command to soft reset the HTU21D sensor
        /// </summary>
        private readonly byte[] SOFT_RESET = { 0xFE };
        /// <summary>
        /// Command to trigger a humidity measurement and hold the value
        /// </summary>
        private readonly byte[] TRIGGER_HUMD_MEASURE_HOLD = { 0xE5 };
        /// <summary>
        /// Command to trigger a temperature measurement and hold the value
        /// </summary>
        private readonly byte[] TRIGGER_TEMP_MEASURE_HOLD = { 0xE3 };
        /// <summary>
        /// Command to read user register
        /// </summary>
        private readonly byte[] READ_USER_REGISTER = {0xE7 };
        /// <summary>
        /// Command to write user register
        /// </summary>
        private readonly byte[] WRITE_USER_REGISTER = { 0xE6 };
        /// <summary>
        /// For CRC check
        /// </summary>
        private const Int32 SHIFTED_DIVISOR = 0x988000;
        /// <summary>
        /// Error value of humidity
        /// </summary>
        private const float ERROR_HUMIDITY = -999.99F;
        /// <summary>
        /// Error value of temperature
        /// </summary>
        private const float ERROR_TEMPERATURE = -999.99F;
        #endregion

        #region Properties
        /// <summary>
        /// Accessor/Mutator for relative humidity %
        /// </summary>
        public float RelativeHumidity { get; private set; }
        /// <summary>
        /// Accessor/Mutator for temperature in celcius
        /// </summary>
        public float TemperatureInCelcius { get; private set; }
        /// <summary>
        /// Sensor resolution
        /// 0000 0000 = 12bit RH, 14bit Temperature
        /// 0000 0001 = 8bit RH, 12bit Temperature
        /// 1000 0000 = 10bit RH, 13bit Temperature
        /// 1000 0001 = 11bit RH, 11bit Temperature
        /// </summary>
        public byte Resolution { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="busSelector">Which I2C bus to use</param>
        /// <param name="deviceAddr">The I2C device address (default is 0x40, the 7-bit, shifted address)</param>
        /// <param name="speed">The I2C bus speed (by default, 100KHz)</param>
        /// <param name="scale">How many decimal places to look for in the temperature and humidity values</param>
        public HTU21D(string busSelector = "I2C1",int deviceAddr = 0x40, I2cBusSpeed speed = I2cBusSpeed.StandardMode, 
            uint scale=2)
        {
            _i2CDevice = I2cDevice.FromId(busSelector, new I2cConnectionSettings(deviceAddr) { BusSpeed = speed, SharingMode = I2cSharingMode.Shared });
            RelativeHumidity = ERROR_HUMIDITY;
            TemperatureInCelcius = ERROR_TEMPERATURE;
            Resolution = 0x81;
            _scale = scale;
        }
        #endregion

        #region IDisposable Support

        /// <summary>
        /// Dispose this object
        /// </summary>
        protected override void DisposeSensor()
        {
            _i2CDevice.Dispose();
            _i2CDevice = null;
        }

        #endregion

        #region Core Methods
        /// <summary>
        /// Initialize the sensor. This is where we setup the sensor resolution.
        /// By default, the resolution is set to 11-bit resolution
        /// for temperature and humidity
        /// </summary>
        public override void Initialize()
        {
            //Set sensor resolution...
            byte userRegister = ReadUserRegister(); //Go get the current register state            
            userRegister &= 0x73; //Turn off the resolution bits
            Resolution &= 0x81; //Turn off all other bits but resolution bits
            userRegister |= Resolution; //Mask in the requested resolution bits

            //Request a write to user register
            _i2CDevice.Write(WRITE_USER_REGISTER); //Write to the user register command
            _i2CDevice.Write(new byte[] { userRegister }); //Write the new resolution bits
        }
        /// <summary>
        /// Prepare sensor to read the data
        /// </summary>
        public override void PrepareToRead()
        {
            ; ;//Nothin needed for now
        }
        /// <summary>
        /// Read sensor data
        /// </summary>
        /// <returns>true on success, else false</returns>
        public override bool Read()
        {
            float rh = ReadHumidity();
            float temp = ReadTemperature();
            if (rh != ERROR_HUMIDITY && temp != ERROR_TEMPERATURE)
            {
                //Limit to the number if decimal places configured
                RelativeHumidity = (float)(Math.Floor(rh *Math.Pow(10, _scale)) / Math.Pow(10, _scale));
                TemperatureInCelcius = (float)(Math.Floor(temp * Math.Pow(10, _scale)) / Math.Pow(10, _scale));
                return true;
            }
            else
                return false;            
        }
        /// <summary>
        /// Reset the sensor...this performs a soft reset. To perform a hard reset, the system must be 
        /// power cycled
        /// </summary>
        public override void Reset()
        {
            _i2CDevice.Write(SOFT_RESET);
            TemperatureInCelcius = ERROR_TEMPERATURE;
            RelativeHumidity = ERROR_HUMIDITY;
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
            float currentTemperature = (float)(Math.Floor(ReadTemperature() * Math.Pow(10, _scale)) / Math.Pow(10, _scale));
            float currentHumidity = (float)(Math.Floor(ReadHumidity() * Math.Pow(10, _scale)) / Math.Pow(10, _scale));
            bool valuesChanged = (TemperatureInCelcius != currentTemperature) || (RelativeHumidity != currentHumidity);
            
            return valuesChanged;
        }
        #endregion        

        #region Helpers
        /// <summary>
        /// Read humidity value from the sensor
        /// </summary>
        /// <returns>humidity as a floating point number</returns>
        protected float ReadHumidity()
        {
            _i2CDevice.Write(TRIGGER_HUMD_MEASURE_HOLD);

            //Hang out while measurement is taken. 50mS max, page 4 of datasheet.
            Thread.Sleep(100);

            //Comes back in three bytes, data(MSB) / data(LSB) / Checksum
            byte[] readHum = new byte[3];
            _i2CDevice.Read(readHum);

            byte msb = readHum[0];
            byte lsb = readHum[1];
            byte checksum = readHum[2];
            
            uint rawHumidity = ((uint)msb << 8) | (uint)lsb;
            if (! IsCRCValid((UInt16)rawHumidity, checksum)) return ERROR_HUMIDITY; //Error out

            rawHumidity &= 0xFFFC; //Zero out the status bits but keep them in place

            //Given the raw humidity data, calculate the actual relative humidity
            float tempRH = rawHumidity / (float)65536; //2^16 = 65536
            float rh = -6 + (125 * tempRH); //From page 14

            return (rh);

        }
        /// <summary>
        /// Read the temperature from the sensor
        /// </summary>
        /// <returns>temperature  as a floating point number in celcius</returns>
        protected float ReadTemperature()
        {
            _i2CDevice.Write(TRIGGER_TEMP_MEASURE_HOLD);
            
            //Hang out while measurement is taken. 50mS max, page 4 of datasheet.
            Thread.Sleep(100);

            //Comes back in three bytes, data(MSB) / data(LSB) / Checksum
            byte[] readTemp = new byte[3];
            _i2CDevice.Read(readTemp);

            byte msb = readTemp[0];
            byte lsb = readTemp[1];
            byte checksum = readTemp[2];

            uint rawTemperature = ((uint)msb << 8) | (uint)lsb;
            if (!IsCRCValid((UInt16)rawTemperature, checksum)) return ERROR_TEMPERATURE; //Error out

            rawTemperature &= 0xFFFC; //Zero out the status bits but keep them in place

            //Given the raw temperature data, calculate the actual temperature
            float tempTemperature = rawTemperature / (float)65536; //2^16 = 65536
            float rt = (float)(-46.85 + (175.72 * tempTemperature)); //From page 14            

            return (rt);
        }
        /// <summary>
        /// Read the user register
        /// </summary>
        /// <returns>byte</returns>
        protected byte ReadUserRegister()
        {
            byte[] result = new byte[1];

            _i2CDevice.Write(READ_USER_REGISTER);
            Thread.Sleep(50);

            _i2CDevice.Read(result);
            return result[0];
        }

        /// <summary>
        /// Check if CRC returned by the sensor matches our calculation.
        /// This calculation is based on the algorithm as given here 
        /// https://github.com/TEConnectivity/HTU21D_Generic_C_Driver/blob/master/htu21d.c
        /// </summary>
        /// <param name="sensorValue">The sensor reading</param>
        /// <param name="crc">The CRC returned by the sensor</param>
        /// <returns>true if our CRC calculation matches the CRC returned by the sensor</returns>
        bool IsCRCValid(UInt16 sensorValue, byte crc)
        {
            UInt32 polynomial = 0x988000;
            UInt32 msb = 0x800000;
            UInt32 mask = 0xFF8000;
            UInt32 result = (UInt32)sensorValue << 8;//pad with zeros as specified in spec

            while (msb != 0x80)
            {
                //Check if msb of current value is 1 and apply XOR mask
                if ((result & msb) == msb)
                    result = ((result ^ polynomial) & mask) | (result & ~mask);
                //shift by one
                msb >>= 1;
                mask >>= 1;
                polynomial >>= 1;
            }
            return (result == crc);
        }
        #endregion
    }
}
