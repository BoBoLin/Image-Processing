using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;//引用System.Diagnostics命名空間
using System.IO;
using System.Drawing.Imaging;

namespace img_hw
{
    public partial class Form1 : Form
    {
        int is_sobel = 0;
        public Form1()
        {
            InitializeComponent();
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
            label2.Text = "";
            label3.Text = "";
            label4.Text = "";
            label5.Text = "";
            chart1.Visible = false;
            chart2.Visible = false;

            if (Directory.Exists(@".\output"))
            {
                Console.WriteLine("the output directory existed !");
                //資料夾存在
            }
            else
            {
                //新增資料夾
                Console.WriteLine("creat the output directory !");
                Directory.CreateDirectory(@".\output");
            }

            

        }
       

        Bitmap openImg, img_R, img_G, img_B, img_Gray, tmp_overlap, tmp_erosion, tmp_dilation;
        string dir_path;
        
        //set directory path
        private void button8_Click(object sender, EventArgs e)
        {
            button7.Enabled = true;
            button5.Enabled = true;
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            DialogResult result = fbd.ShowDialog();

            if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
            {
                //string[] files = Directory.GetFiles(fbd.SelectedPath);
                Console.WriteLine("you select : " + fbd.SelectedPath + @"\");
                label6.Text = "you select : " + fbd.SelectedPath + @"\";
                dir_path = fbd.SelectedPath;
                //System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
            }
        }
        
        


        // Handle the TrackBar.ValueChanged event by calculating a value for
        // TextBox1 based on the TrackBar value.  
        private void trackBar1_ValueChanged(object sender, System.EventArgs e)
        {
            Console.WriteLine("!!!!");
            label7.Text = (System.Math.Round(trackBar1.Value / 10.0)).ToString();
        }

        string file_name = "";
        //load
        private void button1_Click(object sender, EventArgs e) 
        {
            //string[] filenames;
            //filenames = new string[100];
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
            //openFileDialog1.InitialDirectory = "C:";
            openFileDialog1.Filter = "All Files|*.*|Bitmap Files (.bmp)|*.bmp|Jpeg File(.jpg)|*.jpg";
            // 選擇我們需要開檔的類型
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            { // 如果成功開檔
                System.Console.WriteLine("load successed");
                //filenames = openFileDialog1.FileName;
                Console.WriteLine (openFileDialog1.FileName);
                file_name = Path.GetFileNameWithoutExtension(openFileDialog1.FileName);
                openImg = new Bitmap(openFileDialog1.FileName);
                img_Gray = new Bitmap(openImg.Width, openImg.Height);
                img_R = new Bitmap(openImg.Width, openImg.Height);
                img_G = new Bitmap(openImg.Width, openImg.Height);
                img_B = new Bitmap(openImg.Width, openImg.Height);
                tmp_overlap = new Bitmap(openImg.Width, openImg.Height);
                tmp_erosion = new Bitmap(openImg.Width, openImg.Height);
                tmp_dilation = new Bitmap(openImg.Width, openImg.Height);
                // 宣告存取影像的 bitmap

                // 讀取的影像展示到 pictureBox

                Console.WriteLine("pic height: " + openImg.Height);
                Console.WriteLine("pic width: " + openImg.Width);
                pictureBox1.Image = openImg;
                pictureBox2.Image = null;
                pictureBox3.Image = null;
                pictureBox4.Image = null;
                pictureBox5.Image = null;
                label2.Text = "";
                label3.Text = "";
                label4.Text = "";
                label5.Text = "";
                chart1.Visible = false;
                chart2.Visible = false;
                is_sobel = 0;
            }
        }

        //save
        private void button2_Click_1(object sender, EventArgs e) 
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "All Files|*.*|Bitmap Files (.bmp)|*.bmp|Jpeg File(.jpg)|*.jpg";

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                openImg.Save(sfd.FileName);
            }
        }
        
        //one photo detect
        private void button3_Click_1(object sender, EventArgs e) 
        {
            img_G = new Bitmap(openImg.Width, openImg.Height);
            pictureBox1.Image = openImg;

            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw = Stopwatch.StartNew();

            int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] gy = new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
            int new_grayX = 0;
            int new_grayY = 0;
            double combined = 0;
            Bitmap reoriginal = new Bitmap(openImg);
            //Bitmap img_col = new Bitmap(pictureBox1.Image);

            int[] row_total = new int[230];
            int[] row_total_sort = new int[230];
            //int[] row_total = new int[openImg.Height - 1];
            //int[] row_total_sort = new int[openImg.Height-1];
            int[] col_total = new int[400];
            int[] col_total_sort = new int[400];
            int count_row = 0;

            //-------------------find eyes row----
            int[] gray_row_total = new int[openImg.Height - 1];
            int[] low_wave = new int[openImg.Height - 1];

            for (int y = 0; y < openImg.Height - 2; y++)
            {
                for (int x = 0; x < openImg.Width - 2; x++)
                {
                    Color RGB_row = openImg.GetPixel(x, y);
                    int gray_row = (RGB_row.R + RGB_row.G + RGB_row.B) / 3;
                    gray_row_total[y] += gray_row;
                }
            }
            //Console.WriteLine(openImg.Height + "!");
            int low_w_count = 0;

            for (int tmp = 20; tmp < openImg.Height - 21; tmp++)
            {
                int tmp_count = 0;
                for (int tmp2 = 1; tmp2 < 21; tmp2++)
                {
                    if (gray_row_total[tmp] < gray_row_total[tmp - tmp2])
                        tmp_count++;
                    if (gray_row_total[tmp] < gray_row_total[tmp + tmp2])
                        tmp_count++;
                }
                if (tmp_count == 40)
                {
                    low_wave[low_w_count] = tmp;
                    low_w_count++;
                    Console.WriteLine("!! y : " + tmp);
                }
            }
            Console.WriteLine("true eyes y: " + low_wave[1]);
            eyes_y = low_wave[1];

            //-------------------find eyes row end-----

            for (int y = eyes_y - 100; y < eyes_y + 100; y++)
            //for (int y = 170; y < 400; y++)
            {
                //for (int x = 0; x < openImg.Width-2; x++)
                for (int x = 200; x < openImg.Width - 180; x++)
                {
                    new_grayX = 0;
                    new_grayY = 0;

                    for (int j = 0; j < 3; j++)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Color RGB = openImg.GetPixel(x + i, y + j);
                            int gray_rgb = (RGB.R + RGB.G + RGB.B) / 3;
                            new_grayX += gx[i, j] * gray_rgb;
                            new_grayY += gy[i, j] * gray_rgb;
                        }
                    }
                    //Console.WriteLine(new_grayX);
                    new_grayX = Math.Abs(new_grayX);
                    new_grayY = Math.Abs(new_grayY);
                    if (new_grayX > 255)
                        new_grayX = 255;
                    if (new_grayY > 255)
                        new_grayY = 255;
                    double double_new_grayX = Convert.ToDouble(new_grayX);
                    double double_new_grayY = Convert.ToDouble(new_grayY);
                    combined = Math.Sqrt(Math.Pow(double_new_grayX, 2) + Math.Pow(double_new_grayY, 2));
                    if (combined > 255)
                        combined = 255;

                    //thresholding
                    if (combined > 80)
                        combined = 255;
                    else
                        combined = 0;

                    img_G.SetPixel(x + 1, y + 1, Color.FromArgb(Convert.ToInt32(combined), Convert.ToInt32(combined), Convert.ToInt32(combined)));

                    if (x == openImg.Width / 2 / 2 + 180 - 1)
                        x = openImg.Width / 2 + 160;
                }
                count_row++;

            }

            Console.WriteLine("------sobel over-------");
            //System.Environment.Exit(System.Environment.ExitCode);

            pictureBox2.Visible = true;
            //label2.Text = "sobel";
            pictureBox2.Image = img_G;

            //------------------------test

            int[,,] acu = cal_circle(img_G, eyes_y);

            int[] degerler_x = new int[50];
            int[] degerler_y = new int[50];
            int[] degerler_yaricap = new int[50];
            int enbuyuk = 0;
            int en_x = 0, en_y = 0;
            int esik = 0;

            if (eyes_y > old_eye - 25 && eyes_y < old_eye + 25)
            {
                for (int donme = 0; donme < 2; donme++)
                {
                    enbuyuk = 1000;
                    //for (int i = 1; i < img_G.Width - 1; i++)
                    for (int i = 150; i < openImg.Width - 230; i++)
                    {
                        //for (int j = 1; j < img_G.Height - 1; j++)
                        for (int j = eyes_y - 25; j < eyes_y + 25; j++)
                        {
                            for (int r = 0; r < 11; r++)
                            {
                                if (acu[i, j, r] > enbuyuk)
                                {
                                    enbuyuk = acu[i, j, r];
                                    //Console.WriteLine(enbuyuk + "---" + i + " " + j + " " + r);
                                    en_x = i;
                                    en_y = j;
                                    degerler_x[donme] = en_x;
                                    degerler_y[donme] = en_y;
                                    degerler_yaricap[donme] = (r + 15);
                                }
                            }

                        }
                        if (i == openImg.Width / 2 / 2 + 110)
                            i = openImg.Width / 2 + 210;
                    }

                    if (enbuyuk < esik) { break; }
                    if (donme == 0)
                    {
                        //yüzde 90 benzemeli
                        esik = enbuyuk * 90 / 100;
                    }

                    for (int x = en_x - 200; x < en_x + 200; x++)
                    {
                        for (int y = en_y - 200; y < en_y + 200; y++)
                        {
                            for (int r = 0; r < 11; r++)
                            {
                                acu[x, y, r] = 0;
                            }
                        }
                    }

                }
                old_eye = eyes_y;
            }

            //Console.WriteLine("------draw circle --------");
            int r_eye_x = 0;
            int r_eye_y = 0;
            int r_eye_r = 0;
            int l_eye_x = 0;
            int l_eye_y = 0;
            int l_eye_r = 0;
            //draw circle
            for (int donme = 0; donme < 2; donme++)
            {
                if (degerler_x[donme] != 0 && degerler_y[donme] != 0)//row_total_sort[0] < 69999)
                {
                    reoriginal = draw_circle(degerler_x[donme], degerler_y[donme], degerler_yaricap[donme], reoriginal, 1);
                    if (degerler_x[donme] < openImg.Width / 2)
                    {
                        r_eye_x = degerler_x[donme];
                        r_eye_y = degerler_y[donme];
                        r_eye_r = degerler_yaricap[donme];
                    }
                    else
                    {
                        l_eye_x = degerler_x[donme];
                        l_eye_y = degerler_y[donme];
                        l_eye_r = degerler_yaricap[donme];
                    }
                    //Console.WriteLine("x : " + degerler_x[donme] + " y:" + degerler_y[donme] + " r: " + degerler_yaricap[donme]);
                }
                else
                {
                    break;
                }
            }
            StreamWriter wri = new StreamWriter(@".\output\result.txt", true);
            string w_txt_line = r_eye_r + "\t" + r_eye_x + "\t" + r_eye_y + "\t" + l_eye_r + "\t" + l_eye_x + "\t" + l_eye_y + "\n";
            label8.Text = r_eye_r + "　" + r_eye_x + "　" + r_eye_y + "　" + l_eye_r + "　" + l_eye_x + "　" + l_eye_y + "　";
            Console.WriteLine(w_txt_line);
            wri.WriteLine(w_txt_line);
            wri.Flush();
            wri.Close();

            reoriginal.Save(@".\output\" + file_name + "_r.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

            //Console.WriteLine("------put image --------");

            //--------------------------test

            //pictureBox1.Image = openImg;

            //pictureBox3.Visible = true;
            pictureBox4.Visible = true;
            //pictureBox2.Image = img_R;
            //pictureBox3.Image = img_Gray;
            pictureBox4.Image = reoriginal;
            pictureBox5.Visible = false;
            chart1.Visible = false;
            chart2.Visible = false;

            sw.Stop();
            TimeSpan el = sw.Elapsed;
            Console.WriteLine("1 花費 {0}", el);
            label7.Text = ("1 張 cost time : " + el);

            //min_total_row = 0;
        }

        List<string> ground_truth = new List<string>();
        //ground truth txt
        private void button4_Click(object sender, EventArgs e) 
        {
            openFileDialog2.Filter = "All Files|*.*|Bitmap Files (.bmp)|*.bmp|Jpeg File(.jpg)|*.jpg";
            // 選擇我們需要開檔的類型
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            { // 如果成功開檔
                Console.WriteLine("load successed");
                Console.WriteLine(openFileDialog2.FileName);
                label9.Text = openFileDialog2.FileName;
                string line;
                // Read the file and display it line by line.  
                StreamReader file = new StreamReader(openFileDialog2.FileName);
                while ((line = file.ReadLine()) != null)
                {
                    ground_truth.Add(line);
                }
                file.Close();
            }
            /*for (int i = 0; i < ground_truth.Count; i++)
            {
                Console.WriteLine((i + 1) + " line : " + ground_truth[i]);
            }*/
        }
        
        public static Bitmap CreateGrayscaleImage(int width, int height)
        {
            // create new image  
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            // set palette to grayscale  
            SetGrayscalePalette(bmp);
            // return new image  
            return bmp;
        }

        public static void SetGrayscalePalette(Bitmap srcImg)
        {
            // check pixel format  
            if (srcImg.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new ArgumentException();
            // get palette  
            ColorPalette cp = srcImg.Palette;
            // init palette  
            for (int i = 0; i < 256; i++)
            {
                cp.Entries[i] = Color.FromArgb(i, i, i);
            }
            srcImg.Palette = cp;
        }
        
        //lock memory
        public static int[,,] cal_circle(Bitmap srcBitmap, int eyes_y)
        {
            int wide = srcBitmap.Width;
            int height = srcBitmap.Height;
            Rectangle rect = new Rectangle(0, 0, wide, height);

            //將Bitmap鎖定到系統內存中,獲得BitmapData  
            System.Drawing.Imaging.BitmapData srcBmData = srcBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            //創建Bitmap  
            Bitmap dstBitmap = CreateGrayscaleImage(wide, height);//這個函數在後面有定義  
            //BitmapData dstBmData = dstBitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            //位圖中第一個像素數據的地址。它也可以看成是位圖中的第一個掃描行  
            System.IntPtr srcPtr = srcBmData.Scan0;
           // System.IntPtr dstPtr = dstBmData.Scan0;

            //將Bitmap對象的信息存放到byte數組中  
            int src_bytes = srcBmData.Stride * height;
            byte[] srcValues = new byte[src_bytes];
            //int dst_bytes = dstBmData.Stride * height;
            //byte[] dstValues = new byte[dst_bytes];

            //復制GRB信息到byte數組  
            System.Runtime.InteropServices.Marshal.Copy(srcPtr, srcValues, 0, src_bytes);
            //System.Runtime.InteropServices.Marshal.Copy(dstPtr, dstValues, 0, dst_bytes);
            
            int x0, y0;
            double t;
            int real_r = 0;
            int[,,] acu = new int[wide, height, 11];
            Console.WriteLine("------put circle --------");            
            for (int i = 250; i < 1050; i++)
            {
                for (int j = eyes_y - 25; j < eyes_y + 25; j++)
                {
                    //r = 25 ~ 45
                    for (int r = 0; r < 11; r++)
                    {
                        for (int theta = 0; theta < 360; theta += 10)
                        {
                            real_r = (r + 15);
                            t = ((double)theta * 3.14) / 180;
                            x0 = i + (int)(real_r * Math.Cos(t));
                            y0 = j + (int)(real_r * Math.Sin(t));
                            int k = 3 * x0;
                            if (x0 > 0 && y0 > 200 && y0 < 400 && x0 < wide)
                            {
                                acu[i, j, r] += (byte)(srcValues[y0 * srcBmData.Stride + k + 2]);
                            }
                        }//end for theta
                    }//end for r
                }
                if (i == 450)
                    i = 850;
            }
            
            Console.WriteLine("------all circle over --------");
            //System.Runtime.InteropServices.Marshal.Copy(dstValues, 0, dstPtr, dst_bytes);
            //解鎖位圖  
            srcBitmap.UnlockBits(srcBmData);
            //dstBitmap.UnlockBits(dstBmData);
            return acu;
           // return dstBitmap;
        }
        //lock end ----------
        
        //fast detect eyes
        private void button5_Click(object sender, EventArgs e)
        {
            List<string> myList = new List<string>();

            // 執行檔路徑下的 MyDir 資料夾
            string folderName = dir_path + @"\";
            //string folderName = @"D:\研究所\影像處理\hw\final project\test img\";
            // 取得資料夾內所有檔案
            
            foreach (string fname in System.IO.Directory.GetFiles(folderName))
            {
                string[] s_line;
                //Console.WriteLine(fname);
                s_line = fname.Split('.');
                if (s_line[1] == "bmp" || s_line[1] == "jpg")
                {
                    myList.Add(fname);
                }
            }
            Console.WriteLine("list length: " + myList.Count);
            trackBar1.Maximum = myList.Count;

            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw = Stopwatch.StartNew();
            for (int data = 0; data < myList.Count; data++)
            {
                file_name = Path.GetFileNameWithoutExtension(myList[data]);
                openImg = new Bitmap(myList[data]);
                img_G = new Bitmap(openImg.Width, openImg.Height);
                //pictureBox1.Image = openImg;

                int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
                int[,] gy = new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
                int new_grayX = 0;
                int new_grayY = 0;
                double combined = 0;
                Bitmap reoriginal = new Bitmap(openImg);
                //Bitmap img_col = new Bitmap(pictureBox1.Image);

                int[] row_total = new int[230];
                int[] row_total_sort = new int[230];
                //int[] row_total = new int[openImg.Height - 1];
                //int[] row_total_sort = new int[openImg.Height-1];
                int[] col_total = new int[400];
                int[] col_total_sort = new int[400];
                int count_row = 0;
                
                //-------------------find eyes row----
                int[] gray_row_total = new int[openImg.Height - 1];
                int[] low_wave = new int[openImg.Height - 1];

                for (int y = 0; y < openImg.Height - 2; y++)
                {
                    for (int x = 0; x < openImg.Width - 2; x++)
                    {
                        Color RGB_row = openImg.GetPixel(x, y);
                        int gray_row = (RGB_row.R + RGB_row.G + RGB_row.B) / 3;
                        gray_row_total[y] += gray_row;
                    }
                }
                //Console.WriteLine(openImg.Height + "!");
                int low_w_count = 0;

                for (int tmp = 20; tmp < openImg.Height - 21; tmp++)
                {
                    int tmp_count = 0;
                    for (int tmp2 = 1; tmp2 < 21; tmp2++)
                    {
                        if (gray_row_total[tmp] < gray_row_total[tmp - tmp2])
                            tmp_count++;
                        if (gray_row_total[tmp] < gray_row_total[tmp + tmp2])
                            tmp_count++;
                    }
                    if (tmp_count == 40)
                    {
                        low_wave[low_w_count] = tmp;
                        low_w_count++;
                        //Console.WriteLine("!! y : " + tmp);
                    }
                }
                Console.WriteLine("true eyes y: " + low_wave[1]);
                eyes_y = low_wave[1];

                //-------------------find eyes row end-----

                for (int y = eyes_y - 100; y < eyes_y + 100; y++)
                //for (int y = 170; y < 400; y++)
                {
                    //for (int x = 0; x < openImg.Width-2; x++)
                    for (int x = 200; x < openImg.Width - 180; x++)
                    {
                        new_grayX = 0;
                        new_grayY = 0;
                        
                        for (int j = 0; j < 3; j++)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Color RGB = openImg.GetPixel(x + i, y + j);
                                int gray_rgb = (RGB.R + RGB.G + RGB.B) / 3;
                                new_grayX += gx[i, j] * gray_rgb;
                                new_grayY += gy[i, j] * gray_rgb;
                            }
                        }
                        //Console.WriteLine(new_grayX);
                        new_grayX = Math.Abs(new_grayX);
                        new_grayY = Math.Abs(new_grayY);
                        if (new_grayX > 255)
                            new_grayX = 255;
                        if (new_grayY > 255)
                            new_grayY = 255;
                        double double_new_grayX = Convert.ToDouble(new_grayX);
                        double double_new_grayY = Convert.ToDouble(new_grayY);
                        combined = Math.Sqrt(Math.Pow(double_new_grayX, 2) + Math.Pow(double_new_grayY, 2));
                        if (combined > 255)
                            combined = 255;

                        //thresholding
                        if (combined > 80)
                            combined = 255;
                        else
                            combined = 0;
                        
                        img_G.SetPixel(x + 1, y + 1, Color.FromArgb(Convert.ToInt32(combined), Convert.ToInt32(combined), Convert.ToInt32(combined)));

                        if (x == openImg.Width / 2 / 2 + 180 - 1)
                            x = openImg.Width / 2 + 160;
                    }
                    count_row++;

                }
                
                Console.WriteLine("------sobel over-------");
                //System.Environment.Exit(System.Environment.ExitCode);

                //pictureBox2.Visible = true;
                //label2.Text = "sobel";
                //pictureBox2.Image = img_G;

                //------------------------test
                
                int[,,] acu = cal_circle(img_G, eyes_y);
                
                int[] degerler_x = new int[50];
                int[] degerler_y = new int[50];
                int[] degerler_yaricap = new int[50];
                int enbuyuk = 0;
                int en_x = 0, en_y = 0;
                int esik = 0;

                if (eyes_y > old_eye - 25 && eyes_y < old_eye + 25)
                {
                    for (int donme = 0; donme < 2; donme++)
                    {
                        enbuyuk = 1000;
                        //for (int i = 1; i < img_G.Width - 1; i++)
                        for (int i = 150; i < openImg.Width - 230 ; i++)
                        {
                            //for (int j = 1; j < img_G.Height - 1; j++)
                            for (int j = eyes_y - 25; j < eyes_y + 25; j++)
                            {
                                for (int r = 0; r < 11; r++)
                                {
                                    if (acu[i, j, r] > enbuyuk)
                                    {
                                        enbuyuk = acu[i, j, r];
                                        //Console.WriteLine(enbuyuk + "---" + i + " " + j + " " + r);
                                        en_x = i;
                                        en_y = j;
                                        degerler_x[donme] = en_x;
                                        degerler_y[donme] = en_y;
                                        degerler_yaricap[donme] = (r + 15);
                                    }
                                }

                            }
                            if (i == openImg.Width / 2 / 2 + 110)
                                i = openImg.Width / 2 + 210;
                        }

                        if (enbuyuk < esik) { break; }
                        if (donme == 0)
                        {
                            //yüzde 90 benzemeli
                            esik = enbuyuk * 90 / 100;
                        }

                        for (int x = en_x - 200; x < en_x + 200; x++)
                        {
                            for (int y = en_y - 200; y < en_y + 200; y++)
                            {
                                for (int r = 0; r < 11; r++)
                                {
                                    acu[x, y, r] = 0;
                                }
                            }
                        }
                        
                    }
                    old_eye = eyes_y;
                }
                
                //Console.WriteLine("------draw circle --------");
                int r_eye_x = 0;
                int r_eye_y = 0;
                int r_eye_r = 0;
                int l_eye_x = 0;
                int l_eye_y = 0;
                int l_eye_r = 0;
                //draw circle
                for (int donme = 0; donme < 2; donme++)
                {
                    if (degerler_x[donme] != 0 && degerler_y[donme] != 0)//row_total_sort[0] < 69999)
                    {
                        string[] spl_ground = new string[6];
                        spl_ground = ground_truth[data].Split('\t');
                        reoriginal = draw_circle(Int32.Parse(spl_ground[1]), Int32.Parse(spl_ground[2]), Int32.Parse(spl_ground[0]), reoriginal, 0);
                        reoriginal = draw_circle(Int32.Parse(spl_ground[4]), Int32.Parse(spl_ground[5]), Int32.Parse(spl_ground[3]), reoriginal, 0);

                        reoriginal = draw_circle(degerler_x[donme], degerler_y[donme], degerler_yaricap[donme], reoriginal, 1);
                        if (degerler_x[donme] < openImg.Width / 2)
                        {
                            r_eye_x = degerler_x[donme];
                            r_eye_y = degerler_y[donme];
                            r_eye_r = degerler_yaricap[donme];
                        }
                        else
                        {
                            l_eye_x = degerler_x[donme];
                            l_eye_y = degerler_y[donme];
                            l_eye_r = degerler_yaricap[donme];
                        }
                        //Console.WriteLine("x : " + degerler_x[donme] + " y:" + degerler_y[donme] + " r: " + degerler_yaricap[donme]);
                    }
                    else
                    {
                        break;
                    }
                }
                StreamWriter wri = new StreamWriter(@".\output\result.txt", true);
                string w_txt_line = r_eye_r + "\t" + r_eye_x + "\t" + r_eye_y + "\t" + l_eye_r + "\t" + l_eye_x + "\t" + l_eye_y + "\n";
                Console.WriteLine(w_txt_line);
                wri.WriteLine(w_txt_line);
                wri.Flush();
                wri.Close();

                reoriginal.Save(@".\output\" + file_name + "_r.bmp", System.Drawing.Imaging.ImageFormat.Bmp);

                //Console.WriteLine("------put image --------");

                //--------------------------test

                //pictureBox1.Image = openImg;

                //pictureBox3.Visible = true;
                //pictureBox4.Visible = true;
                //pictureBox2.Image = img_R;
                //pictureBox3.Image = img_Gray;
                //pictureBox4.Image = reoriginal;
                pictureBox5.Visible = false;
                chart1.Visible = false;
                chart2.Visible = false;
                //openImg = new Bitmap(img_G);
                //is_sobel = 1;

                //Console.WriteLine(el);

                //Console.Write(bmp_count + " data completed !");
                bmp_count++;
                //min_total_row = 0;
            }
            sw.Stop();
            TimeSpan el = sw.Elapsed;
            Console.WriteLine(bmp_count - 1 + " 花費 {0}", el);
            label7.Text = (bmp_count - 1 + "張 cost time : " + el);
            button11.Enabled = true;

        }

        //Thresholding
        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = true;
            if (is_sobel == 1)
            {
                for (int y = 0; y < openImg.Height; y++)
                {
                    for (int x = 0; x < openImg.Width; x++)
                    {
                        Color RGB = openImg.GetPixel(x, y);
                        img_B.SetPixel(x, y, Color.FromArgb(RGB.R, RGB.G, RGB.B));
                        tmp_overlap.SetPixel(x, y, Color.FromArgb(RGB.R, RGB.G, RGB.B));
                    }
                }

                label2.Text = "Overlap";
                label3.Text = "Overlap by different colors";
                label4.Text = "Thresholding";
                pictureBox2.Visible = true;
                pictureBox2.Image = tmp_overlap;
                pictureBox4.Visible = true;
                pictureBox4.Image = img_G;
            }
            else
            {
                label2.Text = "Thresholding";
                label3.Text = "";
                label4.Text = "";
                pictureBox2.Visible = true;
                pictureBox2.Image = img_Gray;
                pictureBox4.Image = null;
            }
            
            label5.Text = "";
            pictureBox3.Image = null;
            pictureBox5.Image = null;
            chart1.Visible = false;
            chart2.Visible = false;
        }

        //set thresholding pic as source
        private void button10_Click(object sender, EventArgs e)
        {
            if(is_sobel ==1)
                openImg = new Bitmap(img_R);
            else
                openImg = new Bitmap(img_Gray);
            pictureBox1.Image = openImg;
            button10.Enabled = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button10.Enabled = true;
            int t = 0;
            if (int.TryParse(textBox1.Text, out t))
            {
                Console.WriteLine(t);
                
                for (int y = 0; y < openImg.Height; y++)
                {
                    for (int x = 0; x < openImg.Width; x++)
                    {
                        if (is_sobel == 1)
                        {
                            Color RGB_sobel = img_G.GetPixel(x, y);
                            Color RGB_origin = openImg.GetPixel(x, y);
                            int gray_rgb = (RGB_sobel.R + RGB_sobel.G + RGB_sobel.B) / 3;
                            int origin_rgb = (RGB_origin.R + RGB_origin.G + RGB_origin.B) / 3;
                            if (gray_rgb >= t)
                            {
                                img_R.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                                tmp_overlap.SetPixel(x, y, Color.FromArgb(255, 255, 255)); //原圖overlap白色
                                img_B.SetPixel(x, y, Color.Green); //原圖overlap綠色
                            }
                            else
                            {
                                tmp_overlap.SetPixel(x, y, Color.FromArgb(origin_rgb, origin_rgb, origin_rgb));
                                img_B.SetPixel(x, y, Color.FromArgb(origin_rgb, origin_rgb, origin_rgb));
                                img_R.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                            }
                        }
                        else
                        {
                            Color RGB = openImg.GetPixel(x, y);
                            int gray_rgb = (RGB.R + RGB.G + RGB.B) / 3;
                            if (gray_rgb >= t)
                                img_Gray.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                            else
                                img_Gray.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                        }
                        
                    }
                }
            }
            else
                Console.WriteLine("not int!");
            if (is_sobel == 1)
            {
                pictureBox1.Image = openImg;
                pictureBox4.Visible = true;
                pictureBox4.Image = img_R;
                pictureBox2.Visible = true;
                pictureBox2.Image = tmp_overlap;
                pictureBox3.Visible = true;
                pictureBox3.Image = img_B;
                pictureBox5.Visible = false;
            }
            else
            {
                pictureBox1.Image = openImg;
                pictureBox2.Visible = true;
                pictureBox2.Image = img_Gray;
                pictureBox3.Visible = false;
                pictureBox4.Visible = false;
                pictureBox5.Visible = false;
                
            }
            //this.pictureBox2.Size = new System.Drawing.Size(512, 512);
        }

        

        public Bitmap draw_circle(int a, int b, int r, Bitmap resimimiz, int color)
        {
            int x0, y0;
            double t;

            for (int theta = 0; theta < 360; theta++)
            {
                t = ((double)theta * 3.141592653) / 180;
                x0 = a - (int)(r * Math.Cos(t));
                y0 = b - (int)(r * Math.Sin(t));
                if (x0 > 0 && y0 > 0 && y0 < resimimiz.Height && x0 < resimimiz.Width)
                {
                    if(color == 1)
                        resimimiz.SetPixel(x0, y0, Color.Red);
                    else
                        resimimiz.SetPixel(x0, y0, Color.Yellow);
                }
            }
            return resimimiz;
        }

        int bmp_count = 1;
        //int min_total_row = 0;
        int eyes_y = 0;
        int old_eye = 250;

        //slow eyes detection
        private void button7_Click(object sender, EventArgs e)
        {
            List<string> myList = new List<string>();

            // 執行檔路徑下的 MyDir 資料夾
            string folderName = dir_path + @"\";
            //string folderName = @"D:\研究所\影像處理\hw\final project\test img\";
            // 取得資料夾內所有檔案
            foreach (string fname in System.IO.Directory.GetFiles(folderName))
            {
                string[] s_line;
                //Console.WriteLine(fname);
                s_line = fname.Split('.');
                if (s_line[1] == "bmp" || s_line[1] == "jpg")
                {
                    myList.Add(fname);
                }

            }
            Console.WriteLine("list length: " + myList.Count);
            Console.WriteLine(myList[myList.Count - 1]);
            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw = Stopwatch.StartNew();
            for (int data = 0; data < myList.Count; data++)
            {
                file_name = Path.GetFileNameWithoutExtension(myList[data]);
                openImg = new Bitmap(myList[data]);
                img_G = new Bitmap(openImg.Width, openImg.Height);
                pictureBox1.Image = openImg;
                
                int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
                int[,] gy = new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
                int new_grayX = 0;
                int new_grayY = 0;
                double combined = 0;
                Bitmap reoriginal = new Bitmap(openImg);
                //Bitmap img_col = new Bitmap(pictureBox1.Image);

                int[] row_total = new int[230];
                int[] row_total_sort = new int[230];
                //int[] row_total = new int[openImg.Height - 1];
                //int[] row_total_sort = new int[openImg.Height-1];
                int[] col_total = new int[400];
                int[] col_total_sort = new int[400];
                int count_row = 0;
                //int count_col = 0;

                int index, index1, index2, index3;
                //int index_col, index_col1, index_col2, index_col3;

                //-------------------find eyes row----
                int[] gray_row_total = new int[openImg.Height - 1];
                int[] low_wave = new int[openImg.Height - 1];

                for (int y = 0; y < openImg.Height - 2; y++)
                {
                    for (int x = 0; x < openImg.Width - 2; x++)
                    {
                        Color RGB_row = openImg.GetPixel(x, y);
                        int gray_row = (RGB_row.R + RGB_row.G + RGB_row.B) / 3;
                        gray_row_total[y] += gray_row;

                    }
                }
                //Console.WriteLine(openImg.Height + "!");
                int low_w_count = 0;

                for (int tmp = 20; tmp < openImg.Height - 21; tmp++)
                {
                    int tmp_count = 0;
                    for (int tmp2 = 1; tmp2 < 21; tmp2++)
                    {
                        if (gray_row_total[tmp] < gray_row_total[tmp - tmp2])
                            tmp_count++;
                        if (gray_row_total[tmp] < gray_row_total[tmp + tmp2])
                            tmp_count++;
                    }
                    if (tmp_count == 40)
                    {
                        low_wave[low_w_count] = tmp;
                        low_w_count++;
                        Console.WriteLine("!! y : " + tmp);
                    }
                }
                Console.WriteLine("true eyes y: " + low_wave[1]);
                eyes_y = low_wave[1];

                //-------------------find eyes row end-----

                for (int y = eyes_y - 100; y < eyes_y + 100; y++)
                //for (int y = 170; y < 400; y++)
                {
                    //for (int x = 0; x < openImg.Width-2; x++)
                    for (int x = 200; x < 1100; x++)
                    {
                        new_grayX = 0;
                        new_grayY = 0;
                        Color RGB_row = openImg.GetPixel(x, y);
                        int gray_row = (RGB_row.R + RGB_row.G + RGB_row.B) / 3;
                        
                        //row_total[count_row] += gray_row;
                        //row_total_sort[count_row] += gray_row;

                        for (int j = 0; j < 3; j++)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Color RGB = openImg.GetPixel(x + i, y + j);
                                int gray_rgb = (RGB.R + RGB.G + RGB.B) / 3;

                                new_grayX += gx[i, j] * gray_rgb;
                                new_grayY += gy[i, j] * gray_rgb;
                            }

                        }
                        //Console.WriteLine(new_grayX);
                        new_grayX = Math.Abs(new_grayX);
                        new_grayY = Math.Abs(new_grayY);
                        if (new_grayX > 255)
                            new_grayX = 255;
                        if (new_grayY > 255)
                            new_grayY = 255;
                        double double_new_grayX = Convert.ToDouble(new_grayX);
                        double double_new_grayY = Convert.ToDouble(new_grayY);
                        combined = Math.Sqrt(Math.Pow(double_new_grayX, 2) + Math.Pow(double_new_grayY, 2));
                        if (combined > 255)
                            combined = 255;

                        //thresholding
                        if (combined > 80)
                            combined = 255;
                        else
                            combined = 0;

                        //img_Gray.SetPixel(x + 1, y + 1, Color.FromArgb(new_grayX, new_grayX, new_grayX));
                        //img_R.SetPixel(x + 1, y + 1, Color.FromArgb(new_grayY, new_grayY, new_grayY));
                        img_G.SetPixel(x + 1, y + 1, Color.FromArgb(Convert.ToInt32(combined), Convert.ToInt32(combined), Convert.ToInt32(combined)));

                        //if (new_grayX*new_grayX < 128*128)
                        //  img_Gray.SetPixel(x+1, y+1, Color.Black);
                        //else
                        //  img_Gray.SetPixel(x+1, y+1, Color.Transparent);

                        if (x == 499)
                            x = 800;
                    }
                    count_row++;

                }

                /*for (int x = 200; x < 600; x++)
                {
                    for (int y = 170; y < 400; y++)
                    {
                        Color RGB_col = img_col.GetPixel(x, y);
                        int gray_col = (RGB_col.R + RGB_col.G + RGB_col.B) / 3;

                        col_total[count_col] += gray_col;
                        col_total_sort[count_col] += gray_col;

                    }
                    count_col++;
                }*/
                
                //Array.Sort(row_total_sort);

                // Array.Sort(col_total_sort);
                // Console.WriteLine("row--------------");
                //Console.WriteLine("0: " + row_total_sort[0]);
                //Console.WriteLine("1: " + row_total_sort[1]);
                //Console.WriteLine("2: " + row_total_sort[2]);
                /*Console.WriteLine("col--------------");
                Console.WriteLine("0: " + col_total_sort[0]);
                Console.WriteLine("1: " + col_total_sort[1]);
                Console.WriteLine("2: " + col_total_sort[2]);*/
                // Console.WriteLine("min: " + row_total.Min());

                //min_total_row = row_total_sort[0];

                //----row
                /*for (index1 = 0; index1 < row_total.Length; index1++)
                {
                    if (row_total[index1] == row_total_sort[0])
                    {
                        index1 += 170;
                        break;
                    }
                }
                for (index2 = 0; index2 < row_total.Length; index2++)
                {
                    if (row_total[index2] == row_total_sort[1])
                    {
                        index2 += 170;
                        break;
                    }
                }
                for (index3 = 0; index3 < row_total.Length; index3++)
                {
                    if (row_total[index3] == row_total_sort[2])
                    {
                        index3 += 170;
                        break;
                    }
                }*/

                //----col
                /*
                for (index_col1 = 0; index_col1 < col_total.Length; index_col1++)
                {
                    if (col_total[index_col1] == col_total_sort[0])
                    {
                        index_col1 += 200;
                        break;
                    }
                }
                for (index_col2 = 0; index_col2 < col_total.Length; index_col2++)
                {
                    if (col_total[index_col2] == col_total_sort[1])
                    {
                        index_col2 += 200;
                        break;
                    }
                }
                for (index_col3 = 0; index_col3 < col_total.Length; index_col3++)
                {
                    if (col_total[index_col3] == col_total_sort[2])
                    {
                        index_col3 += 200;
                        break;
                    }
                }*/
                //Console.WriteLine("!!!! " + col_total[150]);
                //Console.WriteLine("1 min row: " + index1);
                // Console.WriteLine("2 min row: " + index2);
                //Console.WriteLine("3 min row: " + index3);

                //index = (index1 + index2 + index3) / 3;

                //Console.WriteLine("avg min row: " + index);

                /*Console.WriteLine("1 min col: " + index_col1);
                Console.WriteLine("2 min col: " + index_col2);
                Console.WriteLine("3 min col: " + index_col3);
                index_col = (index_col1 + index_col2 + index_col3) / 3;
                Console.WriteLine("avg min col: " + index_col);*/

                Console.WriteLine("------sobel over-------");
                //System.Environment.Exit(System.Environment.ExitCode);

                //pictureBox2.Visible = true;
                //label2.Text = "sobel";
                //pictureBox2.Image = img_G;

                //------------------------test
                int x0, y0;
                double t;
                int real_r = 0;

                int[,,] acu = new int[img_G.Width, img_G.Height, 11];
                Console.WriteLine("------put circle --------");


                //for (int i = 1; i < img_G.Width - 1; i++)
                for (int i = 250; i < 1050; i++)
                {
                    //for (int j = 1; j < img_G.Height - 1; j++)
                    for (int j = eyes_y - 25; j < eyes_y + 25; j++)
                    {
                        //r = 25 ~ 45
                        for (int r = 0; r < 11; r++)
                        {
                            //Console.WriteLine("i: " + i + " j: " + j + " r: " + r);
                            for (int theta = 0; theta < 360; theta += 10)
                            {
                                real_r = (r + 15);
                                t = ((double)theta * 3.14) / 180;
                                x0 = i + (int)(real_r * Math.Cos(t));
                                y0 = j + (int)(real_r * Math.Sin(t));
                                if (x0 > 0 && y0 > 200 && y0 < 400 && x0 < img_G.Width)
                                {
                                    acu[i, j, r] += img_G.GetPixel(x0, y0).R;
                                }
                            }//end for theta
                             //Console.WriteLine(acu[i, j, r]);

                        }//end for r

                    }
                    if (i == 450)
                        i = 850;
                }
                
                Console.WriteLine("------all circle over --------");

                int[] degerler_x = new int[50];
                int[] degerler_y = new int[50];
                int[] degerler_yaricap = new int[50];
                int enbuyuk = 0;
                int en_x = 0, en_y = 0;

                int esik = 0;

                if (eyes_y > old_eye-25 && eyes_y < old_eye +25)
                {
                    for (int donme = 0; donme < 2; donme++)
                    {
                        enbuyuk = 1000;
                        //for (int i = 1; i < img_G.Width - 1; i++)
                        for (int i = 150; i < 1050; i++)
                        {
                            //for (int j = 1; j < img_G.Height - 1; j++)
                            for (int j = eyes_y - 25; j < eyes_y + 25; j++)
                            {
                                for (int r = 0; r < 11; r++)
                                {
                                    if (acu[i, j, r] > enbuyuk)
                                    {
                                        enbuyuk = acu[i, j, r];
                                        //Console.WriteLine(enbuyuk + "---" + i + " " + j + " " + r);
                                        en_x = i;
                                        en_y = j;
                                        degerler_x[donme] = en_x;
                                        degerler_y[donme] = en_y;
                                        degerler_yaricap[donme] = (r + 15);
                                    }
                                }

                            }
                            if (i == 430)
                                i = 850;
                        }

                        if (enbuyuk < esik) { break; }
                        if (donme == 0)
                        {
                            //yüzde 90 benzemeli
                            esik = enbuyuk * 90 / 100;
                        }

                        for (int x = en_x - 200; x < en_x + 200; x++)
                        {
                            for (int y = en_y - 200; y < en_y + 200; y++)
                            {
                                for (int r = 0; r < 11; r++)
                                {
                                    acu[x, y, r] = 0;
                                }
                            }
                        }

                        /*for (int theta = 0; theta < 360; theta++)
                        {
                            for (int tarama_r = 0; tarama_r <= degerler_yaricap[donme]+100; tarama_r++)
                            {

                                t = ((double)theta * 3.14) / 180;
                                x0 = en_x - (int)(tarama_r * Math.Cos(t));
                                y0 = en_y - (int)(tarama_r * Math.Sin(t));

                                if (x0 > 0 && y0 > 200 && y0 < 300 && x0 < img_G.Width)
                                {
                                    acu[x0, y0, 0] = 0;
                                    acu[x0, y0, 1] = 0;
                                    acu[x0, y0, 2] = 0;
                                    acu[x0, y0, 3] = 0;
                                    acu[x0, y0, 4] = 0;
                                    acu[x0, y0, 5] = 0;
                                    acu[x0, y0, 6] = 0;
                                    acu[x0, y0, 7] = 0;
                                    acu[x0, y0, 8] = 0;
                                    acu[x0, y0, 9] = 0;
                                    acu[x0, y0, 10] = 0;
                                }
                            }
                        }*/

                    }
                    old_eye = eyes_y;
                }
                

                //Console.WriteLine("------draw circle --------");
                int r_eye_x = 0;
                int r_eye_y = 0;
                int r_eye_r = 0;
                int l_eye_x = 0;
                int l_eye_y = 0;
                int l_eye_r = 0;
                //draw circle
                for (int donme = 0; donme < 2; donme++)
                {
                    if (degerler_x[donme] != 0 && degerler_y[donme] != 0)//row_total_sort[0] < 69999)
                    {
                        string[] spl_ground = new string[6];
                        spl_ground = ground_truth[data].Split('\t');
                        reoriginal = draw_circle(Int32.Parse(spl_ground[1]), Int32.Parse(spl_ground[2]), Int32.Parse(spl_ground[0]), reoriginal, 0);
                        reoriginal = draw_circle(Int32.Parse(spl_ground[4]), Int32.Parse(spl_ground[5]), Int32.Parse(spl_ground[3]), reoriginal, 0);
                        reoriginal = draw_circle(degerler_x[donme], degerler_y[donme], degerler_yaricap[donme], reoriginal, 1);
                        if (degerler_x[donme] < openImg.Width / 2)
                        {
                            r_eye_x = degerler_x[donme];
                            r_eye_y = degerler_y[donme];
                            r_eye_r = degerler_yaricap[donme];
                        }
                        else
                        {
                            l_eye_x = degerler_x[donme];
                            l_eye_y = degerler_y[donme];
                            l_eye_r = degerler_yaricap[donme];
                        }
                        //Console.WriteLine("x : " + degerler_x[donme] + " y:" + degerler_y[donme] + " r: " + degerler_yaricap[donme]);

                    }

                    else
                    {
                        break;
                    }
                }
                StreamWriter wri = new StreamWriter(@".\output\result.txt", true);
                string w_txt_line = r_eye_r + "\t" + r_eye_x + "\t" + r_eye_y + "\t" + l_eye_r + "\t" + l_eye_x + "\t" + l_eye_y + "\n";
                Console.WriteLine(w_txt_line);
                wri.WriteLine(w_txt_line);
                wri.Flush();
                wri.Close();

                reoriginal.Save(@".\output\" + file_name + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);

                //Console.WriteLine("------put image --------");

                //--------------------------test

                //pictureBox1.Image = openImg;

                //pictureBox3.Visible = true;
                //pictureBox4.Visible = true;
                //pictureBox2.Image = img_R;
                //pictureBox3.Image = img_Gray;
                //pictureBox4.Image = reoriginal;
                pictureBox5.Visible = false;
                chart1.Visible = false;
                chart2.Visible = false;
                //openImg = new Bitmap(img_G);
                //is_sobel = 1;

                //Console.WriteLine(el);

                //Console.Write(bmp_count + " data completed !");
                bmp_count++;
                //min_total_row = 0;
            }
            sw.Stop();
            TimeSpan el = sw.Elapsed;
            Console.WriteLine(bmp_count - 1 + " 花費 {0}", el);
            label7.Text = (bmp_count - 1 + "張 cost time : " + el);
            button11.Enabled = true;

        }


        //start to see result
        List<string> ori_list = new List<string>();
        List<string> res_list = new List<string>();
        private void button11_Click(object sender, EventArgs e)
        {
            trackBar1.Enabled = true;
            button11.Enabled = false;

            string folderName = dir_path + @"\";
            //string folderName = @"D:\研究所\影像處理\hw\final project\test img\";
            // 取得資料夾內所有檔案
            foreach (string fname in System.IO.Directory.GetFiles(folderName))
            {
                string[] s_line;
                //Console.WriteLine(fname);
                s_line = fname.Split('.');
                if (s_line[1] == "bmp" || s_line[1] == "jpg")
                {
                    ori_list.Add(fname);
                }
            }
            Console.WriteLine("ori length: " + ori_list.Count);

            foreach (string fname in System.IO.Directory.GetFiles(@".\output\"))
            {
                string[] s_line;
                s_line = fname.Split('.');
                if (s_line[2] == "bmp" || s_line[2] == "jpg")
                {
                    res_list.Add(fname);
                    Console.WriteLine(fname);
                }
            }
            Console.WriteLine("res length: " + res_list.Count);
            trackBar1.Maximum = res_list.Count;

            label7.Text = trackBar1.Value.ToString();
            openImg = new Bitmap(ori_list[0]);
            img_B = new Bitmap(res_list[0]);

            pictureBox1.Image = openImg;
            pictureBox4.Image = img_B;

            //openImg = new Bitmap(ori[data]);

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label7.Text = trackBar1.Value.ToString() + "/" + res_list.Count;
            openImg = new Bitmap(ori_list[trackBar1.Value - 1]);
            img_B = new Bitmap(res_list[trackBar1.Value - 1]);

            pictureBox1.Image = openImg;
            pictureBox4.Image = img_B;
        }

        //Morphological operator
        private void button9_Click(object sender, EventArgs e)
        {
            int ero_flag = 0;
            int dil_flag = 0;

            for (int y = 0; y < openImg.Height; y++)
            {
                for (int x = 0; x < openImg.Width; x++)
                {
                    Color RGB = img_R.GetPixel(x, y);
                    int gray_rgb = (RGB.R + RGB.G + RGB.B) / 3;
                    tmp_erosion.SetPixel(x, y, Color.FromArgb(gray_rgb, gray_rgb, gray_rgb));
                    tmp_dilation.SetPixel(x, y, Color.FromArgb(gray_rgb, gray_rgb, gray_rgb));
                }
            }

            for (int y = 0; y < (openImg.Height - 2); y++)
            {
                for (int x = 0; x < (openImg.Width - 2); x++)
                {
                    Color RGB_center = img_R.GetPixel(x + 1, y + 1);
                    ero_flag = 0;
                    dil_flag = 0;

                    if (RGB_center.R == 255) //如果九宮格中間是白色的
                    {
                        for (int j = y; j < y + 3; j++)
                        {
                            for (int i = x; i < x + 3; i++)
                            {
                                Color RGB = img_R.GetPixel(i, j);
                                if (RGB.R == 0)
                                    ero_flag += 1;
                            }
                        }
                    }
                    if (ero_flag > 0)
                        tmp_erosion.SetPixel(x + 1, y + 1, Color.FromArgb(0, 0, 0));

                    if (RGB_center.R == 0)
                    {
                        for (int j = y; j < y + 3; j++)
                        {
                            for (int i = x; i < x + 3; i++)
                            {
                                Color RGB = img_R.GetPixel(i, j);
                                if (RGB.R == 255)
                                    dil_flag += 1;

                            }
                        }
                    }
                    if (dil_flag > 0)
                        tmp_dilation.SetPixel(x + 1, y + 1, Color.White);
                    
                }
            }
            label2.Text = "Erosion";
            label3.Text = "Dilation";
            label4.Text = "Thresholding";
            label5.Text = "";
            pictureBox2.Visible = true;
            pictureBox3.Visible = true;
            pictureBox4.Visible = true;
            pictureBox2.Image = tmp_erosion;
            pictureBox3.Image = tmp_dilation;
            pictureBox4.Image = img_R;
            pictureBox5.Visible = false;
            chart1.Visible = false;
            chart2.Visible = false;

        }


    }
}
