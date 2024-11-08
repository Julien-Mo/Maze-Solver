using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pathfinding_Algorithm
{
    public partial class Form1 : Form
    {
        // Constants
        private const int CellSize = 10;
        private const int GridSize = 50;

        // Variables
        private Tuple<int, int> start = new Tuple<int, int>(0, 0);
        private Tuple<int, int> end = new Tuple<int, int>(GridSize-1, GridSize-1);
        private int[,] maze = new int[GridSize, GridSize];
        /* 
        private int[,] maze = new int[5, 5]  { { 0, 1, 1, 1, 0 },
                                               { 0, 1, 0, 1, 0 },
                                               { 0, 1, 0, 1, 0 },
                                               { 0, 0, 0, 1, 0 },
                                               { 0, 0, 0, 0, 0 } };
        */

        public Form1()
        {
            InitializeComponent();
            InitializeRandomMaze();
        }

        // Creates a random maze (not guaranteed to be solvable)
        private void InitializeRandomMaze()
        {
            Random rand = new Random();

            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    maze[i, j] = rand.Next(4) < 3 ? 0 : 1;
                }
            }

            // Clear the start and end points
            maze[start.Item1, start.Item2] = 0;
            maze[end.Item1, end.Item2] = 0;
        }

        // A* algorithm
        private void findShortestPath()
        {
            var openNodes = new PriorityQueue<Tuple<int, int>>();
            var closedNodes = new List<Tuple<int, int>>();
            var parents = new Tuple<int, int>[GridSize, GridSize];
            var fCosts = new int[GridSize, GridSize];

            // add start node to openNodes
            openNodes.Enqueue(start, 0);

            // loop until openNodes is empty
            while (openNodes.Count > 0)
            {
                // get node with lowest fCost
                var current = openNodes.Dequeue();

                // add current to closedNodes
                closedNodes.Add(current);
                PaintCell(current.Item1, current.Item2, Color.Gray);

                // check if path has been found
                if (current.Equals(end))
                {
                    // reconstruct path
                    while (current != start)
                    {
                        textBoxTestOutput.AppendText(current.ToString() + "\n");
                        current = parents[current.Item1, current.Item2];
                        PaintCell(current.Item1, current.Item2, Color.Blue);
                    }
                    break;
                }

                // for each neighbor of the current node
                foreach (Tuple<int, int> neighbor in GetNeighbors(current))
                {
                    int row = neighbor.Item1;
                    int col = neighbor.Item2;

                    // if neighbor is not traversable or neighbor is in closedNodes
                    if (row < 0 || row >= maze.GetLength(0) ||
                        col < 0 || col >= maze.GetLength(1) ||
                        maze[row, col] == 1 || closedNodes.Contains(neighbor))
                    {
                        continue;
                    }

                    // calculate gCost, hCost, fCost
                    int gCost = (int)Math.Sqrt(Math.Pow(row - start.Item1, 2) + Math.Pow(col - start.Item2, 2)) * 10;
                    int hCost = (int)Math.Sqrt(Math.Pow(row - end.Item1, 2) + Math.Pow(col - end.Item2, 2)) * 10;
                    int fCost = gCost + hCost;

                    // if new path to neighbor is shorter or neighbor is not in openNodes
                    if (fCost < fCosts[row, col] || !openNodes.Contains(neighbor))
                    {
                        // set fCost of neighbor
                        fCosts[row, col] = fCost;

                        // set parent of neighbor to current node
                        parents[row, col] = current;

                        // add neighbor to openNodes
                        if (!openNodes.Contains(neighbor))
                        {
                            openNodes.Enqueue(neighbor, fCost);
                        }
                    }
                }
            }

        }

        // Get the neighbors of the current cell
        private List<Tuple<int, int>> GetNeighbors(Tuple<int, int> current)
        {
            int row = current.Item1;
            int col = current.Item2;

            // Return a list of neighboring cells (including diagonals)
            return new List<Tuple<int, int>>
            {
                Tuple.Create(row - 1, col + 1), // Up-Right
                Tuple.Create(row - 1, col),     // Up
                Tuple.Create(row - 1, col - 1), // Up-Left
                Tuple.Create(row, col - 1),     // Left
                Tuple.Create(row + 1, col - 1), // Down-Left
                Tuple.Create(row + 1, col),     // Down
                Tuple.Create(row + 1, col + 1), // Down-Right
                Tuple.Create(row, col + 1),     // Right
            };
        }

        private void buttonSolve_Click(object sender, EventArgs e)
        {
            findShortestPath();
        }
                
        private void PaintCell(int row, int col, Color color)
        {
            int x = col * CellSize;
            int y = row * CellSize;

            using (Graphics g = gridPanel.CreateGraphics())
            {
                g.FillRectangle(new SolidBrush(color), x, y, CellSize, CellSize);
                g.DrawRectangle(Pens.Black, x, y, CellSize, CellSize); // Redraw the border
            }
        }

        private void gridPanel_Paint(object sender, PaintEventArgs e)
        {
            for (int row = 0; row < maze.GetLength(0); row++)
            {
                for (int col = 0; col < maze.GetLength(1); col++)
                {
                    Color cellColor = GetColor(Tuple.Create(row, col));
                    PaintCell(row, col, cellColor);
                }
            }
        }

        // Method to map array values to colors
        private Color GetColor(Tuple<int, int> cell)
        {
            int value = maze[cell.Item1, cell.Item2];
            if (cell.Equals(start))
            {
                return Color.Green;
            }
            else if (cell.Equals(end))
            {
                return Color.Red;
            }
            else if (value == 1)
            {
                return Color.Black;
            }
            else
            {
                return Color.White;
            }
        }

    }

    // Priority Queue class
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
