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
        bool jeu_termine = false;
        bool estJeuEnPause = false;
        ImageBrush requinImage = new ImageBrush();
        ImageBrush ennemieRose = new ImageBrush();
        ImageBrush ennemieOrange = new ImageBrush();
        int imagePieuvre1 = 1;
        int imagePieuvre3 = 1;
        int imageRequin = 1;
        private double debutX = 145;
        private double debutY = 530;
        private int direction = 0; // 0: haut, 1: droite, 2: en bas, 3: gauche

        public NiveauDifficile()
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
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "sound/bts_dionysus.wav");
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
            if (e.Key == Key.C)
            {
                if (estJeuEnPause)
                {
                    gameTimer.Start();
                    estJeuEnPause = false;
                    mediaElement.Play();
                }
            }
            /*************************    REDEMARRAGE - R   *************************/
            if (e.Key == Key.R && jeu_termine)
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
                if ((string)x.Tag == "poisson")
                {
                    if (x.Visibility == Visibility.Hidden)
                    {
                        x.Visibility = Visibility.Visible;
                    }
                }

                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                requinHitBox = new Rect(Canvas.GetLeft(requin), Canvas.GetTop(requin), requin.Width, requin.Height);

                if ((string)x.Tag == "bonus")
                {
                    if (x.Visibility == Visibility.Hidden)
                    {
                        x.Visibility = Visibility.Visible;
                    }
                }
                /*
                if ((string)x.Tag == "ghost")
                {
                    if (x.Visibility == Visibility.Hidden)
                    {
                        x.Visibility = Visibility.Visible;
                    }
                    if (requinHitBox.IntersectsWith(hitBox))
                    {
                        gameTimer.Stop();
                        jeu_termine = true;
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
                }*/

            }
        }
        private void DeplacerPieuvre()
        {

            /*foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if (x.Name.ToString() == "rosePieuvre")
                {

                    if (Canvas.GetLeft(x) > 470)
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) - vitesse);
                    }
                    if (Canvas.GetTop(x) < 148)
                    {
                        Canvas.SetLeft(x, 10);
                        Canvas.SetTop(x, 275);
                    }
                    if (Canvas.GetLeft(x) < 700 && Canvas.GetTop(x) == 275)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesse);
                    }
                }
            }*/

            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if (x.Name.ToString() == "rosePieuvre")
                {
                    double GaucheActuel = Canvas.GetLeft(x);
                    double TopActuel = Canvas.GetTop(x);


                    /*switch (direction)
                    {
                        case 0: // Move up
                            Canvas.SetTop(x, TopActuel - vitesse);
                            if (GaucheActuel == debutX && TopActuel == debutY - 150)
                                direction = 1; // Change direction to right
                            break;

                        case 1: // Move to the right
                            Canvas.SetLeft(x, GaucheActuel + vitesse);
                            if (GaucheActuel == debutX + 135 && TopActuel == debutY - 150)
                                direction = 2; // Change direction to down
                            break;

                        case 2: // Move down
                            Canvas.SetTop(x, TopActuel + vitesse);
                            if (GaucheActuel == debutX + 135 && TopActuel == debutY)
                                direction = 3; // Change direction to right
                            break;

                        case 3: // Move to the right
                            Canvas.SetLeft(x, GaucheActuel + vitesse);
                            if (GaucheActuel == debutX + 260 && TopActuel == debutY)
                                direction = 4; // Change direction to up
                            break;

                        case 4: // Move up
                            Canvas.SetLeft(x, GaucheActuel - vitesse);
                            if (GaucheActuel == debutX + 260 && TopActuel == debutY - 160)
                                direction = 5; // Change direction to down
                            break;

                        case 5: // Move down
                            Canvas.SetTop(x, TopActuel + vitesse);
                            if (GaucheActuel == debutX + 260 && TopActuel == debutY)
                                direction = 6; // Change direction to left
                            break;

                        case 6: // Move to the left
                            Canvas.SetLeft(x, GaucheActuel - vitesse);
                            if (GaucheActuel == debutX + 135 && TopActuel == debutY)
                                direction = 7; // Change direction to up
                            break;

                        case 7: // Move up
                            Canvas.SetTop(x, TopActuel - vitesse);
                            if (GaucheActuel == debutX + 135 && TopActuel == debutY - 150)
                                direction = 8; // Change direction to left
                            break;

                        case 8: // Move to the left
                            Canvas.SetLeft(x, GaucheActuel - vitesse);
                            if (GaucheActuel == debutX && TopActuel == debutY - 150)
                                direction = 9; // Change direction to down
                            break;

                        case 9: // Move down
                            Canvas.SetTop(x, TopActuel + vitesse);
                            if (GaucheActuel == debutX && TopActuel == debutY)
                                direction = 0; // Change direction to up
                            break;
                    }*/
                }
            }


            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                if ((string)x.Tag == "poisson")
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
                        jeu_termine = true;
                    }

                    /*if (x.Name.ToString() == "orangePieuvre")
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
                    actuellePieuvrePas--;
                    if (actuellePieuvrePas < 1)
                    {
                        actuellePieuvrePas = mouvementPieuvre;
                        vitesseEnnemie = -vitesseEnnemie;
                    }*/
                }

            }

        }
        private void ConfigurationJeu()
        {
            MyCanvas.Focus();
            gameTimer.Tick += BoucleJeu;
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
        private void BoucleJeu(object sender, EventArgs e)
        {
            DeplacerPieuvre();
            txtScore.Content = "Score: " + score + "\nCliquer P pour mettre le jeu en pause et C pour continuer";

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

            switch (imagePieuvre1)
            {
                case 1:
                case 2:
                    ennemieRose.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/PieuvreRose01.png"));
                    break;
                case 3:
                case 4:
                    ennemieRose.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/PieuvreRose02.png"));
                    break;
                case 5:
                case 6:
                    ennemieRose.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/PieuvreRose03.png"));
                    break;
                case 7:
                case 8:
                    ennemieRose.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/PieuvreRose04.png"));
                    break;
                case 9:
                case 10:
                    ennemieRose.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/PieuvreRose05.png"));
                    break;
                case 11:
                case 12:
                    ennemieRose.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/PieuvreRose06.png"));
                    break;
            }
            rosePieuvre.Fill = ennemieRose;
            imagePieuvre1++;
            if (imagePieuvre1 > 12)
            {
                imagePieuvre1 = 1;
            }

            switch (imagePieuvre3)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    ennemieOrange.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/poulpeOrange01.png"));
                    break;
                case 10:
                case 11:
                case 12:
                    ennemieOrange.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/poulpeOrange02.png"));
                    break;
                case 13:
                case 14:
                case 15:
                    ennemieOrange.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/poulpeOrange03.png"));
                    break;
                case 16:
                case 17:
                case 18:
                    ennemieOrange.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/poulpeOrange04.png"));
                    break;
            }
            orangePieuvre.Fill = ennemieOrange;
            imagePieuvre3++;
            if (imagePieuvre3 > 18)
            {
                imagePieuvre3 = 1;
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
                        jeu_termine = true;
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
            if (score == 82)
            {
                JeuTermine("Vous avez gagné ! \nVous avez mangé tous les poissons !");
            }
            if (jeu_termine)
            {
                mediaElement.Close();
                txtScore.Content += "\n        Cliquer R \n        pour Rejouer";

                if (txtScore.Content.Equals("\n        Cliquer R \n        pour Rejouer"))
                {
                    txtScore.Foreground = Brushes.White;
                }
                else
                {
                    txtScore.Foreground = Brushes.Black;
                }
            }
        }
        private void JeuTermine(string message)
        {
            gameTimer.Stop();
            MessageBox.Show(message, "Chasse Aquatique Pac-Requin");

            //System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            //Application.Current.Shutdown();
        }
    }
}
