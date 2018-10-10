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
using nanoFramework.Companion.Core.Utils;

namespace nanoFramework.Companion.Drivers.Memory
{
    /// <summary>
    /// The driver to read/write memory for 24LCXXX (from Microchip) and similar series EEPROMs
    /// A sample datasheet for 24LC256 can be found here https://www.microchip.com/wwwproducts/en/24LC256
    /// </summary>
    public class EEPROM_24LCXXX : AbstractMemory
    {
        #region Implementations
        
        /// <summary>
        /// Addressing requires how many bytes
        /// </summary>
        private int _addrSizeInBytes = 1;
        /// <summary>
        /// What is the addressing mode - little endian or big endian
        /// </summary>
        private bool _addrModeLE = true;
        /// <summary>
        /// The underlying I2C device 
        /// </summary>
        private I2cDevice _i2cDevice = null;
        #endregion

        #region Constructor/Destructor
        /// <summary>
        /// Default constructor. Uses standard speed mode for widest compatibility        
        /// </summary>
        /// <param name="busSelector">The I2C bus selector</param>
        /// <param name="addr">The EEPROM chip address (default for AT24LCXXX series is 0x50, with A0,A1 and A2 lines tied low)</param>
        /// <param name="pageSizeBytes">What is the size of a page in this EEPROM (in bytes)</param>
        /// <param name="memSizeBytes">What is the size of the EEPROM (in bytes)</param>
        /// <param name="addrSizeInBytes">How many bytes are required to generate the address</param>
        /// <param name="addrModeLE">
        /// Is addressing mode little-endian (when address is broken into bytes, do 
        /// we first send MSB or LSB
        /// </param>
        public EEPROM_24LCXXX(string busSelector="I2C1",byte addr=0x50, uint pageSizeBytes=64,uint memSizeBytes=32768,
                        int addrSizeInBytes=2,bool addrModeLE=false)
        {
            _i2cDevice = I2cDevice.FromId(busSelector, new I2cConnectionSettings(addr) { BusSpeed = I2cBusSpeed.StandardMode, SharingMode = I2cSharingMode.Shared });
            PageSizeInBytes = pageSizeBytes;
            MemorySizeInBytes = memSizeBytes;
            _addrSizeInBytes = addrSizeInBytes;
            _addrModeLE = addrModeLE;
        }
        /// <summary>
        /// Dispose of this object
        /// </summary>
        protected override void DisposeMemory()
        {
            if (_i2cDevice != null) _i2cDevice.Dispose();
            _i2cDevice = null;
        }
        #endregion

        #region Operations
        /// <summary>
        /// Read data from given address as bytes
        /// </summary>
        /// <param name="addr">The address to begin reading from</param>
        /// <param name="len">How many data elements to read</param>
        /// <returns>byte[] if read successful, else null</returns>
        public override byte[] ReadBytes(uint addr, uint len)
        {
            CheckRange(addr,len);

            byte[] eepromData = new byte[len];
            byte[] addrAsBytes = AddressAsBytes(addr, _addrSizeInBytes, _addrModeLE);
            I2cTransferResult xferResult = _i2cDevice.WriteReadPartial(addrAsBytes, eepromData);
  
            if (xferResult.Status != I2cTransferStatus.FullTransfer && xferResult.Status != I2cTransferStatus.PartialTransfer)
                eepromData = null;
            return eepromData;
        }
        /// <summary>
        /// Read data from given address until a null character is encountered or we reach the maximum read length
        /// </summary>
        /// <param name="addr">The address to begin reading from</param>
        /// <param name="maxLen">Maximum length of data expected</param>
        /// <returns>The string read from the memory address</returns>
        public override string ReadString(uint addr, uint maxLen)
        {
            CheckRange(addr, maxLen);
            string data = null;

            //we need to first write the address from where we want to read, and then,
            //continue to read until we get a null character or we reach the maximum length limit
            byte[] eepromData = ReadBytes(addr, maxLen);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in eepromData)
            {
                if (b == 0 /*NULL*/) break;
                sb.Append((char)b);
            }
            data = sb.ToString();
            sb.Clear();
            sb = null;

            return data;
        }
        /// <summary>
        /// Write data to EEPROM
        /// </summary>
        /// <param name="addr">The starting address</param>
        /// <param name="data">The data to write</param>
        /// <returns>The total number of bytes written. </returns>
        public override int WriteBytes(uint addr, byte[] data)
        {
            CheckRange(addr, (uint)data.Length);

            byte[] addrBytes = null;
            uint bytesToWrite = 0;
            byte[] dataAndAddr = null;
            I2cTransferResult result;
            uint totalWritten = 0;

            while(totalWritten < data.Length)
            {
                //Determine bytes to write in this pass
                bytesToWrite = PageSizeInBytes - (addr % PageSizeInBytes);
                if (bytesToWrite > (data.Length - totalWritten))
                    bytesToWrite = (uint)(data.Length - totalWritten);

                addrBytes = AddressAsBytes(addr, _addrSizeInBytes, _addrModeLE);
                dataAndAddr = new byte[_addrSizeInBytes + bytesToWrite];
                //copy address to send to I2C slave
                for (int count = 0; count < _addrSizeInBytes; count++)
                    dataAndAddr[count] = addrBytes[count];
                //Copy data contents
                Array.Copy(data,(int)totalWritten, dataAndAddr, _addrSizeInBytes, (int)bytesToWrite);

                result = _i2cDevice.WritePartial(dataAndAddr);
                totalWritten += (uint)(result.BytesTransferred - _addrSizeInBytes);
                addr += (uint)(result.BytesTransferred - _addrSizeInBytes); //move address pointer
                /*
                 * If this wait is not given, then when this method is called in a loop, sometime write fails for no reason
                 * TODO::Investigate more...
                 */
                Thread.Sleep(20);
            }

            return (int)totalWritten;           
        }

        /// <summary>
        /// Write the data to EEPROM...if string length is more than memory size
        /// then wrapping will happen
        /// </summary>
        /// <param name="addr">The starting address</param>
        /// <param name="data">The string data to write. It is assumed that string is in UTF8 encoding</param>
        /// <returns>
        /// The total number of bytes written. This will include the data byte length + 1 (additional 1 digit is the NULL character representation
        /// that gets added to the string);
        /// </returns>
        public override int WriteString(uint addr, string data)
        {
            byte[] strBuf = new byte[data.Length + 1];
            Array.Copy(UTF8Encoding.UTF8.GetBytes(data), strBuf, data.Length);//Leaves one last character for null \0
            return WriteBytes(addr, strBuf);
        }
        /// <summary>
        /// Erase the page
        /// </summary>
        /// <param name="pageStartAddr">
        /// The starting address of the page. Page address is always of multiple of page size in bytes
        /// </param>
        /// <param name="resetTo">
        /// Erase the page and set the contents of individual memory cells to this value (default is reset to
        /// factory value 0xFF)
        /// </param>
        /// <returns>The total number of memory elements erased</returns>
        public override int ErasePage(uint pageStartAddr=0x00,byte resetTo=0xFF)
        {
            if (pageStartAddr % PageSizeInBytes != 0) throw new ArgumentOutOfRangeException("pageStartAddr");

            byte[] data = new byte[PageSizeInBytes];
            ArrayUtils.FillArray(ref data, resetTo);
            return WriteBytes(pageStartAddr, data);
        }

        /// <summary>
        /// Erase everything from memory..use page write mode..
        /// By default erasing means setting all values as 0xFF
        /// </summary>
        public override void Format(byte resetTo=0xFF)        
        {
            uint totalPages = (uint)MemorySizeInBytes / PageSizeInBytes;
            for (uint addr = 0; addr < totalPages; addr++ )
            {
                Thread.Sleep(10);
                if (ErasePage(addr * PageSizeInBytes,resetTo) < PageSizeInBytes )
                {
                    Thread.Sleep(10);//allow other threads to operate
                    ErasePage(addr * PageSizeInBytes, resetTo);//once more...
                }
            }
        }

        #endregion

        #region Helpers
        /// <summary>
        /// Check if we can store the provided data within the available memory range
        /// </summary>
        /// <param name="startAddr">From which address to start storing</param>
        /// <param name="dataLen">What is the length of data (in bytes) that needs to be stored</param>
        protected void CheckRange(uint startAddr,uint dataLen)
        {
            if (startAddr + dataLen > MemorySizeInBytes)
                throw new IndexOutOfRangeException("Fix start address or data lenght. Not enough memory");
        }
        #endregion
    }
}
