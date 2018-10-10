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
using System.Collections;
using System.Text;
using Windows.Devices.I2c;

namespace nanoFramework.Companion.Core.Utils
{
    /// <summary>
    /// Utility to scan the I2C bus and search for devices...This scanner simply tries to read a few bytes from an I2C device.
    /// Since many I2C devices require specific commands to read, this scanner may give a false indicator that the device is not found
    /// (since read may fail even if the device is there)
    /// </summary>
    public class I2CScanner
    {
        #region Properties
        /// <summary>
        /// List of devices found on the bus
        /// </summary>
        public Hashtable FoundDevices { get; private set; }
        #endregion

        #region Events/Delegates
        /// <summary>
        /// Method signature for handling the event when a new I2C device is found during scanning the bus
        /// </summary>
        /// <param name="addr">The device address</param>
        public delegate void OnI2CDeviceFound(byte addr);
        /// <summary>
        /// Event that is raised when a new I2C device is found on the bus
        /// </summary>
        public event OnI2CDeviceFound I2CDeviceFound;
        #endregion
        /// <summary>
        /// Scan bus for devices...Looks in the address range 9....118
        /// Uses standard speed (100KHz) and shared bus mode
        /// </summary>
        /// <param name="deviceSelector">The I2C device selector</param>
        /// <returns>Count of devices found</returns>
        public int ScanForDevices(string deviceSelector)
        {
            if (FoundDevices != null)
                FoundDevices = null;
            FoundDevices = new Hashtable();

            I2cConnectionSettings i2cConfig = new I2cConnectionSettings(0) { BusSpeed = I2cBusSpeed.StandardMode , SharingMode = I2cSharingMode.Shared };
            byte[] i2cBuffer = new byte[32];
            I2cTransferResult i2cResult;

            for (byte addr=9; addr <= 118; addr++)
            {
                i2cConfig.SlaveAddress = addr;
                I2cDevice device = I2cDevice.FromId(deviceSelector, i2cConfig);
                i2cResult = device.ReadPartial(i2cBuffer);
                if (i2cResult.Status == I2cTransferStatus.FullTransfer || i2cResult.Status == I2cTransferStatus.PartialTransfer)
                {
                    FoundDevices.Add(addr, device);
                    if (I2CDeviceFound != null) I2CDeviceFound(addr);
                }
                else
                    device.Dispose();
            }
            i2cBuffer = null;
            return FoundDevices.Count;
        }
    }
}
