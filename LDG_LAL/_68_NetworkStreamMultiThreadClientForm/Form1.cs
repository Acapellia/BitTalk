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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _64_NetworkStreamMultiThreadClientForm
{
    public partial class Form1 : Form
    {
        Socket clientSocket;
        IPEndPoint ipep;        // 서버의 접속 주소

        NetworkStream ns;
        StreamWriter sw;

        Thread tRecv;
        bool isRecv = true;

        delegate void AddLogData(string data);
        AddLogData addLogData = null;

        int leftTabSize = 20;
        int pageIndex = 0;

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.Paint += Form1_Paint;
            this.FormClosed += Form1_FormClosed;
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.isRecv = false;
                if(this.clientSocket != null && this.clientSocket.Connected) {
                    string data = "request:Exit";
                    sw.WriteLine(data);
                    this.clientSocket.Close();
                }
            }catch(Exception ex)
            {
                //AddLogListBox("Exception : " + ex.Message);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.addLogData = AddChatLogBox;

            this.Width = 1500;
            this.Height = 900;
            this.isRecv = true;
            this.clientSocket =
                new Socket(AddressFamily.InterNetwork,
                            SocketType.Stream,
                            ProtocolType.Tcp);
            this.ipep =
                new IPEndPoint(IPAddress.Parse("10.89.30.147"),
                                Int32.Parse("9000"));
            //AddLogListBox("서버 접속 요청중...");
            this.clientSocket.Connect(this.ipep);
            //AddLogListBox("서버 접속 완료");

            this.ns = new NetworkStream(this.clientSocket);
            this.sw = new StreamWriter(this.ns);
            this.sw.AutoFlush = true;
            this.tRecv = new Thread(new ThreadStart(ThreadRecv));
            this.tRecv.IsBackground = true;
            this.tRecv.Start();

            btnConnect.Enabled = false;
            btnDisconnect.Enabled = true;
            //MakeButtons();
            //MakeRoomUI();
        }

        private void Form1_Paint(object sender, PaintEventArgs e) {
            e.Graphics.DrawLine(new Pen(Brushes.YellowGreen, 10), leftTabSize+30, 0, leftTabSize, ClientRectangle.Bottom);
            if(pageIndex == 0) {
                e.Graphics.DrawString("Main Page", new Font("맑은 고딕", 100), Brushes.Blue, 200, 0);
            }
            if(pageIndex == 3) {
                e.Graphics.DrawString("Login Page", new Font("맑은 고딕", 100), Brushes.Blue, 200, 0);
            }
            if(pageIndex == 2) {
                e.Graphics.DrawString("Register Page", new Font("맑은 고딕", 100), Brushes.Blue, 200, 0);
            }
            if(pageIndex == 1) {
                Font font = new Font("맑은 고딕", 30);
                
                e.Graphics.DrawString("Room Title", font, Brushes.Blue, leftTabSize + 10, 0);
                e.Graphics.DrawString("참여자 : n명", new Font("맑은 고딕", 15), Brushes.Blue, leftTabSize + 15, 50);
                //e.Graphics.DrawImage(mentorImg, 10, 50);
                e.Graphics.DrawString("< Mentor >", new Font("맑은 고딕", 15), Brushes.Blue, leftTabSize + 15, 80);
                e.Graphics.DrawRectangle(new Pen(Brushes.Red), leftTabSize + 15, 120, 200, 200);
                e.Graphics.DrawString("Mentor Name", new Font("맑은 고딕", 15), Brushes.Blue, leftTabSize + 15, 320);
                int menteeCnt = 3;
                int menteePro = 350;
                e.Graphics.DrawString("< Mentee >", new Font("맑은 고딕", 15), Brushes.Blue, leftTabSize + 15, menteePro);
                for(int i = 0; i < menteeCnt; i++) {
                    e.Graphics.DrawRectangle(new Pen(Brushes.Red), leftTabSize + 15, menteePro + 40, 100, 100);
                    e.Graphics.DrawString("Mentee Name", new Font("맑은 고딕", 13), Brushes.Blue, leftTabSize + 15, menteePro + 140);
                    menteePro += 160;
                }

            }
            if(pageIndex == 4) {
                e.Graphics.DrawString("Profile Page", new Font("맑은 고딕", 100), Brushes.Blue, 200, 0);
            }
        }

        private void MakeRoomUI() {
            chatLog = new ListBox();
            const int profileTab = 250;
            chatLog.Left = leftTabSize + profileTab;
            chatLog.Top = 30;
            chatLog.Width = ClientRectangle.Right - (leftTabSize + profileTab + 30);
            chatLog.Height = ClientRectangle.Bottom - 100;
            this.Controls.Add(chatLog);

            chatText = new TextBox();
            chatText.Left = leftTabSize + profileTab;
            chatText.Top = ClientRectangle.Bottom - 60;
            chatText.Width = ClientRectangle.Right - (leftTabSize + profileTab + 30);
            chatText.Height = 100;
            chatText.Font = new Font("휴먼편지체", 24);
            chatText.KeyDown += ChatText_KeyDown;
            this.Controls.Add(chatText);
        }

        private void ChatText_KeyDown(object sender, KeyEventArgs e) {
            switch(e.KeyCode) {
                case Keys.Enter:
                    string data = chatText.Text;
                    this.sw.WriteLine(data);
                    this.sw.Flush();
                    AddChatLogBox("← to Client : " + data);
                    chatText.Clear();
                    break;
            }
        }

        /*private void MakeButtons() {
            int BUTTON_SIZE = 100;
            Button[] btn = new Button[5];
            string[] btnString = new string[5] { "Main", "Login", "Register", "Matching", "Profile" };
            for(int i = 0; i < 5; i++) {
                btn[i] = new Button();
                btn[i].Left = 0;
                btn[i].Top = i * BUTTON_SIZE;
                btn[i].Width = BUTTON_SIZE;
                btn[i].Height = BUTTON_SIZE;
                btn[i].Margin = new Padding(0);
                btn[i].Text = btnString[i];
                btn[i].Font = new Font("맑은 고딕", 15, FontStyle.Bold);
                this.Controls.Add(btn[i]);
            }
            btn[0].Click += Btn_Main_Click;
            btn[3].Click += Btn_Matching_Click;
        }*/
        /*private void Btn_Main_Click(object sender, EventArgs e) {
            pageIndex = 0;
            this.Controls.Clear();
            MakeButtons();
            sw.WriteLine("type" + pageIndex.ToString());
            Invalidate();
        }*/
        /*private void Btn_Matching_Click(object sender, EventArgs e) {
            pageIndex = 3;
            this.Controls.Clear();
            lbLog.Hide();
            tbChat.Hide();
            //MakeButtons();
            //MakeRoomUI();
            sw.WriteLine("type" + pageIndex.ToString());
            Invalidate();
        }*/
        private void btnConnect_Click(object sender, EventArgs e)
        {

            /*this.isRecv = true;
            this.clientSocket =
                new Socket(AddressFamily.InterNetwork,
                            SocketType.Stream,
                            ProtocolType.Tcp);
            this.ipep =
                new IPEndPoint(IPAddress.Parse("10.89.30.147"),
                                Int32.Parse("9000"));
            AddLogListBox("서버 접속 요청중...");
            this.clientSocket.Connect(this.ipep);
            AddLogListBox("서버 접속 완료");


            this.ns = new NetworkStream(this.clientSocket);
            this.sw = new StreamWriter(this.ns);
            this.sw.AutoFlush = true;
            this.tRecv = new Thread(new ThreadStart(ThreadRecv));
            this.tRecv.IsBackground = true;
            this.tRecv.Start();

            btnConnect.Enabled = false;
            btnDisconnect.Enabled = true;*/
        }

        /*private void btnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                this.isRecv = false;
                if (this.clientSocket != null &&
                    this.clientSocket.Connected)
                    this.clientSocket.Close();
            }catch(Exception ex)
            {
                AddLogListBox("Exception : " + ex.Message);
            }
            finally // 성공/예외 관계없이 무조건 실행 처리
            {
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
            }
        }*/

        /*private void btnClear_Click(object sender, EventArgs e)
        {
            lbLog.Items.Clear();
        }

        private void tbChat_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    string data = tbChat.Text;
                    this.sw.WriteLine(data);
                    this.sw.Flush();
                    AddLogListBox("← to Client : " + data);
                    tbChat.Clear();
                    break;
            }
        }*/

        /*void AddLogListBox(string data)
        {
            if (lbLog.InvokeRequired)
            {
                Invoke(addLogData, new object[] { data });
            }
            else
            {
                lbLog.Items.Add(data);
                lbLog.SelectedIndex = lbLog.Items.Count - 1;
            }
        }*/
        void AddChatLogBox(string data) {
            if(chatLog.InvokeRequired) {
                Invoke(addLogData, new object[] { data });
            }
            else {
                chatLog.Items.Add(data);
                chatLog.SelectedIndex = chatLog.Items.Count - 1;
            }
        }
        void ThreadRecv()
        {
            StreamReader sr = new StreamReader(this.ns);
            
            while (this.isRecv)
            {
                try
                {
                    string data = sr.ReadLine();
                    string[] parse = data.Split(new char[2] { ',', ':' });
                    string answer = parse[1];
                    Console.WriteLine(data);
                    Console.WriteLine(answer);
                    /*foreach(string s in parse) {
                        Console.WriteLine(s);
                    }*/
                    switch(answer) {
                        case "Login":
                            if(parse[3] == "접속되었습니다.")
                                MessageBox.Show(parse[3]);
                            //AddChatLogBox("→ from Server : " + parse[3]);
                            break;
                        case "Tab2":
                            Console.WriteLine("abc");
                            Console.WriteLine(parse[3]);
                            int lines = Convert.ToInt32(parse[3]);
                            for(int i = 0; i < lines; i++) {
                                AddChatLogBox(parse[5+(i*2)]);
                            }
                            break;
                        case "Chat":
                            AddChatLogBox(parse[3]);
                            break;
                    }
                }
                catch(Exception ex)
                {
                    //AddLogListBox("Exception: " + ex.Message);
                    break;
                }
            }
        }


        private void tabUI_SelectedIndexChanged(object sender, EventArgs e) {
            TabControl tab = sender as TabControl;
            pageIndex = tab.TabPages.IndexOf(tab.SelectedTab)+1;
            string data = "request:Tab" + pageIndex.ToString();
            sw.WriteLine(data);
            Console.WriteLine("Page : " + pageIndex.ToString());
        }

        private void tabPage2_Paint(object sender, PaintEventArgs e) {
            if(pageIndex == 1) {
                Font font = new Font("맑은 고딕", 30);

                e.Graphics.DrawString("Room Title", font, Brushes.Blue, leftTabSize + 10, 0);
                e.Graphics.DrawString("참여자 : n명", new Font("맑은 고딕", 15), Brushes.Blue, leftTabSize + 15, 50);
                //e.Graphics.DrawImage(mentorImg, 10, 50);
                e.Graphics.DrawString("< Mentor >", new Font("맑은 고딕", 15), Brushes.Blue, leftTabSize + 15, 80);
                e.Graphics.DrawRectangle(new Pen(Brushes.Red), leftTabSize + 15, 120, 200, 200);
                e.Graphics.DrawString("Mentor Name", new Font("맑은 고딕", 15), Brushes.Blue, leftTabSize + 15, 320);
                int menteeCnt = 3;
                int menteePro = 350;
                e.Graphics.DrawString("< Mentee >", new Font("맑은 고딕", 15), Brushes.Blue, leftTabSize + 15, menteePro);
                for(int i = 0; i < menteeCnt; i++) {
                    e.Graphics.DrawRectangle(new Pen(Brushes.Red), leftTabSize + 15, menteePro + 40, 100, 100);
                    e.Graphics.DrawString("Mentee Name", new Font("맑은 고딕", 13), Brushes.Blue, leftTabSize + 15, menteePro + 140);
                    menteePro += 140;
                }

            }
        }

        private void chatText_KeyDown(object sender, KeyEventArgs e) {
            switch(e.KeyCode) {
                case Keys.Enter:
                    string data = "request:Chat,Data:"+chatText.Text;
                    this.sw.WriteLine(data);
                    this.sw.Flush();
                    //AddChatLogBox("← to Client : " + data);
                    chatText.Clear();
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            string id = tbID.Text;
            string pw = tbPW.Text;
            string data = "request:Login,ID:" + id + ",PW:" + pw;
            sw.WriteLine(data);
            this.sw.Flush();
        }
    }
}
