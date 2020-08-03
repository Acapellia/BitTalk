using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.IO.Compression;
using System.Collections;

namespace NetworkImageServer {


    public partial class Form1 : Form {
        private TcpListener listener;
        private IPEndPoint serverEndPoint;
        private String _strHostIP;
        private Thread T1, T2;
        private byte[] recevbyte;
        Image[] img = new Image[10];
        int state = 0;
        public Form1() {
            _strHostIP = "127.0.0.1";
            recevbyte = new byte[4000000];
            this.Paint += Form1_Paint;
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e) {
            if(state == 1) {
                e.Graphics.DrawImage(img[0], 0, 0);
            }
        }

        public void ThreadProc1() // 스레드1 클라이언트 받기
        {
            serverEndPoint = new IPEndPoint(IPAddress.Parse(_strHostIP), 9000);
            listener = new TcpListener(IPAddress.Any, 9000);


            listener.Start();
            while(true) {

                TcpClient Client = listener.AcceptTcpClient();

                T2 = new Thread(new ParameterizedThreadStart(ThreadProc2));
                T2.Start(Client);
                if(state == 1)
                    break;
            }

        }
        public void ThreadProc2(object o) // 스레드2 받은 클라이언트에서 텍스트와 이미지 받기
        {
            TcpClient Client = (TcpClient)o;
            Socket s = Client.Client;
            String address = s.RemoteEndPoint.ToString();
            String[] array = address.Split(new char[] { ':' });
            Encoding ASCII = Encoding.ASCII;

            if(Client.Connected) {
                NetworkStream ns = Client.GetStream();
                try {
                    MemoryStream ms = new MemoryStream();
                    int[] imagesss = new int[4000000];
                    ns.Read(recevbyte, 0, recevbyte.Length);
                    Bitmap bitmap = ByteToImage(recevbyte);

                    img[0] = (bitmap);

                }
                catch(Exception ex) {
                    MessageBox.Show(ex.ToString());
                }
                finally {
                    Client.Close();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            T1 = new Thread(new ThreadStart(ThreadProc1));
            T1.Start();
        }

        private void button1_Click(object sender, EventArgs e) {
            state = 1;
            Invalidate();
        }

        public static Bitmap ByteToImage(byte[] blob) {
            using(MemoryStream mStream = new MemoryStream()) {
                mStream.Write(blob, 0, blob.Length);
                mStream.Seek(0, SeekOrigin.Begin);

                Bitmap bm = new Bitmap(mStream);
                return bm;
            }
        }
    }
}
