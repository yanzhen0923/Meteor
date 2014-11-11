using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Meteor
{
    public partial class 一起去看流星雨 : Form
    {
        [DllImport("winmm.dll")]
        public static extern int mciSendString(string m_strCmd, string m_strReceive, int m_v1, int m_v2);
        private double Angle = 2 * Math.PI * ((double)55 / (double)360);
        int CurrentMeterors = 0;
        int CurrentStars = 0;
        int MaxMeteors = 24;
        int MaxStars = 32;
        bool ClockWise0 = false;
        bool Stop = false;
 
        private void SingleMeteor()
        {
            Random random = new Random();
            bool ClockWise = ClockWise0;
            Graphics g = this.CreateGraphics();
            int Thickness = random.Next(1, 5);
            int StartX = random.Next(- 100, Screen.PrimaryScreen.WorkingArea.Width + 100);
            int StartY = random.Next(0, Screen.PrimaryScreen.WorkingArea.Height >> 3);
            int XLenth = random.Next(8, 60);
            int EndX = ClockWise == true ? (StartX - XLenth) : (StartX + XLenth);
            double YLenth = XLenth * Math.Tan(Angle);
            int EndY = StartY + (int)YLenth;
            int SleepInterval = random.Next(2, 30);
            Pen p = new Pen(Color.FromArgb(random.Next(150, 256), random.Next(150, 256), random.Next(150, 256)), Thickness);
            Pen p0 = new Pen(BackColor, Thickness);
            Point p1 = new Point(StartX, StartY);
            Point p2 = new Point(EndX, EndY);
            YLenth = 4 * Math.Tan(Angle);
            for (; p1.Y <= Screen.PrimaryScreen.WorkingArea.Height; )
            {
                if (Stop == false)
                {
                    p0.Color = BackColor;
                    g.DrawLine(p0, p1, p2);
                    p1.X -= ClockWise == true ? 4 : -4;
                    p2.X -= ClockWise == true ? 4 : -4;
                    p1.Y += (int)(YLenth);
                    p2.Y += (int)(YLenth);
                    g.DrawLine(p, p1, p2);
                }
                else
                    break;
                Thread.Sleep(SleepInterval);
            }
            if (Stop == true)
                return;
            p0.Color = BackColor;
            g.DrawLine(p0, p1, p2);
            CurrentMeterors -= 1;
            return;
        }

        private void SingleStar()
        {
            Random rd = new Random();
            bool ExpandFlag = true;
            int LifeTime = 20;
            int rgb = rd.Next(1, 8);
            int MaxSize = rd.Next(3, 7);
            if (MaxSize == 3)
                LifeTime = 10;
            else if (MaxSize == 4)
                LifeTime = 15;
            else if (MaxSize == 5)
                LifeTime = 20;
            else if (MaxSize == 6)
                LifeTime = 35;
            MaxSize <<= 1;
            MaxSize |= 1;
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(-(MaxSize >> 1), -(MaxSize >> 1 ), MaxSize,MaxSize);
            PathGradientBrush pthGrBrush = new PathGradientBrush(gp);
            pthGrBrush.CenterColor = Color.FromArgb(255, 255, 255);
            Color[] colors = { Color.FromArgb(255, rd.Next(100,200),rd.Next(100,200), rd.Next(100,200)) };
            if (rgb == 1)
                colors[0] = Color.FromArgb(rd.Next(20,80), rd.Next(200,256), rd.Next(200,256));
            else if(rgb == 2)
                colors[0] = Color.FromArgb(rd.Next(200, 256), rd.Next(20, 80), rd.Next(200, 256));
            else if(rgb == 3)
                colors[0] = Color.FromArgb(rd.Next(200, 256), rd.Next(200, 256), rd.Next(20, 80));
            else if(rgb == 4)
                colors[0] = Color.FromArgb(rd.Next(200, 256), rd.Next(20, 80), rd.Next(20, 80));
            else if(rgb == 5)
                colors[0] = Color.FromArgb(rd.Next(20, 80), rd.Next(200, 256), rd.Next(20, 80));
            else if(rgb == 6)
                colors[0] = Color.FromArgb(rd.Next(20, 80), rd.Next(20, 80), rd.Next(200, 256));
            pthGrBrush.SurroundColors = colors;
            SolidBrush sb0 = new SolidBrush(BackColor);
            Graphics g = this.CreateGraphics();
            g.TranslateTransform(rd.Next(10, Screen.PrimaryScreen.WorkingArea.Width - 100), rd.Next(10, Screen.PrimaryScreen.WorkingArea.Height));
            float LastWidth = MaxSize;
            float LastHeight = MaxSize;
            ExpandFlag = false;
            for (; ; )
            {
                if (Stop == false)
                {
                    sb0.Color = BackColor;
                    g.FillEllipse(sb0, -((int)LastWidth >> 1), -((int)LastHeight >> 1), LastWidth, LastHeight);
                    if (LifeTime <= 0)
                        break;
                    if (LastHeight == 3)
                        ExpandFlag = true;
                    else if (LastHeight == MaxSize)
                        ExpandFlag = false;
                    if (ExpandFlag == false)
                    {
                        LastHeight -= 2;
                        LastWidth -= 2;
                    }
                    else
                    {
                        LastHeight += 2;
                        LastWidth += 2;
                    }
                    g.FillEllipse(pthGrBrush, -((int)LastWidth >> 1), -((int)LastHeight >> 1), LastWidth, LastHeight);
                }
                else
                    break;
                if (LastWidth == MaxSize)
                    Thread.Sleep(rd.Next(1300, 1700));
                else if (LastWidth == 3)
                    Thread.Sleep(rd.Next(900, 1300));
                if (ExpandFlag == true)
                    Thread.Sleep(90);
                else
                    Thread.Sleep(120);
                LifeTime -= 1;
            }
            if (Stop == true)
                return;
            CurrentStars -= 1;
            return;
        }

        public 一起去看流星雨()
        {
            InitializeComponent();
        }

        private void 一起去看流星雨_Load(object sender, EventArgs e)
        {
            notifyIcon.ContextMenuStrip = 菜单;
            notifyIcon.ShowBalloonTip(4000);

            mciSendString(@"open " + "song.mp3" + " alias song", null, 0, 0);
            mciSendString("play song repeat", null, 0, 0);
            Thread t1 = new Thread(new ThreadStart(ControlMeteors));
            Thread t2 = new Thread(new ThreadStart(ControlStars));
            t1.Start();
            t2.Start();
        }

        private void ControlMeteors()
        {
            Random random = new Random();
            for (; ; )
            {
                if (Stop == false)
                {
                    if (CurrentMeterors < MaxMeteors)
                    {
                        Thread t = new Thread(new ThreadStart(SingleMeteor));
                        t.Start();
                        CurrentMeterors += 1;
                    }
                    Thread.Sleep(random.Next(40,60));
                }
                else
                    break;
            }
        }

        private void ControlStars()
        {
            Random random = new Random();
            for (; ; )
            {
                if (Stop == false)
                {
                    if (CurrentStars < MaxStars)
                    {
                        Thread t = new Thread(new ThreadStart(SingleStar));
                        t.Start();
                        CurrentStars += 1;
                    }
                    Thread.Sleep(random.Next(20,40));
                }
                else
                    break;
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stop = true;
            notifyIcon.Visible = false;
            mciSendString("stop song", null, 0, 0);
            Application.Exit();
        }

        private void 换个方向ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClockWise0 == true)
                ClockWise0 = false;
            else
                ClockWise0 = true;
        }

        private void 让流星雨来得更猛烈些吧ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MaxMeteors += 15;
        }

        private void 恢复原状ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MaxMeteors = 20;
        }

        private void 看看满天繁星ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MaxStars = 64;
        }

        private void 恢复原状ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MaxStars = 32;
        }

        private void 透明ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackColor = Color.Black;
        }

        private void 午夜深蓝ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackColor = Color.FromArgb(0, 10, 38);
        }

        private void 静谧暗灰ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BackColor = Color.FromArgb(64, 64, 64);
        }
    }
}
