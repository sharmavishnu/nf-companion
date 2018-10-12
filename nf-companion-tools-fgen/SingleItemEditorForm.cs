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
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using nanoFramework.Companion.Tools.OLEDFontGenerator.Properties;

namespace nanoFramework.Companion.Tools.OLEDFontGenerator
{
    /// <summary>
    /// Editor to allow editing a single pixel (or item)
    /// </summary>
    public partial class SingleItemEditorForm : Form
    {
        #region Implementation
        /// <summary>
        /// Track if document is dirty or not
        /// </summary>
        private bool _docDirty = false;
        /// <summary>
        /// Holds all pixel maps and representations
        /// </summary>
        private Dictionary<char,SingleCharInfoDisplay> _allPixelMaps = new Dictionary<char, SingleCharInfoDisplay>();
        /// <summary>
        /// Current file path
        /// </summary>
        private string _filePath = null;
       
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public SingleItemEditorForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Form gets loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLoad(object sender, EventArgs e)
        {
            
        }
        /// <summary>
        /// When a new Ascii code is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNewAsciiCodeSelected(object sender, EventArgs e)
        {
            PopulateFontDesignCanvas();
            UpdatePixelFontPreview();
        }
        /// <summary>
        /// User wants to create a new file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNewFile(object sender, EventArgs e)
        {
            if (_docDirty && 
                MessageBox.Show(this, "Current changes not saved. Do you want to proceed?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            FontSizeForm itemSizeDlg = new FontSizeForm();
            if(itemSizeDlg.ShowDialog(this) == DialogResult.OK)
            {
                _docDirty = false;
                Cursor = Cursors.WaitCursor;

                _asciiCodeCbx.Items.Clear();
                if(_allPixelMaps !=null) _allPixelMaps.Clear();
                _allPixelMaps = null;
                _filePath = null;
                

                Text = "Pixel Font Generator";

                //Populate the supported character codes
                PopulateAsciiCodes();
                //A new file is being created...populate the character editor                    
                _allPixelMaps = new Dictionary<char, SingleCharInfoDisplay>();

                foreach (AsciiCode ac in _asciiCodeCbx.Items)
                {
                    SingleCharInfoDisplay ciDisplay = new SingleCharInfoDisplay((byte)itemSizeDlg.PixelHeight, (byte)itemSizeDlg.PixelWidth, ac.Value);
                    ciDisplay.CharEdit += OnPixelClicked;
                    _allPixelMaps.Add(ac.Value, ciDisplay);
                }
                
                _canvasTabs.SelectTab(0);
                _asciiCodeCbx.SelectedIndex = 0;//select the first item by default
                                                
                PopulateFontDesignCanvas();

                _fontSizeLbl.Text = itemSizeDlg.PixelWidth + " (W) X " + itemSizeDlg.PixelHeight + " (H)";                
                
                Cursor = Cursors.Default;
            }
        }
                
        /// <summary>
        /// Called when the pixel is clicked on the editor
        /// </summary>
        /// <param name="source"></param>
        private void OnPixelClicked(SingleCharInfoDisplay source)
        {
            Console.WriteLine("OnPixelClicked...");
            //Show current character being edited                    
            if (source.BackColor != Color.LightGoldenrodYellow)
            {
                source.BackColor = Color.LightGoldenrodYellow;
                source.Update();
                source.Refresh();
            }
            _docDirty = true;
            UpdatePixelFontPreview();
        }
        /// <summary>
        /// Save the font file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileSave(object sender, EventArgs e)
        {
            if (_allPixelMaps == null || _allPixelMaps.Count == 0)
            {
                MessageBox.Show(this, "Nothing to save", "Pixel Font Generator", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SaveFileDialog saveFile = new SaveFileDialog()
            {
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = ".cs",
                Filter = "C# Files|*.cs",
                OverwritePrompt = true,
                RestoreDirectory = true,
                ValidateNames = true
            };

            if (string.IsNullOrWhiteSpace(_filePath))
            {
                //New file...ask for filename
                if (saveFile.ShowDialog(this) == DialogResult.OK)
                    _filePath = saveFile.FileName;
                else
                    return;
            }
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(File.Open(_filePath, FileMode.OpenOrCreate));
                string className = Path.GetFileNameWithoutExtension(_filePath);
                className = className.First().ToString().ToUpper() + className.Substring(1);

                StringBuilder csFileTemplate = new StringBuilder();
                /*Must start with /* for some reason I see ???*/
                csFileTemplate.Append(Encoding.UTF8.GetString(Resources.PixelFontClass));
                csFileTemplate.Replace("$$FONT_WIDTH_IN_PIXEL$$", _allPixelMaps['a'].FontSize.Width.ToString())
                        .Replace("$$FONT_HEIGHT_IN_PIXEL$$", _allPixelMaps['a'].FontSize.Height.ToString())
                        .Replace("$$CLASS_NAME$$", className);

                //Now build the pixel array and replace
                StringBuilder pMap = new StringBuilder();
                foreach (char c in _allPixelMaps.Keys)
                {
                    if (pMap.Length > 0) pMap.Append(",").AppendLine();
                    pMap.Append("// ").AppendLine((c == 32) ? "Space" : c + "");
                    UInt32[] vals = _allPixelMaps[c].FontPixelMapToUInt32();
                    for (int idx = 0; idx < vals.Length; idx++)
                    {
                        if (idx > 0 && idx < vals.Length) pMap.Append(",");
                        pMap.Append("0x").Append(vals[idx].ToString("X8"));
                    }
                }

                csFileTemplate.Replace("$$FONT_CHAR_MAP$$", pMap.ToString());
                //Finally, write it to file stream
                sw.WriteLine(csFileTemplate.ToString());
                sw.Flush();
                sw.Close();
                _docDirty = false;
                MessageBox.Show(this, "Font file saved successfully", "Pixel Font Generator", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Text = "Pixel Font Generator :" + _filePath;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (sw != null) sw.Dispose();
            }
        }
        /// <summary>
        /// Open a font file to edit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFileOpen(object sender, EventArgs e)
        {
            if (_docDirty)
            {
                if (MessageBox.Show(this, "You have unsaved changes. If you proceed without saving the current document, your changes will be lost. " +
                    "Do you want to proceed?", "Save Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
                else
                    _docDirty = false;
            }
            OpenFileDialog ofDlg = new OpenFileDialog()
            {
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = ".cs",
                Filter = "C# Files|*.cs",
                Multiselect = false,
                RestoreDirectory = true,
                ShowReadOnly = true
            };
            if (ofDlg.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    StreamReader sr = File.OpenText(ofDlg.FileName);
                    String contents = sr.ReadToEnd();
                    sr.Close();

                    string firstId = "//7ede8a4c-223c-4389-a683-cb2c5de448d5";
                    string secondId = "//f3b635d2-e762-4110-b202-488f5b6ca5df";
                    //Validate file
                    if (contents.IndexOf(firstId) < 0 || contents.IndexOf(secondId) < 0)
                        MessageBox.Show(this, "Invalid font mapping file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                    {
                        Cursor = Cursors.WaitCursor;
                        string temp = "";
                        //First extract pixel map
                        string pixelMap = "";
                        temp = contents.Substring(contents.IndexOf(secondId) + secondId.Length);
                        temp = temp.Substring(0, temp.IndexOf(secondId));

                        pixelMap = temp.Replace("\t", "").Trim();

                        //Now extract size
                        contents = contents.Replace("\r\n", "");
                        temp = contents.Substring(contents.IndexOf(firstId) + firstId.Length + 2);
                        string wh = temp.Substring(0, temp.IndexOf("//"));
                        int fontWidth = int.Parse(wh.Split(new char[] { ';' })[0]);
                        int fontHeight = int.Parse(wh.Split(new char[] { ';' })[1]);

                        string[] lines = pixelMap.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                        _allPixelMaps = new Dictionary<char, SingleCharInfoDisplay>();
                        char c = (char)32;
                        foreach (string line in lines)
                        {
                            if (line.Trim().StartsWith("//"))
                            {
                                Console.WriteLine("Reading..." + line);
                                //extract the char
                                if (line.Trim().StartsWith("// Space"))
                                    c = (char)32;
                                else
                                {
                                    c = line.Trim().Substring(3)[0];
                                }
                            }
                            else
                            {
                                List<UInt32> charPixels = new List<UInt32>();
                                string[] pixelVals = line.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                foreach (string entry in pixelVals)
                                    charPixels.Add(Convert.ToUInt32(entry.Trim(), 16));
                                SingleCharInfoDisplay ciDisplay = new SingleCharInfoDisplay((byte)fontHeight, (byte)fontWidth, c);
                                ciDisplay.SetPixelMap(charPixels.ToArray());
                                ciDisplay.CharEdit += OnPixelClicked;
                                _allPixelMaps.Add(c, ciDisplay);
                            }
                        }
                        //Now setup window state
                        PopulateAsciiCodes();
                        _asciiCodeCbx.SelectedIndex = 0;
                        _fontSizeLbl.Text = fontWidth + " (W) X " + fontHeight + " (H)";
                        Text = "Pixel Font Generator :" + ofDlg.FileName;
                        _filePath = ofDlg.FileName;
                        //Now redraw
                        PopulateFontDesignCanvas();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Seems like an invalid file !\r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }
            }
        }
        /// <summary>
        /// Show about box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAbout(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.Show(this);
        }
        /// <summary>
        /// Application exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnExit(object sender, EventArgs e)
        {
            if (_docDirty &&
                MessageBox.Show(this, "Current changes not saved. Do you want to proceed?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            Close();
        }
        /// <summary>
        /// Paint all the pixel drawing containers
        /// </summary>
        private void PopulateFontDesignCanvas()
        {
            _fontDesignPage.Controls.Clear();

            char c = ((AsciiCode)_asciiCodeCbx.Items[_asciiCodeCbx.SelectedIndex]).Value;
            if (!_allPixelMaps.ContainsKey(c)) return; //not yet populated
            SingleCharInfoDisplay ciDisplay = _allPixelMaps[c];

            /*
            * Each UInt32 entry in the array, represents a single row of pixels (max 32 pixels).
            * If height in pixel is 7 pixels, then we need to read 7 rows.
            * If width in pixel is 5 pixels, then for each entry, we need to check for bit '1' from left to right
            */
            ciDisplay.Left = (_fontDesignPage.Left + _fontDesignPage.Width - ciDisplay.Width) / 2;
            ciDisplay.Top = (_fontDesignPage.Top + _fontDesignPage.Height - ciDisplay.Height) / 2;

            if (_previewPanel.BackgroundImage != null)
            {
                _previewPanel.BackgroundImage.Dispose();
                _previewPanel.BackgroundImage = null;
            }
            SuspendLayout();
            _fontDesignPage.Controls.Add(ciDisplay);
            ResumeLayout();
            Update();
            Refresh();
        }
        
        /// <summary>
        /// Update the preview for the pixel font
        /// </summary>
        private void UpdatePixelFontPreview()
        {
            Console.WriteLine("UpdatePreview called...");            
            SingleCharInfoDisplay ciDisplay = null;
            foreach(Control c in _fontDesignPage.Controls)
            {
                if (c.GetType() == typeof(SingleCharInfoDisplay))
                {
                    ciDisplay = (SingleCharInfoDisplay)c;
                    break;
                }
            }
            if(ciDisplay != null)
            {                
                Bitmap bmp = null;
                if(_previewPanel.BackgroundImage != null)
                {
                    _previewPanel.BackgroundImage.Dispose();
                    _previewPanel.BackgroundImage = null;
                }
                //refresh everytime...//TODO::improve
                bmp = new Bitmap(ciDisplay.FontSize.Width + 1, ciDisplay.FontSize.Height + 1);
                _previewPanel.BackgroundImage = (Image)bmp;
                _previewPanel.BackgroundImageLayout = ImageLayout.Center;

                int xpos = 0;
                int ypos = 0;
                
                //Draw dots/pixels                
                for(int idx=0;idx < ciDisplay.FontPixelsMap.Length; idx++)
                {
                    if (idx % ciDisplay.FontSize.Width == 0)
                        xpos = 1;
                    else
                        xpos++;

                    ypos = (int)(idx / ciDisplay.FontSize.Width) + 1;
                    
                    if (ciDisplay.FontPixelsMap[idx])
                        bmp.SetPixel(xpos , ypos, Color.Aqua);
                    else
                        bmp.SetPixel(xpos, ypos, Color.Black);
                }
                _previewPanel.BackgroundImage = bmp;
                _previewPanel.Update();
                _previewPanel.Refresh();
            }
        }
        /// <summary>
        /// Populate Ascii codes supported by this editors
        /// </summary>
        private void PopulateAsciiCodes()
        {
            _asciiCodeCbx.Items.Clear();
            //Populate the supported character codes
            for (int i = char.MinValue; i < 256; i++)
            {
                char c = Convert.ToChar(i);
                if (!Char.IsControl(c) && c <= '~')
                    _asciiCodeCbx.Items.Add(new AsciiCode() { Value = c, Text = c.ToString() + "  [" + i.ToString() + "]" });
                else if (c > '~')
                    break;
            }
        }
    }

    public class AsciiCode
    {
        public string Text { get; set; }
        public char Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
