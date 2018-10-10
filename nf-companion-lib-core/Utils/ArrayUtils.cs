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

namespace nanoFramework.Companion.Core.Utils
{
    /// <summary>
    /// Contains various utility functions that can be performed on an array
    /// </summary>
    public class ArrayUtils
    {
        /// <summary>
        /// Fill the array with the specified value
        /// </summary>
        /// <param name="input">The array to fill (as a reference)</param>
        /// <param name="val">The value to fill the array with</param>
        public static void FillArray(ref byte[] input,byte val)
        {
            for (int idx = 0; idx < input.Length; idx++)
                input[idx] = val;
        }
        /// <summary>
        /// Check if two byte arrays are equal. In this case, the byte arrays will be equal, if and only if,
        /// both arrays have same length and each item in corresponding index is also equal. 
        /// If both arrays are null, then they are treated as equal
        /// </summary>
        /// <param name="first">First byte array</param>
        /// <param name="second">Second byte array</param>
        /// <returns>true if both byte arrays are equal else false</returns>
        public static bool AreByteArrayEqual(byte[] first,byte[] second)
        {
            if (first == null && second == null) return true;
            if ((first == null && second != null) || (first != null && second == null)) return false;
            if (first.Length != second.Length) return false;

            for (int index = 0; index < first.Length; index++)
                if (first[index] != second[index]) return false;
            return true;
        }
    }
}
