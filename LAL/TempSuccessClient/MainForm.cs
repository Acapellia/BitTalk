using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace teamproject
{

    public partial class MainForm : Form
    {
        Image image1, image2 = null; 

        Socket clientSocket;

        NetworkStream ns;
        StreamWriter sw;
        //서버

        Image[] image = new Image[7];
        int imageNum = 0;
        int i = 0;
        string[] MentoName = {"0","김포푸","빌게이츠","김성일","데니스 리치", "미하일로 톨로토스" };
        string username = "";
        public MainForm(NetworkStream nl, Socket sl, string username)
        {
            InitializeComponent();
            this.Width = 800;
            this.Height = 900;
            this.KeyDown += MainForm_KeyDown;
            this.Load += MainForm_Load;
            this.Paint += MainForm_Paint;
            ns = nl;
            sw = new StreamWriter(ns);
            clientSocket = sl;
            this.username = username;
        }



        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(image[imageNum], this.ClientRectangle.Right / 4,
                                                  this.ClientRectangle.Bottom / 4,
                                                  this.ClientRectangle.Width / 2,
                                                  this.ClientRectangle.Height / 2);
            e.Graphics.DrawString("(ง •̀_•́)ง 새로운 레전드 멘토를 고르세요 !!", new Font("Ryunji R", 21), Brushes.Black, 150, 12);
            e.Graphics.DrawString("선택 ٩(๑• ₃ -๑)۶♥   Up key", new Font("Ryunji R", 18), Brushes.DarkGreen, 234, 116);
            e.Graphics.DrawImage(image1, 20, 394 ,30, 30);
            e.Graphics.DrawImage(image2, 730, 394,30, 30);
            e.Graphics.DrawString("거절 (｡･ˇ_ˇ･｡)   Down key", new Font("Ryunji R", 18), Brushes.DarkRed, 234, 727);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            //this.ns = new NetworkStream(this.clientSocket); // 연결
            //this.sw = new StreamWriter(this.ns);            // 쓰는데
           
            StreamReader sr = new StreamReader(this.ns);
            string data = "request:SelectMento,Data:";

            if (e.KeyCode == Keys.Up)
            {
                this.Hide();
                if (MentoName[i] != "0")
                {
                    data = data + $"{MentoName[i]}";
                    sw.WriteLine(data);
                }
                this.sw.Flush();
                Form3 form3 = new Form3(ns, clientSocket, username);
                form3.Show();
            }
            else if (e.KeyCode == Keys.Down)
            {
                //data = data + "Down";
                ////sw.WriteLine(data);
                //this.sw.Flush();
            }
            else if (e.KeyCode == Keys.Left)
            {
                if (i != 0)
                {
                    imageNum--;
                    i--;
                }
                //data = data + "Left";
                //Console.WriteLine("Left");
                ////sw.WriteLine(data);
                //this.sw.Flush();
                else if (imageNum == 0) { imageNum = 5; }

            }
            else if (e.KeyCode == Keys.Right)
            {
                Console.WriteLine("Right");
                //data = data + "Right";
                //sw.WriteLine(data);
                //this.sw.Flush();
                if (i != 5)
                {
                    imageNum++;
                    i++;
                }
                else if (imageNum == image.Length) { imageNum = 1; }
            }
            Invalidate();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ////서버
            //this.clientSocket =
            //new Socket(AddressFamily.InterNetwork,
            //       SocketType.Stream,
            //       ProtocolType.Tcp);
            //this.ipep =
            //    new IPEndPoint(IPAddress.Parse("127.0.0.1"),
            //                    Int32.Parse("9000"));
            //this.clientSocket.Connect(this.ipep);

            image1 = Image.FromFile("../../이전1.PNG");
            image2 = Image.FromFile("../../이후1.PNG");

            this.DoubleBuffered = true;
            for (int i = 0; i < 6; i++)
            {
                string path = "../../A" + i + ".jpg";

                image[i] = Image.FromFile(path);
            }

        }
    }
}
