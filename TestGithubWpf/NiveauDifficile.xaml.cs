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
    public partial class NiveauDifficile : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        bool vaGauche, vaDroite, vaEnBas, vaEnHaut;
        bool noLeft, noRight, noDown, noUp;
        int vitesse = 8;
        Rect requinHitBox;
        int vitesseEnnemie = 10;
        int mouvementPieuvre = 160;
        int actuellePieuvrePas;
        int score = 0;
        bool gameover = false;
        bool estJeuEnPause = false;
        ImageBrush requinImage = new ImageBrush();
        int imageRequin = 1;

        public NiveauDifficile()
        {
            InitializeComponent();
            GameSetUp();
        }
        private void ButtonFermer_Click(object sender, RoutedEventArgs e)
        {
            Close();
            Application.Current.Shutdown();
        }
        private void ButtonRetour_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Hide();
            mediaElement.Close();
        }

        private void ButtonMusique_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "sound/dionysus.wav");
            mediaElement.Source = uri;
            mediaElement.Close();
        }

        private void CanvasKeyDown(object sender, KeyEventArgs e)
        {
            /*************************    PAUSE   *************************/
            if (e.Key == Key.P)
            {
                if (!estJeuEnPause)
                {
                    gameTimer.Stop();
                    estJeuEnPause = true;
                    mediaElement.Pause();
                }
            }
            /*************************    RESUME   *************************/
            if (e.Key == Key.R)
            {
                if (estJeuEnPause)
                {
                    gameTimer.Start();
                    estJeuEnPause = false;
                    mediaElement.Play();
                }
            }
            /*************************    REDEMARRAGE - R   *************************/
            if (e.Key == Key.R && gameover)
            {
                CommencerJeu();
            }

            if (e.Key == Key.Left && noLeft == false)
            {
                if (Canvas.GetLeft(requin) >= 10)
                {
                    vaDroite = vaEnHaut = vaEnBas = false;
                    noRight = noUp = noDown = false;
                    vaGauche = true;
                    requin.RenderTransform = new RotateTransform(-180, requin.Width / 2, requin.Height / 2);
                }
                else { Canvas.SetLeft(requin, 10); }
            }
            if (e.Key == Key.Right && noRight == false)
            {
                if (Canvas.GetLeft(requin) <= 790) // si possible remplacer 790 par la largeur de la fenètre
                {
                    noLeft = noUp = noDown = false;
                    vaGauche = vaEnHaut = vaEnBas = false;
                    vaDroite = true;
                    requin.RenderTransform = new RotateTransform(0, requin.Width / 2, requin.Height / 2);
                    requinImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/images/requinvuhaut05.png"));
                    requin.Fill = requinImage;
                }
                else { Canvas.SetLeft(requin, 790); }
            }
            if (e.Key == Key.Up && noUp == false)
            {
                if (Canvas.GetTop(requin) >= 10)
                {

                    noRight = noDown = noLeft = false;
                    vaDroite = vaEnBas = vaGauche = false;
                    vaEnHaut = true;
                    requin.RenderTransform = new RotateTransform(-90, requin.Width / 2, requin.Height / 2);
                }
                else { Canvas.SetTop(requin, 10); }
            }
            if (e.Key == Key.Down && noDown == false)
            {
                if (Canvas.GetTop(requin) <= 590)
                {
                    noUp = noLeft = noRight = false;
                    vaEnHaut = vaGauche = vaDroite = false;
                    vaEnBas = true;
                    requin.RenderTransform = new RotateTransform(90, requin.Width / 2, requin.Height / 2);
                }
                else { Canvas.SetTop(requin, 590); }
            }
        }
        private void Canvas_KeyUp(object sender, KeyEventArgs e)
        {
            // if the space key is pressed AND jumping boolean is true AND player y location is above 260 pixels
            if (e.Key == Key.Left && !vaGauche && Canvas.GetTop(requin) > 260)
            {
                // set jumping to true
                vaGauche = true;
                vaDroite = false;
                vaEnHaut = false;
                vaEnBas = false;
                // set vitesse integer to -12
                vitesse = -12;
                requinImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/coral4.jpg"));
            }
        }
        private void CommencerJeu()
        {
            actuellePieuvrePas = mouvementPieuvre;

            Canvas.SetLeft(requin, 50);
            Canvas.SetTop(requin, 104);

            /*Canvas.SetLeft(rosePieuvre, 173);
            Canvas.SetTop(rosePieuvre, 404);

            Canvas.SetLeft(violetPieuvre, 173);
            Canvas.SetTop(violetPieuvre, 29);

            Canvas.SetLeft(orangePieuvre, 651);
            Canvas.SetTop(orangePieuvre, 104);*/

            gameTimer.Start();
            score = 0;

            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                requinHitBox = new Rect(Canvas.GetLeft(requin), Canvas.GetTop(requin), requin.Width, requin.Height);

                if ((string)x.Tag == "bonus")
                {
                    if (x.Visibility == Visibility.Hidden)
                    {
                        x.Visibility = Visibility.Visible;
                    }
                }
                if ((string)x.Tag == "ghost")
                {
                    if (x.Visibility == Visibility.Hidden)
                    {
                        x.Visibility = Visibility.Visible;
                    }
                    if (requinHitBox.IntersectsWith(hitBox))
                    {
                        gameTimer.Stop();
                        gameover = true;
                    }
                    if (x.Name.ToString() == "orangePieuvre")
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) - vitesseEnnemie);
                    }
                    else
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) + vitesseEnnemie);
                    }
                    actuellePieuvrePas--;
                    if (actuellePieuvrePas < 1)
                    {
                        actuellePieuvrePas = mouvementPieuvre;
                        vitesseEnnemie = - vitesseEnnemie;
                    }
                }

            }
        }
        private void GameSetUp()
        {
            MyCanvas.Focus();
            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();
            actuellePieuvrePas = mouvementPieuvre;

            
            /*ImageBrush redGhost = new ImageBrush();
            redGhost.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/red.jpg"));
            violetPieuvre.Fill = redGhost;
            ImageBrush orangeGhost = new ImageBrush();
            orangeGhost.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/orange.jpg"));
            orangePieuvre.Fill = orangeGhost;
            ImageBrush ennemieRose = new ImageBrush();
            ennemieRose.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/pink.jpg"));
            rosePieuvre.Fill = ennemieRose;*/

            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "poisson")
                {
                    ImageBrush nemo = new ImageBrush();
                    nemo.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/nemo.jpg"));
                    x.Fill = nemo;
                }
            }

            foreach (var alguesBcp in MyCanvas.Children.OfType<Rectangle>())
            {
                if ((string)alguesBcp.Tag == "obstacleVertical")
                {
                    ImageBrush mur = new ImageBrush();
                    mur.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/alguesVert.png"));
                    alguesBcp.Fill = mur;
                }
            }
            foreach (var alguesBcp in MyCanvas.Children.OfType<Rectangle>())
            {
                if ((string)alguesBcp.Tag == "obstacleHorizontal")
                {
                    ImageBrush mur = new ImageBrush();
                    mur.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/alguesHorizontal.png"));
                    alguesBcp.Fill = mur;
                }
            }
        }
        private void GameLoop(object sender, EventArgs e)
        {
            txtScore.Content = "Score: " + score + "\nPress P to Pause and R to Resume";

            switch (imageRequin)
            {
                case 1:
                case 2:
                case 3:
                    requinImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/requinvuhaut01.png"));
                    break;
                case 4:
                case 5:
                case 6:
                    requinImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/requinvuhaut02.png"));
                    break;
                case 7:
                case 8:
                case 9:
                    requinImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/requinvuhaut03.png"));
                    break;
                case 10:
                case 11:
                case 12:
                    requinImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/requinvuhaut04.png"));
                    break;
                case 13:
                case 14:
                case 15:
                    requinImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/requinvuhaut05.png"));
                    break;
                case 16:
                case 17:
                case 18:
                    requinImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/requinvuhaut06.png"));
                    break;
                case 19:
                case 20:
                case 21:
                    requinImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/requinvuhaut07.png"));
                    break;
            }
            requin.Fill = requinImage;
            imageRequin++;
            if (imageRequin > 21)
            {
                imageRequin = 1;
            }

            if (vaDroite)
            {
                Canvas.SetLeft(requin, Canvas.GetLeft(requin) + vitesse);
            }
            if (vaGauche)
            {
                Canvas.SetLeft(requin, Canvas.GetLeft(requin) - vitesse);
            }
            if (vaEnHaut)
            {
                Canvas.SetTop(requin, Canvas.GetTop(requin) - vitesse);
            }
            if (vaEnBas)
            {
                Canvas.SetTop(requin, Canvas.GetTop(requin) + vitesse);
            }
            if (vaEnBas && Canvas.GetTop(requin) + 80 > Application.Current.MainWindow.Height)
            {
                noDown = true;
                vaEnBas = false;
            }
            if (vaEnHaut && Canvas.GetTop(requin) < 1)
            {
                noUp = true;
                vaEnHaut = false;
            }
            if (vaGauche && Canvas.GetLeft(requin) - 10 < 1)
            {
                noLeft = true;
                vaGauche = false;
            }
            if (vaDroite && Canvas.GetLeft(requin) + 70 > Application.Current.MainWindow.Width)
            {
                noRight = true;
                vaDroite = false;
            }
            requinHitBox = new Rect(Canvas.GetLeft(requin), Canvas.GetTop(requin), requin.Width, requin.Height);
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                if ((string)x.Tag == "obstacleVertical" || (string)x.Tag == "obstacleHorizontal")
                {
                    if (vaGauche == true && requinHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetLeft(requin, Canvas.GetLeft(requin) + 10);
                        noLeft = true;
                        vaGauche = false;
                    }
                    if (vaDroite == true && requinHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetLeft(requin, Canvas.GetLeft(requin) - 10);
                        noRight = true;
                        vaDroite = false;
                    }
                    if (vaEnBas == true && requinHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetTop(requin, Canvas.GetTop(requin) - 10);
                        noDown = true;
                        vaEnBas = false;
                    }
                    if (vaEnHaut == true && requinHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetTop(requin, Canvas.GetTop(requin) + 10);
                        noUp = true;
                        vaEnHaut = false;
                    }
                }
                if ((string)x.Tag == "bonus")
                {
                    if (requinHitBox.IntersectsWith(hitBox) && x.Visibility == Visibility.Visible)
                    {
                        x.Visibility = Visibility.Hidden;
                        score++;
                    }
                }
                if ((string)x.Tag == "pieuvre")
                {
                    if (requinHitBox.IntersectsWith(hitBox))
                    {
                        gameTimer.Stop();
                        gameover = true;
                    }
                    /*if (x.Name.ToString() == "orangeGuy")
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseEnnemie);
                    }
                    else
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseEnnemie);
                    }
                    actuellePieuvrePas--;
                    if (actuellePieuvrePas < 1)
                    {
                        actuellePieuvrePas = mouvementPieuvre;
                        vitesseEnnemie = -vitesseEnnemie;
                    }*/
                }
            }
            if (score == 85)
            {
                GameOver("You Win, you collected all of the bonuss");
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
