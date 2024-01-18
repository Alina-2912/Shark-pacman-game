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
    public partial class NiveauFacile : Window
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
        bool modePuissant = false;
        ImageBrush starImage = new ImageBrush();
        List<Rectangle> itemRemover = new List<Rectangle>();
        ImageBrush pacmanImage = new ImageBrush();
        ImageBrush pinkGhost = new ImageBrush();
        int imageRequin = 1;
        int imagePieuvre = 1;
        int modePuissantCompteur = 200;

        public NiveauFacile()
        {
            InitializeComponent();
            GameSetUp();
            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
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
            if (e.Key == Key.C)
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
        private void GameSetUp()
        {

            itemRemover.Clear();

            MyCanvas.Focus();
            gameTimer.Start();
            currentGhostStep = ghostMoveStep;

            ImageBrush redGhost = new ImageBrush();
            redGhost.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/red.jpg"));
            violetPieuvre.Fill = redGhost;
            ImageBrush orangeGhost = new ImageBrush();
            orangeGhost.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/orange.jpg"));
            orangePieuvre.Fill = orangeGhost;
            //ImageBrush pinkGhost = new ImageBrush();
            //pinkGhost.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/pink.jpg"));
            //rosePieuvre.Fill = pinkGhost;
            ImageBrush corail1 = new ImageBrush();
            corail1.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/fish_seaweed.png"));
            corailMillieu.Fill = corail1;
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
                        modePuissant = true;
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

        private void ButtonMusique_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "sound/gogo.wav");
            mediaElement.Source = uri;
            mediaElement.Play();
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
                    if (pacmanHitBox.IntersectsWith(hitBox) && modePuissant == false)
                    {
                        gameTimer.Stop();
                        gameover = true;
                    }
                    if (pacmanHitBox.IntersectsWith(hitBox) && modePuissant == true)
                    {
                        itemRemover.Add(x);
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
            foreach (Rectangle y in itemRemover)
            {
                MyCanvas.Children.Remove(y);
            }

            starImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/images/treasurebox.jpg"));
            txtScore.Content = "Score: " + score + "\nCliquer P pour mettre le jeu en pause et C pour continuer";

            movePacman();
            MoveGhost();

            ////////////////////////////////////////////ANIMATION REQUIN
            switch (imageRequin)
            {
                case 1:
                case 2:
                case 3:
                    pacmanImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/shark_chomp-ani1.png"));
                    break;
                case 4:
                case 5:
                case 6:
                    pacmanImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/shark_chomp-ani2.png"));
                    break;
                case 7:
                case 8:
                case 9:
                    pacmanImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/shark_chomp-ani3.png"));
                    break;
            }
            pacman.Fill = pacmanImage;
            imageRequin++;
            if (imageRequin > 9)
            {
                imageRequin = 1;
            }

            //////////////////////////////////////////ANIMATION PIEUVRE ROSE
            switch (imagePieuvre)
            {
                case 1:
                case 2:
                    pinkGhost.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/PieuvreRose01.png"));
                    break;
                case 3:
                case 4:
                    pinkGhost.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/PieuvreRose02.png"));
                    break;
                case 5:
                case 6:
                    pinkGhost.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/PieuvreRose03.png"));
                    break;
                case 7:
                case 8:
                    pinkGhost.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/PieuvreRose04.png"));
                    break;
                case 9:
                case 10:
                    pinkGhost.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/PieuvreRose05.png"));
                    break;
                case 11:
                case 12:
                    pinkGhost.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/PieuvreRose06.png"));
                    break;
            }
            rosePieuvre.Fill = pinkGhost;
            imagePieuvre++;
            if (imagePieuvre > 12)
            {
                imagePieuvre = 1;
            }
            
            /******************************************            modePuissant     **************************************/
            if (modePuissant == true)
            {
                speed = 9;
                ghostSpeed = 2;
                modePuissantCompteur -= 1;
                if (modePuissantCompteur < 1)
                {
                    speed = 7;
                    ghostSpeed = 10;
                    modePuissant = false;
                }
            }

            if (score == 16)
            {
                for (int i = 0; i < 5; i++)
                {
                    Rectangle rec = new Rectangle()
                    {
                        Width = 30,
                        Height = 30,
                        Fill = starImage,
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
                        StrokeThickness = 2,
                        Tag = "star",
                    };
                    MyCanvas.Children.Add(rec);
                    Canvas.SetTop(rec, 199);
                    Canvas.SetLeft(rec, 489);
                }
            }
            if (score == 4)
            {
                for (int i = 0; i < 5; i++)
                {
                    Rectangle rec = new Rectangle()
                    {
                        Width = 30,
                        Height = 30,
                        Fill = starImage,
                        StrokeThickness = 2,
                        Tag = "star",
                    };
                    MyCanvas.Children.Add(rec);
                    Canvas.SetTop(rec, 50);
                    Canvas.SetLeft(rec, 389);
                }
            }
            if (score == 85)
            {
                GameOver("Vous avez gagné ! \nVous avez mangé tous les poissons !");
            }
            if (gameover)
            {
                mediaElement.Close();
                txtScore.Content += "\n\n\nCliquer R \npour Réessayer";   
            }
        }
        private void GameOver(string message)
        {
            gameTimer.Stop();
            MessageBox.Show(message, "Chasse Aquatique Pac-Requin");
        }
    }
}

