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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nanoFramework.Companion.Tools.OLEDFontGenerator
{
    public partial class SingleCharInfoDisplay : UserControl
    {
        #region Constants
        /// <summary>
        /// Width & height of the drawing pixel
        /// </summary>
        private static readonly int DRAW_PIXEL_WH = 10;
        /// <summary>
        /// Gutter size between pixel representations
        /// </summary>
        private static readonly int GUTTER_PIXELS = 2;
        #endregion

        #region Implementation
        /// <summary>
        /// Font width in pixels
        /// </summary>
        private byte _fontWidthInPixels = 8;
        /// <summary>
        /// Font height in pixels
        /// </summary>
        private byte _fontHeightInPixels = 8;
        /// <summary>
        /// List of rendered pixels
        /// </summary>
        private List<Pixel> _renderedPixels = null;
        
        #endregion

        #region Properties
        /// <summary>
        /// The ASCII character code (from 32-126, is printable characters. From 127-158, another 32 places, is custom characters)
        /// </summary>
        public char CharCode { get; set; }
                
        /// <summary>
        /// The font pixel map
        /// </summary>
        public bool[] FontPixelsMap { get; set; }
        /// <summary>
        /// The font size in pixels
        /// </summary>
        public Size FontSize { get; private set; }
        #endregion

        #region events/delegates
        /// <summary>
        /// Delegate that defines the event that is raised when a character is edited
        /// </summary>
        /// <param name="source">SingleCharInfoDisplay</param>
        public delegate void OnCharEdit(SingleCharInfoDisplay source);
        /// <summary>
        /// Event that is raised when a character is edited
        /// </summary>
        public event OnCharEdit CharEdit;
        #endregion

        #region Constructor/Destructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fontHeight">Font height in pixel</param>
        /// <param name="fontWidth">Font width in pixel</param>
        /// <param name="charCode">Char code</param>
        public SingleCharInfoDisplay(byte fontHeight,byte fontWidth , char charCode)
        {
            if (fontHeight < 5 || fontHeight > 25 || fontWidth < 5 || fontWidth > 25) throw new ArgumentOutOfRangeException("fontHeight,fontWidth");

            InitializeComponent();
            this.DoubleBuffered = true;

            _fontHeightInPixels = fontHeight;
            _fontWidthInPixels = fontWidth;
            CharCode = charCode;

            int drawWidth = (DRAW_PIXEL_WH + GUTTER_PIXELS) * fontWidth;
            int drawHeight = (DRAW_PIXEL_WH + GUTTER_PIXELS) * fontHeight;

            //we need to adjust the size of the overall control based on size needed
            Width = drawWidth + 20; //Gutter of 10 pixel on each side
            Height = drawHeight + 30 ; //Account for the top label and also for the gutter    

            FontPixelsMap = new bool[_fontHeightInPixels * _fontWidthInPixels];
            FontSize = new Size(fontWidth, fontHeight);
        }
        
        #endregion

        #region Operations
        /// <summary>
        /// Set the pixel map
        /// </summary>
        /// <param name="pixelMap">The pixel map</param>
        public void SetPixelMap(UInt32[] pixelMap)
        {
            Array.Clear(FontPixelsMap, 0, FontPixelsMap.Length);

            int mapPos = 0;
            //now populate pixel map
            for (int idx = 0; idx < pixelMap.Length; idx++)
            {
                UInt32 row = pixelMap[idx];
                UInt32 cmp = (UInt32)0x80000000;
                UInt32 temp = 0;
                for (int count = 0; count < _fontWidthInPixels; count++)
                {                    
                    temp = row & cmp;
                    FontPixelsMap[mapPos++] = (temp == cmp);
                    cmp = (UInt32)(cmp >> 1);
                }
            }
            _renderedPixels = null;
        }
        /// <summary>
        /// Clear the drawing canvas
        /// </summary>
        public void ClearCanvas()
        {
            this.Controls.Clear();
            this.Update();
            this.Refresh();
        }
        #endregion

        #region Event handlers
        /// <summary>
        /// Paint the control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPaint(object sender, PaintEventArgs e)
        {            
            if (CharCode == 32)
                _charLbl.Text = "Space";
            else if (CharCode == 38)
                _charLbl.Text = "&&";
            else if (CharCode > 32 && CharCode <= 126)
                _charLbl.Text = (char)CharCode + "";
            else
                _charLbl.Text = "Custom";            

            bool firstTime = (_renderedPixels == null);
            int startX = 10;
            int startY = 20;
            int drawWidth = (DRAW_PIXEL_WH + GUTTER_PIXELS) * _fontWidthInPixels;
            int drawHeight = (DRAW_PIXEL_WH + GUTTER_PIXELS) * _fontHeightInPixels;

            if (_renderedPixels == null) _renderedPixels = new List<Pixel>();

            //Adjust the char code display
            _charLbl.Left =  Width/2 - _charLbl.Width/2;
            _charLbl.Top = 2;

            int pixelIndex = 0;
            for (int yPos = startY; yPos < drawHeight + 20; yPos += DRAW_PIXEL_WH + GUTTER_PIXELS)
            {
                for (int xPos = startX; xPos < drawWidth; xPos += DRAW_PIXEL_WH + GUTTER_PIXELS)
                {                    
                    if (firstTime) //paint all pink...off
                    {
                        Rectangle rc = new Rectangle() { X = xPos, Y = yPos, Height = DRAW_PIXEL_WH, Width = DRAW_PIXEL_WH };
                        _renderedPixels.Add(new Pixel() { Rectangle = rc, Index = pixelIndex, On = FontPixelsMap[pixelIndex] });
                    }
                    //paint based on on or off
                    if (_renderedPixels[pixelIndex].On)
                        e.Graphics.FillRectangle(Brushes.Black, _renderedPixels[pixelIndex].Rectangle);
                    else
                        e.Graphics.FillRectangle(Brushes.LightPink, _renderedPixels[pixelIndex].Rectangle);

                    pixelIndex++;
                }                
            }            
        }
        #endregion

        /// <summary>
        /// Control clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClick(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            ChangePixelState(me.X, me.Y);                        
        }
        
        /// <summary>
        /// Change the state of pixel from on to off or vice cersa
        /// </summary>
        /// <param name="x">A point which is encircled by a pixel</param>
        private void ChangePixelState(int x,int y)
        {
            if (_renderedPixels == null) return;
            Pixel pixel = _renderedPixels.AsQueryable().Where(p => p.Rectangle.X <= x && x <= (p.Rectangle.X + p.Rectangle.Width)).Where(p => p.Rectangle.Y <= y && y <= (p.Rectangle.Y + p.Rectangle.Height)).FirstOrDefault();
            if (pixel != null && pixel.Rectangle.Width > 0)
            {
                pixel.On = !pixel.On;
                Update();
                Refresh();
                //Update pixel map
                FontPixelsMap[pixel.Index] = (pixel.On) ? true : false;
                if (CharEdit != null) CharEdit(this);
            }
        }        
        /// <summary>
        /// Convert the font pixel map to UInt32 array
        /// </summary>
        /// <returns>UInt32[]</returns>
        public UInt32[] FontPixelMapToUInt32()
        {
            UInt32[] map = new UInt32[_fontHeightInPixels];
            int idx = 0;
            //now populate pixel map
            for (int row = 0; row < _fontHeightInPixels; row++)
            {
                map[row] = 0;//initialize
                for(int col=0;col < _fontWidthInPixels; col++)
                {
                    if(FontPixelsMap[idx++])
                    {
                        map[row] |= (0x80000000 >> col);
                    }
                }
            }
            return map;
        }
    }
}
