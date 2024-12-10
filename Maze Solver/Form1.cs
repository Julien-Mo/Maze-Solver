using Newtonsoft.Json;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Point = System.Drawing.Point;

namespace Maze_Solver
{
    public partial class Form1 : Form
    {
        // Constants
        private const int GridHeight = 63;
        private const int GridWidth = 65;
        private const int StraightCost = 10;
        private const int DiagonalCost = 14;

        // Grid Variables
        private Point home = new Point(0, 0);
        private Point start = new Point(0, 0);
        private Point end = new Point(0, 0);
        private Point? lastEndpoint = null;
        private int[,] maze = new int[GridHeight, GridWidth];

        // Packet Variables
        private byte startByte = Byte.MaxValue;
        private byte xByte = 0;
        private byte yByte = 0;
        private byte commandByte = 0;
        private List<byte[]> packets = new List<byte[]>();

        // Flags
        private bool isStartHomingSequence = false;
        private bool isMagnetOn = false;

        // Image Processing
        private VideoCapture capture;
        private Mat frame;
        private Timer captureTimer;
        private bool isCameraDisplayed = true;

        public Form1()
        {
            InitializeComponent();
            InitializeSerialPort();
            InitializeCamera();
        }

        // ============================
        // Serial Port
        // ============================

        private void InitializeSerialPort()
        {
            comboBoxSerialPorts.Items.Clear();
            comboBoxSerialPorts.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
            if (comboBoxSerialPorts.Items.Count == 0)
            {
                comboBoxSerialPorts.Text = "No ports!";
            }
            else
            {
                comboBoxSerialPorts.SelectedIndex = 0;
            }
        }

        private void comboBoxSerialPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort.PortName = comboBoxSerialPorts.SelectedItem.ToString();
        }

        // ============================
        // Image Capture
        // ============================

        private void InitializeCamera()
        {
            capture = new VideoCapture(1);
            if (!capture.IsOpened())
            {
                MessageBox.Show("Unable to access the camera.");    
                return;
            }

            frame = new Mat();
            captureTimer = new Timer { Interval = 30 };
            captureTimer.Tick += (s, args) => CaptureFrame();
            captureTimer.Start();
        }

        private void CaptureFrame()
        {
            capture.Read(frame);
            if (!frame.Empty())
            {
                if (isCameraDisplayed)
                {
                    pictureBox.Image = BitmapConverter.ToBitmap(
                        frame.Resize(new OpenCvSharp.Size(pictureBox.Width, pictureBox.Height))
                    );
                }
                pictureBoxLiveCamera.Image = BitmapConverter.ToBitmap(
                    frame.Resize(new OpenCvSharp.Size(pictureBoxLiveCamera.Width, pictureBoxLiveCamera.Height))
                );
            }
        }

        // ============================
        // Maze Initialization
        // ============================

        private class MazeData
        {
            public int[,] Maze { get; set; }
            public int[] Start { get; set; }
            public int[] End { get; set; }
        }

        private void InitializeMaze()
        {
            string projectRoot = Directory.GetParent(Directory.GetParent(Directory.GetParent(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName).FullName).FullName).FullName;
            string filePath = Path.Combine(projectRoot, "Image Processing", "maze_data.json");
            string json = File.ReadAllText(filePath);
            var mazeData = JsonConvert.DeserializeObject<MazeData>(File.ReadAllText(filePath));

            maze = mazeData.Maze;
            end = new Point(mazeData.Start[0], mazeData.Start[1]);
            start = new Point(mazeData.End[0], mazeData.End[1]);
        }

        // ============================
        // A* Pathfinding
        // ============================

        private void FindShortestPath()
        {
            InitializeCosts(out var gCosts, out var fCosts, out var parents);
            var openNodes = new SimplePriorityQueue<Point>();
            var closedNodes = new HashSet<Point>();

            // Start node setup
            gCosts[start.Y, start.X] = 0;
            fCosts[start.Y, start.X] = CalculateCost(start, end);
            openNodes.Enqueue(start, fCosts[start.Y, start.X]);

            // A* search loop
            while (openNodes.Count > 0)
            {
                var currentNode = openNodes.Dequeue();
                if (currentNode == end)
                {
                    ReconstructPath(currentNode, parents);
                    return;
                }
                ProcessNode(currentNode, openNodes, closedNodes, gCosts, fCosts, parents);
            }
        }

        private void InitializeCosts(out int[,] gCosts, out int[,] fCosts, out Point?[,] parents)
        {
            gCosts = new int[GridHeight, GridWidth];
            fCosts = new int[GridHeight, GridWidth];
            parents = new Point?[GridHeight, GridWidth];

            for (int i = 0; i < GridHeight; i++)
                for (int j = 0; j < GridWidth; j++)
                    gCosts[i, j] = fCosts[i, j] = int.MaxValue;
        }

        private void ProcessNode(Point currentNode, SimplePriorityQueue<Point> openNodes, HashSet<Point> closedNodes, int[,] gCosts, int[,] fCosts, Point?[,] parents)
        {
            closedNodes.Add(currentNode);

            foreach (var neighbor in GetNeighbors(currentNode))
            {
                if (!IsValid(neighbor, closedNodes)) continue;

                int tentativeGCost = gCosts[currentNode.Y, currentNode.X] + (IsDiagonal(currentNode, neighbor) ? DiagonalCost : StraightCost);
                if (tentativeGCost < gCosts[neighbor.Y, neighbor.X])
                {
                    gCosts[neighbor.Y, neighbor.X] = tentativeGCost;
                    fCosts[neighbor.Y, neighbor.X] = tentativeGCost + CalculateCost(neighbor, end);
                    parents[neighbor.Y, neighbor.X] = currentNode;

                    if (!openNodes.Contains(neighbor))
                        openNodes.Enqueue(neighbor, fCosts[neighbor.Y, neighbor.X]);
                    else
                    {
                        openNodes.UpdatePriority(neighbor, fCosts[neighbor.Y, neighbor.X]);
                    }
                }
            }
        }

        private bool IsValid(Point neighbor, HashSet<Point> closedNodes)
        {
            return neighbor.Y >= 0 && neighbor.Y < GridHeight &&
                   neighbor.X >= 0 && neighbor.X < GridWidth &&
                   maze[neighbor.Y, neighbor.X] != 1 && !closedNodes.Contains(neighbor);
        }

        private bool IsDiagonal(Point current, Point neighbor)
        {
            return current.X != neighbor.X && current.Y != neighbor.Y;
        }

        private void ReconstructPath(Point currentNode, Point?[,] parents)
        {
            // Reconstruct the path from the end node to the start node
            var path = new List<Point>();
            while (currentNode != start)
            {
                path.Add(currentNode);
                currentNode = parents[currentNode.Y, currentNode.X].Value;
            }
            path.Add(start);
            path.Reverse();

            // Group path into straight-line segments
            var segments = new List<List<Point>>();
            var currentSegment = new List<Point> { path[0] };
            for (int i = 1; i < path.Count; i++)
            {
                var (dx, dy) = (path[i].X - path[i - 1].X, path[i].Y - path[i - 1].Y);
                if (i > 1)
                {
                    var (prevDx, prevDy) = (path[i - 1].X - path[i - 2].X, path[i - 1].Y - path[i - 2].Y);
                    if (dx != prevDx || dy != prevDy)
                    {
                        segments.Add(new List<Point>(currentSegment));
                        currentSegment.Clear();
                    }
                }
                currentSegment.Add(path[i]);
            }
            if (currentSegment.Any()) segments.Add(currentSegment);

            lastEndpoint = start;
            isMagnetOn = true;
            packets.Add(CreatePacket(start));

            // Process each segment
            foreach (var segment in segments)
            {
                var endNode = segment.Last();
                isMagnetOn = true;
                packets.Add(CreatePacket(endNode));
                DrawLine(endNode);
            }
            lastEndpoint = null;
        }


        private int CalculateCost(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private List<Point> GetNeighbors(Point point)
        {
            int row = point.Y;
            int col = point.X;

            return new List<Point>
            {
                new Point(col + 1, row - 1), // Up-Right
                new Point(col, row - 1),     // Up
                new Point(col - 1, row - 1), // Up-Left
                new Point(col - 1, row),     // Left
                new Point(col - 1, row + 1), // Down-Left
                new Point(col, row + 1),     // Down
                new Point(col + 1, row + 1), // Down-Right
                new Point(col + 1, row),     // Right
            };
        }

        // ============================
        // Packet Creation & Serial Communication
        // ============================

        private byte[] CreatePacket(Point currentNode)
        {
            xByte = (byte)(currentNode.X);
            yByte = (byte)(GridHeight - currentNode.Y);
            commandByte = GetCommandByte();
            return new byte[]
            {
                startByte,
                xByte,
                yByte,
                commandByte,
            };
        }

        private void SendPacket(byte[] packet)
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    serialPort.Write(packet, 0, packet.Length);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private byte GetCommandByte()
        {
            byte commandByte = 0;

            // Bit 0: Y Escape Bit (0 = Do Nothing, 1 = Set Y Byte to 255)
            if (yByte == Byte.MaxValue)
            {
                commandByte |= (1 << 0);
                yByte = Byte.MaxValue - 1;
            }

            // Bit 2: X Escape Bit (0 = Do Nothing, 1 = Set X Byte to 255)
            if (xByte == Byte.MaxValue)
            {
                commandByte |= (1 << 2);
                xByte = Byte.MaxValue - 1;
            }

            // Bit 4: Magnet On (0 = Off, 1 = On)
            if (isMagnetOn)
            {
                commandByte |= (1 << 4);
            }
            
            // Bit 5: Start Homing Sequence (0 = Off, 1 = On)
            if (isStartHomingSequence)
            {
                commandByte |= (1 << 5);
            }

            // Reset the boolean flags
            isStartHomingSequence = false;
            isMagnetOn = false;

            return commandByte;
        }

        // ============================
        // UI Rendering
        // ============================

        private void DrawLine(Point endNode)
        {
            if (pictureBox.Image == null)
            {
                pictureBox.Image = new Bitmap(pictureBox.Width, pictureBox.Height);
            }

            using (Graphics g = Graphics.FromImage(pictureBox.Image))
            {
                using (Pen pen = new Pen(Color.Blue, 2))
                {
                    // Scale the coordinates based on the PictureBox dimensions
                    float scaleX = (float)pictureBox.Width / GridWidth;
                    float scaleY = (float)pictureBox.Height / GridHeight;

                    // If there is a last endpoint, use it as the startNode
                    PointF scaledStartNode = new PointF(lastEndpoint.Value.X * scaleX, lastEndpoint.Value.Y * scaleY);

                    PointF scaledEndNode = new PointF(endNode.X * scaleX, endNode.Y * scaleY);

                    // Draw the line
                    g.DrawLine(pen, scaledStartNode, scaledEndNode);
                }
            }
            lastEndpoint = endNode;

            // Refresh the PictureBox to show the updated image
            pictureBox.Invalidate();
        }

        // ============================
        // Buttons
        // ============================

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!serialPort.IsOpen)
            {
                try
                {
                    serialPort.Open();
                    btnConnect.Text = "Disconnect";
                }
                catch (System.IO.IOException)
                {
                    MessageBox.Show("Unable to open the serial port.");
                }
                catch (System.UnauthorizedAccessException)
                {
                    MessageBox.Show("Access to the port is denied.");
                }
            }
            else
            {
                serialPort.Close();
                btnConnect.Text = "Connect";
            }
        }

        private void btnCapture_Click(object sender, EventArgs e)
        {
            if (frame != null && !frame.Empty())
            {
                try
                {
                    isCameraDisplayed = false;
                    BitmapConverter.ToBitmap(frame).Save("captured_image.png");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving image: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("No live feed to capture an image from.");
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            // Get the path of the application
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string relativeScriptPath = @"..\..\..\Image Processing\image_processing.py";
            string relativeImagePath = @"..\..\..\Image Processing\processed_image.png";
            string pythonScriptPath = Path.GetFullPath(Path.Combine(baseDir, relativeScriptPath));
            string processedImagePath = Path.GetFullPath(Path.Combine(baseDir, relativeImagePath));

            // Check if the Python script exists
            if (!File.Exists(pythonScriptPath))
            {
                MessageBox.Show($"Python script not found at: {pythonScriptPath}");
                return;
            }

            // Configure the process to run Python OpenCV script to process the image
            var psi = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"\"{pythonScriptPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            // Start the process
            var process = Process.Start(psi);
            process.WaitForExit();

            // Check if the processed image exists
            if (!File.Exists(processedImagePath))
            {
                MessageBox.Show($"Processed image not found at: {processedImagePath}");
                return;
            }

            // Load and resize the image to fit the pictureBox
            using (var originalImage = new Bitmap(processedImagePath))
            {
                var resizedImage = new Bitmap(originalImage, pictureBox.Size);
                pictureBox.Image = resizedImage;
            }

            InitializeMaze();
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            FindShortestPath();
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            foreach (var packet in packets)
            {
                SendPacket(packet);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            // Restart the camera
            if (capture == null || !capture.IsOpened())
            {
                InitializeCamera();
            }
            isCameraDisplayed = true;

            // Reset the packets queue
            packets.Clear();

            // Home the stepper motors
            isStartHomingSequence = true;
            var packet = CreatePacket(home);
            SendPacket(packet);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            captureTimer?.Stop();
            captureTimer?.Dispose();
            capture?.Release();
            capture?.Dispose();
            frame?.Dispose();
        }
    }
}
