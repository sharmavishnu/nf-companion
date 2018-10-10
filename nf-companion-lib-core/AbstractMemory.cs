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
using nanoFramework.Companion.Core.Utils;
using System;
using System.Text;

namespace nanoFramework.Companion
{
    /// <summary>
    /// Abstract class that represents a memory device. All memory drivers should inherit this class and then
    /// extend
    /// </summary>
    public abstract class AbstractMemory : IDisposable
    {
        #region Implementation
        /// <summary>
        /// Keep track of object disposal
        /// </summary>
        protected bool _disposed = false;
        #endregion

        #region Properties
        /// <summary>
        /// Page size in bytes
        /// </summary>
        public uint PageSizeInBytes { get; protected set; }
        /// <summary>
        /// Memory size in bytes
        /// </summary>
        public uint MemorySizeInBytes { get; protected set; }
        #endregion

        #region Operations
        /// <summary>
        /// Read from the given address a set of bytes
        /// </summary>
        /// <param name="addr">Address to read from</param>
        /// <param name="len">Number of bytes to read</param>
        /// <returns>byte[]</returns>
        public abstract byte[] ReadBytes(uint addr, uint len);

        /// <summary>
        /// Read from the given address a string. The reading terminates until a null character is 
        /// encountered or the maximum length has been read
        /// </summary>
        /// <param name="addr">Address to read from</param>
        /// <param name="maxLen">Maximum number of bytes to read</param>
        /// <returns>string</returns>
        public abstract string ReadString(uint addr, uint maxLen);

        /// <summary>
        /// Write to the given address a set of bytes
        /// </summary>
        /// <param name="addr">Address to begin writing to</param>
        /// <param name="data">Bytes to write</param>
        /// <returns>Number of bytes written</returns>
        public abstract int WriteBytes(uint addr, byte[] data);

        /// <summary>
        /// Write to the given address a string
        /// </summary>
        /// <param name="addr">Address to begin writing to</param>
        /// <param name="data">string to write</param>
        /// <returns>Number of bytes written</returns>
        public abstract int WriteString(uint addr, string data);
        /// <summary>
        /// Erase
        /// </summary>
        /// <param name="pageStartAddr">
        /// The starting address of the page. Page address is always of multiple of page size in bytes
        /// </param>
        /// <param name="resetTo">
        /// Erase the page and set the contents of individual memory cells to this value (default is reset to
        /// factory value 0xFF)
        /// </param>
        /// <returns>The total number of memory elements erased</returns>
        public abstract int ErasePage(uint pageStartAddr = 0x00, byte resetTo = 0xFF);
        /// <summary>
        /// Erase contents of the entire memory
        /// </summary>
        /// <param name="resetTo">
        /// Erase all memory contents and replace with this byte (byte default reset to factory value 0xFF)
        /// </param>
        public abstract void Format(byte resetTo=0xFF);
        #endregion

        #region Dispose
        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                DisposeMemory();
            _disposed = true;
        }
        /// <summary>
        /// Allow inheriting class to take care of disposal
        /// </summary>
        protected abstract void DisposeMemory();
        #endregion

        #region Helper
        /// <summary>
        /// Embedded memory locations are accessed using addresses. Based on the size of the memory the address pointer could 
        /// be a single byte or multiple bytes. Also, if the address pointer is multiple bytes, we need to know which byte to send
        /// first - msb or lsb. 
        /// This method takes an input address (uint32) and converts into a byte array with needed lsb/msb arrangement
        /// </summary>
        /// <param name="addr">Address to convert</param>
        /// <param name="bytesInAddr">How many bytes needed for this address location</param>
        /// <param name="lsbFirst">If true, then least-significant-byte is in the lowest array location</param>
        /// <returns></returns>
        protected byte[] AddressAsBytes(uint addr, int bytesInAddr, bool lsbFirst)
        {
            byte[] addrBytes = null;
            if (bytesInAddr == 1 /*single byte...for smaller memories like 24C01C*/)
                return new byte[] { (byte)addr };
            else
            {
                if(bytesInAddr == 2 /*can address upto 64K...for medium size memories line 24LC256*/)
                {
                    if (lsbFirst)
                        addrBytes = NumberUtils.UInt16_To_LE((UInt16)addr);
                    else
                        addrBytes = NumberUtils.UInt16_To_BE((UInt16)addr);
                }
                else if(bytesInAddr == 4 /*Can address even higher memories TO-BE-DEFINED...*/)
                {
                    if (lsbFirst)
                        addrBytes = NumberUtils.UInt32_To_LE(addr);
                    else
                        addrBytes = NumberUtils.UInt32_To_BE(addr);
                }
            }
            return addrBytes;
        }
        #endregion
    }
}
