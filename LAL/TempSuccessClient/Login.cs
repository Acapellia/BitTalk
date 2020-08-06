using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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

    public partial class Form2 : Form
    {
        Socket clientSocket;
        IPEndPoint ipep;        // 서버의 접속 주소

        NetworkStream ns;
        StreamWriter sw;
        StreamReader sr;
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.clientSocket =
            new Socket(AddressFamily.InterNetwork,
                   SocketType.Stream,
                   ProtocolType.Tcp);
            this.ipep =
                new IPEndPoint(IPAddress.Parse("10.89.1.241"),
                //new IPEndPoint(IPAddress.Parse("10.89.30.147"),
                                //new IPEndPoint(IPAddress.Parse("127.0.0.1"),
                                Int32.Parse("9000"));
            this.clientSocket.Connect(this.ipep);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //서버
            this.ns = new NetworkStream(this.clientSocket); // 연결
            this.sw = new StreamWriter(this.ns);            // 쓰는데
            this.sr = new StreamReader(this.ns);

            //

            //SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Admin\Documents\Data1.mdf;Integrated Security=True;Connect Timeout=30");

            //SqlDataAdapter sda = new SqlDataAdapter("Select Count(*) from USERINFO where USERNAME='"+ID_txt.Text+"' and PASSWORD= '"+Pw_txt.Text+"'", con);
            //  
            sw.WriteLine($"request:Login,ID:{ID_txt.Text},PW:{Pw_txt.Text}");
            this.sw.Flush();

            //
            /* 데이버베이스 외 방법 if (ID_txt.Text == "3" && Pw_txt.Text == "123") {}*/

            //DataTable newTable = new DataTable();

            //sda.Fill(newTable);
            string data = sr.ReadLine();
            string[] parse = data.Split(new char[2] { ',', ':' });
            string request = "";
            string mento = "";
            string username = "";
            for (int i = 0; i < parse.Length; i += 2)
            {
                if (parse[i] == "Answer")
                {
                    request = parse[i + 1];
                    Console.WriteLine(request);
                }
                else if(parse[i]=="Data")
                { data = parse[i + 1]; }
                else if (parse[i] == "Mento")
                { mento = parse[i + 1]; }
                else if(parse[i] == "UserName") {
                    username = parse[i + 1];
                }
            }
            //if (newTable.Rows[0][0].ToString() == "1")
            //{  //로그인 성공인 경우
            //    this.Hide();

            //    MainForm mainForm1 = new MainForm();
            //    mainForm1.Show();
            //}
            //else
            //{  //로그인 실패시
            //    MessageBox.Show("아이디와 비밀번호를 확인해주세요.");
            //}
            if (data == "접속되었습니다.")
            {  //로그인 성공인 경우
                this.Hide();
                if (mento == "멘토가 이미 있습니다.")
                {
                    Form3 form3 = new Form3(ns, clientSocket,username);
                    form3.Show();
                }
                else
                {
                    MainForm mainForm1 = new MainForm(ns, clientSocket,username);
                    mainForm1.Show();
                }
            }
            else
            {  //로그인 실패시
                MessageBox.Show("아이디와 비밀번호를 확인해주세요.");
            }

        }
    }
}
