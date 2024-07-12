using System;
using System.Windows;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using BLADE.AWP.ImageTools;
using System.Reflection.Metadata;

namespace BLADE.AWP.ImgBatch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private object _ui_lock = new object();
        private object _prv_lock = new object();
        public string timefmt = "yyyy-MM-dd HH:mm:ss";
        private string _pngtranc = "#000000";
        private Color mybackColor = Color.FromArgb(20, 30, 40);
        private Color myTingColor = Color.FromArgb(150, 0, 60);
        public int TingColor { get; set; } = 0;


        public bool Live { get; set; } = false;
        public bool Working { get; set; } = false;
        public int PrgBarVal { get; set; } = 0;
        private string _infotext = "";
        public string InfoText
        {
            get { return _infotext + " "; }
            set
            {
                lock (_prv_lock)
                {
                    string vv = value.Trim();
                    if (vv.Length > 0)
                    {
                        string a = vv + "\r\n" + _infotext;
                        if (a.Length > 6600)
                        {
                            a = a.Substring(0, 5000);
                        }
                        _infotext = a;
                    }
                }
            }
        }

        public void UI_PAll()
        {
            this.Invoke(new Action(() =>
            {
                timeLabel.Text = DateTime.Now.ToString(timefmt);
                infoBox.Text = InfoText;
                progressBar1.Value = PrgBarVal;

                if (TingColor > 1)
                {
                    this.BackColor = myTingColor;
                    TingColor = TingColor - 1;
                }
                else
                {
                    if (TingColor == 1)
                    {
                        this.BackColor = mybackColor;
                        TingColor = 0;
                    }
                }
            }));
        }
        public void UI_Time()
        {
            this.Invoke(new Action(() =>
            {
                timeLabel.Text = DateTime.Now.ToString(timefmt);
            }));
        }

        public void UI_ShowInfo()
        {

            this.Invoke(new Action(() =>
            {
                infoBox.Text = InfoText;
            }));
        }
        public void UI_Progress()
        {
            this.Invoke(new Action(() =>
            {
                progressBar1.Value = PrgBarVal;
            }));
        }
        protected string appstart = "";
        protected PicPosPin PPP = new PicPosPin();
        private void Form1_Load(object sender, EventArgs e)
        {
            Application.EnableVisualStyles();
            appstart = Application.StartupPath;
            if (appstart.EndsWith("\\") || appstart.EndsWith("/")) { } else { appstart = appstart + "/"; }
            Live = true;
            textBox2.Text = appstart + "out/";
            textBox1.Text = appstart + "imgs/";
            try { if (!Directory.Exists(textBox2.Text)) { Directory.CreateDirectory(textBox2.Text); } } catch { }
            try { if (!Directory.Exists(textBox1.Text)) { Directory.CreateDirectory(textBox1.Text); } } catch { }
            Thread.Sleep(30);
            InfoText = "欢迎老婆公主使用";
            timer1.Start();
            try
            {
                if (File.Exists(appstart + "miao.png")) { shuiyinbox.Text = appstart + "miao.png"; pictureBox1.Image = Image.FromFile(appstart + "miao.png"); }
            }
            catch (Exception ze) { InfoText = "Load miao.png fail. " + ze.Message; }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            if (!Live) { timer1.Enabled = true; return; }
            try
            {
                InfoText = _realTick();
            }
            catch (Exception ze)
            {
                try { InfoText = "TickError: " + ze.Message; } catch { }
            }
            timer1.Enabled = true;
        }
        private string _realTick()
        {
            string k = "";
            UI_PAll();
            return k;
        }

        public bool NotWorking
        {
            get
            {
                if (!Live) { return false; }

                if (Working) { TingColor = 2; return false; }

                return true;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void yuanmulushuchu_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (NotWorking)
            {
                DialogResult dr = colorDialog1.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    textBox3.BackColor = colorDialog1.Color;
                    textBox3.Text = ImageTools.ColorHelper.ToRGB_HexStr(colorDialog1.Color);
                    _pngtranc = textBox3.Text;
                }
                else { }

            }
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            try
            {
                if (NotWorking)
                {

                    Color ncc = ColorHelper.FromHexStr(textBox3.Text);
                    textBox3.BackColor = ncc;
                    textBox3.Text = ImageTools.ColorHelper.ToRGB_HexStr(ncc);
                    _pngtranc = textBox3.Text;


                }
                else
                {
                    textBox3.Text = _pngtranc;
                }

            }
            catch
            {
                textBox3.Text = _pngtranc;
            }
        }

        private void guiBILI_CheckedChanged(object sender, EventArgs e)
        {
            if (NotWorking)
            {
                if (guiBILI.Checked)
                {
                    guiGAODU.Checked = false;
                    guiKUANDU.Checked = false;
                    guiKAUNGAO.Checked = false;
                }
            }
        }

        private void guiKUANDU_CheckedChanged(object sender, EventArgs e)
        {
            if (NotWorking)
            {
                if (guiKUANDU.Checked)
                {
                    guiGAODU.Checked = false;
                    guiBILI.Checked = false;
                    guiKAUNGAO.Checked = false;
                }
            }
        }

        private void guiGAODU_CheckedChanged(object sender, EventArgs e)
        {
            if (NotWorking)
            {
                if (guiGAODU.Checked)
                {
                    guiKUANDU.Checked = false;
                    guiBILI.Checked = false;
                    guiKAUNGAO.Checked = false;
                }
            }
        }

        private void guiKAUNGAO_CheckedChanged(object sender, EventArgs e)
        {
            if (NotWorking)
            {
                if (guiKAUNGAO.Checked)
                {
                    guiKUANDU.Checked = false;
                    guiBILI.Checked = false;
                    guiGAODU.Checked = false;
                }
            }
        }

        private void textBox5_Leave(object sender, EventArgs e)
        {
            try
            {

                Color ncc = ColorHelper.FromHexStr(textBox5.Text);
                textBox5.BackColor = ncc;
                textBox5.Text = ImageTools.ColorHelper.ToRGB_HexStr(ncc);


            }
            catch
            {
                textBox5.Text = "#000000";
                textBox5.BackColor = Color.Black;
            }
        }

        private void ssyFUGAI_CheckedChanged(object sender, EventArgs e)
        {
            if (ssyFUGAI.Checked)
            {
                ssyRONGHE.Checked = false;
                ssyXIANGJIA.Checked = false;
                ssyXIANGJIAN.Checked = false;
            }
        }

        private void ssyRONGHE_CheckedChanged(object sender, EventArgs e)
        {
            if (ssyRONGHE.Checked)
            {
                ssyFUGAI.Checked = false;
                ssyXIANGJIA.Checked = false;
                ssyXIANGJIAN.Checked = false;
            }
        }

        private void ssyXIANGJIA_CheckedChanged(object sender, EventArgs e)
        {
            if (ssyXIANGJIA.Checked)
            {
                ssyFUGAI.Checked = false;
                ssyRONGHE.Checked = false;
                ssyXIANGJIAN.Checked = false;
            }
        }

        private void ssyXIANGJIAN_CheckedChanged(object sender, EventArgs e)
        {
            if (ssyXIANGJIAN.Checked)
            {
                ssyFUGAI.Checked = false;
                ssyRONGHE.Checked = false;
                ssyXIANGJIA.Checked = false;
            }
        }

        private Color fx_X = Color.FromArgb(30, 100, 180);
        private Color fx_Y = Color.FromArgb(30, 200, 180);
        private void FX_RB_Click(object sender, EventArgs e)
        {
            if (PPP.RightBottom) { PPP.RightBottom = false; FX_RB.ForeColor = fx_X; }
            else
            { PPP.RightBottom = true; FX_RB.ForeColor = fx_Y; }
        }

        private void FX_B_Click(object sender, EventArgs e)
        {
            if (PPP.Bottom) { PPP.Bottom = false; FX_B.ForeColor = fx_X; }
            else
            { PPP.Bottom = true; FX_B.ForeColor = fx_Y; }
        }

        private void FX_LB_Click(object sender, EventArgs e)
        {
            if (PPP.LeftBottom) { PPP.LeftBottom = false; FX_LB.ForeColor = fx_X; }
            else
            { PPP.LeftBottom = true; FX_LB.ForeColor = fx_Y; }
        }

        private void FX_LT_Click(object sender, EventArgs e)
        {
            if (PPP.LeftTop) { PPP.LeftTop = false; FX_LT.ForeColor = fx_X; }
            else
            { PPP.LeftTop = true; FX_LT.ForeColor = fx_Y; }
        }

        private void FX_RT_Click(object sender, EventArgs e)
        {
            if (PPP.RightTop) { PPP.RightTop = false; FX_RT.ForeColor = fx_X; }
            else
            { PPP.RightTop = true; FX_RT.ForeColor = fx_Y; }
        }

        private void FX_T_Click(object sender, EventArgs e)
        {
            if (PPP.Top) { PPP.Top = false; FX_T.ForeColor = fx_X; }
            else
            { PPP.Top = true; FX_T.ForeColor = fx_Y; }
        }

        private void FX_L_Click(object sender, EventArgs e)
        {
            if (PPP.Left) { PPP.Left = false; FX_L.ForeColor = fx_X; }
            else
            { PPP.Left = true; FX_L.ForeColor = fx_Y; }
        }

        private void FX_R_Click(object sender, EventArgs e)
        {
            if (PPP.Right) { PPP.Right = false; FX_R.ForeColor = fx_X; }
            else
            { PPP.Right = true; FX_R.ForeColor = fx_Y; }
        }

        private void FX_C_Click(object sender, EventArgs e)
        {
            if (PPP.Center) { PPP.Center = false; FX_C.ForeColor = fx_X; }
            else
            { PPP.Center = true; FX_C.ForeColor = fx_Y; }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (NotWorking)
            {
                using (OpenFileDialog FD = new OpenFileDialog())
                {
                    FD.Filter = "Png files (*.png)|*.png|Jpg files (*.jpg)|*.jpg|Bmp files (*.bmp)|*.bmp";
                    FD.InitialDirectory = appstart;

                    DialogResult dr = FD.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        try
                        {

                            Image nii = Image.FromFile(FD.FileName);
                            pictureBox1.Image = new Bitmap(nii);
                            nii.Dispose();
                            shuiyinbox.Text = FD.FileName;
                        }
                        catch (Exception ze)
                        {
                            InfoText = "Load File error: " + ze.Message;
                        }

                    }
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (NotWorking)
            {

                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.InitialDirectory = appstart;
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        string dirstr = fbd.SelectedPath;
                        if (dirstr.EndsWith("/") || dirstr.EndsWith("\\"))
                        {
                        }
                        else { dirstr = dirstr + "/"; }
                        textBox1.Text = dirstr;
                        //  textBox2.Text = dirstr;
                    }

                }
                ListIMG();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (NotWorking)
            {

                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    fbd.InitialDirectory = appstart;
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        string dirstr = fbd.SelectedPath;
                        if (dirstr.EndsWith("/") || dirstr.EndsWith("\\"))
                        {
                        }
                        else { dirstr = dirstr + "/"; }
                        textBox2.Text = dirstr;
                        //  textBox2.Text = dirstr;
                    }

                }
            }
        }

        private void fanCHUIZHI_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void fanCHUIZHI_Click(object sender, EventArgs e)
        {
            fanCHUIZHI.Checked = true;
            fanSHUIPING.Checked = false;
        }

        private void fanSHUIPING_Click(object sender, EventArgs e)
        {
            fanCHUIZHI.Checked = false;
            fanSHUIPING.Checked = true;
        }

        private void radioButton7_Click(object sender, EventArgs e)
        {
            radioButton7.Checked = true;
            radioButton8.Checked = false;
            radioButton9.Checked = false;
        }

        private void radioButton8_Click(object sender, EventArgs e)
        {
            radioButton7.Checked = false;
            radioButton8.Checked = true;
            radioButton9.Checked = false;
        }

        private void radioButton9_Click(object sender, EventArgs e)
        {
            radioButton7.Checked = false;
            radioButton8.Checked = false;
            radioButton9.Checked = true;
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            if (NotWorking)
            {
                radioButton3.Checked = true;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
            }

        }

        private void radioButton4_Click(object sender, EventArgs e)
        {
            if (NotWorking)
            {
                radioButton3.Checked = false;
                radioButton4.Checked = true;
                radioButton5.Checked = false;
                radioButton6.Checked = false;
            }
        }

        private void radioButton5_Click(object sender, EventArgs e)
        {
            if (NotWorking)
            {
                radioButton3.Checked = false;
                radioButton4.Checked = false;
                radioButton5.Checked = true;
                radioButton6.Checked = false;
            }
        }

        private void radioButton6_Click(object sender, EventArgs e)
        {
            if (NotWorking)
            {
                radioButton3.Checked = false;
                radioButton4.Checked = false;
                radioButton5.Checked = false;
                radioButton6.Checked = true;
            }
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            if (NotWorking)
            {
                radioButton1.Checked = false; radioButton2.Checked = true;
            }
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            if (NotWorking)
            {
                radioButton1.Checked = true; radioButton2.Checked = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (NotWorking)
            {
                InfoText = "找到 " + scanIMGs().Length.ToString() + " 个可处理文件.";
            }
        }

        protected string[] tmpWORKFILE()
        {

            List<string> ttt = new List<string>();


            if (wcbmp.Checked) { ttt.Add("BMP"); }
            if (wcjpg.Checked) { ttt.Add("JPG"); }
            if (wcemf.Checked) { ttt.Add("EMF"); }
            if (wcexif.Checked) { ttt.Add("EXIF"); }
            if (wcheif.Checked) { ttt.Add("HEIF"); }
            if (wcjpeg.Checked) { ttt.Add("JPEG"); }
            if (wcpng.Checked) { ttt.Add("PNG"); }
            if (wctif.Checked) { ttt.Add("TIF"); }
            if (wcwebp.Checked) { ttt.Add("WEBP"); }
            if (wcwmf.Checked) { ttt.Add("WMF"); }
            return ttt.ToArray();
        }
        protected string[] scanIMGs()
        {
            DirectoryInfo dd = new DirectoryInfo(textBox1.Text.Trim());
            FileInfo[] fis = dd.GetFiles();
            List<string> flist = new List<string>();
            string[] filefel = tmpWORKFILE();
            foreach (FileInfo fi in fis)
            {
                string[] fpp = fi.Name.ToUpper().Split(".", StringSplitOptions.RemoveEmptyEntries);
                string wei = fpp[fpp.Length - 1];
                foreach (string ff in filefel)
                {
                    if (wei == ff)
                    {
                        flist.Add(fi.FullName);
                        break;
                    }
                }
            }
            return flist.ToArray();

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (NotWorking)
            {
                PrgBarVal = 0;
                UI_Progress();
                if (IFIS.Count < 1)
                {
                    MessageBox.Show("队列中没有选择任何图片文件！");
                    return;
                }
                if (qiSHUIYIN.Checked)
                {
                    try { pictureBox1.Image = Image.FromFile(shuiyinbox.Text.Trim()); }
                    catch
                    {
                        MessageBox.Show("您已经选择使用水印，但是未加载水印文件！");
                        return;
                    }
                }

                InfoText = "准备任务，预计处理 " + IFIS.Count.ToString() + " 个图像文件.";

                string[] fscou = new string[IFIS.Count];
                for (int i = 0; i < IFIS.Count; i++)
                {
                    fscou[i] = IFIS.Values[i].fileFullpath;
                    // CWorking cw= new CWorking()
                }
                ImgSaveSetting niss = new ImgSaveSetting();
                ImgFixConfig nifc = new ImgFixConfig();


                if (radioButton3.Checked) { niss.Sfmt = ImgSaveformat.Original; }
                else
                {
                    if (radioButton4.Checked) { niss.Sfmt = ImgSaveformat.Jpg; }
                    else
                    {
                        if (radioButton5.Checked) { niss.Sfmt = ImgSaveformat.Bmp; }
                        else
                        {
                            if (kouPNGtouming.Checked) { niss.Sfmt = ImgSaveformat.Png_KouTrans; }
                            else { niss.Sfmt = ImgSaveformat.Png_KouTrans; }
                        }
                    }
                }
                if (yuanmulushuchu.Checked) { niss.SavePath = ""; }
                else
                {
                    niss.SavePath = textBox2.Text.Trim();
                }
                if (radioButton2.Checked) { niss.reName = true; } else { niss.reName = false; }
                niss.transColor = ColorHelper.FromHexStr(textBox3.Text.Trim());


                nifc.TransWMC = ColorHelper.FromHexStr(textBox5.Text.Trim());
                nifc.On_flip = qiFANZHUAN.Checked;
                nifc.On_turnDR = qiXUANZHUAN.Checked;
                nifc.On_scale = qiGUIZHENG.Checked;
                nifc.On_WaterMK = qiSHUIYIN.Checked;

                if (fanCHUIZHI.Checked) { nifc.flipDR = TurnDr.ExchangeUpDown_HorAxle; } else { nifc.flipDR = TurnDr.ExchangeLeftRight_VerAxle; }
                if (radioButton7.Checked) { nifc.turnDR = TurnDr.Left90; }
                else
                {
                    if (radioButton8.Checked) { nifc.turnDR = TurnDr.Turn180; }
                    else
                    {
                        nifc.turnDR = TurnDr.Right90;
                    }
                }

                nifc.PPP = this.PPP;
                nifc.scale = ScaleType.Auto;
                if (guiKAUNGAO.Checked) { nifc.scale = ScaleType.ForceHW; }
                else
                {
                    if (guiKUANDU.Checked) { nifc.scale = ScaleType.ForceW; }
                    else
                    {
                        nifc.scale = ScaleType.ForceH;
                    }
                }
                try { nifc.ScaleSize.Width = int.Parse(xianKUAN.Text.Trim()); } catch { }
                try { nifc.ScaleSize.Hight = int.Parse(xianGAO.Text.Trim()); } catch { }
                if (ssyFUGAI.Checked) { nifc.WmF = WaterMKFunc.Hard; }
                else
                {
                    if (ssyRONGHE.Checked) { nifc.WmF = WaterMKFunc.Mix; }
                    else
                    {
                        if (ssyXIANGJIA.Checked) { nifc.WmF = WaterMKFunc.ADD; }
                        else { nifc.WmF = WaterMKFunc.CUT; }
                    }
                }

                nifc.WaterMK = pictureBox1.Image;

                Working = true;
                CWorking cw = new CWorking(fscou, niss, nifc);
                cw.Done += Cw_Done;
                cw.StepUP += Cw_StepUP;
                cw.Start();
                InfoText = "任务开始跑了哦。";
            }
        }

        private void Cw_StepUP(object? sender, EventArgs e)
        {
            MsgEventArgs ee = (MsgEventArgs)e;
            PrgBarVal = ee.Num;
            UI_Progress();
            InfoText = ee.Msg;
        }

        private void Cw_Done(object? sender, EventArgs e)
        {
            MsgEventArgs ee = (MsgEventArgs)e;
            InfoText = ee.Msg;
            InfoText = "Work Down! 老婆辛苦了^_^";
            Thread.Sleep(80);

            Working = false;
            MessageBox.Show("幸不辱命！！ 队列处理完成！", "恭喜！您辛苦了！", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //protected int ThrCount = 0;
        protected SortedList<int, ImgFileItem> IFIS = new SortedList<int, ImgFileItem>();
        protected void ClearIFIS()
        {

            foreach (var i in IFIS.Values)
            {
                i.Dispose();
            }
            IFIS.Clear();
        }
        public void ListIMG()
        {
            ClearIFIS();
            flowLayoutPanel1.Controls.Clear();

            string[] fls = scanIMGs();
            foreach (var i in fls)
            {
                ImgFileItem ifi = new ImgFileItem();
                ifi.SN = ImgFileItem.GetSN();
                ifi.fileFullpath = i.Trim();
                ifi.DelMe += IFI_DELME;
                flowLayoutPanel1.Controls.Add(ifi);
                IFIS.Add(ifi.SN, ifi);
            }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            if (NotWorking)
            {
                ListIMG();
            }
        }
        protected void IFI_DELME(object? sender, EventArgs e)
        {
            int ll = flowLayoutPanel1.VerticalScroll.Value;
            if (sender != null)
            {
                int s = ((ImgFileItem)sender).SN;
                if (IFIS.ContainsKey(s))
                {
                    ImgFileItem tf=  IFIS[s];
                    IFIS.Remove(s);
                    tf.Dispose();
                }
                flowLayoutPanel1.Controls.Clear();
                foreach (var i in IFIS.Values)
                {
                    flowLayoutPanel1.Controls.Add(i);
                }
            }
            flowLayoutPanel1.VerticalScroll.Value = ll;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (NotWorking)
            {
                ClearIFIS();
                flowLayoutPanel1.Controls.Clear();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (NotWorking)
            {
                OpenFileDialog OD = new OpenFileDialog();
                OD.Filter = "Png files (*.png)|*.png|Jpg files (*.jpg)|*.jpg|Bmp files (*.bmp)|*.bmp|webp files (*.webp)|*.webp|JPEG files (*.jpeg)|*.jpeg|TIF files (*.tif)|*.tif|HEIF files (*.heif)|*.heif";
                OD.InitialDirectory = appstart;
                OD.Multiselect = true;
                if (DialogResult.OK == OD.ShowDialog())
                {
                    string[] fs = OD.FileNames;
                    foreach (string f in fs)
                    {
                        ImgFileItem ifi = new ImgFileItem();
                        ifi.fileFullpath = f;
                        ifi.SN = ImgFileItem.GetSN();
                        IFIS.Add(ifi.SN, ifi);
                        flowLayoutPanel1.Controls.Add(ifi);
                        flowLayoutPanel1.Controls.SetChildIndex(ifi, 0);
                    }
                }
            }
        }

        private void xianKUAN_TextChanged(object sender, EventArgs e)
        {

        }

        private void xianKUAN_Leave(object sender, EventArgs e)
        {

            try { int.Parse(xianKUAN.Text.Trim()); }
            catch
            {
                xianKUAN.Text = "800";
            }
        }

        private void xianGAO_Leave(object sender, EventArgs e)
        {
            try { int.Parse(xianGAO.Text.Trim()); }
            catch
            {
                xianGAO.Text = "600";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void label4_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("老婆天下无敌！么么哒！\r\n\r\nMade by Blade Studio. Free For all.\r\n\r\nVer: 1.0.20240712", "老公最厉害了！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
