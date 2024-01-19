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
        bool vaGauche, vaDroite, vaEnBas, vaEnHaut;
        int vitesse = 10;
        Rect requinHitBox;
        int vitesseEnnemie = 10;
        int mouvementPieuvre = 160;
        int actuellePieuvrePas;
        int score = 0;
        int imageRequin = 1;
        int imageTorche = 1;
        bool jeu_termine = false;
        bool gagne = false;
        bool estJeuEnPause = false;
        ImageBrush requinImage = new ImageBrush();
        ImageBrush bonusImage = new ImageBrush();

        public NiveauNormale()
        {
            InitializeComponent();
            ConfigurationJeu();
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
                    //mediaElement.Pause();
                }
            }
            /*************************    RESUME   *************************/
            if (e.Key == Key.R)
            {
                if (estJeuEnPause)
                {
                    gameTimer.Start();
                    estJeuEnPause = false;
                    //mediaElement.Play();
                }
            }
            /*************************    REbonusT - R   *************************/
            if (e.Key == Key.R && jeu_termine)
            {
                CommencerJeu();
            }

            if (e.Key == Key.Left)
            {
                vaGauche = true;
                //pacman.RenderTransform = new RotateTransform(-180, pacman.Width / 2, pacman.Height / 2);
                pacman.RenderTransformOrigin = new Point(0.5, 0.2);
                ScaleTransform flipTrans = new ScaleTransform();
                flipTrans.ScaleX = -1;
                pacman.RenderTransform = flipTrans;
                vaDroite = false;
                vaEnHaut = false;
                vaEnBas = false;
            }
            if (e.Key == Key.Right)
            {
                vaDroite = true;
                pacman.RenderTransform = new RotateTransform(0, pacman.Width / 2, pacman.Height / 2);
                vaGauche = false;
                vaEnHaut = false;
                vaEnBas = false;

            }
            if (e.Key == Key.Up)
            {
                if (!vaEnHaut && !vaEnBas)
                {
                    Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - 7);
                }
                vaEnHaut = true;
                pacman.RenderTransform = new RotateTransform(-90, pacman.Width / 2, pacman.Height / 2);
                vaDroite = false;
                vaGauche = false;
                vaEnBas = false;


            }
            if (e.Key == Key.Down)
            {
                if (!vaEnHaut && !vaEnBas)
                {
                    Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - 7);
                }
                vaEnBas = true;
                pacman.RenderTransform = new RotateTransform(90, pacman.Width / 2, pacman.Height / 2);
                vaDroite = false;
                vaGauche = false;
                vaEnHaut = false;
            }
        }
        private void ConfigurationJeu()
        {
            MyCanvas.Focus();
            gameTimer.Tick += GameLoop;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();
            actuellePieuvrePas = mouvementPieuvre;

            ImageBrush porte1 = new ImageBrush();
            porte1.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/porte.png"));
            porte.Fill = porte1;

            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "wall")
                {
                    ImageBrush mur = new ImageBrush();
                    mur.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/rochets2.jpg"));
                    x.Fill = mur;
                }
                if ((string)x.Tag == "meduses")
                {
                    ImageBrush meduse = new ImageBrush();
                    meduse.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/meduse.png"));
                    x.Fill = meduse;
                }
            }
                //requinHitBox = new Rect(Canvas.GetLeft(pacman), Canvas.GetTop(pacman), pacman.Width, pacman.Height);
        }
        private void DeplacerRequin()
        {
            if (vaDroite && Canvas.GetLeft(pacman) < Application.Current.MainWindow.Width - 60)
            {
                Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + vitesse);
            }
            if (vaGauche && Canvas.GetLeft(pacman) > 20)
            {
                Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - vitesse);
            }
            if (vaEnHaut && Canvas.GetTop(pacman) > 20)
            {
                Canvas.SetTop(pacman, Canvas.GetTop(pacman) - vitesse);
            }
            if (vaEnBas && Canvas.GetTop(pacman) < Application.Current.MainWindow.Height - 60)
            {
                Canvas.SetTop(pacman, Canvas.GetTop(pacman) + vitesse);
            }

            requinHitBox = new Rect(Canvas.GetLeft(pacman), Canvas.GetTop(pacman), pacman.Width, pacman.Height);
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                if ((string)x.Tag == "bonus")
                {
                    if (requinHitBox.IntersectsWith(hitBox) && x.Visibility == Visibility.Visible)
                    {
                        x.Visibility = Visibility.Hidden;
                    }
                }

                if ((string)x.Tag == "wall")
                {
                    if (requinHitBox.IntersectsWith(hitBox))
                    {
                        if (vaDroite)
                        {
                            Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) - vitesse);
                            vaDroite = false;
                        }

                        if (vaGauche)
                        {
                            Canvas.SetLeft(pacman, Canvas.GetLeft(pacman) + vitesse);
                            vaGauche = false;
                        }

                        if (vaEnHaut)
                        {
                            Canvas.SetTop(pacman, Canvas.GetTop(pacman) + vitesse);
                            vaEnHaut = false;
                        }

                        if (vaEnBas)
                        {
                            Canvas.SetTop(pacman, Canvas.GetTop(pacman) - vitesse);
                            vaEnBas = false;
                        }
                    }
                }
            }
        }
        private void CommencerJeu()
        {
            actuellePieuvrePas = mouvementPieuvre;

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
                requinHitBox = new Rect(Canvas.GetLeft(pacman), Canvas.GetTop(pacman), pacman.Width, pacman.Height);

                if ((string)x.Tag == "meduses")
                {
                    if (x.Visibility == Visibility.Hidden)
                    {
                        x.Visibility = Visibility.Visible;
                    }
                }
                if ((string)x.Tag == "pieuvre")
                {
                    if (requinHitBox.IntersectsWith(hitBox))
                    {
                        gameTimer.Stop();
                        jeu_termine = true;
                    }
                    if (x.Name.ToString() == "orangePieuvre")
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseEnnemie);
                    }
                    else
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemie);
                    }
                    actuellePieuvrePas--;
                    if (actuellePieuvrePas < 1)
                    {
                        actuellePieuvrePas = mouvementPieuvre;
                        vitesseEnnemie = -vitesseEnnemie;
                    }
                }

            }
        }

        private void DeplacerPieuvre()
        {
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                if ((string)x.Tag == "meduses")
                {
                    if (requinHitBox.IntersectsWith(hitBox) && x.Visibility == Visibility.Visible)
                    {
                        x.Visibility = Visibility.Hidden;
                        score++;
                    }
                }
                if ((string)x.Tag == "fin")
                {
                    if (requinHitBox.IntersectsWith(hitBox) && gagne == true)
                    {
                        x.Visibility = Visibility.Hidden;
                        GameOver("Vous avez gagne");
                    }
                }

                if ((string)x.Tag == "pieuvre")
                {
                    if (requinHitBox.IntersectsWith(hitBox))
                    {
                        gameTimer.Stop();
                        jeu_termine = true;
                    }
                    if (x.Name.ToString() == "orangePieuvre")
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseEnnemie);
                    }
                    if (x.Name.ToString() == "violetPieuvre")
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemie);
                    }
                    if (x.Name.ToString() == "rosePieuvre")
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemie);
                    }
                    if (x.Name.ToString() == "greenGuy")
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemie);
                    }
                    actuellePieuvrePas--;
                    if (actuellePieuvrePas < 1)
                    {
                        actuellePieuvrePas = mouvementPieuvre;
                        vitesseEnnemie = -vitesseEnnemie;
                    }
                }

            }
        }
        private void GameLoop(object sender, EventArgs e)
        {
            bonusImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/images/treasurebox.jpg"));
            txtScore.Content = "Score: " + score + "\nPress P to Pause and R to Resume";

            DeplacerRequin();
            DeplacerPieuvre();


            switch (imageRequin)
            {
                case 1:
                case 2:
                case 3:
                    requinImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/req1.png"));
                    break;
                case 4:
                case 5:
                case 6:
                    requinImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/req2.png"));
                    break;
                case 7:
                case 8:
                case 9:
                    requinImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/req3.png"));
                    break;
                case 10:
                case 11:
                case 12:
                    requinImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/req4.png"));
                    break;
            }
            pacman.Fill = requinImage;
            imageRequin++;
            if (imageRequin > 12)
            {
                imageRequin = 1;
            }

            ImageBrush torcheImage = new ImageBrush();
            switch (imageTorche)
            {
                case 1:
                case 2:
                case 3:
                    torcheImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/torche1.png"));
                    break;
                case 4:
                case 5:
                case 6:
                    torcheImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/torche2.png"));
                    break;
                case 7:
                case 8:
                case 9:
                    torcheImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/torche3.png"));
                    break;
                case 10:
                case 11:
                case 12:
                    torcheImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/torche4.png"));
                    break;
                case 13:
                case 14:
                case 15:
                    torcheImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/torche5.png"));
                    break;
            }
            torche.Fill = torcheImage;
            imageTorche++;
            if (imageTorche > 15)
            {
                imageTorche = 1;
            }

            if (score == 16)
            {
                for (int i = 0; i < 5; i++)
                {
                    Rectangle rec = new Rectangle()
                    {
                        Width = 30,
                        Height = 30,
                        Fill = bonusImage,
                        StrokeThickness = 2,
                        Tag = "bonus",
                    };
                    MyCanvas.Children.Add(rec);
                    Canvas.SetTop(rec, 529);
                    Canvas.SetLeft(rec, 220);

                }
            }
            if (score == 31)
            {
                for (int i = 0; i < 5; i++)
                {
                    Rectangle rec = new Rectangle()
                    {
                        Width = 30,
                        Height = 30,
                        Fill = bonusImage,
                        //Stroke = Brushes.Red,
                        //Visibility = Visibility.Hidden,
                        StrokeThickness = 2,
                        Tag = "bonus",
                    };
                    MyCanvas.Children.Add(rec);
                    Canvas.SetTop(rec, 199);
                    Canvas.SetLeft(rec, 489);
                }
            }
            if (score == 48)
            {
                for (int i = 0; i < 5; i++)
                {
                    Rectangle rec = new Rectangle()
                    {
                        Width = 30,
                        Height = 30,
                        Fill = bonusImage,
                        //Stroke = Brushes.Red,
                        Visibility = Visibility.Hidden,
                        StrokeThickness = 2,
                        Tag = "bonus",
                    };
                    MyCanvas.Children.Add(rec);
                    Canvas.SetTop(rec, 50);
                    Canvas.SetLeft(rec, 650);
                }
            }
            if (score == 89)
            {
                gagne = true;
            }
            if (jeu_termine)
            {
                txtScore.Content += "   Press R to Retry";
            }
        }
        private void GameOver(string message)
        {
            gameTimer.Stop();
            MessageBox.Show(message, "Chasse Aquatique Pac-Requin");

            //System.Diagnostics.Process.bonust(Application.ResourceAssembly.Location);
            //Application.Current.Shutdown();
        }
    }
}
