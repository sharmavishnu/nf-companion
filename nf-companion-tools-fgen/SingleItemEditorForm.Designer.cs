namespace nanoFramework.Companion.Tools.OLEDFontGenerator
{
    partial class SingleItemEditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SingleItemEditorForm));
            this._formToolStrip = new System.Windows.Forms.ToolStrip();
            this._newFileBtn = new System.Windows.Forms.ToolStripButton();
            this._openFileBtn = new System.Windows.Forms.ToolStripButton();
            this._saveFileBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._aboutBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this._exitBtn = new System.Windows.Forms.ToolStripButton();
            this.label1 = new System.Windows.Forms.Label();
            this._asciiCodeCbx = new System.Windows.Forms.ComboBox();
            this._canvasTabs = new System.Windows.Forms.TabControl();
            this._fontDesignPage = new System.Windows.Forms.TabPage();
            this._previewPanel = new System.Windows.Forms.Panel();
            this._fontSizeLbl = new System.Windows.Forms.Label();
            this._formToolStrip.SuspendLayout();
            this._canvasTabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // _formToolStrip
            // 
            this._formToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._newFileBtn,
            this._openFileBtn,
            this._saveFileBtn,
            this.toolStripSeparator1,
            this._aboutBtn,
            this.toolStripSeparator2,
            this.toolStripButton1,
            this.toolStripSeparator3,
            this._exitBtn});
            this._formToolStrip.Location = new System.Drawing.Point(0, 0);
            this._formToolStrip.Name = "_formToolStrip";
            this._formToolStrip.Size = new System.Drawing.Size(331, 25);
            this._formToolStrip.TabIndex = 1;
            // 
            // _newFileBtn
            // 
            this._newFileBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._newFileBtn.Image = global::nanoFramework.Companion.Tools.OLEDFontGenerator.Properties.Resources.file_16;
            this._newFileBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._newFileBtn.Name = "_newFileBtn";
            this._newFileBtn.Size = new System.Drawing.Size(23, 22);
            this._newFileBtn.Text = "New File";
            this._newFileBtn.Click += new System.EventHandler(this.OnNewFile);
            // 
            // _openFileBtn
            // 
            this._openFileBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._openFileBtn.Image = global::nanoFramework.Companion.Tools.OLEDFontGenerator.Properties.Resources.open_16;
            this._openFileBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._openFileBtn.Name = "_openFileBtn";
            this._openFileBtn.Size = new System.Drawing.Size(23, 22);
            this._openFileBtn.Text = "Open File";
            this._openFileBtn.Click += new System.EventHandler(this.OnFileOpen);
            // 
            // _saveFileBtn
            // 
            this._saveFileBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._saveFileBtn.Image = global::nanoFramework.Companion.Tools.OLEDFontGenerator.Properties.Resources.save_16;
            this._saveFileBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._saveFileBtn.Name = "_saveFileBtn";
            this._saveFileBtn.Size = new System.Drawing.Size(23, 22);
            this._saveFileBtn.Text = "Save File";
            this._saveFileBtn.Click += new System.EventHandler(this.OnFileSave);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // _aboutBtn
            // 
            this._aboutBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._aboutBtn.Image = global::nanoFramework.Companion.Tools.OLEDFontGenerator.Properties.Resources.user_manual_16;
            this._aboutBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._aboutBtn.Name = "_aboutBtn";
            this._aboutBtn.Size = new System.Drawing.Size(23, 22);
            this._aboutBtn.Text = "Help/About";
            this._aboutBtn.Click += new System.EventHandler(this.OnAbout);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::nanoFramework.Companion.Tools.OLEDFontGenerator.Properties.Resources.export_16;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "Export as image";
            this.toolStripButton1.Click += new System.EventHandler(this.OnExportToImage);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // _exitBtn
            // 
            this._exitBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._exitBtn.Image = global::nanoFramework.Companion.Tools.OLEDFontGenerator.Properties.Resources.exit_16;
            this._exitBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._exitBtn.Name = "_exitBtn";
            this._exitBtn.Size = new System.Drawing.Size(23, 22);
            this._exitBtn.Text = "Exit";
            this._exitBtn.Click += new System.EventHandler(this.OnExit);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "ASCII Code:";
            // 
            // _asciiCodeCbx
            // 
            this._asciiCodeCbx.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._asciiCodeCbx.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._asciiCodeCbx.FormattingEnabled = true;
            this._asciiCodeCbx.Location = new System.Drawing.Point(84, 31);
            this._asciiCodeCbx.Name = "_asciiCodeCbx";
            this._asciiCodeCbx.Size = new System.Drawing.Size(69, 24);
            this._asciiCodeCbx.TabIndex = 3;
            this._asciiCodeCbx.SelectedIndexChanged += new System.EventHandler(this.OnNewAsciiCodeSelected);
            // 
            // _canvasTabs
            // 
            this._canvasTabs.Controls.Add(this._fontDesignPage);
            this._canvasTabs.Location = new System.Drawing.Point(4, 58);
            this._canvasTabs.Name = "_canvasTabs";
            this._canvasTabs.SelectedIndex = 0;
            this._canvasTabs.Size = new System.Drawing.Size(324, 369);
            this._canvasTabs.TabIndex = 5;
            // 
            // _fontDesignPage
            // 
            this._fontDesignPage.Location = new System.Drawing.Point(4, 22);
            this._fontDesignPage.Name = "_fontDesignPage";
            this._fontDesignPage.Padding = new System.Windows.Forms.Padding(3);
            this._fontDesignPage.Size = new System.Drawing.Size(316, 343);
            this._fontDesignPage.TabIndex = 0;
            this._fontDesignPage.Text = "Font Design";
            this._fontDesignPage.UseVisualStyleBackColor = true;
            // 
            // _previewPanel
            // 
            this._previewPanel.BackColor = System.Drawing.Color.Black;
            this._previewPanel.ForeColor = System.Drawing.Color.Cyan;
            this._previewPanel.Location = new System.Drawing.Point(276, 25);
            this._previewPanel.Name = "_previewPanel";
            this._previewPanel.Size = new System.Drawing.Size(48, 48);
            this._previewPanel.TabIndex = 6;
            // 
            // _fontSizeLbl
            // 
            this._fontSizeLbl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._fontSizeLbl.Location = new System.Drawing.Point(160, 32);
            this._fontSizeLbl.Name = "_fontSizeLbl";
            this._fontSizeLbl.Size = new System.Drawing.Size(100, 23);
            this._fontSizeLbl.TabIndex = 7;
            this._fontSizeLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SingleItemEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 433);
            this.Controls.Add(this._fontSizeLbl);
            this.Controls.Add(this._previewPanel);
            this.Controls.Add(this._canvasTabs);
            this.Controls.Add(this._asciiCodeCbx);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._formToolStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SingleItemEditorForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Pixel Font Generator";
            this.Load += new System.EventHandler(this.OnLoad);
            this._formToolStrip.ResumeLayout(false);
            this._formToolStrip.PerformLayout();
            this._canvasTabs.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip _formToolStrip;
        private System.Windows.Forms.ToolStripButton _newFileBtn;
        private System.Windows.Forms.ToolStripButton _openFileBtn;
        private System.Windows.Forms.ToolStripButton _saveFileBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton _aboutBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton _exitBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox _asciiCodeCbx;
        private System.Windows.Forms.TabControl _canvasTabs;
        private System.Windows.Forms.TabPage _fontDesignPage;
        private System.Windows.Forms.Panel _previewPanel;
        private System.Windows.Forms.Label _fontSizeLbl;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}