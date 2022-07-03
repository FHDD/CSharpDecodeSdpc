using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Slide;
using Slide.Sdpc;
using Slide.Sdpc.ColorControl;
using Slide.Sdpc.ColorControl.Gorgeous;
using Slide.Sdpc.ColorControl.Real;

namespace CSharpDecodeSdpc
{
    class Program
    {
        static void Main(string[] args)
        {
            
            //string path = @"G:\jpeg.sdpc";
            string path = @"F:\中山一附院\中山医院病理切片数据-汇总版\N58943\N58943-1B-HE.sdpc";
            SdpcImage si = (SdpcImage)LoadImage.LoadObjectImage(path);
            si.InitColorTable();
            //si.Sp.Sdpl.LabelRoot;
            ////thumbnail、label、macrograph图像在LoadImage.DisposeObjectImage(si);会自动释放资源
            ////获取缩略图
            //si.Thumbnail.Save(@"G:\img\thumb.bmp", ImageFormat.Bmp);
            //Bitmap thumbCorrect = ColorCorrectByBmp(si.Thumbnail, si.Sp.ColorTable);
            //thumbCorrect.Save(@"G:\img\thumbCorrect.bmp", ImageFormat.Bmp);
            //thumbCorrect.Dispose();
            ////获取label图像
            //si.Label.Save(@"G:\img\label.bmp", ImageFormat.Bmp);
            ////获取宏观图
            //si.Macrograph.Save(@"G:\img\macro.bmp", ImageFormat.Bmp);

            ////获取指定的瓦片
            //int sliceLevel = 1, xSlice = 1, ySlice = 2;
            //byte[] sliceArgb = si.GetSliceArgb(sliceLevel, new Point(xSlice, ySlice));
            //Bitmap sliceBmp = ColorCorrect2Bmp(sliceArgb, si.TileSize, si.Sp.ColorTable);
            //sliceBmp.Save($@"G:\img\{xSlice}_{ySlice}.bmp", ImageFormat.Bmp);
            //sliceBmp.Dispose();

            //获取ROI图像
            //zoom = 0.25/s
            //si.Sp.PicHead.Value.scale == 0.25
            foreach (var item in si.Sp.Sdpl?.LabelRoot?.LabelInfoList)
            {
                double zoom = 1.0 / item.LabelInfo.ZoomScale;
                double X = (item.LabelInfo.Rect.X - item.LabelInfo.ImgLeftTopPoint.X + item.LabelInfo.CurPicRect.X) * zoom;
                double Y = (item.LabelInfo.Rect.Y - item.LabelInfo.ImgLeftTopPoint.Y + item.LabelInfo.CurPicRect.Y) * zoom;
                double Width = item.LabelInfo.Rect.Width * zoom;
                double Height = item.LabelInfo.Rect.Height * zoom;

                int a, b, c, d = 0;
                a = (int)X;b = (int)Y;c = (int)Width;d = (int)Height;

                //int roiLevel = 0, roiX = 402, roiY = 249, roiWidth = 915 - 402, roiHeight = 526 - 249;
                int roiLevel = 0, roiX = a, roiY = b, roiWidth = c, roiHeight = d;
                byte[] roiArgb = si.GetRegionArgb(roiLevel, new Rectangle(roiX, roiY, roiWidth, roiHeight));
                Bitmap roiBmp = ColorCorrect2Bmp(roiArgb, new Size(roiWidth, roiHeight), si.Sp.ColorTable);
                roiBmp.Save($@"F:\中山一附院\中山医院病理切片数据-汇总版\N58943\{roiWidth}_{roiHeight}.bmp", ImageFormat.Bmp);
                roiBmp.Dispose();
            }
            Console.WriteLine(JsonConvert.SerializeObject(si.Sp.Sdpl?.LabelRoot));
            Console.WriteLine(si.Description);
            LoadImage.DisposeObjectImage(si);
            Console.ReadKey();
        }

        /// <summary>
        /// 颜色校正
        /// </summary>
        /// <param name="argb">输入未校正的图像数据</param>
        /// <param name="size">图像大小</param>
        /// <param name="colorTable">颜色校正参数，null表示不进行颜色校正</param>
        /// <returns>输出已校正后的图像数组</returns>
        public static byte[] ColorCorrect2Argb(byte[] argb, Size size, ColorTable colorTable)
        {
            return colorTable != null ? new RealStyle(colorTable).SetArgbColorStyle(argb, size) : argb;
        }

        /// <summary>
        /// 颜色校正
        /// </summary>
        /// <param name="argb">输入未校正的图像数据</param>
        /// <param name="size">图像大小</param>
        /// <param name="colorTable">颜色校正参数，null表示不进行颜色校正</param>
        /// <returns>输出已校正后的图像数组</returns>
        public static Bitmap ColorCorrect2Bmp(byte[] argb, Size size, ColorTable colorTable)
        {
            byte[] dst = colorTable != null ? new RealStyle(colorTable).SetArgbColorStyle(argb, size) : argb;
            return Argb2Bitmap(dst, size);
        }

        /// <summary>
        /// 颜色校正
        /// </summary>
        /// <param name="bmp">输入未校正的图像</param>
        /// <param name="colorTable">颜色校正参数，null表示不进行颜色校正</param>
        /// <returns>输出已校正后的图像</returns>
        public static Bitmap ColorCorrectByBmp(Bitmap bmp, ColorTable colorTable)
        {
            return colorTable != null ? new RealStyle(colorTable).SetArgbColorStyle(bmp) : bmp;
        }

        /// <summary>
        /// 将argb转换成bitmap格式图像
        /// </summary>
        /// <param name="argb">输入数据</param>
        /// <param name="size">输入数据的大小</param>
        /// <returns>bitmap图像</returns>
        public static Bitmap Argb2Bitmap(byte[] argb, Size size)
        {
            Bitmap bmp = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
            BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            Marshal.Copy(argb, 0, bd.Scan0, argb.Length);
            bmp.UnlockBits(bd);
            return bmp;
        }
    }
}
