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

namespace Server_01
{
    public partial class Form1 : Form
    {
        Socket acceptSocket;    // 안내 역할 소켓(연결 처리 소켓)
        IPEndPoint ipep;        // 서버의 주소(ip, port)

        Thread tAccept;         // 연결담당 스레드
        bool isAccept = true;   // 연결담당 스레드 반복 플래그

        bool isRecv = true;     // 수신담당 스레드 반복 플래그
        Dictionary<string, user> userKeyManager = new Dictionary<string, user>();
        List<user> userM = new List<user>();
        class user
        {
            public string ID
            { get; set; }
            public string Password
            { get; set; } = "0";
            public string Name
            { get; set; }
            public string MentoName
            { get; set; } = "0";

            public string ProfileLocation
            { get; set; }
            public Image ProfileImage
            { get; set; }

            public string IP
            { get; set; } = "0";
            public Socket socket
            { get; set; } = null;
          
            public int UserType
            { get; set; } = 0;

            public List<Socket> MentoSocket
            { get; set; } = null;
            public bool MentoChk
            { get; set; } = false;
        }

        delegate void AddMsgData(string data);
        AddMsgData addMsgData = null;

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosed += Form1_FormClosed;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.isAccept = false;
                this.isRecv = false;
                this.acceptSocket.Close();
            }
            catch (Exception ex)
            {
                AddLogListBox("Exception : " + ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            addMsgData = AddLogListBox;
            loadingUserData();
        }

        private void loadingUserData()
        {
            string[] userText = File.ReadAllLines(@"../../userData.txt");
            int i = 0; int j = 0;
            foreach (string usData in userText)
            {
                if (i == 0)
                { userM.Add(new user()); }
                switch (i)
                {
                    case 0:
                        userM[j].ID = usData;
                        break;
                    case 1:
                        userM[j].Password = usData;
                        break;
                    case 2:
                        userM[j].Name = usData;
                        break;
                    case 3:
                        userM[j].MentoName = usData;
                        break;
                    case 4:
                        userM[j].ProfileLocation = usData;
                        userM[j].ProfileImage = Image.FromFile(userM[j].ProfileLocation);
                        break;
                }
                i++;
                if (i == 5)
                { userKeyManager.Add(userM[j].ID, userM[j]); j++; i = 0; }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.isAccept = true;
            this.isRecv = true;
            this.acceptSocket =
                new Socket(AddressFamily.InterNetwork,
                            SocketType.Stream,
                            ProtocolType.Tcp);
            this.ipep = new IPEndPoint(IPAddress.Any,
                                Int32.Parse(tbPort.Text));
            this.acceptSocket.Bind(this.ipep);
            this.acceptSocket.Listen(1000);
            AddLogListBox("서버 대기중...");

            this.tAccept = new Thread(new ThreadStart(ThreadAccept));
            this.tAccept.IsBackground = true;
            this.tAccept.Start();

            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                this.isAccept = false;
                this.isRecv = false;
                this.acceptSocket.Close();
            }
            catch (Exception ex)
            {
                AddLogListBox("Exception : " + ex.Message);
            }
            finally
            {
                btnStart.Enabled = true;
                btnStop.Enabled = false;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lbLog.Items.Clear();
        }

        void AddLogListBox(string data)
        {
            if (lbLog.InvokeRequired)
            {
                Invoke(addMsgData, new object[] { data });
            }
            else
            {
                lbLog.Items.Add(data);
                lbLog.SelectedIndex = lbLog.Items.Count - 1;
            }
        }

        void ThreadAccept()
        {
            while (this.isAccept)
            {
                try
                {
                    Socket partnerSocket = this.acceptSocket.Accept();
                    string address = partnerSocket.RemoteEndPoint.ToString();

                    AddLogListBox("클라이언트 접속");
                    NetworkStream ns = new NetworkStream(partnerSocket);
                    StreamReader sr = new StreamReader(ns);
                    StreamWriter sw = new StreamWriter(ns);

                    string data = sr.ReadLine();
                    string pass = sr.ReadLine();
                    if (!userKeyManager.ContainsKey(data))
                    {
                        sw.WriteLine("등록되지 않은 사용자입니다. 접속이 거부되었습니다.");
                    }
                    else if (userKeyManager[data].Password != pass)
                    {
                        sw.WriteLine("잘못된 비밀번호입니다.");
                    }
                    else
                    {
                        userKeyManager[data].IP = address;
                        userKeyManager[data].socket = partnerSocket;
                        if (userKeyManager[data].MentoChk)
                        { userKeyManager[data].MentoSocket = new List<Socket>(); userKeyManager[data].MentoSocket.Add(userKeyManager[data].socket); }
                        Thread tRecv = new Thread(() => ThreadRecv(ns, data));
                        tRecv.IsBackground = true;
                        tRecv.Start();
                    }
                    sw.Flush();
                }
                catch (Exception ex)
                {
                    AddLogListBox("Exception : " + ex.Message);
                }
            }
        }
        void ThreadRecv(object obj, string name)
        {
            NetworkStream ns = obj as NetworkStream;
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            StreamWriter tw;

            while (this.isRecv)
            {
                try
                {
                    if (userKeyManager[name].UserType == 0)
                    {
                        userKeyManager[name].UserType = Convert.ToInt32(sr.ReadLine()) - 48;
                    }
                    else
                    {
                        if (userKeyManager[name].UserType == 1) //멘토 평가
                        { }
                        else if (userKeyManager[name].UserType == 2) //멘토 단톡방
                        {
                            string data = sr.ReadLine();
                            if (data == "type(1)")
                            { userKeyManager[name].UserType = 1; }
                            else
                            {
                                foreach (Socket socket in userKeyManager[userKeyManager[name].MentoName].MentoSocket)
                                {
                                    NetworkStream us = new NetworkStream(socket);
                                    StreamWriter uw = new StreamWriter(us);
                                    uw.AutoFlush = true;
                                    uw.WriteLine(data);
                                }
                            }
                        }
                    }
                    //sw.WriteLine("누구에게 보내시겠습니까?");
                    //sw.Flush();
                    //string who = sr.ReadLine();
                    //bool chk = false;
                    //bool nop = false;
                    //if (who == "NO")
                    //{
                    //    sw.WriteLine("메시지를 입력하세요.");
                    //    sw.Flush();
                    //}
                    //else if (!userKeyManager.ContainsKey(who))
                    //{
                    //    sw.WriteLine("없는 사람입니다.");
                    //    sw.Flush();
                    //    nop = true;
                    //}
                    //else if (userKeyManager[who].IP == "0")
                    //{
                    //    sw.WriteLine($"{who}가 접속해있지 않습니다.");
                    //    sw.Flush();
                    //    nop = true;
                    //}
                    //else
                    //{
                    //    sw.WriteLine("보낼 메시지를 입력하세요.");
                    //    sw.Flush();
                    //    chk = true;
                    //}
                    //if (!nop)
                    //{
                    //    string data = sr.ReadLine();
                    //    AddLogListBox("← Client 수신 : " + data);
                    //    if (chk)
                    //    {
                    //        tw = new StreamWriter(new NetworkStream(userKeyManager[who].socket));
                    //        tw.WriteLine(data);
                    //        tw.Flush();
                    //    }
                    //    sw.WriteLine(data);
                    //    sw.Flush();
                    //    AddLogListBox("→ Client Echo : " + data);
                    //}
                }
                catch (Exception ex)
                {
                    AddLogListBox("Exception : " + ex.Message);
                    break;
                }
            }
        }
    }
}
