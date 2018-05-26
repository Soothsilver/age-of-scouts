namespace Age_of_Scouts_Launcher
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chDoNotCollect = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbResolution = new System.Windows.Forms.ComboBox();
            this.rbBorderless = new System.Windows.Forms.RadioButton();
            this.rbWindow = new System.Windows.Forms.RadioButton();
            this.rbFullscreen = new System.Windows.Forms.RadioButton();
            this.lblVersion = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chDoNotCollect);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(21, 176);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(289, 96);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = " Sběr informací ";
            // 
            // chDoNotCollect
            // 
            this.chDoNotCollect.AutoSize = true;
            this.chDoNotCollect.Location = new System.Drawing.Point(16, 67);
            this.chDoNotCollect.Name = "chDoNotCollect";
            this.chDoNotCollect.Size = new System.Drawing.Size(160, 17);
            this.chDoNotCollect.TabIndex = 1;
            this.chDoNotCollect.Text = "Zakázat odesílání informací";
            this.chDoNotCollect.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(262, 39);
            this.label2.TabIndex = 0;
            this.label2.Text = "Tato hra sbírá informace o tom, jak hrajete, a odesílá\r\nje vývojáři. Toto mi pomá" +
    "há testovat a vylepšovat hru.\r\nNijak to hru nezpomaluje.";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.button1.Location = new System.Drawing.Point(21, 120);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(289, 50);
            this.button1.TabIndex = 8;
            this.button1.Text = "Hrát Age of Scouts";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Rozlišení:";
            // 
            // cbResolution
            // 
            this.cbResolution.FormattingEnabled = true;
            this.cbResolution.Items.AddRange(new object[] {
            "1024x768",
            "1152x864",
            "1280x800",
            "1366x768",
            "1280x1024",
            "1440x900",
            "1536x864",
            "1600x900",
            "1680x1050",
            "1920x1080",
            "1920x1200",
            "2560x1080",
            "2560x1440",
            "3440x1440",
            "3840x2160"});
            this.cbResolution.Location = new System.Drawing.Point(84, 24);
            this.cbResolution.Name = "cbResolution";
            this.cbResolution.Size = new System.Drawing.Size(226, 21);
            this.cbResolution.TabIndex = 5;
            // 
            // rbBorderless
            // 
            this.rbBorderless.AutoSize = true;
            this.rbBorderless.Checked = true;
            this.rbBorderless.Location = new System.Drawing.Point(64, 51);
            this.rbBorderless.Name = "rbBorderless";
            this.rbBorderless.Size = new System.Drawing.Size(148, 17);
            this.rbBorderless.TabIndex = 10;
            this.rbBorderless.Text = "Okno na celou obrazovku";
            this.rbBorderless.UseVisualStyleBackColor = true;
            // 
            // rbWindow
            // 
            this.rbWindow.AutoSize = true;
            this.rbWindow.Location = new System.Drawing.Point(64, 97);
            this.rbWindow.Name = "rbWindow";
            this.rbWindow.Size = new System.Drawing.Size(59, 17);
            this.rbWindow.TabIndex = 11;
            this.rbWindow.Text = "V okně";
            this.rbWindow.UseVisualStyleBackColor = true;
            // 
            // rbFullscreen
            // 
            this.rbFullscreen.AutoSize = true;
            this.rbFullscreen.Location = new System.Drawing.Point(64, 74);
            this.rbFullscreen.Name = "rbFullscreen";
            this.rbFullscreen.Size = new System.Drawing.Size(134, 17);
            this.rbFullscreen.TabIndex = 12;
            this.rbFullscreen.Text = "Celoobrazovkový režim";
            this.rbFullscreen.UseVisualStyleBackColor = true;
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(18, 286);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(113, 13);
            this.lblVersion.TabIndex = 2;
            this.lblVersion.Text = "Verze hry není známá.";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 311);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.rbFullscreen);
            this.Controls.Add(this.rbWindow);
            this.Controls.Add(this.rbBorderless);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbResolution);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Age of Scouts";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chDoNotCollect;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbResolution;
        private System.Windows.Forms.RadioButton rbBorderless;
        private System.Windows.Forms.RadioButton rbWindow;
        private System.Windows.Forms.RadioButton rbFullscreen;
        private System.Windows.Forms.Label lblVersion;
    }
}

