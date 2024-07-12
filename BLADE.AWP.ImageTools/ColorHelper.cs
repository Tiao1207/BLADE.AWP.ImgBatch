using System;
using System.Drawing;
using System.Windows;
using System.Drawing.Imaging;
using System.CodeDom.Compiler;
using System.IO;
using System.Threading;
using System.Threading.Tasks;




namespace BLADE.AWP.ImageTools
{
    public class ImgCodes
    {
        public SortedList<Guid, ImageCodecInfo> Encoder = new SortedList<Guid, ImageCodecInfo>();
        public SortedList<Guid, ImageCodecInfo> Decoder = new SortedList<Guid, ImageCodecInfo>();

        public ImgCodes()
        {
            ImageCodecInfo[] iar = ImageCodecInfo.GetImageDecoders();
            foreach (var i in iar)
            {
                if (Decoder.ContainsKey(i.FormatID))
                {

                }
                else { Decoder.Add(i.FormatID, i); }
            }
            iar = ImageCodecInfo.GetImageEncoders();
            foreach (var i in iar)
            {
                if (Encoder.ContainsKey(i.FormatID))
                {

                }
                else { Encoder.Add(i.FormatID, i); }
            }
        }
    }
    public class ColorHelper
    {
        public static string ToRGB_HexStr(Color incolor)
        {
            string a = "#";
            a = a + $"{incolor.R:X2}{incolor.G:X2}{incolor.B:X2}";
            return a.ToUpper();
        }
        public static string ToARGB_HexStr(Color inC)
        {
            string a = "#";
            a = a + $"{inC.A:X2}{inC.R:X2}{inC.G:X2}{inC.B:X2}";
            return a.ToUpper();
        }

        /// <summary>
        /// Change Hex String #003322 or #aaBBCCDD  to System.Drawing.Color
        /// </summary>
        /// <param name="ch"> Color Hex string.  start with "#" , like #001122 or #FF223344 , 6 or 8 hex char . </param>
        /// <returns></returns>
        /// <exception cref="Exception">Input Hex String is not a Color !</exception>
        public static Color FromHexStr(string ch)
        {
            string C = ch.Trim().Replace("#", "").Replace(" ", "").Replace(":", "")
                    .Replace(".", "").Replace("/", "").Replace(",", "")
                    .Replace("'", "").Replace("\"", "").Replace("\\", "").ToUpper();
            if (C.Length == 6 || C.Length == 8)
            {
                
                if (C.Length == 6) { C = "FF" + C; }
                byte[] argb =  Convert.FromHexString(C);
                int i32= BitConverter.ToInt32(argb, 0);
                return Color.FromArgb(i32);
                 
            }
            else
            {
                throw new Exception("Error: is not ColorHex:"+ C);
            }

        }
    }

    public class PicPosPin
    {
        public bool Left = false;
        public bool Right = false;
        public bool Top = false;
        public bool Bottom = false;
        public bool LeftTop = false;
        public bool RightTop = false;
        public bool LeftBottom = false;
        public bool RightBottom = false;
        public bool Center = false;

        public PicPosPin(int defPOS = 3)
        {
            switch (defPOS)
            {
                case 1:
                    LeftBottom = true;
                    break;
                case 2:
                    Bottom = true;
                    break;
                case 3:
                    RightBottom = true;
                    break;
                case 4:
                    Left = true;
                    break;
                case 5:
                    Center = true;
                    break;
                case 6:
                    Right = true;
                    break;
                case 7:
                    LeftTop = true;
                    break;
                case 8:
                    Top = true;
                    break;
                case 9:
                    RightTop = true;
                    break;
                default:
                    break;
            }



        }
    }

    public class ImgFix
    {
        public static ImageCodecInfo? GetCodeInfo(ImageFormat inFMT)
        {
            if (IMCD.Encoder.ContainsKey(inFMT.Guid))
            {
                return IMCD.Encoder[inFMT.Guid];
            }
            else
            {
                if (IMCD.Decoder.ContainsKey(inFMT.Guid))
                {
                    return IMCD.Decoder[inFMT.Guid];
                }
            }
            return null;
        }
        public static ImgCodes IMCD = new ImgCodes();
        /// <summary>
        ///   Rotate and Flip  Image
        /// </summary>
        /// <param name="inMAP"></param>
        /// <param name="inTD"></param>
        /// <returns>inMAP self turned !</returns>
        public static async Task<Image> TurnImg(Image inMAP, TurnDr indr)
        {
             
                switch (indr)
                {
                    case TurnDr.Right90:
                        await Task.Run(() => inMAP.RotateFlip(RotateFlipType.Rotate90FlipNone));
                        break;

                    case TurnDr.Left90:
                        await Task.Run(() => inMAP.RotateFlip(RotateFlipType.Rotate270FlipNone));
                        break;
                    case TurnDr.Turn180:
                        await Task.Run(() => inMAP.RotateFlip(RotateFlipType.Rotate180FlipNone));
                        break;

                    case TurnDr.ExchangeLeftRight_VerAxle:
                        await Task.Run(() => inMAP.RotateFlip(RotateFlipType.RotateNoneFlipX));
                        break;
                    case TurnDr.ExchangeUpDown_HorAxle:
                        await Task.Run(() => inMAP.RotateFlip(RotateFlipType.RotateNoneFlipY));
                        break;
                    default: break;
                }
             
            return inMAP;
        }

        /// <summary>
        /// Scale Image with cfg 
        /// </summary>
        /// <param name="inMAP"></param>
        /// <param name="cfg"></param>
        /// <returns>if cfg.On_Sacle is TRUE , return Image object is a new instance . OrgImage was Disposed.
        ///     if cfg.On_Sacle is false , return the input Image self.
        /// </returns>
        public static async Task<Image> ScaleImg(Image inMAP, ImgFixConfig cfg)
        {
            if (cfg.On_scale)
            {
                int oldHight = inMAP.Height;
                int oldWidth = inMAP.Width;
                float scH = (float)cfg.ScaleSize.Hight / inMAP.Height;
                float scW = (float)cfg.ScaleSize.Width / inMAP.Width;
                if (cfg.scale == ScaleType.ForceH)
                { scW = scH; }
                if (cfg.scale == ScaleType.ForceW)
                { scH = scW; }
                if (cfg.scale == ScaleType.Auto)
                {
                    if (scH < scW) { scW = scH; } else { scH = scW; }
                }
                int newHight = (int)(oldHight * scH);
                int newWidth = (int)(oldWidth * scW);

                Bitmap   newmap = new Bitmap(inMAP, newWidth, newHight);
                Thread.Sleep(1);
                inMAP.Dispose();
               
                return newmap;
            }
            return inMAP;
        }


        /// <summary>
        /// Put WaterMarking on the new instance BitMap(Image). width hight must >= 3px
        /// </summary>
        /// <param name="inMAP"></param>
        /// <param name="cfg"></param>
        /// <returns>
        ///  if input Image.Width or Hight < 3px , return the input Image self.
        ///  if cfg.On_WaterMK is TRUE , return New instance Bitmap(IMAGE) with WaterMarking.  OrgImage was Disposed.
        ///  if cfg.On_WaterMK is false , return the input Image self.
        /// </returns>
        public static async Task<Image> WaterMarking(Image inMAP, ImgFixConfig cfg)
        {

            if (cfg.On_WaterMK)
            {
                if (inMAP.Width < 3 || inMAP.Height < 3)
                {
                    return inMAP;
                }

                Bitmap Nmap = new Bitmap(inMAP);
                Thread.Sleep(1);
                inMAP.Dispose();
                Thread.Sleep(1);
                using (Bitmap waterMM = new Bitmap(cfg.WaterMK))
                {

                    /// 踢掉水印中的指定颜色的alpha。使其在后期的处理中忽略。
                    Color jiance;
                    for (int mw = 0; mw < waterMM.Width; mw++)
                    {
                        for (int mh = 0; mh < waterMM.Height; mh++)
                        {
                            jiance = waterMM.GetPixel(mw, mh);
                            if (jiance.R == cfg.TransWMC.R && jiance.G == cfg.TransWMC.G && jiance.B == cfg.TransWMC.B)
                            {

                                waterMM.SetPixel(mw, mh, Color.FromArgb(0, jiance.R, jiance.G, jiance.B));

                            }
                        }
                    }


                    ImgSize startPP = new ImgSize(0, 0);
                    ImgSize workRR = new ImgSize(cfg.WaterMK.Width, cfg.WaterMK.Height);
                    if (cfg.PPP.LeftTop)
                    {
                        //startPP = new ImgSize(0,0);
                        if (workRR.Width > Nmap.Width) { workRR.Width = Nmap.Width - 1; }
                        if (workRR.Hight > Nmap.Height) { workRR.Hight = Nmap.Height - 1; }
                        await Task.Run(() => waterOne(Nmap, startPP, workRR, waterMM, cfg.WmF));
                    }

                    if (cfg.PPP.RightBottom)
                    {
                        startPP.Width = Nmap.Width - waterMM.Width;
                        startPP.Hight = Nmap.Height - waterMM.Height;
                        if (startPP.Width < 0) { startPP.Width = 0; }
                        if (startPP.Hight < 0) { startPP.Hight = 0; }

                        workRR = new ImgSize(waterMM.Width, waterMM.Height);
                        if (startPP.Width + workRR.Width > Nmap.Width) { workRR.Width = Nmap.Width - startPP.Width - 1; }
                        if (startPP.Hight + workRR.Hight > Nmap.Height) { workRR.Hight = Nmap.Height - startPP.Hight - 1; }
                        await Task.Run(() => waterOne(Nmap, startPP, workRR, waterMM, cfg.WmF));
                    }

                    if (cfg.PPP.Center)
                    {
                        startPP.Width = (int)(Nmap.Width / 2);
                        startPP.Hight = (int)(Nmap.Height / 2);
                        //if (startPP.Width < 0) { startPP.Width = 0; }
                        //if (startPP.Hight < 0) { startPP.Hight = 0; }

                        workRR = new ImgSize(waterMM.Width, waterMM.Height);
                        if (startPP.Width + workRR.Width > Nmap.Width) { workRR.Width = Nmap.Width - startPP.Width - 1; }
                        if (startPP.Hight + workRR.Hight > Nmap.Height) { workRR.Hight = Nmap.Height - startPP.Hight - 1; }
                        await Task.Run(() => waterOne(Nmap, startPP, workRR, waterMM, cfg.WmF));
                    }

                    if (cfg.PPP.Right)
                    {
                        startPP.Width = Nmap.Width - waterMM.Width;
                        startPP.Hight = (int)(Nmap.Height / 2);
                        if (startPP.Width < 0) { startPP.Width = 0; }
                        //  if (startPP.Hight < 0) { startPP.Hight = 0; }

                        workRR = new ImgSize(waterMM.Width, waterMM.Height);
                        if (startPP.Width + workRR.Width > Nmap.Width) { workRR.Width = Nmap.Width - startPP.Width - 1; }
                        if (startPP.Hight + workRR.Hight > Nmap.Height) { workRR.Hight = Nmap.Height - startPP.Hight - 1; }
                        await Task.Run(() => waterOne(Nmap, startPP, workRR, waterMM, cfg.WmF));
                    }

                    if (cfg.PPP.Bottom)
                    {
                        startPP.Width = (int)(Nmap.Width / 2);
                        startPP.Hight = Nmap.Height - waterMM.Height;
                        //  if (startPP.Width < 0) { startPP.Width = 0; }
                        if (startPP.Hight < 0) { startPP.Hight = 0; }

                        workRR = new ImgSize(waterMM.Width, waterMM.Height);
                        if (startPP.Width + workRR.Width > Nmap.Width) { workRR.Width = Nmap.Width - startPP.Width - 1; }
                        if (startPP.Hight + workRR.Hight > Nmap.Height) { workRR.Hight = Nmap.Height - startPP.Hight - 1; }
                        await Task.Run(() => waterOne(Nmap, startPP, workRR, waterMM, cfg.WmF));
                    }

                    if (cfg.PPP.LeftBottom)
                    {
                        startPP.Width = 0;
                        startPP.Hight = Nmap.Height - waterMM.Height;
                        //  if (startPP.Width < 0) { startPP.Width = 0; }
                        if (startPP.Hight < 0) { startPP.Hight = 0; }

                        workRR = new ImgSize(waterMM.Width, waterMM.Height);
                        if (startPP.Width + workRR.Width > Nmap.Width) { workRR.Width = Nmap.Width - startPP.Width - 1; }
                        if (startPP.Hight + workRR.Hight > Nmap.Height) { workRR.Hight = Nmap.Height - startPP.Hight - 1; }
                        await Task.Run(() => waterOne(Nmap, startPP, workRR, waterMM, cfg.WmF));
                    }

                    if (cfg.PPP.RightTop)
                    {
                        startPP.Width = Nmap.Width - waterMM.Width;
                        startPP.Hight = 0;
                        if (startPP.Width < 0) { startPP.Width = 0; }
                        // if (startPP.Hight < 0) { startPP.Hight = 0; }

                        workRR = new ImgSize(waterMM.Width, waterMM.Height);
                        if (startPP.Width + workRR.Width > Nmap.Width) { workRR.Width = Nmap.Width - startPP.Width - 1; }
                        if (startPP.Hight + workRR.Hight > Nmap.Height) { workRR.Hight = Nmap.Height - startPP.Hight - 1; }
                        await Task.Run(() => waterOne(Nmap, startPP, workRR, waterMM, cfg.WmF));
                    }

                    if (cfg.PPP.Left)
                    {
                        startPP.Width = 0;
                        startPP.Hight = (int)(Nmap.Height / 2);
                        //  if (startPP.Width < 0) { startPP.Width = 0; }
                        //  if (startPP.Hight < 0) { startPP.Hight = 0; }

                        workRR = new ImgSize(waterMM.Width, waterMM.Height);
                        if (startPP.Width + workRR.Width > Nmap.Width) { workRR.Width = Nmap.Width - startPP.Width - 1; }
                        if (startPP.Hight + workRR.Hight > Nmap.Height) { workRR.Hight = Nmap.Height - startPP.Hight - 1; }
                        await Task.Run(() => waterOne(Nmap, startPP, workRR, waterMM, cfg.WmF));
                    }

                    if (cfg.PPP.Top)
                    {
                        startPP.Width = (int)(Nmap.Width / 2);
                        startPP.Hight = 0;
                        //  if (startPP.Width < 0) { startPP.Width = 0; }
                        //  if (startPP.Hight < 0) { startPP.Hight = 0; }

                        workRR = new ImgSize(waterMM.Width, waterMM.Height);
                        if (startPP.Width + workRR.Width > Nmap.Width) { workRR.Width = Nmap.Width - startPP.Width - 1; }
                        if (startPP.Hight + workRR.Hight > Nmap.Height) { workRR.Hight = Nmap.Height - startPP.Hight - 1; }
                        await Task.Run(() => waterOne(Nmap, startPP, workRR, waterMM, cfg.WmF));
                    }

                }
                return Nmap;
            }
            return inMAP;
        }


        /// <summary>
        /// trans Color alpha = 0 ;
        /// </summary>
        /// <param name="inMap">org Image</param>
        /// <param name="kouColor"></param>
        /// <returns> New  instance Bitmap(Image)</returns>
        public static Image FlushTransColorPNG(Image inMap, Color kouColor)
        {
            Bitmap nnn = new Bitmap(inMap);

            Color ccc;
            for (int w = 0; w < nnn.Width; w++)
            {
                for (int h = 0; h < nnn.Height; h++)
                {
                    ccc = nnn.GetPixel(w, h);
                    if (ccc.R == kouColor.R && ccc.G == kouColor.G && ccc.B == kouColor.B)
                    {
                        nnn.SetPixel(w, h, Color.FromArgb(0, kouColor.R, kouColor.G, kouColor.B));
                    }
                }
            }
            inMap.Dispose();
            return nnn;
        }
        protected static void waterOne(Bitmap inmap, ImgSize StartPoint, ImgSize workRang, Bitmap waterImg, WaterMKFunc wmk)
        {
            Color TMC;
            Color TWC;
            Color JGC;
            for (int w = 0; w < workRang.Width; w++)
            {
                for (int h = 0; h < workRang.Hight; h++)
                {
                    TWC = waterImg.GetPixel(w, h);
                    if (wmk == WaterMKFunc.Hard) { if (TWC.A < 64) { continue; } }

                    if (TWC.A < 8) { continue; }

                    TMC = inmap.GetPixel(StartPoint.Width + w, StartPoint.Hight + h);

                    if (wmk == WaterMKFunc.Mix)
                    { JGC = wm_Mix(TMC, TWC); }
                    else
                    {
                        if (wmk == WaterMKFunc.ADD)
                        { JGC = wm_ADD(TMC, TWC); }
                        else
                        {
                            if (wmk == WaterMKFunc.CUT)
                            { JGC = wm_CUT(TMC, TWC); }
                            else { JGC = wm_Hard(TMC, TWC); }
                        }
                    }

                    inmap.SetPixel(StartPoint.Width + w, StartPoint.Hight + h, JGC);

                }

            }

        }
        protected static Color wm_Hard(Color imgColor, Color wmColor)
        {
            if (wmColor.A < 64)
            {
                return imgColor;
            }
            byte nA = wmColor.A;
            if (imgColor.A > nA) { nA = imgColor.A; }
            return Color.FromArgb(nA, wmColor.R, wmColor.G, wmColor.B);

        }
        protected static Color wm_Mix(Color imgColor, Color wmColor)
        {
            if (wmColor.A < 16)
            {
                return imgColor;
            }

            float yb = (float)wmColor.A / 255;
            int nR = (int)((imgColor.R + wmColor.R * yb) / 2);
            if (nR > 255) { nR = 255; } else { if (nR < 0) { nR = 0; } }

            int nG = (int)((imgColor.G + wmColor.G * yb) / 2);
            if (nG > 255) { nG = 255; } else { if (nG < 0) { nG = 0; } }

            int nB = (int)((imgColor.B + wmColor.B * yb) / 2);
            if (nB > 255) { nB = 255; } else { if (nB < 0) { nB = 0; } }

            byte nA = wmColor.A;
            if (imgColor.A > nA) { nA = imgColor.A; }

            return Color.FromArgb(nA, nR, nG, nB);
        }
        protected static Color wm_ADD(Color imgColor, Color wmColor)
        {
            float yb = (float)wmColor.A / 255;
            int nR = (int)(imgColor.R + wmColor.R * yb);
            if (nR > 255) { nR = 255; } else { if (nR < 0) { nR = 0; } }

            int nG = (int)(imgColor.G + wmColor.G * yb);
            if (nG > 255) { nG = 255; } else { if (nG < 0) { nG = 0; } }

            int nB = (int)(imgColor.B + wmColor.B * yb);
            if (nB > 255) { nB = 255; } else { if (nB < 0) { nB = 0; } }

            byte nA = wmColor.A;
            if (imgColor.A > nA) { nA = imgColor.A; }
            return Color.FromArgb(nA, nR, nG, nB);
        }
        protected static Color wm_CUT(Color imgColor, Color wmColor)
        {
            float yb = (float)wmColor.A / 255;
            int nR = (int)(imgColor.R - wmColor.R * yb);
            if (nR > 255) { nR = 255; } else { if (nR < 0) { nR = 0; } }

            int nG = (int)(imgColor.G - wmColor.G * yb);
            if (nG > 255) { nG = 255; } else { if (nG < 0) { nG = 0; } }

            int nB = (int)(imgColor.B - wmColor.B * yb);
            if (nB > 255) { nB = 255; } else { if (nB < 0) { nB = 0; } }

            byte nA = wmColor.A;
            if (imgColor.A > nA) { nA = imgColor.A; }
            return Color.FromArgb(nA, nR, nG, nB);
        }

       
        public static async Task<string> FixImageFile(string fileFullname, ImgFixConfig Cfg, ImgSaveSetting Iss)
        {
            string info = "";
            string ffn = fileFullname.Trim();
            Image tempimg = Image.FromFile(fileFullname);

            ImageFormat ifmt = tempimg.RawFormat;

            Image OPENIMG = new Bitmap(tempimg);

            string[] flll = ffn.Split(new string[] { "\\", "/" }, StringSplitOptions.RemoveEmptyEntries);

            string oldfilename = flll[flll.Length - 1];
            info = "Fix:" + oldfilename;
            string oldfilepath = ffn.Substring(0, ffn.Length - oldfilename.Length);
            flll = oldfilename.Split(".", StringSplitOptions.RemoveEmptyEntries);
            string oldfiletype = flll[flll.Length - 1];
            string oldfileJustname = oldfilename.Substring(0, oldfilename.Length - oldfiletype.Length);
            tempimg.Dispose();
            try
            {
                if (Cfg.On_flip)
                {
                    OPENIMG = await TurnImg(OPENIMG, Cfg.flipDR);
                    info += "  FlipImage  ";
                }
                if (Cfg.On_turnDR)
                {
                    OPENIMG = await TurnImg(OPENIMG, Cfg.turnDR);
                    info += "  TurnImage  ";
                }

                if (Cfg.On_scale)
                {
                    OPENIMG = await ScaleImg(OPENIMG, Cfg);
                    info += "  ScaleImage  ";
                }
                if (Cfg.On_WaterMK)
                {
                    OPENIMG = await WaterMarking(OPENIMG, Cfg);
                    info += "  WaterMarkImage  ";
                }
                if (Iss.Sfmt == ImgSaveformat.Png_KouTrans)
                {
                    OPENIMG = FlushTransColorPNG(OPENIMG, Iss.transColor);
                    info += "  FlushTransColor  ";
                }
                else
                {
                    if (Iss.Sfmt == ImgSaveformat.Original && oldfiletype.ToUpper() == "PNG")
                    {
                        OPENIMG = FlushTransColorPNG(OPENIMG, Iss.transColor);
                        info += "  FlushTransColor  ";
                    }
                }
            }
            catch (Exception kze) { info += " sub EX: " + kze.Message; }
            string newpath = "";
            if (Iss.SavePath == "")
            {
                newpath = oldfilepath;
            }
            else { newpath = Iss.SavePath; }
            if (newpath.EndsWith("/") || newpath.EndsWith("\\"))
            { }
            else { newpath = newpath + "/"; }


            if (Iss.reName)
            {
                newpath = newpath + "fix_";
            }

            newpath = newpath + oldfileJustname;
            if (newpath.EndsWith("."))
            { }
            else { newpath = newpath + "."; }
            if (Iss.Sfmt == ImgSaveformat.Original) { newpath = newpath + oldfiletype; }
            else
            {
                if (Iss.Sfmt == ImgSaveformat.Jpg) { newpath = newpath + "JPG"; ifmt = ImageFormat.Jpeg; }
                else
                {
                    if (Iss.Sfmt == ImgSaveformat.Bmp) { newpath = newpath + "BMP"; ifmt = ImageFormat.Bmp; }
                    else
                    {
                        newpath = newpath + "PNG"; ifmt = ImageFormat.Png;
                    }
                }
            }

            try
            {
                ImageCodecInfo? ccode = GetCodeInfo(ifmt);
                if (ccode == null)
                {
                    await Task.Run(() => OPENIMG.Save(newpath, ImageFormat.Jpeg));
                }
                else
                {

                    EncoderParameter enP = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
                    EncoderParameters enPs = new EncoderParameters(1);
                    enPs.Param[0] = enP;
                    await Task.Run(() => OPENIMG.Save(newpath, ccode, enPs));
                    enPs.Dispose();
                }
                info += "  SAVE:" + newpath;
                OPENIMG.Dispose();
            }
            catch (Exception sze)
            {
                info += " SaveFile ["+newpath+"] EX:"+sze.Message;
            }
            return info;
        }
    }

    public class ImgFixConfig
    {
        public TurnDr turnDR = TurnDr.Left90;
        public bool On_turnDR = false;

        public bool On_flip = false;
        public TurnDr flipDR = TurnDr.ExchangeUpDown_HorAxle;

        public ScaleType scale = ScaleType.Auto;
        public bool On_scale = false;
        public ImgSize ScaleSize = new ImgSize();

        public bool On_WaterMK = false;
        public PicPosPin PPP =new PicPosPin();
        public WaterMKFunc WmF = WaterMKFunc.Hard;
        public Color TransWMC = Color.Black;
        public Image WaterMK = null;

      
        public   ImgFixConfig(bool Turn_ON,bool Flip_ON, bool Scale_ON,bool WaterMK_ON, ImgSize scaleSizeSet, PicPosPin MarkPos ,  Color transClr,
            TurnDr tdr=TurnDr.Left90, TurnDr fdr=TurnDr.ExchangeUpDown_HorAxle, ScaleType sct=ScaleType.Auto, WaterMKFunc wmFunc= WaterMKFunc.Hard,  Image waterMarkImg = null )
        { 
            On_turnDR = Turn_ON;
            On_scale=Scale_ON;
            On_WaterMK = WaterMK_ON;
            On_flip = Flip_ON;

            flipDR = fdr;
            turnDR = tdr;
            scale = sct;
            ScaleSize= scaleSizeSet;

            PPP= MarkPos;
            WaterMK = waterMarkImg;
            TransWMC = transClr;

            WmF = wmFunc;
        }

        public ImgFixConfig()
        { }
    }
    public class MsgEventArgs : EventArgs
    {
        public string Msg = "";
        public int Num = 0;
        public object? Obj = null;
        public MsgEventArgs() : base() { }
        public MsgEventArgs(string inMsg, int inNum, object? inObj = null) : base()
        {
            Msg = inMsg + ""; Num = inNum; Obj = inObj;
        }
    }
    public struct ImgSize
    {
        public int Hight = 1;
        public int Width = 1;
        public ImgSize(int w,int h )
        {
            Hight = h;
            Width = w;
        }
        public ImgSize() { }
    }
    public enum TurnDr
    {
        Left90 = 1,
        Right90 = 2,
        Turn180 = 3,
        ExchangeLeftRight_VerAxle = 4,
        ExchangeUpDown_HorAxle = 5

    }
    public enum ScaleType
    { 
       ForceH,
       ForceW,
       ForceHW,
       Auto,
    }

    public enum WaterMKFunc
    { 
        /// <summary>
        /// 硬覆盖
        /// </summary>
       Hard,
       /// <summary>
       /// 融合平均值
       /// </summary>
       Mix,
       /// <summary>
       ///  相加亮
       /// </summary>
       ADD,
       /// <summary>
       ///  相减暗
       /// </summary>
        CUT,
    }
    public enum ImgSaveformat
    {
        Original,
        Jpg,
        Bmp,
        Png,
        Png_KouTrans,
    }
    public class ImgSaveSetting
    {
        public ImgSaveformat Sfmt = ImgSaveformat.Original;
        public string SavePath = "";
        public bool reName = true;
        public Color transColor = Color.Black;
    }

    public class CWorking
    {
        public string outputtext = "";
        public bool Working = false;
        public int WorkLine { get; private set; } = 0;
        protected object linglock=new object();
        protected string[] fls=new string[0];
        protected  ImgSaveSetting ISS;
        protected ImgFixConfig IFC;
        
        public event EventHandler? Done;

        public event EventHandler? StepUP;
        public CWorking(string[] infls,ImgSaveSetting inISS,ImgFixConfig inIFC)
        {
            fls = infls; ISS = inISS; IFC = inIFC;
        }

        public void Start()
        {
            outputtext = "Start("+DateTime.Now.ToString("yyyyMMdd HHmmss")+")\r\n";
            Working = true;
            Thread CCCT= new Thread(new ThreadStart(PUTWORK));
            CCCT.Start();
        }
        protected void PUTWORK()
        {
            int fef = 0;
            while (Working)
            {
                if (fef < fls.Length)
                {
                    try
                    {
                        if (WorkLine < 5)
                        {
                            
                            lock (linglock)
                            {
                                WorkLine = WorkLine + 1;
                            }
                            ThreadPool.QueueUserWorkItem(new WaitCallback(workOne), fls[fef]);
                            fef++;
                            if (StepUP != null)
                            {
                                string a = "";
                                lock (linglock)
                                {
                                    a = outputtext;
                                    outputtext = "";
                                }
                                MsgEventArgs e = new MsgEventArgs(a, (int)(((float)fef * 100) / fls.Length), null);

                                StepUP(this, e);
                            }
                        }
                        else { Thread.Sleep(5); }
                    }
                    catch(Exception ze) {
                        outputtext = outputtext + "\r\nEX: " + ze.Message;
                    }
                }
                else {

                    Thread.Sleep(100);
                    if (WorkLine < 1)
                    {
                        Working = false;
                    }
                }
            
            }
            if (Done != null) {
                outputtext=outputtext+"\r\nDoen("+ DateTime.Now.ToString("yyyyMMdd HHmmss") + ")";
                Done(null, new MsgEventArgs(outputtext, 0, null));
            }
        }

        protected async void workOne(object? obj)
        {
            string a = "preRun work.";
            try
            {
                 a = await ImgFix.FixImageFile((string)obj, IFC, ISS);
            } catch (Exception zee) { a += " Error: " + zee.Message + " " + obj.ToString(); }
            outputtext = outputtext + " \r\n" + a;
            lock (linglock)
            {
                WorkLine = WorkLine - 1;
            }
        }
    }
}
