using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Pathfinding_Algorithm
{
    public partial class Form1 : Form
    {
        private const int PointSize = 4;
        private const int GridSize = 100;
        private Point start = new Point(0, 0);
        private Point end = new Point(0, 0);
        private int[,] maze = new int[GridSize, GridSize];

        public Form1()
        {
            InitializeComponent();
            InitializeMaze();
            //InitializeRandomMaze();
        }

        private void InitializeMaze()
        {
            string filePath = "C:/Users/julie/source/repos/Maze Solver/Image Processing/maze_data.json";
            // Read the JSON file as a string
            string json = File.ReadAllText(filePath);

            // Deserialize JSON into a dictionary
            var mazeData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            // Deserialize the "maze" property directly into a 2D array
            maze = JsonConvert.DeserializeObject<int[,]>(mazeData["maze"].ToString());

            // Convert start and end to Points
            int[] startArray = JsonConvert.DeserializeObject<int[]>(mazeData["start"].ToString());
            start = new Point(startArray[0], startArray[1]);

            int[] endArray = JsonConvert.DeserializeObject<int[]>(mazeData["end"].ToString());
            end = new Point(endArray[0], endArray[1]);
        }

        private void InitializeRandomMaze()
        {
            // Creates a flappy bird style maze with 1 opening in each barrier column
            Random rand = new Random();

            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    if (j % 2 == 0)
                    {
                        // Open column: all cells are 0
                        maze[i, j] = 0;
                    }
                    else
                    {
                        // Barrier column: set all cells to 1 initially
                        maze[i, j] = 1;
                    }
                }
            }

            // Add a single opening (0) in each barrier column
            for (int j = 1; j < maze.GetLength(1); j += 2)
            {
                maze[rand.Next(maze.GetLength(0)), j] = 0;  // First random opening
                //maze[rand.Next(maze.GetLength(0)), j] = 0;  // Second random opening
            }

            // Clear the start and end points if needed
            maze[start.Y, start.X] = 0;
            maze[end.Y, end.X] = 0;
        }

        private void buttonSolve_Click(object sender, EventArgs e)
        {
            FindShortestPath();
        }

        private void FindShortestPath()
        {
            // A* algorithm
            var openNodes = new PriorityQueue<Point>();
            var closedNodes = new HashSet<Point>();
            var parents = new Point?[GridSize, GridSize];
            var fCosts = new int[GridSize, GridSize];

            // Add start node to openNodes
            openNodes.Enqueue(start, 0);

            // Loop until openNodes is empty
            while (openNodes.Count > 0)
            {
                // Get node with lowest fCost
                var current = openNodes.Dequeue();

                // Add current to closedNodes
                closedNodes.Add(current);
                PaintPoint(current, Color.Gray);

                // Check if path has been found
                if (current.Equals(end))
                {
                    // Reconstruct path
                    while (current != start)
                    {
                        textBoxTestOutput.AppendText(current.ToString() + "\n");
                        current = parents[current.Y, current.X].Value;
                        PaintPoint(current, Color.Blue);
                    }
                    break;
                }

                // For each neighbor of the current node
                foreach (Point neighbor in GetNeighbors(current))
                {
                    int row = neighbor.Y;
                    int col = neighbor.X;

                    // If neighbor is not traversable or neighbor is in closedNodes
                    if (row < 0 || row >= maze.GetLength(0) ||
                        col < 0 || col >= maze.GetLength(1) ||
                        maze[row, col] == 1 || closedNodes.Contains(neighbor))
                    {
                        continue;
                    }

                    // Calculate distance from start, distance to end, and total distance (fCost)
                    int gCost = (int)Math.Sqrt(Math.Pow(row - start.Y, 2) + Math.Pow(col - start.X, 2)) * 10;
                    int hCost = (int)Math.Sqrt(Math.Pow(row - end.Y, 2) + Math.Pow(col - end.X, 2)) * 10;
                    int fCost = gCost + hCost;

                    // If new path to neighbor is shorter or neighbor is not in openNodes
                    if (fCost < fCosts[row, col] || !openNodes.Contains(neighbor))
                    {
                        // Set fCost of neighbor
                        fCosts[row, col] = fCost;

                        // Set parent of neighbor to current node
                        parents[row, col] = current;

                        // Add neighbor to openNodes
                        if (!openNodes.Contains(neighbor))
                        {
                            openNodes.Enqueue(neighbor, fCost);
                        }
                    }
                }
            }
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

    public class PriorityQueue<T>
    {
        private SortedDictionary<int, Queue<T>> _dict = new SortedDictionary<int, Queue<T>>();

        public void Enqueue(T item, int priority)
        {
            if (!_dict.ContainsKey(priority))
            {
                _dict[priority] = new Queue<T>();
            }
            _dict[priority].Enqueue(item);
        }

        public T Dequeue()
        {
            if (_dict.Count == 0)
            {
                throw new InvalidOperationException("The priority queue is empty.");
            }

            var firstKey = _dict.Keys.GetEnumerator();
            firstKey.MoveNext();
            var key = firstKey.Current;

            var item = _dict[key].Dequeue();
            if (_dict[key].Count == 0)
            {
                _dict.Remove(key);
            }
            return item;
        }

        public int Count
        {
            get
            {
                int count = 0;
                foreach (var queue in _dict.Values)
                {
                    count += queue.Count;
                }
                return count;
            }
        }

        public bool Contains(T item)
        {
            foreach (var queue in _dict.Values)
            {
                if (queue.Contains(item))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
