using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GameOfLife
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();

        List<int> changed = new List<int>();

        public MainWindow()
        {
            InitializeComponent();

            StartGrid();

            stopBtn.IsEnabled = false;
            timer.Tick += new EventHandler(Next);
            SetSpeed();
            speed.ChangeSpeed = SetSpeed;
        }

        private void StartGrid()
        {
            int columnDefinitions = 30, rowDefinitions = 30;
            ColumnsCreation(columnDefinitions);
            RowsCreation(rowDefinitions);

            grid.Children.Clear();
            for (int i = 0; i < grid.RowDefinitions.Count; i++)
                for (int j = 0; j < grid.ColumnDefinitions.Count; j++)
                {
                    Cell cell = new Cell();
                    cell.MouseDown += OnClick;
                    cell.Location = GetLocation(i, j);
                    Grid.SetRow(cell, i);
                    Grid.SetColumn(cell, j);
                    grid.Children.Add(cell);
                }
        }
        private void ColumnsCreation(int columns)
        {
            if (columns == 0)
                return;
            ColumnDefinition column = new ColumnDefinition();
            column.Width = new GridLength(1.0, GridUnitType.Star);
            grid.ColumnDefinitions.Add(column);
            ColumnsCreation(columns - 1);
        }
        private void RowsCreation(int rows)
        {
            if (rows == 0)
                return;
            RowDefinition row = new RowDefinition();
            row.Height = new GridLength(1.0, GridUnitType.Star);
            grid.RowDefinitions.Add(row);
            RowsCreation(rows - 1);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            startBtn.IsEnabled = false;
            stopBtn.IsEnabled = true;
        }
        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            startBtn.IsEnabled = true;
            stopBtn.IsEnabled = false;
        }
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            Next(sender, e);
        }
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Stop_Click(null, null);
            foreach (UIElement element in grid.Children)
            {
                Cell cell = element as Cell;
                if (cell.IsAlive)
                    cell.Kill();
            }
            changed.Clear();
        }
        private void OnClick(object sender, MouseButtonEventArgs e)
        {
            Cell cell = sender as Cell;
            int location = cell.Location;
            if (!changed.Contains(location))
                changed.Add(location);
            else changed.Remove(location);

            if (cell.IsAlive)
                cell.Kill();
            else cell.Lives();
        }
        private void SetSpeed()
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, speed.NumValue);
        }

        private void Neighbors(int location, int index, List<int> neighbors)
        {
            if (index == 2)
                return;
            neighbors.Add(location - grid.ColumnDefinitions.Count + index);
            neighbors.Add(location + grid.ColumnDefinitions.Count + index);
            if (index != 0)
                neighbors.Add(location + index);
            Neighbors(location, index + 1, neighbors);
        }

        private int LiveNeighbourCount(Cell cell)
        { 
            int count = 0;
            int location = cell.Location;

            count += LiveTopNeighbors(cell.Location, -1);
            count += LiveBottomNeighbors(cell.Location, -1);

            if (Grid.GetColumn(cell) != 0)
            {
                Cell neighbor = grid.Children[location - 1] as Cell;
                if (neighbor.IsAlive)
                    count++;
            }
            if (Grid.GetColumn(cell) != grid.ColumnDefinitions.Count - 1)
            {
                Cell neighbor = grid.Children[location + 1] as Cell;
                if (neighbor.IsAlive)
                    count++;
            }
            return count;
        }
        private int LiveTopNeighbors(int location, int index)
        {
            if (index == 2)
                return 0;
            try
            {
                Cell neighbor = grid.Children[location - grid.ColumnDefinitions.Count + index] as Cell;
                if (neighbor.IsAlive)
                    return LiveTopNeighbors(location, index + 1) + 1;
            } catch { }
            return LiveTopNeighbors(location, index + 1);
        }
        private int LiveBottomNeighbors(int location, int index)
        {
            if (index == 2)
                return 0;
            try
            {
                Cell neighbor = grid.Children[location + grid.ColumnDefinitions.Count + index] as Cell;
                if (neighbor.IsAlive)
                    return LiveBottomNeighbors(location, index + 1) + 1;
            }
            catch { }
            return LiveBottomNeighbors(location, index + 1);
        }

        private void Next(object sender, EventArgs e)
        {
            Dictionary<int, int> neighborCount = new Dictionary<int, int>();
            // key = cell location, value = number of live neighbors

            List<int> changedNeighbors = new List<int>();
            foreach (int location in changed)
            {
                List<int> neighbors = new List<int>();
                Neighbors(location, -1, neighbors);
                changedNeighbors.AddRange(neighbors);
            }
            changed.AddRange(changedNeighbors);
            changed = changed.Distinct().ToList();

            foreach (int location in changed)
            {
                try
                {
                    Cell cell = grid.Children[location] as Cell;
                    int count = LiveNeighbourCount(cell);
                    neighborCount.Add(cell.Location, count);
                } catch { }
            }

            foreach (KeyValuePair<int, int> count in neighborCount)
            {
                Cell cell = grid.Children[count.Key] as Cell;
                cell.UpdateState(count.Value);
            }
        }

        private int GetLocation(int row, int col)
        {
            if (row == 0)
                return col;
            return (row * grid.ColumnDefinitions.Count) + col;
        }
    }
}
