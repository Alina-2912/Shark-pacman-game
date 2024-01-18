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
    /// Logique d'interaction pour Difficile.xaml
    /// </summary>
    public partial class Difficile : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        bool goLeft, goRight, goDown, goUp;
        bool noLeft, noRight, noDown, noUp;
        int speed = 8;
        Rect pacmanHitBox;
        int ghostSpeed = 10;
        int ghostMoveStep = 160;
        int currentGhostStep;
        int score = 0;
        bool gameover = false;
        bool isGamePaused = false;
        ImageBrush pacmanImage = new ImageBrush();
        int imageRequin = 1;

        public Difficile()
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
            this.Close();
            mediaElement.Close();
        }
        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            Normale mw = new Normale();
            mw.Show();
           
            this.Close();
            mediaElement.Close();
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
                    mediaElement.Pause();
                }
            }
            /*************************    RESUME   *************************/
            if (e.Key == Key.R)
            {
                if (isGamePaused)
                {
                    gameTimer.Start();
                    isGamePaused = false;
                    mediaElement.Play();
                }
            }
            /*************************    RESTART - R   *************************/
            if (e.Key == Key.R && gameover)
            {
                Difficile mw = new Difficile();
                mw.Show();
                this.Close();
            }

            if (e.Key == Key.Left && noLeft == false)
            {
                if (Canvas.GetLeft(pacman) >= 10)
                {
                    goRight = goUp = goDown = false;
                    noRight = noUp = noDown = false;
                    goLeft = true;
                    pacman.RenderTransform = new RotateTransform(-180, pacman.Width / 2, pacman.Height / 2);
                }
                else { Canvas.SetLeft(pacman, 10); }
            }
            if (e.Key == Key.Right && noRight == false)
            {
                if (Canvas.GetLeft(pacman) <= 790) // si possible remplacer 790 par la largeur de la fenètre
                {
                    noLeft = noUp = noDown = false;
                    goLeft = goUp = goDown = false;
                    goRight = true;
                    pacman.RenderTransform = new RotateTransform(0, pacman.Width / 2, pacman.Height / 2);
                    pacmanImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/images/requinvuhaut05.png"));
                    pacman.Fill = pacmanImage;
                }
                else { Canvas.SetLeft(pacman, 790); }
            }
            if (e.Key == Key.Up && noUp == false)
            {
                if (Canvas.GetTop(pacman) >= 10)
                {

                    noRight = noDown = noLeft = false;
                    goRight = goDown = goLeft = false;
                    goUp = true;
                    pacman.RenderTransform = new RotateTransform(-90, pacman.Width / 2, pacman.Height / 2);
                }
                else { Canvas.SetTop(pacman, 10); }
            }
            if (e.Key == Key.Down && noDown == false)
            {
                if (Canvas.GetTop(pacman) <= 590)
                {
                    noUp = noLeft = noRight = false;
                    goUp = goLeft = goRight = false;
                    goDown = true;
                    pacman.RenderTransform = new RotateTransform(90, pacman.Width / 2, pacman.Height / 2);
                }
                else { Canvas.SetTop(pacman, 590); }
            }
        }
        private void Canvas_KeyUp(object sender, KeyEventArgs e)
        {
            // if the space key is pressed AND jumping boolean is true AND player y location is above 260 pixels
            if (e.Key == Key.Left && !goLeft && Canvas.GetTop(pacman) > 260)
            {
                // set jumping to true
                goLeft = true;
                goRight = false;
                goUp = false;
                goDown = false;
                // set speed integer to -12
                speed = -12;
                pacmanImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/coral4.  jpg"));
            }
        }
        /*private void StartGame()
        {
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "sound/gogo2.wav");
            mediaElement.Source = uri;
            mediaElement.Play();

            currentGhostStep = ghostMoveStep;

            Canvas.SetLeft(pacman, 50);
            Canvas.SetTop(pacman, 104);

            Canvas.SetLeft(pinkGuy, 173);
            Canvas.SetTop(rosePieuvre, 404);

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
        }*/
        private void GameSetUp()
        {
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "sound/bts_dionysus.wav");
            mediaElement.Source = uri;
            mediaElement.Play();

            MyCanvas.Focus();
            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();
            currentGhostStep = ghostMoveStep;

            
            ImageBrush redGhost = new ImageBrush();
            redGhost.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/red.jpg"));
            violetPieuvre.Fill = redGhost;
            ImageBrush orangeGhost = new ImageBrush();
            orangeGhost.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/orange.jpg"));
            orangePieuvre.Fill = orangeGhost;
            ImageBrush pinkGhost = new ImageBrush();
            pinkGhost.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/pink.jpg"));
            rosePieuvre.Fill = pinkGhost;
        }
        private void GameLoop(object sender, EventArgs e)
        {
            txtScore.Content = "Score: " + score + "\nPress P to Pause and R to Resume";

            switch (imageRequin)
            {
                case 1:
                case 2:
                case 3:
                    pacmanImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/requinvuhaut01.png"));
                    break;
                case 4:
                case 5:
                case 6:
                    pacmanImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/requinvuhaut02.png"));
                    break;
                case 7:
                case 8:
                case 9:
                    pacmanImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/requinvuhaut03.png"));
                    break;
                case 10:
                case 11:
                case 12:
                    pacmanImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/requinvuhaut04.png"));
                    break;
                case 13:
                case 14:
                case 15:
                    pacmanImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/requinvuhaut05.png"));
                    break;
                case 16:
                case 17:
                case 18:
                    pacmanImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/requinvuhaut06.png"));
                    break;
                case 19:
                case 20:
                case 21:
                    pacmanImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/requinvuhaut07.png"));
                    break;
            }
            pacman.Fill = pacmanImage;
            imageRequin++;
            if (imageRequin > 21)
            {
                imageRequin = 1;
            }

            if (goRight)
            {
                Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + speed);
            }
            if (goLeft)
            {
                Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - speed);
            }
            if (goUp)
            {
                Canvas.SetTop(pacman, Canvas.GetTop(pacman) - speed);
            }
            if (goDown)
            {
                Canvas.SetTop(pacman, Canvas.GetTop(pacman) + speed);
            }
            if (goDown && Canvas.GetTop(pacman) + 80 > Application.Current.MainWindow.Height)
            {
                noDown = true;
                goDown = false;
            }
            if (goUp && Canvas.GetTop(pacman) < 1)
            {
                noUp = true;
                goUp = false;
            }
            if (goLeft && Canvas.GetLeft(pacman) - 10 < 1)
            {
                noLeft = true;
                goLeft = false;
            }
            if (goRight && Canvas.GetLeft(pacman) + 70 > Application.Current.MainWindow.Width)
            {
                noRight = true;
                goRight = false;
            }
            pacmanHitBox = new Rect(Canvas.GetLeft(pacman), Canvas.GetTop(pacman), pacman.Width, pacman.Height);
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                if ((string)x.Tag == "wall")
                {
                    if (goLeft == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + 10);
                        noLeft = true;
                        goLeft = false;
                    }
                    if (goRight == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - 10);
                        noRight = true;
                        goRight = false;
                    }
                    if (goDown == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetTop(pacman, Canvas.GetTop(pacman) - 10);
                        noDown = true;
                        goDown = false;
                    }
                    if (goUp == true && pacmanHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetTop(pacman, Canvas.GetTop(pacman) + 10);
                        noUp = true;
                        goUp = false;
                    }
                }
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
