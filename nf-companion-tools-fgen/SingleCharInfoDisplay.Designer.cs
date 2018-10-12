namespace nanoFramework.Companion.Tools.OLEDFontGenerator
{
    partial class SingleCharInfoDisplay
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._charLbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _charLbl
            // 
            this._charLbl.AutoSize = true;
            this._charLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._charLbl.Location = new System.Drawing.Point(95, 2);
            this._charLbl.Name = "_charLbl";
            this._charLbl.Size = new System.Drawing.Size(42, 13);
            this._charLbl.TabIndex = 0;
            this._charLbl.Text = "Custom";
            this._charLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CharInfoDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this._charLbl);
            this.DoubleBuffered = true;
            this.Name = "CharInfoDisplay";
            this.Size = new System.Drawing.Size(238, 238);
            this.Click += new System.EventHandler(this.OnClick);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _charLbl;
    }
}
