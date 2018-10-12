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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nanoFramework.Companion.Tools.OLEDFontGenerator
{
    /// <summary>
    /// Form to allow user to select/specify the font size (height and width)
    /// </summary>
    public partial class FontSizeForm : Form
    {
        public int PixelWidth { get; set; }
        public int PixelHeight { get; set; }

        public FontSizeForm()
        {
            InitializeComponent();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            if (PixelWidth <= 5) PixelWidth = 5;
            if (PixelHeight <= 7) PixelHeight = 7;

            _fontHeightPixels.Value = PixelHeight;
            _fontWidthPixels.Value = PixelWidth;

            OnHeightChange(sender, e);
            OnWidthChange(sender, e);
        }

        private void OnHeightChange(object sender, EventArgs e)
        {
            _fontHeightPixelsLbl.Text = _fontHeightPixels.Value.ToString();
            PixelHeight = _fontHeightPixels.Value;
        }

        private void OnWidthChange(object sender, EventArgs e)
        {
            _fontWidthPixelsLbl.Text = _fontWidthPixels.Value.ToString();
            PixelWidth = _fontWidthPixels.Value;
        }

        private void OnOK(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
