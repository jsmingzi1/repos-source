namespace rdphelper
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.m_rdpview = new AxMSTSCLib.AxMsRdpClient8NotSafeForScripting();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            ((System.ComponentModel.ISupportInitialize)(this.m_rdpview)).BeginInit();
            this.SuspendLayout();
            // 
            // m_rdpview
            // 
            this.m_rdpview.Enabled = true;
            this.m_rdpview.Location = new System.Drawing.Point(12, 12);
            this.m_rdpview.Name = "m_rdpview";
            this.m_rdpview.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("m_rdpview.OcxState")));
            this.m_rdpview.Size = new System.Drawing.Size(884, 500);
            this.m_rdpview.TabIndex = 2;
            this.m_rdpview.OnConnecting += new System.EventHandler(this.m_rdpview_OnConnecting);
            this.m_rdpview.OnConnected += new System.EventHandler(this.m_rdpview_OnConnected);
            this.m_rdpview.OnLoginComplete += new System.EventHandler(this.m_rdpview_OnLoginComplete);
            this.m_rdpview.OnDisconnected += new AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEventHandler(this.m_rdpview_OnDisconnected);
            this.m_rdpview.OnFatalError += new AxMSTSCLib.IMsTscAxEvents_OnFatalErrorEventHandler(this.m_rdpview_OnFatalError);
            this.m_rdpview.OnWarning += new AxMSTSCLib.IMsTscAxEvents_OnWarningEventHandler(this.m_rdpview_OnWarning);
            this.m_rdpview.OnAuthenticationWarningDisplayed += new System.EventHandler(this.m_rdpview_OnAuthenticationWarningDisplayed);
            this.m_rdpview.OnAuthenticationWarningDismissed += new System.EventHandler(this.m_rdpview_OnAuthenticationWarningDismissed);
            this.m_rdpview.OnLogonError += new AxMSTSCLib.IMsTscAxEvents_OnLogonErrorEventHandler(this.m_rdpview_OnLogonError);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(908, 524);
            this.Controls.Add(this.m_rdpview);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.m_rdpview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private AxMSTSCLib.AxMsRdpClient8NotSafeForScripting m_rdpview;
        private System.Windows.Forms.ColorDialog colorDialog1;
    }
}

