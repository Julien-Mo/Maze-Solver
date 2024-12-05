namespace Maze_Solver
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
            this.comboBoxSerialPorts = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnCapture = new System.Windows.Forms.Button();
            this.btnSolve = new System.Windows.Forms.Button();
            this.btnMove = new System.Windows.Forms.Button();
            this.buttonReset = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.buttonProcess = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxSerialPorts
            // 
            this.comboBoxSerialPorts.FormattingEnabled = true;
            this.comboBoxSerialPorts.Location = new System.Drawing.Point(24, 23);
            this.comboBoxSerialPorts.Margin = new System.Windows.Forms.Padding(6);
            this.comboBoxSerialPorts.Name = "comboBoxSerialPorts";
            this.comboBoxSerialPorts.Size = new System.Drawing.Size(238, 33);
            this.comboBoxSerialPorts.TabIndex = 6;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(278, 19);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(6);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(150, 44);
            this.btnConnect.TabIndex = 13;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnCapture
            // 
            this.btnCapture.Location = new System.Drawing.Point(440, 19);
            this.btnCapture.Margin = new System.Windows.Forms.Padding(6);
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(150, 44);
            this.btnCapture.TabIndex = 14;
            this.btnCapture.Text = "Capture";
            this.btnCapture.UseVisualStyleBackColor = true;
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // btnSolve
            // 
            this.btnSolve.Location = new System.Drawing.Point(764, 19);
            this.btnSolve.Margin = new System.Windows.Forms.Padding(6);
            this.btnSolve.Name = "btnSolve";
            this.btnSolve.Size = new System.Drawing.Size(150, 44);
            this.btnSolve.TabIndex = 15;
            this.btnSolve.Text = "Solve";
            this.btnSolve.UseVisualStyleBackColor = true;
            this.btnSolve.Click += new System.EventHandler(this.btnSolve_Click);
            // 
            // btnMove
            // 
            this.btnMove.Location = new System.Drawing.Point(926, 19);
            this.btnMove.Margin = new System.Windows.Forms.Padding(6);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(150, 44);
            this.btnMove.TabIndex = 16;
            this.btnMove.Text = "Move";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // buttonReset
            // 
            this.buttonReset.Location = new System.Drawing.Point(1088, 19);
            this.buttonReset.Margin = new System.Windows.Forms.Padding(6);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(150, 44);
            this.buttonReset.TabIndex = 17;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(24, 75);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(6);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(960, 720);
            this.pictureBox.TabIndex = 18;
            this.pictureBox.TabStop = false;
            // 
            // serialPort
            // 
            this.serialPort.PortName = "COM4";
            // 
            // buttonProcess
            // 
            this.buttonProcess.Location = new System.Drawing.Point(602, 19);
            this.buttonProcess.Margin = new System.Windows.Forms.Padding(6);
            this.buttonProcess.Name = "buttonProcess";
            this.buttonProcess.Size = new System.Drawing.Size(150, 44);
            this.buttonProcess.TabIndex = 19;
            this.buttonProcess.Text = "Process";
            this.buttonProcess.UseVisualStyleBackColor = true;
            this.buttonProcess.Click += new System.EventHandler(this.buttonProcess_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1255, 814);
            this.Controls.Add(this.buttonProcess);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.btnMove);
            this.Controls.Add(this.btnSolve);
            this.Controls.Add(this.btnCapture);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.comboBoxSerialPorts);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.Text = "Maze Solver";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxSerialPorts;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnCapture;
        private System.Windows.Forms.Button btnSolve;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.Button buttonProcess;
    }
}

