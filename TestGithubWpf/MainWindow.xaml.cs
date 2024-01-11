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

namespace TestGithubWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        bool goLeft, goRight, goDown, goUp;
        int speed = 5;
        Rect pacmanHitBox;
        int ghostSpeed = 10;
        int ghostMoveStep = 160;
        int currentGhostStep;
        int score = 0;
        bool gameover = false;
        bool isGamePaused = false;
        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine("hello world");
            Console.WriteLine("hello");
            
        }
        public MainWindow()
        {
            InitializeComponent();
            GameSetUp();
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
                }
            }
            /*************************    REPLAY   *************************/
            if (e.Key == Key.R)
            {
                if (isGamePaused)
                {
                    gameTimer.Start();
                    isGamePaused = false;
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
        private void StartGame()
        {
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "sound/gogo2.wav");
            mediaElement.Source = uri;
            mediaElement.Play();

            currentGhostStep = ghostMoveStep;

            Canvas.SetLeft(pacman, 50);
            Canvas.SetTop(pacman, 104);

            Canvas.SetLeft(pinkGuy, 173);
            Canvas.SetTop(pinkGuy, 404);

            Canvas.SetLeft(redGuy, 173);
            Canvas.SetTop(redGuy, 29);

            Canvas.SetLeft(orangeGuy, 651);
            Canvas.SetTop(orangeGuy, 104);

            gameTimer.Start();
            score = 0;

            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                pacmanHitBox = new Rect(Canvas.GetLeft(pacman), Canvas.GetTop(pacman), pacman.Width, pacman.Height);

                /*************************    TO MAKE THE COINS VISIBLE AGAIN *************************/
                if ((string)x.Tag == "coin")
                {
                    if (x.Visibility == Visibility.Hidden)
                    {
                        x.Visibility = Visibility.Visible;
                    }
                }
                if ((string)x.Tag == "ghost")
                {
                    if (pacmanHitBox.IntersectsWith(hitBox))
                    {
                        gameTimer.Stop();
                        gameover = true;
                    }
                    if (x.Name.ToString() == "orangeGuy")
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
        private void GameSetUp()
        {
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "sound/gogo2.wav");
            mediaElement.Source = uri;
            mediaElement.Play();

            MyCanvas.Focus();
            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();
            currentGhostStep = ghostMoveStep;

            //pacmanHitBox = new Rect(Canvas.GetLeft(pacman), Canvas.GetTop(pacman), pacman.Width, pacman.Height);

            ImageBrush pacmanImage = new ImageBrush();
            pacmanImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/pacman.jpg"));
            pacman.Fill = pacmanImage;
            ImageBrush redGhost = new ImageBrush();
            redGhost.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/red.jpg"));
            redGuy.Fill = redGhost;
            ImageBrush orangeGhost = new ImageBrush();
            orangeGhost.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/orange.jpg"));
            orangeGuy.Fill = orangeGhost;
            ImageBrush pinkGhost = new ImageBrush();
            pinkGhost.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/pink.jpg"));
            pinkGuy.Fill = pinkGhost;
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

        private void MoveGhost()
        {
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                if ((string)x.Tag == "coin")
                {
                    if (pacmanHitBox.IntersectsWith(hitBox) && x.Visibility == Visibility.Visible)
                    {
                        x.Visibility = Visibility.Hidden;
                        score++;
                    }
                }

                if ((string)x.Tag == "ghost")
                {
                    if (pacmanHitBox.IntersectsWith(hitBox))
                    {
                        gameTimer.Stop();
                        gameover = true;
                    }
                    if (x.Name.ToString() == "orangeGuy")
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
        private void GameLoop(object sender, EventArgs e)
        {
            txtScore.Content = "Score: " + score + "\n Press P to Pause and R to Resume";

            movePacman();
            MoveGhost();

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
}
