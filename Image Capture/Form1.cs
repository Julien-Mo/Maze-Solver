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
        private VideoCapture _videoCapture;
        private Timer _frameTimer;
        public Form1()
        {
            InitializeComponent();
            InitializeWebcam();
        }
        private void InitializeWebcam()
        {
            _videoCapture = new VideoCapture(1); // Open the default webcam
            if (!_videoCapture.IsOpened())
            {
                MessageBox.Show("Unable to access the webcam.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _frameTimer = new Timer { Interval = 33 }; // ~30 FPS
            _frameTimer.Tick += FrameTimer_Tick;
            _frameTimer.Start();
        }
        private void FrameTimer_Tick(object sender, EventArgs e)
        {
            using (var frame = new Mat())
            {
                if (_videoCapture.Read(frame) && !frame.Empty())
                {
                    pictureBox.Image?.Dispose(); // Dispose of the old image to prevent memory leaks

                    // Resize frame to match the PictureBox dimensions
                    using (var resizedFrame = frame.Resize(new OpenCvSharp.Size(pictureBox.Width, pictureBox.Height)))
                    {
                        pictureBox.Image = BitmapConverter.ToBitmap(resizedFrame); // Convert Mat to Bitmap
                    }
                }
            }
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _frameTimer?.Stop();
            _frameTimer?.Dispose();

            _videoCapture?.Release();
            _videoCapture?.Dispose();

            base.OnFormClosed(e);
        }

        private void btnCaptureImage_Click(object sender, EventArgs e)
        {
            using (var frame = new Mat())
            {
                if (_videoCapture != null && _videoCapture.Read(frame) && !frame.Empty())
                {
                    // Save the image to a file
                    const string fileName = "captured_image.png";
                    frame.SaveImage(fileName);

                    // Stop the timer and release the webcam
                    _frameTimer.Stop();
                    _videoCapture.Release();

                    pictureBox.Image?.Dispose();
                    using (var resizedFrame = frame.Resize(new OpenCvSharp.Size(pictureBox.Width, pictureBox.Height)))
                    {
                        pictureBox.Image = BitmapConverter.ToBitmap(resizedFrame);
                    }
                    MessageBox.Show($"Image captured and saved as {fileName}", "Capture Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to capture image.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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

        private void buttonReset_Click(object sender, EventArgs e)
        {
            // Restart the webcam
            if (_videoCapture == null || !_videoCapture.IsOpened())
            {
                InitializeWebcam();
            }

            // Restart the frame timer
            _frameTimer?.Start();
        }
    }
}
