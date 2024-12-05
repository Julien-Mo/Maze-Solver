using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Priority_Queue;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Diagnostics;
using Point = System.Drawing.Point;

namespace Pathfinding_Algorithm
{
    public partial class Form1 : Form
    {
        // Constants
        private const int GridHeight = 63;
        private const int GridWidth = 65;
        private const int PointSize = 6;
        private const int StraightCost = 10;
        private const int DiagonalCost = 14;

        // Grid Variables
        private Point start = new Point(0, 0);
        private Point end = new Point(0, 0);
        private int[,] maze = new int[GridHeight, GridWidth];

        // Packet Variables
        private byte startByte = Byte.MaxValue;
        private byte xByte = 0;
        private byte yByte = 0;
        private byte commandByte = 0;

        // Flags
        private bool isStartHomingSequence = false;
        private bool isMagnetOn = false;

        // Image Processing
        private VideoCapture capture;
        private Mat frame;
        private Bitmap image;
        private bool isCameraRunning = false;
        private Timer captureTimer = new Timer();

        public Form1()
        {
            InitializeComponent();
            InitializeSerialPort();
            InitializeMaze();
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
                comboBoxSerialPorts.Text = "No COM ports!";
            }
            else
            {
                comboBoxSerialPorts.SelectedIndex = 0;
            }
        }

        private void comboBoxSerialPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            serialPort1.PortName = comboBoxSerialPorts.SelectedItem.ToString();
        }

        // ============================
        // Maze Initialization & Serialization
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
        // A* Pathfinding Logic
        // ============================

        private void FindShortestPath()
        {
            InitializeCosts(out var gCosts, out var fCosts, out var parents);
            var openNodes = new SimplePriorityQueue<Point>();
            var closedNodes = new HashSet<Point>();

            // Start node setup
            gCosts[start.Y, start.X] = 0;
            fCosts[start.Y, start.X] = CalculateHeuristic(start, end);
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
            PaintPoint(currentNode, Color.Gray);

            foreach (var neighbor in GetNeighbors(currentNode))
            {
                if (!IsValid(neighbor, closedNodes)) continue;

                int tentativeGCost = gCosts[currentNode.Y, currentNode.X] + (IsDiagonal(currentNode, neighbor) ? DiagonalCost : StraightCost);
                if (tentativeGCost < gCosts[neighbor.Y, neighbor.X])
                {
                    gCosts[neighbor.Y, neighbor.X] = tentativeGCost;
                    fCosts[neighbor.Y, neighbor.X] = tentativeGCost + CalculateHeuristic(neighbor, end);
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
            // Create a list to store the path from end to start
            List<Point> path = new List<Point>();

            // Traverse the path from end to start
            Point node = currentNode;
            while (node != start)
            {
                path.Add(node);
                node = parents[node.Y, node.X].Value;
            }
            path.Add(start);

            // Reverse the list to get the path from start to end
            path.Reverse();

            // Combine the path to straight-line segments
            List<List<Point>> segments = new List<List<Point>>();
            List<Point> currentSegment = new List<Point> { path[0] };

            for (int i = 1; i < path.Count; i++)
            {
                int dx = path[i].X - path[i - 1].X;
                int dy = path[i].Y - path[i - 1].Y;
                if (i > 1)
                {
                    int prevDx = path[i - 1].X - path[i - 2].X;
                    int prevDy = path[i - 1].Y - path[i - 2].Y;
                    if (dx != prevDx || dy != prevDy)
                    {
                        segments.Add(new List<Point>(currentSegment));
                        currentSegment.Clear();
                    }
                }
                currentSegment.Add(path[i]);
            }
            if (currentSegment.Count > 0)
            {
                segments.Add(currentSegment);
            }

            isMagnetOn = true;
            var packet = CreatePacket(start);
            SendPacket(packet);

            // Send packets for each segment
            foreach (var segment in segments)
            {
                var startNode = segment.First();
                var endNode = segment.Last();

                listBoxTestOutput.Items.Add($"Segment from {startNode} to {endNode}");
                PaintPoint(startNode, Color.Blue);
                PaintPoint(endNode, Color.Blue);
                isMagnetOn = true;
                packet = CreatePacket(endNode);
                SendPacket(packet);
            }
        }

        private int CalculateHeuristic(Point a, Point b)
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
            yByte =  (byte)(GridHeight - currentNode.Y);
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
            // Transmit the command to the COM port
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Write(packet, 0, packet.Length);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            // Update the listbox
            listBoxPacketOutput.Items.Add($"{string.Join(", ", packet)}");
        }

        private byte GetCommandByte()
        {
            // Process command byte (0xABCDEFGH)
            byte commandByte = 0;

            // C: Start Homing Sequence (0 = Off, 1 = On)
            if (isStartHomingSequence)
            {
                commandByte |= (1 << 5);
            }

            // D: Magnet On (0 = Off, 1 = On)
            if (isMagnetOn)
            {
                commandByte |= (1 << 4);
            }

            // F: X Escape Bit (0 = Do Nothing, 1 = Set X Low Byte to 255)
            if (xByte == Byte.MaxValue)
            {
                commandByte |= (1 << 2);
                xByte = Byte.MaxValue - 1;
            }

            // H: Y Escape Bit (0 = Do Nothing, 1 = Set Y Low Byte to 255)
            if (yByte == Byte.MaxValue)
            {
                commandByte |= (1 << 0);
                yByte = Byte.MaxValue - 1;
            }

            // Reset the boolean flags
            isStartHomingSequence = false;
            isMagnetOn = false;

            return commandByte;
        }

        // ============================
        // UI Rendering
        // ============================

        private void gridPanel_Paint(object sender, PaintEventArgs e)
        {
            for (int row = 0; row < maze.GetLength(0); row++)
            {
                for (int col = 0; col < maze.GetLength(1); col++)
                {
                    Color pointColor = GetColor(new Point(col, row));
                    PaintPoint(new Point(col, row), pointColor);
                }
            }
        }
        private void PaintPoint(Point point, Color color)
        {
            using (Graphics g = gridPanel.CreateGraphics())
            {
                g.FillRectangle(new SolidBrush(color), point.X * PointSize, point.Y * PointSize, PointSize, PointSize);
                g.DrawRectangle(Pens.Black, point.X * PointSize, point.Y * PointSize, PointSize, PointSize); // Redraw the border
            }
        }

        private Color GetColor(Point point)
        {
            int value = maze[point.Y, point.X];
            if (point.Equals(start))
            {
                return Color.Green;
            }
            else if (point.Equals(end))
            {
                return Color.Red;
            }
            else if (value == 1)
            {
                return Color.Black;
            }
            else if (value == 0)
            {
                return Color.White;
            }
            else
            {
                return Color.Orange;
            }
        }

        // ============================
        // Buttons
        // ============================

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen)
            {
                serialPort1.Open();
                btnConnect.Text = "Disconnect Serial";
            }
            else
            {
                serialPort1.Close();
                btnConnect.Text = "Connect Serial";
            }
            //PaintPoint(new Point(3, 7), Color.Red);
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            FindShortestPath();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            isStartHomingSequence = true;
            var packet = CreatePacket(new Point(0,0));
            SendPacket(packet);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            var packet = CreatePacket(start);
            SendPacket(packet);
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
                captureTimer.Interval = 30; // Adjust as necessary
                captureTimer.Tick += (s, args) =>
                {
                    capture.Read(frame);
                    if (!frame.Empty())
                    {
                        image = BitmapConverter.ToBitmap(frame);
                        pictureBox1.Image = image;
                    }
                };
                captureTimer.Start();
            }
        }

        private void btnCaptureImage_Click(object sender, EventArgs e)
        {
            if (isCameraRunning && image != null)
            {
                captureTimer?.Stop();
                //capture?.Dispose();
                //frame?.Dispose();
                pictureBox1.Image = null;

                // Stop the live camera feed
                isCameraRunning = false;
                //capture.Release();
                //capture.Dispose();

                // Save the captured image
                string filePath = $"captured_image.png";
                image.Save(filePath);

                // Display the captured image in the PictureBox
                pictureBox1.Image = image;

                MessageBox.Show($"Image saved and displayed. File path: {filePath}");
            }
            else
            {
                MessageBox.Show("No live feed to capture an image from.");
            }
        }

        //private void btnProcessImage_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        // Get the base directory of the application
        //        string baseDir = AppDomain.CurrentDomain.BaseDirectory;

        //        // Calculate the relative path to the Python script
        //        string relativePath = @"..\..\..\Image Processing\image_processing.py";
        //        string pythonScriptPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(baseDir, relativePath));

        //        // Check if the Python script exists
        //        if (!System.IO.File.Exists(pythonScriptPath))
        //        {
        //            MessageBox.Show($"Python script not found at: {pythonScriptPath}");
        //            return;
        //        }

        //        // Configure the process to run Python
        //        var psi = new ProcessStartInfo
        //        {
        //            FileName = "python", // Ensure 'python' is in PATH
        //            Arguments = $"\"{pythonScriptPath}\"", // Path to Python script
        //            UseShellExecute = false,   // Don't use shell execution
        //            CreateNoWindow = true,     // Run without creating a command prompt window
        //        };

        //        // Start the Python process
        //        Process.Start(psi);

        //        MessageBox.Show("Python script executed!");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Error: {ex.Message}");
        //    }
        //}
        private void btnProcessImage_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the base directory of the application
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                // Calculate the relative path to the Python script
                string relativePath = @"..\..\..\Image Processing\image_processing.py";
                string pythonScriptPath = Path.GetFullPath(Path.Combine(baseDir, relativePath));

                // Check if the Python script exists
                if (!File.Exists(pythonScriptPath))
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
                    RedirectStandardOutput = true, // Capture output
                    RedirectStandardError = true
                };

                using (var process = Process.Start(psi))
                {
                    if (process != null)
                    {
                        process.WaitForExit();
                        string errors = process.StandardError.ReadToEnd();
                        if (!string.IsNullOrEmpty(errors))
                        {
                            MessageBox.Show($"Python script error: {errors}");
                            return;
                        }
                    }
                }

                //Thread.Sleep(1000);

                // Reload maze data
                InitializeMaze();

                // Redraw the grid
                gridPanel.Invalidate();

                MessageBox.Show("Python script executed and maze updated!");
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
                try
                {
                    capture.Release();
                    capture.Dispose();
                }
                catch
                {
                    
                }
            }
            if (frame != null)
            {
                frame.Dispose();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
