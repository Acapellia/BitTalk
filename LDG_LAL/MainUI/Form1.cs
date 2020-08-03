using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainUI {
    public partial class Form1 : Form {
        int leftTabSize = 100;
        int pageIndex = 0;
        public Form1() {
            InitializeComponent();
            this.Paint += Form1_Paint;
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e) {
            this.Width = 1500;
            this.Height = 900;
            MakeButtons();
            MakeRoomUI();
        }

        private void Form1_Paint(object sender, PaintEventArgs e) {
            e.Graphics.DrawLine(new Pen(Brushes.YellowGreen,10), leftTabSize, 0, leftTabSize, ClientRectangle.Bottom);
            if(pageIndex == 0) {
                e.Graphics.DrawString("Main Page", new Font("맑은 고딕", 100), Brushes.Blue, 200, 0);
            }
            if(pageIndex == 1) {
                e.Graphics.DrawString("Login Page", new Font("맑은 고딕", 100), Brushes.Blue, 200, 0);
            }
            if(pageIndex == 2) {
                e.Graphics.DrawString("Register Page", new Font("맑은 고딕", 100), Brushes.Blue, 200, 0);
            }
            if(pageIndex == 3) {
                Font font = new Font("맑은 고딕", 30);
                e.Graphics.DrawString("Room Title", font, Brushes.Blue, leftTabSize+10, 0);
                e.Graphics.DrawString("참여자 : n명", new Font("맑은 고딕", 15), Brushes.Blue, leftTabSize+15, 50);
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
            ListBox chatLog = new ListBox();
            const int profileTab = 250;
            chatLog.Left = leftTabSize+ profileTab;
            chatLog.Top = 30;
            chatLog.Width = ClientRectangle.Right - (leftTabSize + profileTab + 30);
            chatLog.Height = ClientRectangle.Bottom - 100;
            this.Controls.Add(chatLog);

            TextBox chatText = new TextBox();
            chatText.Left = leftTabSize + profileTab;
            chatText.Top = ClientRectangle.Bottom - 60;
            chatText.Width = ClientRectangle.Right - (leftTabSize + profileTab + 30);
            chatText.Height = 100;
            chatText.Font = new Font("휴먼편지체", 24);
            this.Controls.Add(chatText);
        }

        private void MakeButtons() {
            int BUTTON_SIZE = 100;
            Button[] btn = new Button[5];
            string[] btnString = new string[5] { "Main", "Login", "Register", "Matching","Profile"};
            for(int i = 0; i < 5; i++) {
                btn[i] = new Button();
                btn[i].Left = 0;
                btn[i].Top = i* BUTTON_SIZE;
                btn[i].Width = BUTTON_SIZE;
                btn[i].Height = BUTTON_SIZE;
                btn[i].Margin = new Padding(0);
                btn[i].Text = btnString[i];
                btn[i].Font = new Font("맑은 고딕", 15, FontStyle.Bold);
                this.Controls.Add(btn[i]);
            }
            btn[0].Click += Btn_Main_Click;
            btn[3].Click += Btn_Matching_Click;
        }
        private void Btn_Main_Click(object sender, EventArgs e) {
            pageIndex = 0;
            
            Invalidate();
        }
        private void Btn_Matching_Click(object sender, EventArgs e) {
            pageIndex = 3;
            Invalidate();
        }
    }
}