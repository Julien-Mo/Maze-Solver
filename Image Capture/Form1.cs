using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
            _videoCapture = new VideoCapture(1);
            if (!_videoCapture.IsOpened())
            {
                MessageBox.Show("Unable to access the webcam.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _frameTimer = new Timer { Interval = 33 };
            _frameTimer.Tick += FrameTimer_Tick;
            _frameTimer.Start();
        }
        private void FrameTimer_Tick(object sender, EventArgs e)
        {
            using (var frame = new Mat())
            {
                if (_videoCapture.Read(frame) && !frame.Empty())
                {
                    pictureBox.Image?.Dispose();

                    // Resize frame to match the PictureBox dimensions
                    using (var resizedFrame = frame.Resize(new OpenCvSharp.Size(pictureBox.Width, pictureBox.Height)))
                    {
                        pictureBox.Image = BitmapConverter.ToBitmap(resizedFrame);
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
                string relativeImagePath = @"..\..\..\Image Processing\processed_image.png";
                string pythonScriptPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(baseDir, relativePath));
                string processedImagePath = Path.GetFullPath(Path.Combine(baseDir, relativeImagePath));

                // Check if the Python script exists
                if (!System.IO.File.Exists(pythonScriptPath))
                {
                    MessageBox.Show($"Python script not found at: {pythonScriptPath}");
                    return;
                }

                // Configure the process to run Python
                var psi = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"\"{pythonScriptPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                // Start the Python process
                var process = Process.Start(psi);
                process.WaitForExit();

                // Load and resize the image to fit the pictureBox
                using (var originalImage = new Bitmap(processedImagePath))
                {
                    var resizedImage = new Bitmap(originalImage, pictureBox.Size);
                    pictureBox.Image = resizedImage;
                }

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
