using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

namespace a2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button_choose_Click(object sender, EventArgs e)
        {

            OpenFileDialog file = new OpenFileDialog();
            file.ShowDialog();
            //this.textBox.Text = file.SafeFileName;
            this.textBox.Text = file.FileName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string path = @"F:\中山一附院\中山医院病理切片数据-汇总版\N58943\N58943-1B-HE.sdpc";
            string path = this.textBox.Text;
            //string path_dictionary = System.IO.Path.GetDirectoryName(path);
            //给每个wsi创建一个文件夹
            string _path_dictionary = System.IO.Path.GetDirectoryName(path);
            string[] path_split = path.Split('\\');
            //int w1 = path_split.Length;
            string wsi_name = path_split[path_split.Length-1].Split('.')[0];

            string path_dictionary = System.IO.Path.Combine(_path_dictionary, wsi_name);
            System.IO.Directory.CreateDirectory(path_dictionary);



            SdpcImage si = (SdpcImage)LoadImage.LoadObjectImage(path);
            si.InitColorTable();
            float levelScale0 = si.Sp.PicHead.Value.scale;
            // 【add】判断是否新建文件夹
            string folder_name = "_folder_name";
            string new_path = "_new_path";

            if (levelScale0==0.5)
            {
                int n = 1;
                /*
                 *          【BEGIN】
                 * 将levelScale0==0.25的【add】程序(在foreach前的)，复制到此
                 */

                //【add】新建文件夹相关的参数（第一个符合的不用看后一个item，直接新建；同时将第一个item的参数保留一次）
                int i_newfolder = 1;
                //【add】建立数组，储存上一个item的X,Y,Width,Height
                int[] former_item = new int[] { 0, 0, 0, 0 };

                /*
                 *          【END】
                 * 将levelScale0==0.25的【add】程序(在foreach前的)，复制到此
                 */

                foreach (var item in si.Sp.Sdpl?.LabelRoot?.LabelInfoList)
                {
                    if (item.LabelInfo.Rect.X == 0 && item.LabelInfo.Rect.Y == 0 && item.LabelInfo.Rect.Width == 0 && item.LabelInfo.Rect.Height == 0)
                    {
                        n = n + 1;
                        continue;
                    }

                    /*
                     *          【BEGIN】
                     * 将levelScale0==0.25的【add】程序（在continue后、int roiLevel = 1前），复制到此
                     */
                    if (i_newfolder == 1)
                    {
                        // 【add】判断是否新建文件夹
                        folder_name = item.LabelInfo.Rect.X.ToString() + "-" + item.LabelInfo.Rect.Y.ToString();
                        new_path = System.IO.Path.Combine(path_dictionary, folder_name);
                        System.IO.Directory.CreateDirectory(new_path);
                        i_newfolder = i_newfolder + 1;
                        //留下当前item的横纵坐标值及矩形大小值，以对照下一个item是否超出区域范围
                        former_item[0] = item.LabelInfo.Rect.X; former_item[1] = item.LabelInfo.Rect.Y; former_item[2] = item.LabelInfo.Rect.Width; former_item[3] = item.LabelInfo.Rect.Height;

                    }
                    else if (i_newfolder != 1)
                    {
                        //if (System.Math.Abs(item.LabelInfo.Rect.X-former_item[0])> ((item.LabelInfo.Rect.Width)+10) || System.Math.Abs(item.LabelInfo.Rect.Y - former_item[1]) > ((item.LabelInfo.Rect.Width)+10))
                        if (System.Math.Abs(item.LabelInfo.Rect.X - former_item[0]) != ((item.LabelInfo.Rect.Width) + 128) && System.Math.Abs(item.LabelInfo.Rect.Y - former_item[1]) != ((item.LabelInfo.Rect.Width) + 128))
                        {
                            int q1 = item.LabelInfo.Rect.X; int q2 = former_item[0]; int Q = System.Math.Abs(item.LabelInfo.Rect.X - former_item[0]);
                            int q4 = item.LabelInfo.Rect.Y; int q5 = former_item[1]; int QQ = System.Math.Abs(item.LabelInfo.Rect.Y - former_item[1]);
                            // 【add】判断是否新建文件夹
                            folder_name = item.LabelInfo.Rect.X.ToString() + "-" + item.LabelInfo.Rect.Y.ToString();
                            new_path = System.IO.Path.Combine(path_dictionary, folder_name);
                            //int q = System.Math.Abs(item.LabelInfo.Rect.X - former_item[0]);int qq = System.Math.Abs(item.LabelInfo.Rect.Y - former_item[1]);
                            System.IO.Directory.CreateDirectory(new_path);
                            i_newfolder = i_newfolder + 1;
                            //留下当前item的横纵坐标值及矩形大小值，以对照下一个item是否超出区域范围
                            former_item[0] = item.LabelInfo.Rect.X; former_item[1] = item.LabelInfo.Rect.Y; former_item[2] = item.LabelInfo.Rect.Width; former_item[3] = item.LabelInfo.Rect.Height;

                        }
                        else
                        {
                            //留下当前item的横纵坐标值及矩形大小值，以对照下一个item是否超出区域范围
                            former_item[0] = item.LabelInfo.Rect.X; former_item[1] = item.LabelInfo.Rect.Y; former_item[2] = item.LabelInfo.Rect.Width; former_item[3] = item.LabelInfo.Rect.Height;

                        }
                    }

                    /*
                     *          【END】
                     * 将levelScale0==0.25的【add】程序（在continue后、int roiLevel = 1前），复制到此
                     */

                    int roiLevel = 1;
                    float levelScale = si.Sp.PicHead.Value.scale;
                    double aa = Math.Pow(levelScale, roiLevel);
                    double zoom = aa / item.LabelInfo.ZoomScale;
                    double X = (item.LabelInfo.Rect.X - item.LabelInfo.ImgLeftTopPoint.X + item.LabelInfo.CurPicRect.X) * zoom;
                    double Y = (item.LabelInfo.Rect.Y - item.LabelInfo.ImgLeftTopPoint.Y + item.LabelInfo.CurPicRect.Y) * zoom;
                    double Width = item.LabelInfo.Rect.Width * zoom;
                    double Height = item.LabelInfo.Rect.Height * zoom;

                    int a, b, c, d = 0;
                    a = (int)X; b = (int)Y; c = (int)Width; d = (int)Height;

                    //int roiLevel = 0, roiX = 402, roiY = 249, roiWidth = 915 - 402, roiHeight = 526 - 249;

                    int roiX = a, roiY = b, roiWidth = c, roiHeight = d;
                    byte[] roiArgb = si.GetRegionArgb(roiLevel, new Rectangle(roiX, roiY, roiWidth, roiHeight));
                    Bitmap roiBmp = ColorCorrect2Bmp(roiArgb, new Size(roiWidth, roiHeight), si.Sp.ColorTable);
                    //roiBmp.Save($@"F:\中山一附院\中山医院病理切片数据-汇总版\N58943\{roiWidth}_{roiHeight}.bmp", ImageFormat.Bmp);
                    //roiBmp.Save(path_dictionary + $@"\{n}_{roiX}_{roiY}.png", ImageFormat.Png);
                    roiBmp.Save(new_path + $@"\{n}_{roiX}_{roiY}.png", ImageFormat.Png);  //【add】

                    //string path_save_dictionary = "F:\\HER2\\her2tiles_size100\\0"; roiBmp.Save(path_save_dictionary + $@"\{n}_{roiX}_{roiY}.png", ImageFormat.Png);
                    //string path_save_dictionary = "F:\\HER2\\her2tiles_size100\\1"; roiBmp.Save(path_save_dictionary + $@"\{n}_{roiX}_{roiY}.png", ImageFormat.Png);
                    //string path_save_dictionary = "F:\\HER2\\her2tiles_size100\\2"; roiBmp.Save(path_save_dictionary + $@"\{n}_{roiX}_{roiY}.png", ImageFormat.Png);
                    //string path_save_dictionary = "F:\\HER2\\her2tiles_size100\\3"; roiBmp.Save(path_save_dictionary + $@"\{n}_{roiX}_{roiY}.png", ImageFormat.Png);

                    roiBmp.Dispose();
                    n = n + 1;
                }
                MessageBox.Show("生成成功");

            }

            else if(levelScale0 == 0.25)
            {
                int n = 1;
                //【add】新建文件夹相关的参数（第一个符合的不用看后一个item，直接新建；同时将第一个item的参数保留一次）
                int i_newfolder = 1;
                //【add】建立数组，储存上一个item的X,Y,Width,Height
                int[] former_item = new int[] { 0, 0, 0, 0 };
                foreach (var item in si.Sp.Sdpl?.LabelRoot?.LabelInfoList)
                {
                    if (item.LabelInfo.Rect.X == 0 && item.LabelInfo.Rect.Y == 0 && item.LabelInfo.Rect.Width == 0 && item.LabelInfo.Rect.Height == 0)
                    {
                        n = n + 1;
                        continue;
                    }



                    if (i_newfolder == 1)
                    {
                        // 【add】判断是否新建文件夹
                        folder_name = item.LabelInfo.Rect.X.ToString() + "-" + item.LabelInfo.Rect.Y.ToString();
                        new_path = System.IO.Path.Combine(path_dictionary, folder_name);
                        System.IO.Directory.CreateDirectory(new_path);
                        i_newfolder = i_newfolder + 1;
                        //留下当前item的横纵坐标值及矩形大小值，以对照下一个item是否超出区域范围
                        former_item[0] = item.LabelInfo.Rect.X; former_item[1] = item.LabelInfo.Rect.Y; former_item[2] = item.LabelInfo.Rect.Width; former_item[3] = item.LabelInfo.Rect.Height;

                    }
                    else if (i_newfolder != 1)
                    {
                        //if (System.Math.Abs(item.LabelInfo.Rect.X-former_item[0])> ((item.LabelInfo.Rect.Width)+10) || System.Math.Abs(item.LabelInfo.Rect.Y - former_item[1]) > ((item.LabelInfo.Rect.Width)+10))
                        if (System.Math.Abs(item.LabelInfo.Rect.X - former_item[0]) != ((item.LabelInfo.Rect.Width) + 128) && System.Math.Abs(item.LabelInfo.Rect.Y - former_item[1]) != ((item.LabelInfo.Rect.Width) + 128))
                        {
                            int q1 = item.LabelInfo.Rect.X; int q2 = former_item[0]; int Q = System.Math.Abs(item.LabelInfo.Rect.X - former_item[0]);
                            int q4 = item.LabelInfo.Rect.Y; int q5 = former_item[1]; int QQ = System.Math.Abs(item.LabelInfo.Rect.Y - former_item[1]);
                            // 【add】判断是否新建文件夹
                            folder_name = item.LabelInfo.Rect.X.ToString() + "-" + item.LabelInfo.Rect.Y.ToString();
                            new_path = System.IO.Path.Combine(path_dictionary, folder_name);
                            //int q = System.Math.Abs(item.LabelInfo.Rect.X - former_item[0]);int qq = System.Math.Abs(item.LabelInfo.Rect.Y - former_item[1]);
                            System.IO.Directory.CreateDirectory(new_path);
                            i_newfolder = i_newfolder + 1;
                            //留下当前item的横纵坐标值及矩形大小值，以对照下一个item是否超出区域范围
                            former_item[0] = item.LabelInfo.Rect.X; former_item[1] = item.LabelInfo.Rect.Y; former_item[2] = item.LabelInfo.Rect.Width; former_item[3] = item.LabelInfo.Rect.Height;

                        }
                        else
                        {
                            //留下当前item的横纵坐标值及矩形大小值，以对照下一个item是否超出区域范围
                            former_item[0] = item.LabelInfo.Rect.X; former_item[1] = item.LabelInfo.Rect.Y; former_item[2] = item.LabelInfo.Rect.Width; former_item[3] = item.LabelInfo.Rect.Height;

                        }
                    }
                    


                    int roiLevel = 0;
                    float levelScale = si.Sp.PicHead.Value.scale;
                    double aa = Math.Pow(levelScale, roiLevel);
                    double zoom = aa / item.LabelInfo.ZoomScale;
                    double X = (item.LabelInfo.Rect.X - item.LabelInfo.ImgLeftTopPoint.X + item.LabelInfo.CurPicRect.X) * zoom;
                    double Y = (item.LabelInfo.Rect.Y - item.LabelInfo.ImgLeftTopPoint.Y + item.LabelInfo.CurPicRect.Y) * zoom;
                    double Width = item.LabelInfo.Rect.Width * zoom;
                    double Height = item.LabelInfo.Rect.Height * zoom;

                    int a, b, c, d = 0;
                    a = (int)X; b = (int)Y; c = (int)Width; d = (int)Height;

                    //int roiLevel = 0, roiX = 402, roiY = 249, roiWidth = 915 - 402, roiHeight = 526 - 249;

                    int roiX = a, roiY = b, roiWidth = c, roiHeight = d;
                    byte[] roiArgb = si.GetRegionArgb(roiLevel, new Rectangle(roiX, roiY, roiWidth, roiHeight));
                    Bitmap roiBmp = ColorCorrect2Bmp(roiArgb, new Size(roiWidth, roiHeight), si.Sp.ColorTable);
                    //roiBmp.Save($@"F:\中山一附院\中山医院病理切片数据-汇总版\N58943\{roiWidth}_{roiHeight}.bmp", ImageFormat.Bmp);
                    Image bgImage = ZoomPicture(roiBmp, 0.5f, 0.5f);  //M,N大于1，为放大图片，小于1为缩小图片  //add
                    Bitmap bmap = new Bitmap(bgImage);                                                         //add
                    //bmap.Save(path_dictionary + $@"\{n}_{roiX}_{roiY}.png", ImageFormat.Png);
                    bmap.Save(new_path + $@"\{n}_{roiX}_{roiY}.png", ImageFormat.Png);  //【add】

                    //string path_save_dictionary = "F:\\HER2\\her2tiles_size100\\0"; bmap.Save(path_save_dictionary + $@"\{n}_{roiX}_{roiY}.png", ImageFormat.Png);
                    //string path_save_dictionary = "F:\\HER2\\her2tiles_size100\\1"; bmap.Save(path_save_dictionary + $@"\{n}_{roiX}_{roiY}.png", ImageFormat.Png);
                    //string path_save_dictionary = "F:\\HEr2\\her2tiles_size100\\2"; bmap.Save(path_save_dictionary + $@"\{n}_{roiX}_{roiY}.png", ImageFormat.Png);
                    //string path_save_dictionary = "F:\\HER2\\her2tiles_size100\\3"; bmap.Save(path_save_dictionary + $@"\{n}_{roiX}_{roiY}.png", ImageFormat.Png);
                    bmap.Dispose();

                    //roiBmp.Save(path_dictionary + $@"\{n}_{roiX}_{roiY}.png", ImageFormat.Png);
                    //roiBmp.Dispose();
                    n = n + 1;
                }
                MessageBox.Show("生成成功");

            }


        }

        // 按比例缩放图片
        public Image ZoomPicture(Image SourceImage, float M, float N)
        {
            int IntWidth; //新的图片宽
            int IntHeight; //新的图片高
            int TargetWidth = (int)(M * SourceImage.Width); //取整，是因为下面的Bitmap的两个参数只能为整数
            int TargetHeight = (int)(N * SourceImage.Height);
            try
            {
                ImageFormat format = SourceImage.RawFormat;
                Bitmap SaveImage = new Bitmap(TargetWidth, TargetHeight);
                Graphics g = Graphics.FromImage(SaveImage);
                g.Clear(Color.White);

                //计算缩放图片的大小
                IntHeight = TargetHeight;
                IntWidth = TargetWidth;
                g.DrawImage(SourceImage, 0, 0, IntWidth, IntHeight); //在指定坐标处画指定大小的图片
                SourceImage.Dispose();

                return SaveImage;
            }
            catch (Exception ex)
            {

            }

            return null;
        }

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

        private void label_title_Click(object sender, EventArgs e)
        {

        }
    }
}
