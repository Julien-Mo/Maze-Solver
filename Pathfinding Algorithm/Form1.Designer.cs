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
            this.buttonSolve = new System.Windows.Forms.Button();
            this.gridPanel = new System.Windows.Forms.Panel();
            this.listBoxTestOutput = new System.Windows.Forms.ListBox();
            this.listBoxPacketOutput = new System.Windows.Forms.ListBox();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.SuspendLayout();
            // 
            // buttonSolve
            // 
            this.buttonSolve.Location = new System.Drawing.Point(12, 344);
            this.buttonSolve.Name = "buttonSolve";
            this.buttonSolve.Size = new System.Drawing.Size(75, 23);
            this.buttonSolve.TabIndex = 1;
            this.buttonSolve.Text = "Solve";
            this.buttonSolve.UseVisualStyleBackColor = true;
            this.buttonSolve.Click += new System.EventHandler(this.buttonSolve_Click);
            // 
            // gridPanel
            // 
            this.gridPanel.Location = new System.Drawing.Point(12, 12);
            this.gridPanel.Name = "gridPanel";
            this.gridPanel.Size = new System.Drawing.Size(445, 329);
            this.gridPanel.TabIndex = 2;
            this.gridPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.gridPanel_Paint);
            // 
            // listBoxTestOutput
            // 
            this.listBoxTestOutput.FormattingEnabled = true;
            this.listBoxTestOutput.Location = new System.Drawing.Point(463, 12);
            this.listBoxTestOutput.Name = "listBoxTestOutput";
            this.listBoxTestOutput.Size = new System.Drawing.Size(99, 355);
            this.listBoxTestOutput.TabIndex = 3;
            // 
            // listBoxPacketOutput
            // 
            this.listBoxPacketOutput.FormattingEnabled = true;
            this.listBoxPacketOutput.Location = new System.Drawing.Point(568, 12);
            this.listBoxPacketOutput.Name = "listBoxPacketOutput";
            this.listBoxPacketOutput.Size = new System.Drawing.Size(171, 355);
            this.listBoxPacketOutput.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(855, 427);
            this.Controls.Add(this.listBoxPacketOutput);
            this.Controls.Add(this.buttonSolve);
            this.Controls.Add(this.listBoxTestOutput);
            this.Controls.Add(this.gridPanel);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button buttonSolve;
        private System.Windows.Forms.Panel gridPanel;
        private System.Windows.Forms.ListBox listBoxTestOutput;
        private System.Windows.Forms.ListBox listBoxPacketOutput;
        private System.IO.Ports.SerialPort serialPort1;
    }
}

