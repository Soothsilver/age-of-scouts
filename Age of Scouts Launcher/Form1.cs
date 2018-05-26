using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Age_of_Scouts_Launcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string directory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            string pathtorealgame = System.IO.Path.Combine(directory, "Age of Scouts.exe");
            string windowType = "borderless";
            if (rbBorderless.Checked) windowType = "borderless";
            if (rbFullscreen.Checked) windowType = "fullscreen";
            if (rbWindow.Checked) windowType = "window";
            string parameters = this.cbResolution.Text + " " + windowType + " " + (this.chDoNotCollect.Checked ? "donottrack" : "trackatwill");
            try
            {
                System.Diagnostics.Process.Start(pathtorealgame, parameters);
                this.Close();
            }
            catch (Win32Exception message)
            {
                MessageBox.Show("Spouštěč nenašel soubor 'Age of Scouts.exe'. Zkuste spustit soubor Age of Scouts.exe přímo dvojklikem na něj.\n\n" + message.Message,
                    "Hru nemůžeme spustit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Rectangle bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            this.cbResolution.Text = bounds.Width + "x" + bounds.Height;
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            this.lblVersion.Text = "Verze " + version.Major + "." + version.Minor + "." + version.Build;
        }
    }
}
