using PLANET_proj01.Life;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace PLANET_proj01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double populationPercentage = 0.3;
        int cellSize = 25;
        string baseTitle = "Game of Life";
        GameOfLife game;
        CellFactory factory;
        public ObservableCollection<Rectangle> rectangles = new ObservableCollection<Rectangle>();
        bool animationRunning = false;
        bool followCamera = false;
        (int x, int y) mainContainerPos = (0, 0);
        (int w, int h) mainContainerSize;
        List<(int x, int y)> startPositions;
        public MainWindow()
        {
            InitializeComponent();
            mainContainerSize.w = (int)mainContainer.Width / cellSize;
            mainContainerSize.h = (int)mainContainer.Height / cellSize;
            var rand = new Random();
            var numberOfAliveCells = (int)((mainContainerSize.w * mainContainerSize.h) * populationPercentage * rand.NextDouble());
            startPositions = new List<(int x, int y)>()
            {
                (5,5),
                (5,6),
                (5,4),
                (4,5),
                (6,5)
            };
            /*for (int i = 0; i < numberOfAliveCells; i++)
            {
                (int x, int y) pos;
                do
                {
                    pos = (x: rand.Next(mainContainerSize.w), y: rand.Next(mainContainerSize.h));
                }
                while (startPositions.Contains(pos));
                startPositions.Add(pos);
            }*/
            game = new GameOfLife(startPositions);
            factory = new CellFactory(cellSize);
            UpdateUI();
        }

        void UpdateUI()
        {
            if (followCamera)
            {
                mainContainerPos.x = (int)(game.DisplayAxisMiddleCoordinate(a => a.x) * cellSize) - mainContainerSize.w * cellSize / 2;
                mainContainerPos.y = (int)(game.DisplayAxisMiddleCoordinate(a => a.y) * cellSize) - mainContainerSize.h * cellSize / 2;
            }
            Title = baseTitle + " " + mainContainerPos.ToString();
            mainContainer.Children.Clear();
            game.allCells.ForEach(c => mainContainer.Children.Add(factory.GetRectangle(c, mainContainerPos)));
        }

        void Display()
        {
            game.DoTick();
            UpdateUI();
        }

        private void Tick(object sender, RoutedEventArgs e)
        {
            Display();
        }

        private void MakeTextBoxNumberOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out int _);
        }

        private async void Animate(object sender, RoutedEventArgs e)
        {
            if (!animationRunning)
            {
                animationRunning = true;
                int count = int.Parse(times.Text);
                int sleep = int.Parse(speed.Text);
                await Task.Run(() =>
                {
                    for (int i = 0; count <= 0 || i < count; i++)
                    {
                        Application.Current.Dispatcher.Invoke(Display);
                        Thread.Sleep(sleep);
                        if (!animationRunning)
                            break;
                    }
                    animationRunning = false;
                });
            }
            else
                animationRunning = false;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            followCamera = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            followCamera = false;
            mainContainerPos = (0, 0);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            game = new GameOfLife(startPositions);
            UpdateUI();
        }

        private void SaveState(object sender, RoutedEventArgs e)
        {
            game.SaveState();
        }

        private void LoadState(object sender, RoutedEventArgs e)
        {
            game.LoadState();
            UpdateUI();
        }

        private void SpawnCell(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(mainContainer);
            int x = (int)(pos.X / cellSize);
            int y = (int)(pos.Y / cellSize);
            if(!game.allCells.Any(c => c.pos == (x, y)))
                game.allCells.Add(new Cell() { pos = (x, y) });
            UpdateUI();
        }
    }
}
