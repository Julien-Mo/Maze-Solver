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
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.panelControls = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelLiveCamera = new System.Windows.Forms.Label();
            this.pictureBoxLiveCamera = new System.Windows.Forms.PictureBox();
            this.buttonProcess = new System.Windows.Forms.Button();
            this.btnSolve = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnCapture = new System.Windows.Forms.Button();
            this.comboBoxSerialPorts = new System.Windows.Forms.ComboBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnMove = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panelControls.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLiveCamera)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // serialPort
            // 
            this.serialPort.PortName = "COM4";
            // 
            // panelControls
            // 
            this.panelControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panelControls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panelControls.Controls.Add(this.panel1);
            this.panelControls.Controls.Add(this.buttonProcess);
            this.panelControls.Controls.Add(this.btnSolve);
            this.panelControls.Controls.Add(this.btnConnect);
            this.panelControls.Controls.Add(this.btnCapture);
            this.panelControls.Controls.Add(this.comboBoxSerialPorts);
            this.panelControls.Controls.Add(this.btnReset);
            this.panelControls.Controls.Add(this.btnMove);
            this.panelControls.Location = new System.Drawing.Point(0, 0);
            this.panelControls.Margin = new System.Windows.Forms.Padding(4);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(600, 1214);
            this.panelControls.TabIndex = 20;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelLiveCamera);
            this.panel1.Controls.Add(this.pictureBoxLiveCamera);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(600, 577);
            this.panel1.TabIndex = 21;
            // 
            // labelLiveCamera
            // 
            this.labelLiveCamera.AutoSize = true;
            this.labelLiveCamera.BackColor = System.Drawing.Color.Transparent;
            this.labelLiveCamera.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.labelLiveCamera.Font = new System.Drawing.Font("Cascadia Code", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLiveCamera.ForeColor = System.Drawing.Color.Red;
            this.labelLiveCamera.Location = new System.Drawing.Point(372, 6);
            this.labelLiveCamera.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelLiveCamera.Name = "labelLiveCamera";
            this.labelLiveCamera.Size = new System.Drawing.Size(220, 50);
            this.labelLiveCamera.TabIndex = 20;
            this.labelLiveCamera.Text = "Live Feed";
            // 
            // pictureBoxLiveCamera
            // 
            this.pictureBoxLiveCamera.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxLiveCamera.Location = new System.Drawing.Point(6, 6);
            this.pictureBoxLiveCamera.Margin = new System.Windows.Forms.Padding(6);
            this.pictureBoxLiveCamera.Name = "pictureBoxLiveCamera";
            this.pictureBoxLiveCamera.Size = new System.Drawing.Size(588, 565);
            this.pictureBoxLiveCamera.TabIndex = 19;
            this.pictureBoxLiveCamera.TabStop = false;
            // 
            // buttonProcess
            // 
            this.buttonProcess.BackColor = System.Drawing.Color.SlateBlue;
            this.buttonProcess.Font = new System.Drawing.Font("Cascadia Code", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonProcess.ForeColor = System.Drawing.SystemColors.Control;
            this.buttonProcess.Location = new System.Drawing.Point(6, 764);
            this.buttonProcess.Margin = new System.Windows.Forms.Padding(6);
            this.buttonProcess.Name = "buttonProcess";
            this.buttonProcess.Size = new System.Drawing.Size(586, 77);
            this.buttonProcess.TabIndex = 19;
            this.buttonProcess.Text = "Process";
            this.buttonProcess.UseVisualStyleBackColor = false;
            this.buttonProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // btnSolve
            // 
            this.btnSolve.BackColor = System.Drawing.Color.SlateBlue;
            this.btnSolve.Font = new System.Drawing.Font("Cascadia Code", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSolve.ForeColor = System.Drawing.SystemColors.Control;
            this.btnSolve.Location = new System.Drawing.Point(6, 853);
            this.btnSolve.Margin = new System.Windows.Forms.Padding(6);
            this.btnSolve.Name = "btnSolve";
            this.btnSolve.Size = new System.Drawing.Size(586, 77);
            this.btnSolve.TabIndex = 15;
            this.btnSolve.Text = "Solve";
            this.btnSolve.UseVisualStyleBackColor = false;
            this.btnSolve.Click += new System.EventHandler(this.btnSolve_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.SlateBlue;
            this.btnConnect.Font = new System.Drawing.Font("Cascadia Code", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.ForeColor = System.Drawing.SystemColors.Control;
            this.btnConnect.Location = new System.Drawing.Point(6, 587);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(6);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(586, 77);
            this.btnConnect.TabIndex = 13;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnCapture
            // 
            this.btnCapture.BackColor = System.Drawing.Color.SlateBlue;
            this.btnCapture.Font = new System.Drawing.Font("Cascadia Code", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCapture.ForeColor = System.Drawing.SystemColors.Control;
            this.btnCapture.Location = new System.Drawing.Point(6, 675);
            this.btnCapture.Margin = new System.Windows.Forms.Padding(6);
            this.btnCapture.Name = "btnCapture";
            this.btnCapture.Size = new System.Drawing.Size(586, 77);
            this.btnCapture.TabIndex = 14;
            this.btnCapture.Text = "Capture";
            this.btnCapture.UseVisualStyleBackColor = false;
            this.btnCapture.Click += new System.EventHandler(this.btnCapture_Click);
            // 
            // comboBoxSerialPorts
            // 
            this.comboBoxSerialPorts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxSerialPorts.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.comboBoxSerialPorts.Font = new System.Drawing.Font("Cascadia Code", 7.875F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBoxSerialPorts.ForeColor = System.Drawing.SystemColors.Window;
            this.comboBoxSerialPorts.FormattingEnabled = true;
            this.comboBoxSerialPorts.Location = new System.Drawing.Point(6, 1120);
            this.comboBoxSerialPorts.Margin = new System.Windows.Forms.Padding(6);
            this.comboBoxSerialPorts.Name = "comboBoxSerialPorts";
            this.comboBoxSerialPorts.Size = new System.Drawing.Size(196, 36);
            this.comboBoxSerialPorts.TabIndex = 6;
            // 
            // btnReset
            // 
            this.btnReset.BackColor = System.Drawing.Color.SlateBlue;
            this.btnReset.Font = new System.Drawing.Font("Cascadia Code", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.ForeColor = System.Drawing.SystemColors.Control;
            this.btnReset.Location = new System.Drawing.Point(6, 1031);
            this.btnReset.Margin = new System.Windows.Forms.Padding(6);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(586, 77);
            this.btnReset.TabIndex = 17;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnMove
            // 
            this.btnMove.BackColor = System.Drawing.Color.SlateBlue;
            this.btnMove.Font = new System.Drawing.Font("Cascadia Code", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMove.ForeColor = System.Drawing.SystemColors.Control;
            this.btnMove.Location = new System.Drawing.Point(6, 942);
            this.btnMove.Margin = new System.Windows.Forms.Padding(6);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(586, 77);
            this.btnMove.TabIndex = 16;
            this.btnMove.Text = "Move";
            this.btnMove.UseVisualStyleBackColor = false;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pictureBox.Location = new System.Drawing.Point(606, 6);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(6);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(1322, 1202);
            this.pictureBox.TabIndex = 18;
            this.pictureBox.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.panel2.Controls.Add(this.pictureBox);
            this.panel2.Controls.Add(this.panelControls);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1934, 1214);
            this.panel2.TabIndex = 21;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(51)))), ((int)(((byte)(73)))));
            this.ClientSize = new System.Drawing.Size(1934, 1214);
            this.Controls.Add(this.panel2);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "Form1";
            this.Text = "Maze Solver";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.panelControls.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLiveCamera)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelLiveCamera;
        private System.Windows.Forms.PictureBox pictureBoxLiveCamera;
        private System.Windows.Forms.Button buttonProcess;
        private System.Windows.Forms.Button btnSolve;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnCapture;
        private System.Windows.Forms.ComboBox comboBoxSerialPorts;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnMove;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Panel panel2;
    }
}

