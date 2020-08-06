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

namespace teamproject {
    public partial class Form3 : Form {
        Socket clientSocket;
        IPEndPoint ipep;        // 서버의 접속 주소
        NetworkStream ns;
        StreamWriter sw;
        Thread tRecv;
        bool isRecv = true;
        delegate void AddLogData(string data);
        AddLogData addLogData = null;

        //List<Image> menteeImage = new List<Image>(1);
        //List<string> menteeName = new List<string>();
        List<Image> menteeImage;
        List<string> menteeName;
        Image mentorImage;
        int leftTabSize = 20;
        int pageIndex = 0;
        int totalMentee = 0;
        string mentoName = "";
        string userName = "";
        string chatRoomName = "";

        public Form3(NetworkStream nl, Socket sl, string username) {
            InitializeComponent();
            this.Load += Form3_Load;
            this.Paint += Form3_Paint;
            this.FormClosed += Form3_FormClosed;
            ns = nl;
            sw = new StreamWriter(ns);
            sw.AutoFlush = true;
            clientSocket = sl;
            this.userName = username;
        }
        private void Form3_FormClosed(object sender, FormClosedEventArgs e) {
            try {
                this.isRecv = false;
                if(this.clientSocket != null && this.clientSocket.Connected) {
                    string data = "request:Exit";
                    sw.WriteLine(data);
                    this.clientSocket.Close();
                }
            }
            catch(Exception ex) {
                Console.WriteLine("Exception : " + ex.Message);
            }
        }
        private void Form3_Load(object sender, EventArgs e) {
            this.addLogData = AddChatLogBox;
            this.Width = 1500;
            this.Height = 900;
            this.isRecv = true;
            this.tRecv = new Thread(new ThreadStart(ThreadRecv));
            this.tRecv.IsBackground = true;
            this.tRecv.Start();
        }

        private void Form3_Paint(object sender, PaintEventArgs e) {
            e.Graphics.DrawLine(new Pen(Brushes.YellowGreen, 10), leftTabSize + 30, 0, leftTabSize, ClientRectangle.Bottom);
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

        void AddChatLogBox(string data) {
            if(chatLog.InvokeRequired) {
                Invoke(addLogData, new object[] { data });
            }
            else {
                chatLog.Items.Add(data);
                chatLog.SelectedIndex = chatLog.Items.Count - 1;
            }
        }
        void ThreadRecv() {
            StreamReader sr = new StreamReader(this.ns);
            bool enterRoom = true;
            Console.WriteLine(isRecv.ToString());
            while(this.isRecv) {
                try {
                    string data = sr.ReadLine();
                    string[] parse = data.Split(new char[2] { ',', ':' });
                    string answer = parse[1];
                    Console.WriteLine(data);
                    Console.WriteLine(answer);
                    switch(answer) {
                        case "Login":
                            for(int i = 0; i < parse.Length; i += 2) {
                                if(parse[i] == "Data")
                                    MessageBox.Show(parse[i + 1]);
                                else if(parse[i] == "UserName")
                                    userName = parse[i + 1];
                            }
                            break;
                        case "Tab2":
                            menteeName = new List<string>();
                            menteeImage = new List<Image>();
                            int lines = 0;
                            Console.WriteLine("Parse : " + parse.Length);
                            for(int i = 0; i < parse.Length; i += 2) {
                                Console.WriteLine("Parse : " + parse[i] + " " + parse[i + 1]);
                                if(parse[i] == "Lines")
                                    lines = Convert.ToInt32(parse[i + 1]);
                                else if(parse[i] == "ChatLog" && enterRoom == true) {
                                    int idx = 0;
                                    string chatName;
                                    for(int j = 0; j < parse[i + 1].Length; j++) {
                                        if((parse[i + 1][j] == '>') || (parse[i + 1][j] == ']'))
                                            idx = j;
                                    }
                                    chatName = parse[i + 1].Substring(1, (idx - 1));
                                    Console.WriteLine(chatName);
                                    if(chatName == userName) {
                                        AddChatLogBox($"--> {parse[i + 1]}");
                                    }
                                    else
                                        AddChatLogBox($"<-- {parse[i + 1]}");
                                }
                                else if(parse[i] == "ChatRoom" && enterRoom == true) {
                                    chatRoomName = parse[i + 1];
                                }
                                else if(parse[i] == "MentoName")
                                    mentoName = parse[i + 1];
                                else if(parse[i] == "TotalMentee") {
                                    totalMentee = Convert.ToInt32(parse[i + 1]);
                                }
                                else if(parse[i] == "MenteeName") {
                                    menteeName.Add(parse[i + 1]);
                                    //if(menteeName.Count == 0)
                                    //    menteeName.Add(parse[i + 1]);
                                    //else if(userName != parse[i + 1])
                                    //    menteeName.Add(parse[i + 1]);
                                }
                                else if(parse[i] == "MentorImgPath") {
                                    mentorImage = Image.FromFile(parse[i + 1]);
                                }
                                else if(parse[i] == "MenteeImgPath") {
                                    menteeImage.Add(Image.FromFile(parse[i + 1]));
                                    //if(menteeImage.Count == 0) {
                                    //    menteeImage.Add(Image.FromFile(parse[i + 1]));
                                    //    Console.WriteLine("My");
                                    //}
                                    //else if(userName != parse[i - 1]) {
                                    //    menteeImage.Add(Image.FromFile(parse[i + 1]));
                                    //    Console.WriteLine("Other");
                                    //}
                                }
                            }
                            enterRoom = false;
                            //Console.WriteLine("totalMentee : " + totalMentee);
                            Invalidate();
                            tabPage2.Invalidate();
                            break;
                        case "Chat":
                            if(parse[3] == userName) {
                                AddChatLogBox($"--> [{userName}] {parse[5]}");
                            }
                            else {
                                AddChatLogBox($"<-- [{parse[3]}] {parse[5]}");
                            }
                            break;
                    }
                }
                catch(Exception ex) {
                    Console.WriteLine("Exception : " + ex.Message);
                    break;
                }
            }
        }

        private void tabUI_SelectedIndexChanged(object sender, EventArgs e) {
            TabControl tab = sender as TabControl;
            //pageIndex = tab.TabPages.IndexOf(tab.SelectedTab) + 1;
            pageIndex = 2;
            string data = "request:Tab" + pageIndex.ToString();
            sw.WriteLine(data);
            Console.WriteLine("Page : " + pageIndex.ToString());
        }

        private void tabPage2_Paint(object sender, PaintEventArgs e) {
            if(pageIndex == 2) {
                Font font = new Font("맑은 고딕", 30);
                e.Graphics.DrawString(chatRoomName, font, Brushes.Blue, leftTabSize + 10, 0);
                e.Graphics.DrawString("참여자 : " + (totalMentee + 1).ToString() + "명", new Font("맑은 고딕", 15), Brushes.Blue, leftTabSize + 15, 50);
                e.Graphics.DrawString("< Mentor >", new Font("맑은 고딕", 15), Brushes.Blue, leftTabSize + 15, 80);
                //e.Graphics.DrawRectangle(new Pen(Brushes.Red), leftTabSize + 15, 120, 200, 200);
                if(mentorImage != null)
                    e.Graphics.DrawImage(mentorImage, leftTabSize + 15, 120, 200, 200);
                e.Graphics.DrawString(mentoName, new Font("맑은 고딕", 15), Brushes.Blue, leftTabSize + 15, 320);
                //int menteeCnt = 3;
                int menteePro = 350;
                e.Graphics.DrawString("< Mentee >", new Font("맑은 고딕", 15), Brushes.Blue, leftTabSize + 15, menteePro);
                //Console.WriteLine(totalMentee + " " + menteeImage.Count);
                for(int i = 0; i < totalMentee; i++) {
                    //e.Graphics.DrawRectangle(new Pen(Brushes.Red), leftTabSize + 15, menteePro + 40, 100, 100);
                    if(menteeImage.Count > i) {
                        e.Graphics.DrawImage(menteeImage[i], leftTabSize + 15, menteePro + 40, 100, 100);
                        e.Graphics.DrawString(menteeName[i], new Font("맑은 고딕", 13), Brushes.Blue, leftTabSize + 15, menteePro + 140);
                        menteePro += 140;
                    }
                }
            }
        }

        private void chatText_KeyDown(object sender, KeyEventArgs e) {
            switch(e.KeyCode) {
                case Keys.Enter:
                    string data = "request:Chat,Data:" + chatText.Text;
                    this.sw.WriteLine(data);
                    this.sw.Flush();
                    chatText.Clear();
                    break;
            }
        }

    }
}

