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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nanoFramework.Companion.Tools.OLEDFontGenerator
{
    /// <summary>
    /// A logical representation of a pixel on an OLED screen as stored in this
    /// application temporarily before generating the font file.
    /// </summary>
    public class Pixel
    {
        /// <summary>
        /// Accessor/Mutator of the index of the pixel in the pixel array
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// Accessor/Mutator to specific if the specific pixel is ON or OFF
        /// </summary>
        public bool On { get; set; }
        /// <summary>
        /// Accessor/Mutator for the pixel size 
        /// </summary>
        public Rectangle Rectangle { get; set; }
    }
}
