using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Data.Odbc;

namespace NetworkImageClient {
    public partial class Form1 : Form {
        private String _strHostIP;
        private Image I;

        public Form1() {
            _strHostIP = "127.0.0.1";
            InitializeComponent();
        }

        public static byte[] ImageToByte(Image img) {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        private void button1_Click(object sender, EventArgs e) {
            TcpClient tclient = new TcpClient(_strHostIP, 9000);
            String send_str;
            byte[] recev = new byte[513];
            try {

                NetworkStream ns = tclient.GetStream();
                OpenFileDialog ofd = new OpenFileDialog();
                if(ofd.ShowDialog() == DialogResult.OK) {
                    I = Image.FromFile(ofd.FileName);
                }
                Bitmap bitmap = new Bitmap(I);
                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Position = 0;
                byte[] data = ms.ToArray();
                ns.Write(data, 0, data.Length);

            }
            catch(Exception ex) {
                Console.WriteLine("Exceotion Thrown :" + ex.ToString());
            }
            finally {

                tclient.Close();
            }
        }
    }
}
