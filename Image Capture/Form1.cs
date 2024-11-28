using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Image_Capture
{
    public partial class Form1 : Form
    {
        private VideoCapture capture;
        private Mat frame;
        private Bitmap image;
        private bool isCameraRunning = false;
        public Form1()
        {
            InitializeComponent();
        }
        private void btnStartCamera_Click(object sender, EventArgs e)
        {
            if (!isCameraRunning)
            {
                capture = new VideoCapture(); // Open default camera
                capture.Open(1);

                if (!capture.IsOpened())
                {
                    MessageBox.Show("Could not open camera.");
                    return;
                }

                frame = new Mat();
                isCameraRunning = true;

                // Start a timer to capture frames
                Timer timer = new Timer();
                timer.Interval = 30; // Adjust as necessary
                timer.Tick += (s, args) =>
                {
                    capture.Read(frame);
                    if (!frame.Empty())
                    {
                        image = BitmapConverter.ToBitmap(frame);
                        pictureBox1.Image = image;
                    }
                };
                timer.Start();
            }
        }

        private void btnCaptureImage_Click(object sender, EventArgs e)
        {
            if (isCameraRunning && image != null)
            {
                string filePath = $"CapturedImage.png";
                image.Save(filePath);
                MessageBox.Show($"Image saved at {filePath}");
            }
            else
            {
                MessageBox.Show("No image to capture.");
            }
        }
        private void btnRunPython_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the base directory of the application
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                // Calculate the relative path to the Python script
                string relativePath = @"..\..\..\Image Processing\image_processing.py";
                string pythonScriptPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(baseDir, relativePath));

                // Check if the Python script exists
                if (!System.IO.File.Exists(pythonScriptPath))
                {
                    MessageBox.Show($"Python script not found at: {pythonScriptPath}");
                    return;
                }

                // Configure the process to run Python
                var psi = new ProcessStartInfo
                {
                    FileName = "python", // Ensure 'python' is in PATH
                    Arguments = $"\"{pythonScriptPath}\"", // Path to Python script
                    UseShellExecute = false,   // Don't use shell execution
                    CreateNoWindow = true,     // Run without creating a command prompt window
                };

                // Start the Python process
                Process.Start(psi);

                MessageBox.Show("Python script executed!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Release resources when the form is closing
            if (capture != null)
            {
                capture.Release();
                capture.Dispose();
            }
            if (frame != null)
            {
                frame.Dispose();
            }
        }
    }
}
