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
    /// This class implements a I2C shared bus. Using this class, multiple I2C devices can be 
    /// interfaced on a single I2C bus.
    /// </summary>
    public class I2CSharedBus : IDisposable
    {
        #region Constants
        /// <summary>
        /// While in theory, upto 127 devices can be supported by an I2C bus, we have limited resources. 
        /// Therefore, limit the number of devices
        /// </summary>
        private byte MAX_DEVICES_ON_BUS = 32;
        #endregion

        #region Implementation
        /// <summary>
        /// Instance of itself for singleton implementation
        /// </summary>
        private static I2CSharedBus _self = null;
        /// <summary>
        /// Holds all I2C devices currently on the bus
        /// </summary>
        private Hashtable _i2CDevices = null;
        /// <summary>
        /// The I2C device selector
        /// </summary>
        private string _deviceSelector = null;
        #endregion

        #region Constructor/Destructor
        /// <summary>
        /// Private constructor for singleton
        /// </summary>
        private I2CSharedBus()
        {
            _i2CDevices = new Hashtable();
        }
        /// <summary>
        /// Dispose this object
        /// </summary>
        public void Dispose()
        {
            foreach(I2cDevice device in _i2CDevices)
            {
                try { device.Dispose(); } finally {; ; }
            }
            _i2CDevices = null;
            _self = null;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Create the instance of the shared bus for the given device selector
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static I2CSharedBus CreateInstance(string selector)
        {
            lock(typeof(I2CSharedBus))
            {
                if (selector == null || selector.Trim().Length == 0) throw new ArgumentException("selector is null");
                if (I2cDevice.GetDeviceSelector().ToUpper().IndexOf(selector.ToUpper()) < 0) throw new ArgumentException("Invalid device selector");

                if (_self != null) _self.Dispose();
                _self = new I2CSharedBus();
                _self.SetDeviceSelector(selector);
            }
            
            return _self;
        }

        /// <summary>
        /// Set the device selector
        /// </summary>
        /// <param name="selector">I2C device selector</param>
        private void SetDeviceSelector(string selector)
        {
            _deviceSelector = selector;
        }
        /// <summary>
        /// Check if the device, identified by the address exists
        /// </summary>
        /// <param name="addr">Address of device to check</param>
        /// <returns>true if device is added to the bus else false</returns>
        public bool DeviceExists(int addr)
        {
            return (_i2CDevices.Contains(addr));
        }
        /// <summary>
        /// Find the I2C device by the address
        /// </summary>
        /// <param name="addr">Address of the I2C device to search for</param>
        /// <returns>I2cDevice instance if found, else null</returns>
        public I2cDevice FindDeviceByAddress(int addr)
        {
            I2cDevice foundDevice = null;
            if (DeviceExists(addr))
                foundDevice = (I2cDevice)_i2CDevices[addr];
            return foundDevice;
        }
        /// <summary>
        /// Remove the given device 
        /// </summary>
        /// <param name="addr">Address of the device to remove</param>
        /// <returns>true if device found and removed else false</returns>
        public bool RemoveDeviceByAddress(int addr)
        {
            I2cDevice foundDevice = null;
            if (DeviceExists(addr))
            {
                foundDevice = (I2cDevice) _i2CDevices[addr];
                foundDevice.Dispose();
                _i2CDevices.Remove(addr);
            }
            return (foundDevice != null);
        }
        /// <summary>
        /// Add a new device to the I2C bus...
        /// </summary>
        /// <param name="addr">The device address</param>
        /// <param name="speed">The clock speed in hertz</param>
        public void AddI2CDevice(int addr, I2cBusSpeed speed)
        {
            if (_i2CDevices.Count >= MAX_DEVICES_ON_BUS) throw new IndexOutOfRangeException("Cannot add more devices on the shared I2C bus. Limit exhausted");
            if (DeviceExists(addr)) throw new Exception("Device exists");
            
            I2cDevice device = I2cDevice.FromId(_deviceSelector, new I2cConnectionSettings(addr) { BusSpeed = speed , SharingMode= I2cSharingMode.Shared });
            _i2CDevices.Add(addr, device);
        }
        /// <summary>
        /// Scan for devices on the I2C bus and populate the internal list. By default,
        /// the standard speed (100KHz) and shared bus mode is used
        /// </summary>
        /// <param name="startAddr">The I2C address to start scanning</param>
        /// <param name="endAddr">The I2C address to end scanning</param>
        /// <returns>Total devices found on the bus</returns>
        public int ScanDevicesOnBus(int startAddr, int endAddr)
        {
            if (startAddr < 0 || startAddr > 127 || startAddr > endAddr || endAddr > 127)
                throw new ArgumentException("Invalid I2C address range");

            byte[] tempBuffer = new byte[8]; //why 8??
            I2cDevice device = null;
            I2cTransferStatus xferStatus = I2cTransferStatus.FullTransfer;

            for(int addr=startAddr; addr <= endAddr; addr++)
            {
                device = I2cDevice.FromId(_deviceSelector, new I2cConnectionSettings(addr) { BusSpeed = I2cBusSpeed.StandardMode, SharingMode = I2cSharingMode.Shared });
                xferStatus = device.ReadPartial(tempBuffer).Status;
                if(xferStatus ==  I2cTransferStatus.FullTransfer || xferStatus == I2cTransferStatus.PartialTransfer)
                {
                    //we have a device...
                    _i2CDevices.Add(addr, device);
                }
            }
            return _i2CDevices.Count;
        }
        #endregion
    }
}
