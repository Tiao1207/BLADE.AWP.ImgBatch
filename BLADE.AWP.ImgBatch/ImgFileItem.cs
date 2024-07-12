using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLADE.AWP.ImgBatch
{
    public partial class ImgFileItem : UserControl
    {
        public ImgFileItem()
        {
            InitializeComponent();
        }
        public string fileFullpath = "";

        public int SN { get; set; } = 0;
        protected static int _seednum = 10000;
        public static int GetSN()
        {
            _seednum += 3;
            return _seednum + 1;
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            string[] spf = fileFullpath.Split('.');
            string filetpye = spf[spf.Length - 1].ToUpper().Trim();
            label1.Text = filetpye;
            textBox1.Text = fileFullpath.Trim();

            try
            {
                Image cii = Image.FromFile(fileFullpath);
                pictureBox1.Image = new Bitmap(cii);
                cii.Dispose();
            }
            catch { }
        }
        public event EventHandler? DelMe;

        private void button1_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == MessageBox.Show("清除此图片吗？\r\n" + fileFullpath, "确认？", MessageBoxButtons.OKCancel))
            {
                if (DelMe != null) {
                  DelMe(this,   EventArgs.Empty);
                }
            }
        }
    }
}
