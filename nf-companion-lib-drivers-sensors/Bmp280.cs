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

using nanoFramework.Companion;

namespace nanoFramework.Companion.Drivers.Sensors
{    
    /// <summary>
    /// Bmp280 is a pressure, temperature and altitude measuring sensor from Bosch.
    /// At the time of writing this code, the data was available at https://ae-bst.resource.bosch.com/media/_tech/media/datasheets/BST-BMP280-DS001.pdf
    /// </summary>
    public class Bmp280 : AbstractSensor
    {
        #region Enums
        /// <summary>
        /// Enum to define power modes.
        /// See section 3.6 of the datasheet
        /// </summary>
        public enum PowerModes : byte
        {
            SleepMode = 0x00, //Sensor is in sleep mode
            ForcedMode = 0x02, //Sensor measures when needed and then goes to sleep
            NormalMode = 0x03 //Sensor continuously measures with a standby time in-between
        }
        /// <summary>
        /// When normal mode is set, the sensor makes periodic measurements. This standby time 
        /// indicates what is the wait time, after which a measurement will be made. This time 
        /// has impact only when "Normal Mode" is selected.
        /// </summary>
        public enum StandbyTime : byte
        {
            MillisPoint5 = 0, // 0.5 milliseconds
            Millis63 = 1,   //63 milliseconds
            Millis125 = 2,  //125 milliseconds
            Millis250 = 3,  //250 milliseconds
            Millis500 = 4,  //500 milliseconds
            Millis1000=5,   //1000 milliseconds
            Millis2000=6,   //2000 milliseconds
            Millis4000=7    //4000 milliseconds
        }
        /// <summary>
        /// The various pressure and temperature sampling options. Refer to section 3.3.1 of the datasheet
        /// for more information. Additionally, read section 3.4 to understand various 
        /// dependencies
        /// </summary>
        public enum Sampling : byte
        {
            Skip = 0, //Skip pressure sampling
            UltraLowPower_1X = 1, //16-bit pressure resolution
            LowPower_2X = 2, //17-but pressure resolution
            StandardResolution_4X = 3, //18-bit resolution
            HighResolution_8X=4, //19-bit resolution
            UltraHighResolution_16X=5, //20 bit resolution
        }
        /// <summary>
        /// The sensor contains an IIR filter to ensure bursts are managed correctly (e.g. strong, sudden wind)
        /// Read section 3.3.3 to understand more. This enum provides the filter co-efficient to use
        /// </summary>
        public enum Filter : byte
        {
            Off=0, //Filter is off.1 sample to reach 75% of step response
            Coefficient_X2 = 1, //2 sample to reach 75% of step response
            Coefficient_X4 = 2, //5 samples to reach 75% of step response
            Coefficient_X8 = 3, //11 sample to reach 75% of step response 
            Coefficient_X16 = 4 //22 sample to reach 75% of step response
        }
        #endregion

        #region Structures
        /// <summary>
        /// Structure to hold calibration data. This data is burned into the sensor during manufacturing.
        /// See section 3.11.3 to understand how to use the compensation formula
        /// </summary>
        protected struct CalibrationData
        {
            /*Temperature calibration*/
            public UInt16 DigT1;
            public Int16 DigT2;
            public Int16 DigT3;

            /*Pressure calibration*/
            public UInt16 DigP1;
            public Int16 DigP2;
            public Int16 DigP3;
            public Int16 DigP4;
            public Int16 DigP5;
            public Int16 DigP6;
            public Int16 DigP7;
            public Int16 DigP8;
            public Int16 DigP9;
        }
        #endregion

        #region Constants
        /// <summary>
        /// Address of reset register
        /// </summary>
        private readonly byte RESET_REG_ADDR = 0xE0;
        /// <summary>
        /// Address of the ID register
        /// </summary>
        private readonly byte ID_REG_ADDR = 0xD0;
        /// <summary>
        /// Address of status register
        /// </summary>
        private readonly byte STATUS_REG_ADDR = 0xF3;
        /// <summary>
        /// Address of the control register
        /// </summary>
        private readonly byte CONTROL_REG_ADDR = 0xF4;
        /// <summary>
        /// Address of the configuration register
        /// </summary>
        private readonly byte CONFIG_REG_ADDR = 0xF5;
        /// <summary>
        /// Error value of pressure
        /// </summary>
        private const float ERROR_PRESSURE = -999.99F;
        /// <summary>
        /// Error value of temperature
        /// </summary>
        private const float ERROR_TEMPERATURE = -999.99F;
        #endregion

        #region Implementation
        /// <summary>
        /// The I2cDevice object
        /// </summary>
        protected I2cDevice _device = null;
        /// <summary>
        /// The write buffer
        /// </summary>
        protected byte[] _wBuf = null;
        /// <summary>
        /// The read buffer
        /// </summary>
        protected byte[] _rBuf = null;
        /// <summary>
        /// How many decimal places to account in temperature , pressure and altitude measurements
        /// </summary>
        protected uint _scale = 2;
        /// <summary>
        /// Holds the calibration data
        /// </summary>
        protected CalibrationData _calibration;
        /// <summary>
        /// Current contents of configuration register
        /// </summary>
        protected byte _currentConfigReg = 0;
        /// <summary>
        /// Current contents of control register
        /// </summary>
        protected byte _currentControlReg = 0;
        /// <summary>
        /// Compensation value required from temperature measurement and to be used for pressure computation
        /// </summary>
        protected Int32 t_fine = Int32.MinValue;
        #endregion

        #region Properties
        /// <summary>
        /// Accessor/Mutator for pressure in Pa
        /// </summary>
        public float PressureInPa { get; private set; }
        /// <summary>
        /// Accessor/Mutator for temperature in celcius
        /// </summary>
        public float TemperatureInCelcius { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="busSelector">Which I2C bus to use</param>
        /// <param name="deviceAddr">The I2C device address (default is 0x76, the 7-bit, shifted address)</param>
        /// <param name="speed">The I2C bus speed (by default, 100KHz)</param>
        /// <param name="scale">How many decimal places to look for in the temperature and humidity values</param>
        public Bmp280(string busSelector = "I2C1", int deviceAddr = 0x76, I2cBusSpeed speed = I2cBusSpeed.StandardMode,uint scale = 2)
        {
            _device = I2cDevice.FromId(busSelector, new I2cConnectionSettings(deviceAddr) { BusSpeed = speed, SharingMode= I2cSharingMode.Shared });
            _scale = scale;
            _wBuf = new byte[1] { 0x00 };
            _rBuf = new byte[1] { 0x00 };
            PressureInPa = ERROR_PRESSURE;
            TemperatureInCelcius = ERROR_TEMPERATURE;
        }
        #endregion

        #region Sensor Abstract Method Implementations
        /// <summary>
        /// Initialize the sensor. The current defaults, with which the sensor is inialized are:
        /// 1. Power mode - Normal
        /// 2. Pressure sampling - Standard
        /// 3. Temperature sampling - Standard
        /// 4. Standby - 125 ms
        /// 5. Filter coefficient - X2
        /// </summary>
        public override void Initialize()
        {
            //Load the calibration coefficients
            LoadCalibrationCoefficients();
            //Set reasonable defaults
            SetOptions(PowerModes.NormalMode, Sampling.StandardResolution_4X, Sampling.StandardResolution_4X, Filter.Coefficient_X2, StandbyTime.Millis125);
            _currentConfigReg = ReadRegister(CONFIG_REG_ADDR);
            _currentControlReg = ReadRegister(CONTROL_REG_ADDR);
            TemperatureInCelcius = ERROR_TEMPERATURE;
            PressureInPa = ERROR_PRESSURE;
        }
        /// <summary>
        /// Reset sensor.
        /// </summary>
        public override void Reset()
        {
            WriteRegister(RESET_REG_ADDR, 0xB6);
            Thread.Sleep(1000);
        }
        /// <summary>
        /// Prepare the sensor to read the data.         
        /// </summary>
        /// <returns>bool</returns>
        public override void PrepareToRead()
        {
            while (IsMeasuring() || IsUpdatingMemory()) ; ;//wait
        }
        /// <summary>
        /// Read sensor data
        /// </summary>
        /// <returns>bool</returns>
        public override bool Read()
        {
            var temperature = ReadTemperature();
            var pressure = ReadPressure();

            if (temperature != ERROR_TEMPERATURE && pressure != ERROR_PRESSURE)
            {
                //Limit to number of decimal places configured
                PressureInPa = (float)(Math.Floor(pressure * Math.Pow(10, _scale)) / Math.Pow(10, _scale));
                TemperatureInCelcius = (float)(Math.Floor(temperature * Math.Pow(10, _scale)) / Math.Pow(10, _scale));
                return true;
            }
            else
                return false;
        }
        
        /// <summary>
        /// Dispose this sensor
        /// </summary>
        protected override void DisposeSensor()
        {
            if (_device != null) _device.Dispose();
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
        /// Check if sensor value changed since last read
        /// </summary>
        /// <returns></returns>
        public override bool HasSensorValueChanged()
        {
            if((_currentControlReg & (byte)PowerModes.ForcedMode) == (byte)PowerModes.ForcedMode)
            {
                //It is forced more, we need to setup options again...
                WriteRegister(CONTROL_REG_ADDR, _currentControlReg);
                WriteRegister(CONFIG_REG_ADDR, _currentConfigReg);
                Thread.Sleep(1000);
            }
            //now read
            float currentPressure = (float)(Math.Floor(ReadPressure() * Math.Pow(10, _scale)) / Math.Pow(10, _scale));
            float currentTemperature = (float)(Math.Floor(ReadTemperature()* Math.Pow(10, _scale)) / Math.Pow(10, _scale));

            bool valuesChanged = (TemperatureInCelcius != currentTemperature) || (PressureInPa != currentPressure);
            return valuesChanged;
        }
        
        #endregion

        /// <summary>
        /// Read the Id of the chip. For a valid BMP280 chip, this value must be 0x58
        /// </summary>
        /// <returns>byte</returns>
        public byte ReadId()
        {
            var chipId = ReadRegister(ID_REG_ADDR);            
            return chipId;
        }
        /// <summary>
        /// Check if sensor is busy measuring
        /// </summary>
        /// <returns>bool</returns>
        public bool IsMeasuring()
        {
            var statusReg = ReadRegister(STATUS_REG_ADDR);
            return ((statusReg & 0x08) == 1);
        }
        /// <summary>
        /// Check if sensor is busy updating its non-volatile memory with new values
        /// </summary>
        /// <returns>bool</returns>
        public bool IsUpdatingMemory()
        {
            var statusReg = ReadRegister(STATUS_REG_ADDR);
            return ((statusReg & 1) == 1);
        }
        /// <summary>
        /// Set the various sensor options. Remember, if you set the mode as "Forced", then you must set
        /// the sampling options everytime before you attempt to read the sensor data
        /// </summary>
        /// <param name="mode">The power mode that determines how/when to sample</param>
        /// <param name="tSample">How to sample for temperature</param>
        /// <param name="pSample">How to sample for pressure</param>
        /// <param name="fCoeff">The IIR filter coefficient</param>
        /// <param name="st">The standby time (if Normal Sampling mode is used)</param>
        public void SetOptions(Bmp280.PowerModes mode, Bmp280.Sampling tSample, Bmp280.Sampling pSample, Bmp280.Filter fCoeff, Bmp280.StandbyTime st)
        {
            //Set "Config" register for standby time and filter co-efficient option
            var configReg = ReadRegister(CONFIG_REG_ADDR);
            configReg |= (byte)(((byte)st << 5) | ((byte)fCoeff << 3));
            WriteRegister(CONFIG_REG_ADDR,configReg);
            //Set control register for mode,temperature and pressure sampling
            var ctrlReg = ReadRegister(CONFIG_REG_ADDR);
            ctrlReg |= (byte)(((byte)tSample << 5) | ((byte)pSample << 3) | (byte)(mode));
            WriteRegister(CONTROL_REG_ADDR, ctrlReg);
            //Update current configuration backup
            _currentConfigReg = ReadRegister(CONFIG_REG_ADDR);
            _currentControlReg = ReadRegister(CONTROL_REG_ADDR);
        }

        #region Helpers
        /// <summary>
        /// Every Bmp280 sensor is factory burned with a set of calibration constants that cannot be altered.
        /// To perform pressure / temperature computation, these constants are used. Read these constants and
        /// store for computation
        /// </summary>
        protected void LoadCalibrationCoefficients()
        {
            _calibration = new CalibrationData();
            //Temperature coefficients
            _calibration.DigT1 = (ushort)(ReadRegister(0x89) << 8 | ReadRegister(0x88));
            _calibration.DigT2 = (short)(ReadRegister(0x8B) << 8 | ReadRegister(0x8A));
            _calibration.DigT3 = (short)(ReadRegister(0x8D) << 8 | ReadRegister(0x8C));

            //Pressure coefficients
            _calibration.DigP1 = (ushort)(ReadRegister(0x8F) << 8 | ReadRegister(0x8E));
            _calibration.DigP2 = (short)(ReadRegister(0x91) << 8 | ReadRegister(0x90));
            _calibration.DigP3 = (short)(ReadRegister(0x93) << 8 | ReadRegister(0x92));
            _calibration.DigP4 = (short)(ReadRegister(0x95) << 8 | ReadRegister(0x94));
            _calibration.DigP5 = (short)(ReadRegister(0x97) << 8 | ReadRegister(0x96));
            _calibration.DigP6 = (short)(ReadRegister(0x99) << 8 | ReadRegister(0x98));
            _calibration.DigP7 = (short)(ReadRegister(0x9B) << 8 | ReadRegister(0x9A));
            _calibration.DigP8 = (short)(ReadRegister(0x9D) << 8 | ReadRegister(0x9C));
            _calibration.DigP9 = (short)(ReadRegister(0x9F) << 8 | ReadRegister(0x9E));
        }
        /// <summary>
        /// Read pressure value
        /// </summary>
        /// <returns>Pressure in Pa</returns>
        protected float ReadPressure()
        {
            if(t_fine == Int32.MinValue)
                ReadTemperature();//update the t_fine value

            Int64 var1, var2, p;
            
            Int32 adcP = ReadRegister(0xF7) << 16 | ReadRegister(0xF8) << 8 | ReadRegister(0xF9);
            adcP >>= 4;

            var1 = ((Int64)t_fine) - 128000;
            var2 = var1 * var1 * (Int64)_calibration.DigP6;
            var2 = var2 + ((var1 * (Int64)_calibration.DigP5) << 17);
            var2 = var2 + (((Int64)_calibration.DigP4) << 35);
            var1 = ((var1 * var1 * (Int64)_calibration.DigP3) >> 8) + ((var1 * (Int64)_calibration.DigP2) << 12);
            var1 = (((((Int64)1) << 47) + var1)) * ((Int64)_calibration.DigP1) >> 33;

            if (var1 == 0)
                return ERROR_PRESSURE;  // avoid exception caused by division by zero
            
            p = 1048576 - adcP;
            p = (((p << 31) - var2) * 3125) / var1;
            var1 = (((Int64)_calibration.DigP9) * (p >> 13) * (p >> 13)) >> 25;
            var2 = (((Int64)_calibration.DigP8) * p) >> 19;

            p = ((p + var1 + var2) >> 8) + (((Int64)_calibration.DigP7) << 4);
            return (float)p / 256;
        }
        /// <summary>
        /// Read pressure value
        /// </summary>
        /// <returns>Temperature in celcius</returns>
        protected float ReadTemperature()
        {
            Int32 var1, var2 = 0;
            Int32 adcT = ReadRegister(0xFA) << 16 | ReadRegister(0xFB) << 8 | ReadRegister(0xFC);

            adcT >>= 4;

            var1 = ((((adcT >> 3) - ((Int32)_calibration.DigT1 << 1))) * ((Int32)_calibration.DigT2)) >> 11;
            var2 = (((((adcT >> 4) - ((Int32)_calibration.DigT1)) * ((adcT >> 4) - ((Int32)_calibration.DigT1))) >> 12) * ((Int32)_calibration.DigT3)) >> 14;

            t_fine = var1 + var2;
            float T = ((t_fine * 5 + 128) >> 8)/100;
            return T;
        }
        /// <summary>
        /// Read a register and return single byte data
        /// </summary>
        /// <param name="regAddr">Address of the register to read</param>
        /// <returns>byte. On read failure, 0xFF</returns>
        protected byte ReadRegister(byte regAddr)
        {
            _wBuf[0] = regAddr;
            _rBuf[0] = 0xFF;
            var xferResult = _device.WriteReadPartial(_wBuf, _rBuf);
            if (xferResult.Status == I2cTransferStatus.FullTransfer)
                return _rBuf[0];
            else
                return 0xFF;
        }
        /// <summary>
        /// Write a register
        /// </summary>
        /// <param name="regAddr">Address of the register to write</param>
        /// <param name="data">The data to write</param>
        /// <returns>true on success else false</returns>
        protected bool WriteRegister(byte regAddr,byte data)
        {
            _wBuf[0] = regAddr;            
            var xferResult = _device.WritePartial(_wBuf);
            if (xferResult.Status == I2cTransferStatus.FullTransfer)
            {
                _wBuf[0] = data;
                xferResult = _device.WritePartial(_wBuf);
                if (xferResult.Status == I2cTransferStatus.FullTransfer) return true;
            }
            return false;
        }
        
        #endregion
    }
}
