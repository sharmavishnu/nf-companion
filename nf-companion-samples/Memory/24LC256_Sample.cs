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
using nanoFramework.Companion.Drivers.Memory;

namespace nanoFramework.Companion.Samples.Memory
{
    /// <summary>
    /// This sample demonstrates how to use the 24LCXXX driver to read/write a 24LC256 EEPROM from Microchip.
    /// The 24LC256 is a 256Kbit EEPROM. It is organized as 32K x 8 bit memory (meaning, 32Kbytes of memory)
    /// The memory is arranged into 512 pages with each page having 64 bytes of storage capacity.
    /// For more information, please read the datasheet at <see cref="https://www.microchip.com/wwwproducts/en/24LC256"/>
    /// </summary>
    public class EEPROM_24LC256_Sample : ISample
    {
        /// <summary>
        /// Run the sample...tested on an STM32F401RE nucleo board
        /// </summary>
        public void RunSample()
        {
            /**************************************************
             * Constructing the driver class
             **************************************************/ 
            EEPROM_24LCXXX eeprom = new EEPROM_24LCXXX("I2C1", /*STM32F401RE board is using I2C1 bus*/
                                                    0x50, /*Address of this EEPROM with A0,A1 and A2 connected to ground*/
                                                    64 /*The page size in bytes*/,
                                                    32768,/*The memory size in bytes*/
                                                    2, /*The address size in bytes..word or byte address*/
                                                    false /*Address is sent MSB first*/);
            /******************************************************
             * Format the entire memory
             * This is a time consuming action and for
             * 32KB could take upto 20 seconds...
             ******************************************************/
            eeprom.Format(); //We are setting all contents of the EEPROM to 0xFF, this is what is there in the chip when it is shipped to you

            uint addr00 = 0x00; //0th byte memory location
            uint addr08 = 0x08; //8th byte memory location
            uint addr4090 = 0x0FFA; //4090th byte memory location...to test for page spillover
            string testStringSmall = "This is a 24-byte string";
            string testStringLarge = "This input string is treated as UTB-8 input and bytes are extracted. This contains 92-bytes.";

            //Write 8 bytes to memory address 0...writing less than page size
            eeprom.WriteBytes(addr00 , new byte[] { 1,2,3,4,5,6,7,8 });
            //Write 16 bytes to memory address 8...writing less than page size
            eeprom.WriteBytes(addr08, new byte[] { 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16 });
            
            //Read the bytes
            byte[] addr00Bytes = eeprom.ReadBytes(addr00, 8);
            byte[] addr08Bytes = eeprom.ReadBytes(addr08, 16);
            
            //Erase 1st page
            eeprom.ErasePage(addr00);
            //Erase 2nd page
            eeprom.ErasePage(addr00 + eeprom.PageSizeInBytes /*Next page starting memory location*/);
            //Write a string that fits into a page size...then read
            eeprom.WriteString(addr00, testStringSmall);
            string aShortStr = eeprom.ReadString(addr00, eeprom.PageSizeInBytes);
            eeprom.ErasePage(addr00);
            //Write a string that spans over page boundary...then read...
            eeprom.WriteString(addr00, testStringLarge);
            string aLongStr = eeprom.ReadString(addr00, 100/*A number larger than page size*/);
            //Revert back the EEPROM to factory reset state.
            eeprom.ErasePage(addr00);
            eeprom.ErasePage(addr00 + eeprom.PageSizeInBytes);
            //Write really long byte array...larger than one page size
            byte[] longByteArray = new byte[] { 0,1,2,3,4,5,6,7,8,9,
                                                10,11,12,13,14,15,16,17,18,19,
                                                20,21,22,23,24,25,26,27,28,29,
                                                30,31,32,33,34,35,36,37,38,39,
                                                40,41,42,43,44,45,46,47,48,49,
                                                50,51,52,53,54,55,56,57,58,59,
                                                60,61,62,63,64,65,66,67,68,69,
                                                70,71,72,73,74,75,76,77,78,79};
            eeprom.WriteBytes(addr4090, longByteArray);//This should write first 6 bytes in one page, then other 64-bytes in next page and last 10 bytes in next page.
            byte[] addr4090Bytes = eeprom.ReadBytes(addr4090, (uint)longByteArray.Length);//This will read all bytes in one-shot across pages
            //now read the bytes page by page
            byte[] bytesOnPage58 = eeprom.ReadBytes(addr4090, 6); //this should return a byte array with values {0,1,2,3,4,5}
            byte[] bytesOnPage59 = eeprom.ReadBytes(addr4090 + 6, eeprom.PageSizeInBytes);//should return a byte array with values {6....69}
            byte[] bytesOnPage60 = eeprom.ReadBytes(addr4090 + 6 + eeprom.PageSizeInBytes, 10);//should return a byte array with values {70...79}

            eeprom.Format();//Reset to factory state...

            eeprom.Dispose();
        }
    }
}
