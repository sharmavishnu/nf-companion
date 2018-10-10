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

namespace nanoFramework.Companion.Core.Utils
{
    /// <summary>
    /// Number utilities
    /// </summary>
    public class NumberUtils
    {
        /// <summary>
        /// Convert UInt16 to byte array in Little Ending format
        /// </summary>
        /// <param name="input">Input number to format</param>
        /// <returns>byte[]</returns>
        public static byte[] UInt16_To_LE(UInt16 input)
        {
            byte[] output = new byte[2] { 0,0};
            output[0] = (byte)input;
            output[1] = (byte)((input & 0xFF00) >> 8);
            return output;
        }
        /// <summary>
        /// Convert UInt16 to byte array in Big Ending format
        /// </summary>
        /// <param name="input">Input number to format</param>
        /// <returns>byte[]</returns>
        public static byte[] UInt16_To_BE(UInt16 input)
        {
            byte[] output = new byte[2] { 0, 0 };
            output[1] = (byte)input;
            output[0] = (byte)((input & 0xFF00) >> 8);
            return output;
        }

        /// <summary>
        /// Convert UInt32 to byte array in Little Ending format
        /// </summary>
        /// <param name="input">Input number to format</param>
        /// <returns>byte[]</returns>
        public static byte[] UInt32_To_LE(UInt32 input)
        {
            byte[] output = new byte[4] { 0, 0 , 0, 0 };
            output[0] = (byte)input;
            output[1] = (byte)((input & 0x0000FF00) >> 8);
            output[2] = (byte)((input & 0x00FF0000) >> 16);
            output[3] = (byte)((input & 0xFF000000) >> 24);
            return output;
        }
        /// <summary>
        /// Convert UInt32 to byte array in big Ending format
        /// </summary>
        /// <param name="input">Input number to format</param>
        /// <returns>byte[]</returns>
        public static byte[] UInt32_To_BE(UInt32 input)
        {
            byte[] output = new byte[4] { 0, 0, 0, 0 };
            output[3] = (byte)input;
            output[2] = (byte)((input & 0x0000FF00) >> 8);
            output[1] = (byte)((input & 0x00FF0000) >> 16);
            output[0] = (byte)((input & 0xFF000000) >> 24);
            return output;
        }
    }
}
