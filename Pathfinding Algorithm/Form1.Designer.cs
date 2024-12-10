namespace Pathfinding_Algorithm
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnSolve = new System.Windows.Forms.Button();
            this.gridPanel = new System.Windows.Forms.Panel();
            this.listBoxTestOutput = new System.Windows.Forms.ListBox();
            this.listBoxPacketOutput = new System.Windows.Forms.ListBox();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.comboBoxSerialPorts = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnHome = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnStartCamera = new System.Windows.Forms.Button();
            this.btnCaptureImage = new System.Windows.Forms.Button();
            this.btnProcessImage = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSolve
            // 
            this.btnSolve.Location = new System.Drawing.Point(764, 19);
            this.btnSolve.Margin = new System.Windows.Forms.Padding(6);
            this.btnSolve.Name = "btnSolve";
            this.btnSolve.Size = new System.Drawing.Size(150, 44);
            this.btnSolve.TabIndex = 1;
            this.btnSolve.Text = "Solve";
            this.btnSolve.UseVisualStyleBackColor = true;
            this.btnSolve.Click += new System.EventHandler(this.btnSolve_Click);
            // 
            // gridPanel
            // 
            this.gridPanel.Location = new System.Drawing.Point(24, 75);
            this.gridPanel.Margin = new System.Windows.Forms.Padding(6);
            this.gridPanel.Name = "gridPanel";
            this.gridPanel.Size = new System.Drawing.Size(785, 720);
            this.gridPanel.TabIndex = 2;
            this.gridPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.gridPanel_Paint);
            // 
            // listBoxTestOutput
            // 
            this.listBoxTestOutput.FormattingEnabled = true;
            this.listBoxTestOutput.ItemHeight = 25;
            this.listBoxTestOutput.Location = new System.Drawing.Point(15, 807);
            this.listBoxTestOutput.Margin = new System.Windows.Forms.Padding(6);
            this.listBoxTestOutput.Name = "listBoxTestOutput";
            this.listBoxTestOutput.Size = new System.Drawing.Size(385, 104);
            this.listBoxTestOutput.TabIndex = 3;
            // 
            // listBoxPacketOutput
            // 
            this.listBoxPacketOutput.FormattingEnabled = true;
            this.listBoxPacketOutput.ItemHeight = 25;
            this.listBoxPacketOutput.Location = new System.Drawing.Point(424, 807);
            this.listBoxPacketOutput.Margin = new System.Windows.Forms.Padding(6);
            this.listBoxPacketOutput.Name = "listBoxPacketOutput";
            this.listBoxPacketOutput.Size = new System.Drawing.Size(385, 104);
            this.listBoxPacketOutput.TabIndex = 4;
            // 
            // comboBoxSerialPorts
            // 
            this.comboBoxSerialPorts.FormattingEnabled = true;
            this.comboBoxSerialPorts.Location = new System.Drawing.Point(24, 23);
            this.comboBoxSerialPorts.Margin = new System.Windows.Forms.Padding(6);
            this.comboBoxSerialPorts.Name = "comboBoxSerialPorts";
            this.comboBoxSerialPorts.Size = new System.Drawing.Size(238, 33);
            this.comboBoxSerialPorts.TabIndex = 5;
            this.comboBoxSerialPorts.SelectedIndexChanged += new System.EventHandler(this.comboBoxSerialPorts_SelectedIndexChanged);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(278, 19);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(6);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(150, 44);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnHome
            // 
            this.btnHome.Location = new System.Drawing.Point(440, 19);
            this.btnHome.Margin = new System.Windows.Forms.Padding(6);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(150, 44);
            this.btnHome.TabIndex = 7;
            this.btnHome.Text = "Home";
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(602, 19);
            this.btnStart.Margin = new System.Windows.Forms.Padding(6);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(150, 44);
            this.btnStart.TabIndex = 8;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(819, 75);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1391, 836);
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // btnStartCamera
            // 
            this.btnStartCamera.Location = new System.Drawing.Point(926, 19);
            this.btnStartCamera.Margin = new System.Windows.Forms.Padding(6);
            this.btnStartCamera.Name = "btnStartCamera";
            this.btnStartCamera.Size = new System.Drawing.Size(150, 44);
            this.btnStartCamera.TabIndex = 10;
            this.btnStartCamera.Text = "Camera";
            this.btnStartCamera.UseVisualStyleBackColor = true;
            this.btnStartCamera.Click += new System.EventHandler(this.btnStartCamera_Click);
            // 
            // btnCaptureImage
            // 
            this.btnCaptureImage.Location = new System.Drawing.Point(1088, 19);
            this.btnCaptureImage.Margin = new System.Windows.Forms.Padding(6);
            this.btnCaptureImage.Name = "btnCaptureImage";
            this.btnCaptureImage.Size = new System.Drawing.Size(150, 44);
            this.btnCaptureImage.TabIndex = 11;
            this.btnCaptureImage.Text = "Capture";
            this.btnCaptureImage.UseVisualStyleBackColor = true;
            this.btnCaptureImage.Click += new System.EventHandler(this.btnCaptureImage_Click);
            // 
            // btnProcessImage
            // 
            this.btnProcessImage.Location = new System.Drawing.Point(1250, 19);
            this.btnProcessImage.Margin = new System.Windows.Forms.Padding(6);
            this.btnProcessImage.Name = "btnProcessImage";
            this.btnProcessImage.Size = new System.Drawing.Size(150, 44);
            this.btnProcessImage.TabIndex = 12;
            this.btnProcessImage.Text = "Process";
            this.btnProcessImage.UseVisualStyleBackColor = true;
            this.btnProcessImage.Click += new System.EventHandler(this.btnProcessImage_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(2224, 927);
            this.Controls.Add(this.listBoxTestOutput);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnProcessImage);
            this.Controls.Add(this.btnCaptureImage);
            this.Controls.Add(this.btnStartCamera);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnHome);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.comboBoxSerialPorts);
            this.Controls.Add(this.listBoxPacketOutput);
            this.Controls.Add(this.btnSolve);
            this.Controls.Add(this.gridPanel);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnSolve;
        private System.Windows.Forms.Panel gridPanel;
        private System.Windows.Forms.ListBox listBoxTestOutput;
        private System.Windows.Forms.ListBox listBoxPacketOutput;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.ComboBox comboBoxSerialPorts;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnHome;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnStartCamera;
        private System.Windows.Forms.Button btnCaptureImage;
        private System.Windows.Forms.Button btnProcessImage;
    }
}

