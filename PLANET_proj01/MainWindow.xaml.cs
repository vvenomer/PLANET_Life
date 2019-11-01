using PLANET_proj01.Life;
using PLANET_proj01.Template;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace PLANET_proj01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double populationPercentage = 0.3;

        private int cellSize
        {
            get { return scale.Text == "" ? 1 : int.Parse(scale.Text); }
        }

        private string baseTitle = "Game of Life";
        private GameOfLife game;
        private CellFactory factory;
        public ObservableCollection<Rectangle> rectangles = new ObservableCollection<Rectangle>();
        private bool animationRunning = false;
        private bool followCamera = false;
        private (int x, int y) mainContainerPos = (0, 0);

        private (int w, int h) mainContainerSize
        {
            get
            {
                return ((int)mainContainer.Width / cellSize, (int)mainContainer.Height / cellSize);
            }
        }

        int dieWhenAliveFor
        {
            get
            {
                int res;
                if (!int.TryParse(dieAfter.Text, out res))
                    res = -1;
                return res;
            }
        }

        public MainWindow()
        {
            game = new GameOfLife();
            factory = new CellFactory(() => cellSize, () => mainContainerSize);
            InitializeComponent();
        }

        private void UpdateUI()
        {
            if (followCamera)
            {
                mainContainerPos.x = (int)(game.DisplayAxisMiddleCoordinate(a => a.x) * cellSize) - mainContainerSize.w * cellSize / 2;
                mainContainerPos.y = (int)(game.DisplayAxisMiddleCoordinate(a => a.y) * cellSize) - mainContainerSize.h * cellSize / 2;
            }
            Title = baseTitle + " " + mainContainerPos.ToString();
            mainContainer.Children.Clear();

            game.allCells.ForEach(c =>
            {
                if (factory.IsWithinBounds(c))
                {
                    var rect = factory.GetRectangle(c, mainContainerPos);
                    mainContainer.Children.Add(rect);

                    if (c.died)
                        (Resources["fadeOut"] as Storyboard).Begin(rect as FrameworkElement);
                }
            });
        }

        public void Display()
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
                var parameters = animateControls.Content as AnimateParamsTemplate;
                animationRunning = true;
                int count = parameters.times;
                int sleep = parameters.speed;
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
            game = new GameOfLife();
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
            if (!game.allCells.Any(c => c.pos == (x, y)))
            {
                game.allCells.Add(new Cell() { pos = (x, y) });
                UpdateUI();
            }
        }

        private void GeneretePreset1(object sender, RoutedEventArgs e)
        {
            var startPositions = new List<(int x, int y)>()
            {
                (5,5),
                (5,6),
                (5,4),
                (4,5),
                (6,5)
            };
            game.AddCells(startPositions);
            UpdateUI();
        }

        private void GenerateRandomCells(object sender, RoutedEventArgs e)
        {
            var startPositions = new List<(int x, int y)>();
            var rand = new Random();
            var numberOfAliveCells = (int)((mainContainerSize.w * mainContainerSize.h) * populationPercentage * rand.NextDouble());
            for (int i = 0; i < numberOfAliveCells; i++)
            {
                (int x, int y) pos;
                do
                {
                    pos = (x: rand.Next(mainContainerSize.w), y: rand.Next(mainContainerSize.h));
                }
                while (startPositions.Contains(pos));
                startPositions.Add(pos);
            }
            game.AddCells(startPositions);
            UpdateUI();
        }

        private void UpdateUI(object sender, TextChangedEventArgs e)
        {
            UpdateUI();
        }
        private void TurnHighlightOnOff(object sender, RoutedEventArgs e)
        {
            factory.ColorEndangered = !factory.ColorEndangered;
            UpdateUI();
        }
        private void TurnColorOnOff(object sender, RoutedEventArgs e)
        {
            factory.ColorBorn = !factory.ColorBorn;
            UpdateUI();
        }

        private void UpdateCondt(object sender, RoutedEventArgs e)
        {
            game.BornCondt = bornCondt.Text.Split(",").Select(s => int.Parse(s.Trim())).ToList();
            game.DeadCondt = deadCondt.Text.Split(",").Select(s => int.Parse(s.Trim())).ToList();
        }
        private void SetCellRect(object sender, RoutedEventArgs e)
        {
            factory.Type = Type.Rect;
            UpdateUI();
        }
        private void SetCellImg1(object sender, RoutedEventArgs e)
        {
            factory.Type = Type.Img1;
            UpdateUI();
        }
        private void SetCellImg2(object sender, RoutedEventArgs e)
        {
            factory.Type = Type.Img2;
            UpdateUI();
        }

        Window2 window2;

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            window2 = new Window2();
            window2.Closed += (sender, args) => window2 = null;
            window2.Show();
        }

        private void NewCommand_CanExec(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = window2 == null;
        }

        private void dieAfter_TextChanged(object sender, TextChangedEventArgs e)
        {
            game.dieWhenAliveFor = dieWhenAliveFor;
            game.SetEndangeredStates();
            UpdateUI();
        }
    }
}