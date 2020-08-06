namespace teamproject
{
    partial class Form3
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.tbChat = new System.Windows.Forms.TextBox();
            this.tabUI = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.chatText = new System.Windows.Forms.TextBox();
            this.chatLog = new System.Windows.Forms.ListBox();
            this.tabUI.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbChat
            // 
            this.tbChat.Font = new System.Drawing.Font("휴먼편지체", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tbChat.Location = new System.Drawing.Point(25, 756);
            this.tbChat.Name = "tbChat";
            this.tbChat.Size = new System.Drawing.Size(1375, 45);
            this.tbChat.TabIndex = 4;
            // 
            // tabUI
            // 
            this.tabUI.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.tabUI.Controls.Add(this.tabPage1);
            this.tabUI.Controls.Add(this.tabPage2);
            this.tabUI.Font = new System.Drawing.Font("굴림", 15F);
            this.tabUI.ItemSize = new System.Drawing.Size(27, 30);
            this.tabUI.Location = new System.Drawing.Point(12, 12);
            this.tabUI.Multiline = true;
            this.tabUI.Name = "tabUI";
            this.tabUI.SelectedIndex = 0;
            this.tabUI.Size = new System.Drawing.Size(1450, 830);
            this.tabUI.TabIndex = 5;
            this.tabUI.SelectedIndexChanged += new System.EventHandler(this.tabUI_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tbChat);
            this.tabPage1.Font = new System.Drawing.Font("굴림", 1F);
            this.tabPage1.Location = new System.Drawing.Point(34, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1412, 822);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.chatText);
            this.tabPage2.Controls.Add(this.chatLog);
            this.tabPage2.Location = new System.Drawing.Point(34, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1412, 822);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "ChatRoom";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPage2_Paint);
            // 
            // chatText
            // 
            this.chatText.Font = new System.Drawing.Font("휴먼편지체", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.chatText.Location = new System.Drawing.Point(261, 761);
            this.chatText.Name = "chatText";
            this.chatText.Size = new System.Drawing.Size(1134, 45);
            this.chatText.TabIndex = 5;
            this.chatText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chatText_KeyDown);
            // 
            // chatLog
            // 
            this.chatLog.Font = new System.Drawing.Font("휴먼편지체", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.chatLog.FormattingEnabled = true;
            this.chatLog.ItemHeight = 31;
            this.chatLog.Location = new System.Drawing.Point(261, 23);
            this.chatLog.Name = "chatLog";
            this.chatLog.Size = new System.Drawing.Size(1134, 717);
            this.chatLog.TabIndex = 4;
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1484, 861);
            this.Controls.Add(this.tabUI);
            this.Name = "Form3";
            this.Text = "Form1";
            this.tabUI.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TextBox tbChat;
        private System.Windows.Forms.TabControl tabUI;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TextBox chatText;
        private System.Windows.Forms.ListBox chatLog;
    }
}

