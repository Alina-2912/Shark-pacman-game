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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TestGithubWpf
{
    /// <summary>
    /// Logique d'interaction pour Normale.xaml
    /// </summary>
    public partial class NiveauNormale : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        bool goLeft, goRight, goDown, goUp;
        int speed = 7;
        Rect pacmanHitBox;
        int ghostSpeed = 10;
        int ghostMoveStep = 160;
        int currentGhostStep;
        int score = 0;
        bool gameover = false;
        bool isGamePaused = false;
        ImageBrush starImage = new ImageBrush();
        List<Rectangle> itemRemover = new List<Rectangle>();

        public NiveauNormale()
        {
            InitializeComponent();
            GameSetUp();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Hide();
            mediaElement.Close();
        }
        private void GameSetUp()
        {
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "sound/bts_idol1.wav");
            mediaElement.Source = uri;
            mediaElement.Play();
            MyCanvas.Focus();
            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();
            currentGhostStep = ghostMoveStep;

            //pacmanHitBox = new Rect(Canvas.GetLeft(pacman), Canvas.GetTop(pacman), pacman.Width, pacman.Height);
        }
        private void movePacman()
        {
            if (goRight && Canvas.GetLeft(pacman) < Application.Current.MainWindow.Width - 60)
            {
                Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + speed);
            }
            if (goLeft && Canvas.GetLeft(pacman) > 20)
            {
                Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - speed);
            }
            if (goUp && Canvas.GetTop(pacman) > 20)
            {
                Canvas.SetTop(pacman, Canvas.GetTop(pacman) - speed);
            }
            if (goDown && Canvas.GetTop(pacman) < Application.Current.MainWindow.Height - 60)
            {
                Canvas.SetTop(pacman, Canvas.GetTop(pacman) + speed);
            }

            pacmanHitBox = new Rect(Canvas.GetLeft(pacman), Canvas.GetTop(pacman), pacman.Width, pacman.Height);
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                if ((string)x.Tag == "star")
                {
                    if (pacmanHitBox.IntersectsWith(hitBox) && x.Visibility == Visibility.Visible)
                    {
                        x.Visibility = Visibility.Hidden;
                    }
                }

                if ((string)x.Tag == "wall")
                {
                    if (pacmanHitBox.IntersectsWith(hitBox))
                    {
                        if (goRight)
                        {
                            Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - speed);
                            goRight = false;
                        }

                        if (goLeft)
                        {
                            Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + speed);
                            goLeft = false;
                        }

                        if (goUp)
                        {
                            Canvas.SetTop(pacman, Canvas.GetTop(pacman) + speed);
                            goUp = false;
                        }

                        if (goDown)
                        {
                            Canvas.SetTop(pacman, Canvas.GetTop(pacman) - speed);
                            goDown = false;
                        }
                    }
                }
            }
        }
        private void StartGame()
        {
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "sound/gogo2.wav");
            mediaElement.Source = uri;
            mediaElement.Play();

            currentGhostStep = ghostMoveStep;

            Canvas.SetLeft(pacman, 50);
            Canvas.SetTop(pacman, 104);

            Canvas.SetLeft(rosePieuvre, 173);
            Canvas.SetTop(rosePieuvre, 404);

            Canvas.SetLeft(violetPieuvre, 173);
            Canvas.SetTop(violetPieuvre, 29);

            Canvas.SetLeft(orangePieuvre, 651);
            Canvas.SetTop(orangePieuvre, 104);

            gameTimer.Start();
            score = 0;

            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                pacmanHitBox = new Rect(Canvas.GetLeft(pacman), Canvas.GetTop(pacman), pacman.Width, pacman.Height);

                if ((string)x.Tag == "poisson")
                {
                    if (x.Visibility == Visibility.Hidden)
                    {
                        x.Visibility = Visibility.Visible;
                    }
                }
                if ((string)x.Tag == "pieuvre")
                {
                    if (pacmanHitBox.IntersectsWith(hitBox))
                    {
                        gameTimer.Stop();
                        gameover = true;
                    }
                    if (x.Name.ToString() == "orangePieuvre")
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - ghostSpeed);
                    }
                    else
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + ghostSpeed);
                    }
                    currentGhostStep--;
                    if (currentGhostStep < 1)
                    {
                        currentGhostStep = ghostMoveStep;
                        ghostSpeed = -ghostSpeed;
                    }
                }

            }
        }
        private void CanvasKeyDown(object sender, KeyEventArgs e)
        {
            /*************************    PAUSE   *************************/
            if (e.Key == Key.P)
            {
                if (!isGamePaused)
                {
                    gameTimer.Stop();
                    isGamePaused = true;
                    //mediaElement.Pause();
                }
            }
            /*************************    RESUME   *************************/
            if (e.Key == Key.R)
            {
                if (isGamePaused)
                {
                    gameTimer.Start();
                    isGamePaused = false;
                    //mediaElement.Play();
                }
            }
            /*************************    RESTART - R   *************************/
            if (e.Key == Key.R && gameover)
            {
                StartGame();
            }

            if (e.Key == Key.Left)
            {
                goLeft = true;
                //pacman.RenderTransform = new RotateTransform(-180, pacman.Width / 2, pacman.Height / 2);
                pacman.RenderTransformOrigin = new Point(0.5, 0.2);
                ScaleTransform flipTrans = new ScaleTransform();
                flipTrans.ScaleX = -1;
                pacman.RenderTransform = flipTrans;
                goRight = false;
                goUp = false;
                goDown = false;
            }
            if (e.Key == Key.Right)
            {
                goRight = true;
                pacman.RenderTransform = new RotateTransform(0, pacman.Width / 2, pacman.Height / 2);
                goLeft = false;
                goUp = false;
                goDown = false;

            }
            if (e.Key == Key.Up)
            {
                if (!goUp && !goDown)
                {
                    Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - 7);
                }
                goUp = true;
                pacman.RenderTransform = new RotateTransform(-90, pacman.Width / 2, pacman.Height / 2);
                goRight = false;
                goLeft = false;
                goDown = false;


            }
            if (e.Key == Key.Down)
            {
                if (!goUp && !goDown)
                {
                    Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - 7);
                }
                goDown = true;
                pacman.RenderTransform = new RotateTransform(90, pacman.Width / 2, pacman.Height / 2);
                goRight = false;
                goLeft = false;
                goUp = false;

            }
        }

        private void MoveGhost()
        {
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                if ((string)x.Tag == "poisson")
                {
                    if (pacmanHitBox.IntersectsWith(hitBox) && x.Visibility == Visibility.Visible)
                    {
                        x.Visibility = Visibility.Hidden;
                        score++;
                    }
                }

                if ((string)x.Tag == "pieuvre")
                {
                    if (pacmanHitBox.IntersectsWith(hitBox))
                    {
                        gameTimer.Stop();
                        gameover = true;
                    }
                    if (x.Name.ToString() == "orangePieuvre")
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - ghostSpeed);
                    }
                    if (x.Name.ToString() == "violetPieuvre")
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + ghostSpeed);
                    }
                    if (x.Name.ToString() == "rosePieuvre")
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + ghostSpeed);
                    }
                    if (x.Name.ToString() == "greenGuy")
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + ghostSpeed);
                    }
                    currentGhostStep--;
                    if (currentGhostStep < 1)
                    {
                        currentGhostStep = ghostMoveStep;
                        ghostSpeed = -ghostSpeed;
                    }
                }

            }
        }
        private void GameLoop(object sender, EventArgs e)
        {
            starImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/images/treasurebox.jpg"));
            txtScore.Content = "Score: " + score + "\nPress P to Pause and R to Resume";

            movePacman();
            MoveGhost();

            if (score == 16)
            {
                for (int i = 0; i < 5; i++)
                {
                    Rectangle rec = new Rectangle()
                    {
                        Width = 30,
                        Height = 30,
                        Fill = starImage,
                        //Stroke = Brushes.Red,
                        //Visibility = Visibility.Hidden,
                        StrokeThickness = 2,
                        Tag = "star",
                    };
                    MyCanvas.Children.Add(rec);
                    Canvas.SetTop(rec, 529);
                    Canvas.SetLeft(rec, 220);

                }
            }
            if (score == 47)
            {
                for (int i = 0; i < 5; i++)
                {
                    Rectangle rec = new Rectangle()
                    {
                        Width = 30,
                        Height = 30,
                        Fill = starImage,
                        //Stroke = Brushes.Red,
                        //Visibility = Visibility.Hidden,
                        StrokeThickness = 2,
                        Tag = "star",
                    };
                    MyCanvas.Children.Add(rec);
                    Canvas.SetTop(rec, 199);
                    Canvas.SetLeft(rec, 489);
                }
            }
            if (score == 68)
            {
                for (int i = 0; i < 5; i++)
                {
                    Rectangle rec = new Rectangle()
                    {
                        Width = 30,
                        Height = 30,
                        Fill = starImage,
                        //Stroke = Brushes.Red,
                        Visibility = Visibility.Hidden,
                        StrokeThickness = 2,
                        Tag = "star",
                    };
                    MyCanvas.Children.Add(rec);
                    Canvas.SetTop(rec, 50);
                    Canvas.SetLeft(rec, 650);
                }
            }
            if (score == 85)
            {
                GameOver("You Win, you collected all of the coins");
            }
            if (gameover)
            {
                txtScore.Content += "   Press R to Retry";
            }
        }
        private void GameOver(string message)
        {
            gameTimer.Stop();
            MessageBox.Show(message, "The Pac Man Game WPF MOO ICT");

            //System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            //Application.Current.Shutdown();
        }
    }
}
