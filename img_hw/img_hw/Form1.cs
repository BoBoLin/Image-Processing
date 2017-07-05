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

namespace img_hw
{
    public partial class Form1 : Form
    {
        

        int is_sobel = 0;
        public Form1()
        {
            
            InitializeComponent();
            label2.Text = "";
            label3.Text = "";
            label4.Text = "";
            label5.Text = "";
            chart1.Visible = false;
            chart2.Visible = false;
        }

        Bitmap openImg, img_R, img_G, img_B, img_Gray, tmp_overlap, tmp_erosion, tmp_dilation;
        

        //reset initial
        private void button8_Click(object sender, EventArgs e)
        {
            is_sobel = 0;
            label2.Text = "";
            label3.Text = "";
            label4.Text = "";
            label5.Text = "";
            chart1.Visible = false;
            chart2.Visible = false;
            pictureBox1.Image = null;
            pictureBox2.Image = null;
            pictureBox3.Image = null;
            pictureBox4.Image = null;
            pictureBox5.Image = null;
        }


        //load
        private void button1_Click(object sender, EventArgs e) 
        {
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





        //gray r g b channel
        private void button3_Click_1(object sender, EventArgs e) 
        {
            for (int y = 0; y < openImg.Height; y++)
            {
                for (int x = 0; x < openImg.Width; x++)
                {
                    // 讀取影像平面上(x,y)的RGB資訊
                    Color RGB = openImg.GetPixel(x, y);

                    // RGB 是 VS 內建的 class 可以直接讀取影像的色彩資訊 R = Red, G = Green, B =Blue                        
                    /*int invR = Convert.ToInt32(255 - RGB.R);
                    int invG = Convert.ToInt32(255 - RGB.G);
                    int invB = Convert.ToInt32(255 - RGB.B);*/
                    //System.Console.WriteLine(RGB.B + "----");
                    int gray_rgb = (RGB.R + RGB.G + RGB.B) / 3;
                    img_Gray.SetPixel(x, y, Color.FromArgb(gray_rgb, gray_rgb, gray_rgb));
                    img_R.SetPixel(x, y, Color.FromArgb(RGB.R, RGB.R, RGB.R));
                    img_G.SetPixel(x, y, Color.FromArgb(RGB.G, RGB.G, RGB.G));
                    img_B.SetPixel(x, y, Color.FromArgb(RGB.B, RGB.B, RGB.B));
                }
            }
            pictureBox1.Image = openImg;
            openImg = new Bitmap(img_Gray);
            pictureBox2.Visible = true;
            pictureBox3.Visible = true;
            pictureBox4.Visible = true;
            pictureBox5.Visible = true;
            pictureBox2.Image = img_Gray;
            pictureBox3.Image = img_R;
            pictureBox4.Image = img_G;
            pictureBox5.Image = img_B;
            label2.Text = "Gray";
            label3.Text = "R Channel";
            label4.Text = "G Channel";
            label5.Text = "B Channel";
            chart1.Visible = false;
            chart2.Visible = false;
        }

        

        //smooth filter
        private void button4_Click(object sender, EventArgs e) 
        {
            int[] array = new int[9];
            int sum, tmp = 0;
            for (int y = 0; y < (openImg.Height - 2) ; y++)
            { 
                for (int x = 0; x < (openImg.Width - 2) ; x++)
                {
                    sum = 0;
                    tmp = 0;
                    for (int j = y; j < y + 3; j++)
                    {
                        for (int i = x; i < x + 3; i++)
                        {
                            // 讀取影像平面上(x,y)的RGB資訊
                            Color RGB = openImg.GetPixel(i, j);
                            sum += RGB.R;
                            array[tmp] = RGB.R;
                            tmp++;
                            //Console.WriteLine(sum);
                        }
                    }
                    Array.Sort(array);
                    sum /= 9;
                    img_Gray.SetPixel(x + 1, y + 1, Color.FromArgb(sum, sum, sum));
                    img_R.SetPixel(x + 1, y + 1, Color.FromArgb(array[4], array[4], array[4]));
                }
            }
            pictureBox1.Image = openImg;
            pictureBox2.Visible = true;
            pictureBox3.Visible = true;
            pictureBox2.Image = img_Gray;
            pictureBox3.Image = img_R;
            pictureBox4.Image = null;
            pictureBox5.Image = null;
            label2.Text = "Mean Filter";
            label3.Text = "Median Filter";
            label4.Text = "";
            label5.Text = "";
            chart1.Visible = false;
            chart2.Visible = false;
            pictureBox4.Visible = false;
            pictureBox5.Visible = false;
            openImg = new Bitmap(img_R);
        }
        
        //histogram equaliztion
        private void button5_Click(object sender, EventArgs e) 
        {
            int[] array = new int[256];
            int[] array2 = new int[256];
            int[] after_gray = new int[256];
            int[] x_value = new int[256];
            for (int i = 0; i < 256; i++)
            {
                x_value[i] = i;
            }
            chart1.Visible = true;
            chart2.Visible = true;
            chart1.Series["Series1"].ChartType = SeriesChartType.Column;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisX.Minimum = 0;

            //initialize
            for (int i = 0; i < 256; i++)
            {
                array[i] = 0;
                array2[i] = 0;
            }
            
            for (int y = 0; y < openImg.Height; y++)
            {
                for (int x = 0; x < openImg.Width; x++)
                {
                    Color RGB = openImg.GetPixel(x, y);
                    int gray_rgb = (RGB.R + RGB.G + RGB.B) / 3;
                    array[gray_rgb]++;
                }
            }
            chart1.Series["Series1"].Points.DataBindXY(x_value, array);

            int all = 0;
            for (int i = 0; i < 256; i++)
            {
                all += array[i];
                array2[i] = (Math.Abs(all - 1) * 255) / (openImg.Height * openImg.Width);
            }
            
            for (int y = 0; y < openImg.Height; y++)
            {
                for (int x = 0; x < openImg.Width; x++)
                {
                    Color RGB = openImg.GetPixel(x, y);
                    img_Gray.SetPixel(x, y, Color.FromArgb(array2[RGB.R], array2[RGB.R], array2[RGB.R]));
                }
            }

            for (int y = 0; y < img_Gray.Height; y++)
            {
                for (int x = 0; x < img_Gray.Width; x++)
                {
                    Color RGB = img_Gray.GetPixel(x, y);
                    int gray_rgb = (RGB.R + RGB.G + RGB.B) / 3;
                    after_gray[gray_rgb]++;
                }
            }

            chart2.Series["Series1"].Points.DataBindXY(x_value, after_gray);
            pictureBox1.Image = openImg;
            pictureBox2.Visible = true;
            pictureBox2.Image = img_Gray;
            pictureBox3.Image = null;
            pictureBox4.Image = null;
            pictureBox5.Image = null;
            label2.Text = "histogram Equalization";
            label3.Text = "";
            label4.Text = "";
            label5.Text = "";
            pictureBox3.Visible = false;
            pictureBox4.Visible = false;
            pictureBox5.Visible = false;
           openImg = new Bitmap(img_Gray);

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

        //sobel edge detection
        private void button7_Click(object sender, EventArgs e)
        {
            button11.Enabled = true;
            int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] gy = new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };
            int new_grayX = 0;
            int new_grayY = 0;
            double combined = 0;
            for (int y = 0; y < openImg.Height-2; y++)
            {
                for (int x = 0; x < openImg.Width-2; x++)
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
                    img_Gray.SetPixel(x + 1, y + 1, Color.FromArgb(new_grayX, new_grayX, new_grayX));
                    img_R.SetPixel(x + 1, y + 1, Color.FromArgb(new_grayY, new_grayY, new_grayY));
                    img_G.SetPixel(x + 1, y + 1, Color.FromArgb(Convert.ToInt32(combined), Convert.ToInt32(combined), Convert.ToInt32(combined)));
                    //if (new_grayX*new_grayX < 128*128)
                    //  img_Gray.SetPixel(x+1, y+1, Color.Black);
                    //else
                    //  img_Gray.SetPixel(x+1, y+1, Color.Transparent);
                }
            }
            label2.Text = "Vertical";
            label3.Text = "Horizontal";
            label4.Text = "Combined";
            pictureBox1.Image = openImg;
            pictureBox2.Visible = true;
            pictureBox3.Visible = true;
            pictureBox4.Visible = true;
            pictureBox2.Image = img_R;
            pictureBox3.Image = img_Gray;
            pictureBox4.Image = img_G;
            pictureBox5.Visible = false;
            chart1.Visible = false;
            chart2.Visible = false;
            //openImg = new Bitmap(img_G);
            is_sobel = 1;
        }
        //set sobel as source
        private void button11_Click(object sender, EventArgs e)
        {
            openImg = new Bitmap(img_G);
            button11.Enabled = false;
            pictureBox1.Image = openImg;
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
