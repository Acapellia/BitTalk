using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace teamproject
{
    public partial class Form1 : Form
    {
        Image img1, img2, img3, img4, img5, img6, img7, img8, img9 = null;
        Random r = new Random();
        Timer timer = new Timer();
        int loading = 2;
        public Form1()
        {
            InitializeComponent();
            this.Width = 800;
            this.Height = 900;
            this.Load += Form1_Load;
            this.Paint += Form1_Paint;
            timer.Tick += Timer_Tick;
            this.KeyDown += Form1_KeyDown;
            timer.Interval = 200;
            timer.Start();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    {
                        loading = 1;
                        timer.Start();
                        break;
                    }
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            loading++;
            Invalidate();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if(loading == 2 || loading == 3 || loading == 4)
            {
                e.Graphics.DrawImage(img1, this.ClientRectangle.Right / 4,
                            this.ClientRectangle.Bottom / 7,
                            this.ClientRectangle.Width / 2,
                            this.ClientRectangle.Height / 2);
            }

            if (loading == 2)
            {
                e.Graphics.DrawImage(img2, 300, 600, 150, 150);
                e.Graphics.DrawImage(img5, 300, 750, 100, 30);
                Console.WriteLine("aa");
            }
            if (loading == 3)
            {
                e.Graphics.DrawImage(img3, 300, 600, 150, 150);
                e.Graphics.DrawImage(img6, 300, 750, 100, 30);
                Console.WriteLine("bb");
            }
            if (loading == 4)
            {
                e.Graphics.DrawImage(img4, 300, 600, 150, 150);
                e.Graphics.DrawImage(img7, 300, 750, 100, 30);
                Console.WriteLine("cc");
            }
            if (loading == 5)
            {
                e.Graphics.DrawImage(img9, 200, 200, 400, 400);
            }
            if (loading == 6)
            {
                timer.Stop();
                this.Hide();
                Form2 LoginForm = new Form2();
                LoginForm.Show();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.BackColor = System.Drawing.Color.White;
            this.DoubleBuffered = true;

            img1 = Image.FromFile("../../LALlogo.jpg");
            img2 = Image.FromFile("../../2.jpg");
            img3 = Image.FromFile("../../3.jpg");
            img4 = Image.FromFile("../../4.jpg");
            img5 = Image.FromFile("../../loading.jpg");
            img6 = Image.FromFile("../../loading1.jpg");
            img7 = Image.FromFile("../../loading2.jpg");
            img8 = Image.FromFile("../../connect.jpg");
            img9 = Image.FromFile("../../Myleg.jpg");
        }
    }
}
