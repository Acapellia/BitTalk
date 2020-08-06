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
            public string ID { get; set; }
            public string Password { get; set; } = "0";
            public string Name { get; set; }
            public string MentoID { get; set; } = "0";
            public string ProfileLocation { get; set; }
            public Image ProfileImage { get; set; }

            public string ChatFileLocation { get; set; } = "0";
            public string UserRequest { get; set; } = "0";
            public List<StreamWriter> MentoMemberSW{ get; set; } = null;

            public List<string> MentoMember { get; set; } = null;

            public string MentoChk { get; set; } = "0";
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
            LoadingUserData();
        }

        private void LoadingUserData()
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
                        userM[j].MentoID = usData;
                        break;
                    case 4:
                        userM[j].ProfileLocation = usData;
                        userM[j].ProfileImage = Image.FromFile(userM[j].ProfileLocation);
                        break;
                    case 5:
                        userM[j].MentoChk = usData;
                        break;
                }
                i++;
                if (i == 6)
                {
                    userKeyManager.Add(userM[j].ID, userM[j]);
                    if (userKeyManager[userM[j].ID].MentoChk == "1")
                    {
                        userKeyManager[userM[j].ID].MentoMemberSW = new List<StreamWriter>();
                        userKeyManager[userM[j].ID].MentoMember = new List<string>();
                    }
                    ; j++; i = 0;
                }
            }
        }
        private void ReviseUserData()
        {
            string[] userText = File.ReadAllLines(@"../../userData.txt");
            StreamWriter RUW = new StreamWriter("../../userData.txt", false);

            int i = 0, j = 0;
            string reviseTxt = "오류";
            for (int TXT = 0; TXT < userText.Length; TXT++)
            {
                switch (i)
                {
                    case 0:
                        reviseTxt = userM[j].ID;
                        break;
                    case 1:
                        reviseTxt = userM[j].Password;
                        break;
                    case 2:
                        reviseTxt = userM[j].Name;
                        break;
                    case 3:
                        reviseTxt = userM[j].MentoID;
                        break;
                    case 4:
                        reviseTxt = userM[j].ProfileLocation;
                        break;
                    case 5:
                        reviseTxt = userM[j].MentoChk;
                        break;
                }
                RUW.WriteLine(reviseTxt);
                i++;
                if (i == 6)
                {
                    j++;
                    i = 0;
                }
            }
            RUW.Close();
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

                    Thread tRecv = new Thread(() => ThreadRecv(ns, partnerSocket));
                    tRecv.IsBackground = true;
                    tRecv.Start();
                }
                catch (Exception ex)
                {
                    AddLogListBox("Exception : " + ex.Message);
                }
            }
        }


        void ThreadRecv(NetworkStream ns, Socket partnerSocket)
        {

            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(ns);
            string id = "nop";
            string pass = "nop";
            bool exit = false;
            bool login = false;

            while (this.isRecv)
            {
                string data = sr.ReadLine();
                string[] parse = data.Split(new char[2] { ',', ':' });
                string request = "";
                for (int i = 0; i < parse.Length; i += 2)
                {
                    if (parse[i] == "request")
                    {
                        request = parse[i + 1];
                        if (request != "Login")
                            userKeyManager[id].UserRequest = request;
                        Console.WriteLine(request);
                    }
                    else if (parse[i] == "ID")
                    {
                        id = parse[i + 1];
                        if (request == "Login")
                            userKeyManager[id].UserRequest = request;
                        Console.WriteLine(id);
                    }
                    else if (parse[i] == "PW")
                    {
                        pass = parse[i + 1];
                        Console.WriteLine(pass);
                    }
                    else if (parse[i] == "Data") { data = parse[i + 1]; }
                }
                try
                {
                    switch (userKeyManager[id].UserRequest)
                    {
                        case "Login":
                            login = Login(ns, sw, id, pass);
                            sw.Flush();
                            break;
                        case "Register":
                            //UserRegister();
                            break;
                        case "Tab2":
                            if (login && userKeyManager[id].MentoID != "0")
                                SwitchTab1(sw, id);
                            else
                            { sw.WriteLine($"Answer:Tab2,Data:비로그인 상태입니다."); }
                            //////
                            break;
                        case "Chat":
                            if (login && userKeyManager[id].MentoID != "0")
                            {
                                string chat = "[" + userKeyManager[id].Name + "]" + data;

                                StreamWriter writer;
                                foreach (StreamWriter MemberWriter in userKeyManager[userKeyManager[id].MentoID].MentoMemberSW)
                                {
                                    string answer = "";
                                    answer += $"Answer:Chat,User:{userKeyManager[id].Name},Data:{data}";
                                    MemberWriter.AutoFlush = true;
                                    MemberWriter.WriteLine(answer);

                                    if (userKeyManager[id].ChatFileLocation == "0") {
                                        userKeyManager[id].ChatFileLocation = $"../../{userKeyManager[userKeyManager[id].MentoID].ID}.txt";
                                    }
                                }
                                writer = File.AppendText($"../../{userKeyManager[userKeyManager[id].MentoID].ID}.txt");
                                writer.WriteLine(chat);
                                writer.Close();
                            }
                            else
                            {
                                sw.WriteLine($"Answer:Tab2,Data:비로그인 상태입니다.");
                                //////
                            }
                            break;
                        case "SelectMento":
                            if (userKeyManager[id].MentoID != "0") { sw.WriteLine("이미 멘토가 지정되어있습니다."); }
                            else { userKeyManager[id].MentoID = data; sw.WriteLine($"멘토가 지정되었습니다. 멘토 이름 :{data}"); ReviseUserData(); }
                            break;
                        case "Exit":
                            exit = true;
                            Console.WriteLine("끝이기 전이다.");
                            break;
                    }
                    if (exit)
                    {
                        Console.WriteLine("다 끝나간다.");
                        if (login)
                        {
                            userKeyManager[userKeyManager[id].MentoID].MentoMemberSW.Remove(sw);
                            userKeyManager[userKeyManager[id].MentoID].MentoMember.Remove(id);
                            ExitCharRoom(id);
                        }
                        partnerSocket.Close();
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception[2][2] : " + ex.Message);
                    AddLogListBox("Exception[2][2] : " + ex.Message);
                    break;
                }
            }
            Console.WriteLine("끝이다.");
        }

        private bool Login(NetworkStream ns, StreamWriter sw, string id, string pass)
        {
            bool chk = false;
            string answer = "Answer:Login,";
            if (!userKeyManager.ContainsKey(id))
            {
                answer = answer + "등록되지 않은 사용자입니다. 접속이 거부되었습니다.";
                chk = false;
            }
            else if (userKeyManager[id].Password != pass)
            {
                answer = answer + "잘못된 비밀번호입니다.";
                chk = false;
            }
            else
            {
                answer += "UserName:" + userKeyManager[id].Name;
                answer += ",Data:접속되었습니다.";
                //저장된 멘토가 있으면
                if (userKeyManager[id].MentoID != "0")
                {
                    answer += ",Mento:멘토가 이미 있습니다.";
                    userKeyManager[userKeyManager[id].MentoID].MentoMemberSW.Add(sw);
                    userKeyManager[userKeyManager[id].MentoID].MentoMember.Add(id);
                }
                chk = true;
            }
            sw.WriteLine(answer);
            sw.Flush();
            return chk;
        }
        private void SwitchTab1(StreamWriter sw, string id)
        {
            string sendChatData = null;
            string[] readAll = File.ReadAllLines($"../../{userKeyManager[userKeyManager[id].MentoID].ID}.txt");
            string otherChatData = "";
            sendChatData = "Answer:Tab2,";
            for (int i = readAll.Length-5; i < readAll.Length; i++)
            {
                if (i == 0)
                { sendChatData += readAll[i] +  "," + "Lines:5,"; }
                else
                { sendChatData += $"ChatLog:" + readAll[i] + ","; }
            }
            sendChatData += $"TotalMentee:{userKeyManager[userKeyManager[id].MentoID].MentoMember.Count}" + ",";
            foreach(string MemberName in userKeyManager[userKeyManager[id].MentoID].MentoMember) {
                Console.WriteLine("Member : " + MemberName);
                Console.WriteLine("Connect : " + userKeyManager[id].Name);
                if(userKeyManager[id].ID == MemberName)
                    sendChatData += $"MenteeName:{userKeyManager[MemberName].Name},MenteeImgPath:{userKeyManager[MemberName].ProfileLocation}" + ",";
                else
                    otherChatData += $"MenteeName:{userKeyManager[MemberName].Name},MenteeImgPath:{userKeyManager[MemberName].ProfileLocation}" + ",";
            }
            sendChatData += otherChatData;
            sendChatData += $"MentoName:{userKeyManager[userKeyManager[id].MentoID].Name},MentorImgPath:{userKeyManager[userKeyManager[id].MentoID].ProfileLocation}";
            foreach(StreamWriter MemberWriter in userKeyManager[userKeyManager[id].MentoID].MentoMemberSW) {
                MemberWriter.WriteLine(sendChatData);
                MemberWriter.Flush();
            }
            Console.WriteLine(sendChatData);
           
            //string sendChatData = null;
            //string[] readAll = File.ReadAllLines($"../../{userKeyManager[userKeyManager[id].MentoID].ID}.txt");
            //sendChatData = "Answer:Tab2,Lines:5,";
            //for (int i = 1; i < 6; i++)
            //{
            //    sendChatData = sendChatData + $"ChatLog:" + readAll[readAll.Length - i] + ",";
            //}
            //sendChatData = sendChatData + $"MentoName:{userKeyManager[userKeyManager[id].MentoID].Name}" + ",";
            ////int j = 1;
            //sendChatData = sendChatData + $"TotalMentee:{userKeyManager[userKeyManager[id].MentoID].MentoMember.Count}" + ",";
            //foreach (string MemberName in userKeyManager[userKeyManager[id].MentoID].MentoMember)
            //{
            //    sendChatData = sendChatData + $"MenteeName:{userKeyManager[MemberName].Name}" + ",";
            //}
            //sw.WriteLine(sendChatData);
            //Console.WriteLine(sendChatData);
            //sw.Flush();
        }
        private void ExitCharRoom(string id) {
            string sendChatData = null;
            sendChatData = "Answer:Tab2,";
            string otherChatData = "";
            sendChatData += $"TotalMentee:{userKeyManager[userKeyManager[id].MentoID].MentoMember.Count}" + ",";
            foreach(string MemberName in userKeyManager[userKeyManager[id].MentoID].MentoMember) {
                if(userKeyManager[id].ID == MemberName)
                    sendChatData += $"MenteeName:{userKeyManager[MemberName].Name},MenteeImgPath:{userKeyManager[MemberName].ProfileLocation}" + ",";
                else
                    otherChatData += $"MenteeName:{userKeyManager[MemberName].Name},MenteeImgPath:{userKeyManager[MemberName].ProfileLocation}" + ",";
            }
            sendChatData += otherChatData;
            sendChatData += $"MentoName:{userKeyManager[userKeyManager[id].MentoID].Name},MentorImgPath:{userKeyManager[userKeyManager[id].MentoID].ProfileLocation}";
            foreach(StreamWriter MemberWriter in userKeyManager[userKeyManager[id].MentoID].MentoMemberSW) {
                MemberWriter.WriteLine(sendChatData);
                MemberWriter.Flush();
            }
        }
    }
}
