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

namespace nanoFramework.Companion.Drivers.Display
{
    /// <summary>
    /// The PixelFont interface. A font is created using a collection of pixels. Every font is an array of
    /// UInt32 numbers. So a 5x7 font, means, width is 5 pixels and height is 7 pixels. The array will contain, 7 UInt32 numbers.
    /// Each bit represents a pixel and reading left-2-right, if bit is 1, then pixel is switched on else pixel is switched off. 
    /// 
    /// One can design their own font using the provided 'Font Designer Tool'. The final output from the tool is a *.cs file that contains
    /// the required definitions. It's a class that implements this interface.
    /// </summary>
    public interface IPixelFont
    {
        /// <summary>
        /// Accessor for font height in pixels
        /// </summary>
        byte Height { get; }
        /// <summary>
        /// Accessor for font width in pixels
        /// </summary>
        byte Width { get; }
        /// <summary>
        /// Accessor for the character map
        /// </summary>
        UInt32[] CharMap { get; }
    }
}
