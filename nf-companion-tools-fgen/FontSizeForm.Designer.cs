namespace nanoFramework.Companion.Tools.OLEDFontGenerator
{
    partial class FontSizeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FontSizeForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._fontHeightPixels = new System.Windows.Forms.TrackBar();
            this._fontHeightPixelsLbl = new System.Windows.Forms.Label();
            this._fontWidthPixelsLbl = new System.Windows.Forms.Label();
            this._fontWidthPixels = new System.Windows.Forms.TrackBar();
            this._okBtn = new System.Windows.Forms.Button();
            this._cancelBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._fontHeightPixels)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._fontWidthPixels)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Font Height in Pixel:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Font Width in Pixel:";
            // 
            // _fontHeightPixels
            // 
            this._fontHeightPixels.Location = new System.Drawing.Point(111, 1);
            this._fontHeightPixels.Maximum = 24;
            this._fontHeightPixels.Minimum = 5;
            this._fontHeightPixels.Name = "_fontHeightPixels";
            this._fontHeightPixels.Size = new System.Drawing.Size(260, 45);
            this._fontHeightPixels.TabIndex = 2;
            this._fontHeightPixels.Value = 7;
            this._fontHeightPixels.Scroll += new System.EventHandler(this.OnHeightChange);
            // 
            // _fontHeightPixelsLbl
            // 
            this._fontHeightPixelsLbl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._fontHeightPixelsLbl.Location = new System.Drawing.Point(377, 5);
            this._fontHeightPixelsLbl.Name = "_fontHeightPixelsLbl";
            this._fontHeightPixelsLbl.Size = new System.Drawing.Size(49, 26);
            this._fontHeightPixelsLbl.TabIndex = 3;
            this._fontHeightPixelsLbl.Text = "7";
            this._fontHeightPixelsLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _fontWidthPixelsLbl
            // 
            this._fontWidthPixelsLbl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._fontWidthPixelsLbl.Location = new System.Drawing.Point(377, 38);
            this._fontWidthPixelsLbl.Name = "_fontWidthPixelsLbl";
            this._fontWidthPixelsLbl.Size = new System.Drawing.Size(49, 26);
            this._fontWidthPixelsLbl.TabIndex = 5;
            this._fontWidthPixelsLbl.Text = "5";
            this._fontWidthPixelsLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _fontWidthPixels
            // 
            this._fontWidthPixels.Location = new System.Drawing.Point(111, 34);
            this._fontWidthPixels.Maximum = 24;
            this._fontWidthPixels.Minimum = 5;
            this._fontWidthPixels.Name = "_fontWidthPixels";
            this._fontWidthPixels.Size = new System.Drawing.Size(260, 45);
            this._fontWidthPixels.TabIndex = 4;
            this._fontWidthPixels.Value = 5;
            this._fontWidthPixels.Scroll += new System.EventHandler(this.OnWidthChange);
            // 
            // _okBtn
            // 
            this._okBtn.Location = new System.Drawing.Point(271, 72);
            this._okBtn.Name = "_okBtn";
            this._okBtn.Size = new System.Drawing.Size(75, 23);
            this._okBtn.TabIndex = 6;
            this._okBtn.Text = "&OK";
            this._okBtn.UseVisualStyleBackColor = true;
            this._okBtn.Click += new System.EventHandler(this.OnOK);
            // 
            // _cancelBtn
            // 
            this._cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelBtn.Location = new System.Drawing.Point(351, 72);
            this._cancelBtn.Name = "_cancelBtn";
            this._cancelBtn.Size = new System.Drawing.Size(75, 23);
            this._cancelBtn.TabIndex = 7;
            this._cancelBtn.Text = "&Cancel";
            this._cancelBtn.UseVisualStyleBackColor = true;
            // 
            // FontSizeForm
            // 
            this.AcceptButton = this._okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelBtn;
            this.ClientSize = new System.Drawing.Size(431, 102);
            this.Controls.Add(this._cancelBtn);
            this.Controls.Add(this._okBtn);
            this.Controls.Add(this._fontWidthPixelsLbl);
            this.Controls.Add(this._fontWidthPixels);
            this.Controls.Add(this._fontHeightPixelsLbl);
            this.Controls.Add(this._fontHeightPixels);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FontSizeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Font Size";
            this.Load += new System.EventHandler(this.OnLoad);
            ((System.ComponentModel.ISupportInitialize)(this._fontHeightPixels)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._fontWidthPixels)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar _fontHeightPixels;
        private System.Windows.Forms.Label _fontHeightPixelsLbl;
        private System.Windows.Forms.Label _fontWidthPixelsLbl;
        private System.Windows.Forms.TrackBar _fontWidthPixels;
        private System.Windows.Forms.Button _okBtn;
        private System.Windows.Forms.Button _cancelBtn;
    }
}