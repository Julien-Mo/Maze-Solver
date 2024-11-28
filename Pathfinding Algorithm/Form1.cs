using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Priority_Queue;

namespace Pathfinding_Algorithm
{
    public partial class Form1 : Form
    {
        // Constants
        private const int GridHeight = 16 * 3;
        private const int GridWidth = 22 * 3;
        private const int PointSize = 6;
        private const int StraightCost = 10;
        private const int DiagonalCost = 14;

        // Grid Variables
        private Point start = new Point(0, 0);
        private Point end = new Point(0, 0);
        private int[,] maze = new int[GridHeight, GridWidth];

        // Packet Variables
        private byte startByte = Byte.MaxValue;
        private byte xHighByte = 0;
        private byte xLowByte = 0;
        private byte yHighByte = 0;
        private byte yLowByte = 0;
        private byte commandByte = 0;

        // Flags
        private bool isStartHomingSequence = false;
        private bool isMagnetOn = false;

        public Form1()
        {
            InitializeComponent();
            InitializeMaze();
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
            start = new Point(mazeData.Start[0], mazeData.Start[1]);
            end = new Point(mazeData.End[0], mazeData.End[1]);
        }

        // ============================
        // A* Pathfinding Logic
        // ============================

        private void buttonSolve_Click(object sender, EventArgs e)
        {
            FindShortestPath();
        }

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
            // Stack to hold the path from start to end
            Stack<Point> path = new Stack<Point>();

            // Traverse the path from end to start and push points onto the stack
            while (currentNode != start)
            {
                path.Push(currentNode);
                currentNode = parents[currentNode.Y, currentNode.X].Value;
            }
            path.Push(start);  // Add the start point

            // Now, send packets from start to end
            foreach (var node in path)
            {
                listBoxTestOutput.Items.Add(node.ToString());
                PaintPoint(node, Color.Blue);
                isMagnetOn = true;
                var packet = CreatePacket(node);
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
            return new byte[]
            {
                startByte,
                (byte)((currentNode.X * UInt16.MaxValue / GridWidth) >> 8),
                (byte)((currentNode.X * UInt16.MaxValue / GridWidth) & 0xFF),
                (byte)((currentNode.Y * UInt16.MaxValue / GridHeight) >> 8),
                (byte)((currentNode.Y * UInt16.MaxValue / GridHeight) & 0xFF),
                GetCommandByte()
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

            // A: Nothing

            // B: Nothing

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

            // E: X High Byte Escape (0 = Do Nothing, 1 = Set X High Byte to 255)
            if (xHighByte == Byte.MaxValue)
            {
                commandByte |= (1 << 3);
                xHighByte = Byte.MaxValue - 1;
            }

            // F: X Low Byte Escape (0 = Do Nothing, 1 = Set X Low Byte to 255)
            if (xLowByte == Byte.MaxValue)
            {
                commandByte |= (1 << 2);
                xLowByte = Byte.MaxValue - 1;
            }

            // G: Y High Byte Escape (0 = Do Nothing, 1 = Set Y High Byte to 255)
            if (yHighByte == Byte.MaxValue)
            {
                commandByte |= (1 << 1);
                yHighByte = Byte.MaxValue - 1;
            }

            // F: Y Low Byte Escape (0 = Do Nothing, 1 = Set Y Low Byte to 255)
            if (yLowByte == Byte.MaxValue)
            {
                commandByte |= (1 << 0);
                yLowByte = Byte.MaxValue - 1;
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
    }
}
