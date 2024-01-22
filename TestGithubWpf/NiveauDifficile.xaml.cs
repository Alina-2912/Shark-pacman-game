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
        bool gaucheNon, droiteNon, basNon, hautNon;
        Rect requinHitBox;
        Rect roiHitbox;
        int vitesse = 8;
        int vitesseEnnemie = 10;
        int mouvementPieuvre = 160;
        int pieuvrePasActuel;
        int score = 0;
        bool jeu_termine = false;
        bool jeuEstEnPause = false;
        bool gagne = false;
        bool modePuissant = false;
        int modePuissantCompteur = 200;
        ImageBrush requinImage = new ImageBrush();
        ImageBrush ennemieRose = new ImageBrush();
        ImageBrush ennemieOrange = new ImageBrush();
        ImageBrush bonusImage = new ImageBrush();
        int imagePieuvre1 = 1;
        int imagePieuvre3 = 1;
        int imageRequin = 1;

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
            //*************************    PAUSE   ******************************
            if (e.Key == Key.P)
            {
                if (!jeuEstEnPause)
                {
                    gameTimer.Stop();
                    jeuEstEnPause = true;
                    mediaElement.Pause();
                    txtScore_pause.Visibility = Visibility.Visible;
                }
            }
            //*************************    RESUME   *******************************
            if (e.Key == Key.C)
            {
                if (jeuEstEnPause)
                {
                    gameTimer.Start();
                    jeuEstEnPause = false;
                    mediaElement.Play();
                    txtScore_pause.Visibility = Visibility.Hidden;
                }
            }
            //*************************    REDEMARRAGE - R   *************************
            if (e.Key == Key.R && jeu_termine)
            {
                CommencerJeu();
            }
            //*************************    TricheMod - T   *************************
            if (e.Key == Key.T)
            {
                TricheMod();
            }

            if (e.Key == Key.Left && gaucheNon == false)
            {
                if (Canvas.GetLeft(requin) >= 10)
                {
                    vaDroite = vaEnHaut = vaEnBas = false;
                    droiteNon = hautNon = basNon = false;
                    vaGauche = true;
                    requin.RenderTransform = new RotateTransform(-180, requin.Width / 2, requin.Height / 2);
                }
                else { Canvas.SetLeft(requin, 10); }
            }
            if (e.Key == Key.Right && droiteNon == false)
            {
                if (Canvas.GetLeft(requin) <= 790) // si possible remplacer 790 par la largeur de la fenètre
                {
                    gaucheNon = hautNon = basNon = false;
                    vaGauche = vaEnHaut = vaEnBas = false;
                    vaDroite = true;
                    requin.RenderTransform = new RotateTransform(0, requin.Width / 2, requin.Height / 2);
                }
                else { Canvas.SetLeft(requin, 790); }
            }
            if (e.Key == Key.Up && hautNon == false)
            {
                if (Canvas.GetTop(requin) >= 10)
                {

                    droiteNon = basNon = gaucheNon = false;
                    vaDroite = vaEnBas = vaGauche = false;
                    vaEnHaut = true;
                    requin.RenderTransform = new RotateTransform(-90, requin.Width / 2, requin.Height / 2);
                }
                else { Canvas.SetTop(requin, 10); }
            }
            if (e.Key == Key.Down && basNon == false)
            {
                if (Canvas.GetTop(requin) <= 590)
                {
                    hautNon = gaucheNon = droiteNon = false;
                    vaEnHaut = vaGauche = vaDroite = false;
                    vaEnBas = true;
                    requin.RenderTransform = new RotateTransform(90, requin.Width / 2, requin.Height / 2);
                }
                else { Canvas.SetTop(requin, 590); }
            }
        }

        private void CommencerJeu()
        {
            pieuvrePasActuel = mouvementPieuvre;

            Canvas.SetLeft(requin, 50);
            Canvas.SetTop(requin, 104);

            gameTimer.Start();
            score = 0;

            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "poisson" || (string)x.Tag == "poissons")
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
                
                if ((string)x.Tag == "pieuvre")
                {
                    if (x.Visibility == Visibility.Hidden)
                    {
                        x.Visibility = Visibility.Visible;
                    }
                   
                }
            }
        }
        private void TricheMod()
        {
            vitesse = 13;
            vitesseEnnemie = 2;
            pieuvrePasActuel = mouvementPieuvre;
        }
        private void DeplacerPieuvre()
        {

            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                //////////////////////////////////////////////DEPLACEMENT PIEUVRE ORANGE
                if (x.Name.ToString() == "orangePieuvre")
                {
                    if (Canvas.GetTop(x) == 260) //droite
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemie);
                        if (Canvas.GetLeft(x) > 400)
                        {
                            Canvas.SetLeft(x, 395);
                        }
                    }
                    if (Canvas.GetLeft(x) == 395) //haut
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) - vitesseEnnemie);
                        if (Canvas.GetTop(x) < 145)
                        {
                            Canvas.SetTop(x, 146);
                        }
                    }
                    if (Canvas.GetTop(x) == 146) //droite
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemie);
                        if (Canvas.GetLeft(x) > 530)
                        {
                            Canvas.SetLeft(x, 529);
                        }
                    }
                    if (Canvas.GetLeft(x) == 529) //bas
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) + vitesseEnnemie);
                        if (Canvas.GetTop(x) > 335)
                        {
                            Canvas.SetTop(x, 333);
                        }
                    }
                    if (Canvas.GetTop(x) == 333) //droite
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemie);
                        if (Canvas.GetLeft(x) > 730)
                        {
                            Canvas.SetLeft(x, 729);
                        }
                    }
                    if (Canvas.GetLeft(x) == 729) //haut
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) - vitesseEnnemie);
                        if (Canvas.GetTop(x) < 230)
                        {
                            Canvas.SetTop(x, 235);
                        }
                    }
                    if (Canvas.GetTop(x) == 235) //changement
                    {
                        Canvas.SetLeft(x, 728);
                    }
                    if (Canvas.GetLeft(x) == 728) // retour bas
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) + vitesseEnnemie);
                        if (Canvas.GetTop(x) > 336)
                        {
                            Canvas.SetTop(x, 334);
                        }
                    }
                    if (Canvas.GetTop(x) == 334) //retour gauche
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseEnnemie);
                        if (Canvas.GetLeft(x) < 518)
                        {
                            Canvas.SetLeft(x, 519);
                        }
                    }
                    if (Canvas.GetLeft(x) == 519) //retour haut
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) - vitesseEnnemie);
                        if (Canvas.GetTop(x) < 144)
                        {
                            Canvas.SetTop(x, 147);
                        }
                    }
                    if (Canvas.GetTop(x) == 147) //retour gauche
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseEnnemie);
                        if (Canvas.GetLeft(x) < 399)
                        {
                            Canvas.SetLeft(x, 403);
                        }
                    }
                    if (Canvas.GetLeft(x) == 403) //retour bas
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) + vitesseEnnemie);
                        if (Canvas.GetTop(x) > 265)
                        {
                            Canvas.SetTop(x, 264);
                        }
                    }
                    if (Canvas.GetTop(x) == 264) //retour gauche
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseEnnemie);
                        if (Canvas.GetLeft(x) < 18)
                        {
                            Canvas.SetLeft(x, 19);
                        }
                    }
                    if (Canvas.GetLeft(x) == 19) //retour haut
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) - vitesseEnnemie);
                        if (Canvas.GetTop(x) < 257)
                        {
                            Canvas.SetTop(x, 260);
                        }
                    }
                }

                //////////////////////////////////////////////DEPLACEMENT PIEUVRE ROSE
                if (x.Name.ToString() == "rosePieuvre")
                {
                    if (Canvas.GetTop(x) == 530) //droite
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemie);
                        if (Canvas.GetLeft(x) > 146)
                        {
                            Canvas.SetLeft(x, 145);
                        }
                    }
                    if (Canvas.GetLeft(x) == 145) //haut
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) - vitesseEnnemie);
                        if (Canvas.GetTop(x) < 370)
                        {
                            Canvas.SetTop(x, 372);
                        }
                    }
                    if (Canvas.GetTop(x) == 372) //droite
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemie);
                        if (Canvas.GetLeft(x) > 281)
                        {
                            Canvas.SetLeft(x, 280);
                        }
                    }
                    if (Canvas.GetLeft(x) == 280) //bas
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) + vitesseEnnemie);
                        if (Canvas.GetTop(x) > 532)
                        {
                            Canvas.SetTop(x, 531);
                        }
                    }
                    if (Canvas.GetTop(x) == 531) //droite
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemie);
                        if (Canvas.GetLeft(x) > 406)
                        {
                            Canvas.SetLeft(x, 405);
                        }
                    }
                    if (Canvas.GetLeft(x) == 405) //haut
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) - vitesseEnnemie);
                        if (Canvas.GetTop(x) < 368)
                        {
                            Canvas.SetTop(x, 369);
                        }
                    }
                    if (Canvas.GetTop(x) == 369) //changement
                    {
                        Canvas.SetLeft(x, 404);
                    }
                    if (Canvas.GetLeft(x) == 404) // retour bas
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) + vitesseEnnemie);
                        if (Canvas.GetTop(x) > 529)
                        {
                            Canvas.SetTop(x, 528);
                        }
                    }
                    if (Canvas.GetTop(x) == 528) // retour gauche
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseEnnemie);
                        if (Canvas.GetLeft(x) < 276)
                        {
                            Canvas.SetLeft(x, 277);
                        }
                    }
                    if (Canvas.GetLeft(x) == 277) // retour haut
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) - vitesseEnnemie);
                        if (Canvas.GetTop(x) < 365)
                        {
                            Canvas.SetTop(x, 366);
                        }
                    }
                    if (Canvas.GetTop(x) == 366) // retour gauche
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseEnnemie);
                        if (Canvas.GetLeft(x) < 147)
                        {
                            Canvas.SetLeft(x, 148);
                        }
                    }
                    if (Canvas.GetLeft(x) == 148) // retour bas
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) + vitesseEnnemie);
                        if (Canvas.GetTop(x) > 528)
                        {
                            Canvas.SetTop(x, 527);
                        }
                    }
                    if (Canvas.GetTop(x) == 527) // retour gauche
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseEnnemie);
                        if (Canvas.GetLeft(x) < 18)
                        {
                            Canvas.SetLeft(x, 19);
                        }
                    }
                    if (Canvas.GetLeft(x) == 19) // retour haut
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) - vitesseEnnemie);
                        if (Canvas.GetTop(x) < 529)
                        {
                            Canvas.SetTop(x, 530);
                        }
                    }
                }
            }



            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                //////////////////////////////////////////////MANGER LES POISSONS
                if ((string)x.Tag == "poisson" || (string)x.Tag == "poissons")
                {
                    if (requinHitBox.IntersectsWith(hitBox) && x.Visibility == Visibility.Visible)
                    {
                        x.Visibility = Visibility.Hidden;
                        score++;
                    }
                }

                /////////////////////////////////////////////MOURIR PAR LES ENNEMIES
                if ((string)x.Tag == "pieuvre")
                {
                    if (requinHitBox.IntersectsWith(hitBox) && modePuissant == false)
                    {
                        if (x.Visibility == Visibility.Visible)
                        {
                            gameTimer.Stop();
                            jeu_termine = true;
                        }
                    }
                    if (requinHitBox.IntersectsWith(hitBox) && x.Visibility == Visibility.Hidden)
                    {
                        jeu_termine = false;
                    }
                    if (requinHitBox.IntersectsWith(hitBox) && modePuissant == true)
                    {
                        x.Visibility = Visibility.Hidden;
                    }
                }

            }

        }
        private void ConfigurationJeu()
        {
            MyCanvas.Focus();
            gameTimer.Tick += BoucleJeu;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();
            pieuvrePasActuel = mouvementPieuvre;

            ImageBrush roiRequin = new ImageBrush();
            roiRequin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/roi_requin.png"));
            Roi.Fill = roiRequin;

            /////////////////////////////////////////////IMAGES POISSONS
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "poisson")
                {
                    ImageBrush nemo = new ImageBrush();
                    nemo.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/poisson_violet.png"));
                    x.Fill = nemo;
                }
            }
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "poissons")
                {
                    ImageBrush nemo = new ImageBrush();
                    nemo.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/poisson_couleurs.png"));
                    x.Fill = nemo;
                }
            }

            //////////////////////////////////////////////IMAGES MURS
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

            ///////////////////////////////////////////////////////ANIMATION REQUIN
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

            ///////////////////////////////////////////////////////ANIMATION PIEUVRE ROSE
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

            ///////////////////////////////////////////////////////ANIMATION PIEUVRE ORANGE
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

            //////////////////////////////////////////////////////DEPLACER REQUIN
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
                basNon = true;
                vaEnBas = false;
            }
            if (vaEnHaut && Canvas.GetTop(requin) < 1)
            {
                hautNon = true;
                vaEnHaut = false;
            }
            if (vaGauche && Canvas.GetLeft(requin) - 10 < 1)
            {
                gaucheNon = true;
                vaGauche = false;
            }
            if (vaDroite && Canvas.GetLeft(requin) + 70 > Application.Current.MainWindow.Width)
            {
                droiteNon = true;
                vaDroite = false;
            }
            //////////////////////////////////////////////////////////INTERSECTION REQUIN AVEC MUR
            requinHitBox = new Rect(Canvas.GetLeft(requin), Canvas.GetTop(requin), requin.Width, requin.Height);
            roiHitbox = new Rect(Canvas.GetLeft(Roi), Canvas.GetTop(Roi), Roi.Width, Roi.Height);
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                if ((string)x.Tag == "obstacleVertical" || (string)x.Tag == "obstacleHorizontal" || x.Name.ToString() == "Roi")
                {
                    if (vaGauche == true && requinHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetLeft(requin, Canvas.GetLeft(requin) + 10);
                        gaucheNon = true;
                        vaGauche = false;
                    }
                    if (vaDroite == true && requinHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetLeft(requin, Canvas.GetLeft(requin) - 10);
                        droiteNon = true;
                        vaDroite = false;
                    }
                    if (vaEnBas == true && requinHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetTop(requin, Canvas.GetTop(requin) - 10);
                        basNon = true;
                        vaEnBas = false;
                    }
                    if (vaEnHaut == true && requinHitBox.IntersectsWith(hitBox))
                    {
                        Canvas.SetTop(requin, Canvas.GetTop(requin) + 10);
                        hautNon = true;
                        vaEnHaut = false;
                    }
                }
                if ((string)x.Tag == "bonus")
                {
                    if (requinHitBox.IntersectsWith(hitBox) && x.Visibility == Visibility.Visible)
                    {
                        modePuissant = true;
                        modePuissantCompteur = 200;
                        x.Visibility = Visibility.Hidden;
                    }
                }
                //////////////////////////////////////////////MOURIR PAR LES ENNEMIES
                if ((string)x.Tag == "pieuvre")
                {
                    if (requinHitBox.IntersectsWith(hitBox) && modePuissant == false)
                    {
                        if (x.Visibility == Visibility.Visible)
                        {
                            gameTimer.Stop();
                            jeu_termine = true;
                        }
                    }
                    if (requinHitBox.IntersectsWith(hitBox) && x.Visibility == Visibility.Hidden)
                    {
                        jeu_termine = false;
                    }
                    if (requinHitBox.IntersectsWith(hitBox) && modePuissant == true)
                    {
                        x.Visibility = Visibility.Hidden;
                        jeu_termine = false;
                    }
                }
            }
            if (score == 83 && requinHitBox.IntersectsWith(roiHitbox))
            {
                jeu_termine = true;
                JeuTermine("Vous avez gagné ! \nVous avez mangé tous les poissons et rejoint le roi des requins !");
            }
            bonusImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/images/treasurebox.jpg"));
            if (score == 25)
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
                    Canvas.SetTop(rec, 260);
                    Canvas.SetLeft(rec, 279);

                }
            }
            if (score == 45)
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
                    Canvas.SetTop(rec, 373);
                    Canvas.SetLeft(rec, 17);
                }
            }
            if (score == 65)
            {
                for (int i = 0; i < 5; i++)
                {
                    Rectangle rec = new Rectangle()
                    {
                        Width = 30,
                        Height = 30,
                        Fill = bonusImage,
                        Visibility = Visibility.Hidden,
                        StrokeThickness = 2,
                        Tag = "bonus",
                    };
                    MyCanvas.Children.Add(rec);
                    Canvas.SetTop(rec, 256);
                    Canvas.SetLeft(rec, 695);
                }
            }
            if (score == 82)
            {
                gagne = true;
            }
            if (jeu_termine)
            {
                txtScore.Content += "\n\n\nCliquer R \npour Rejouer";
            }

            /*******************************    ModePuissant    ******************************/
            if (modePuissant == true)
            {
                vitesse = 12;
                vitesseEnnemie = 4;
                modePuissantCompteur -= 1;
                if (modePuissantCompteur < 1)
                {
                    vitesse = 7;
                    vitesseEnnemie = 10;
                    modePuissant = false;
                }
            }

        }
        private void JeuTermine(string message)
        {
            gameTimer.Stop();
            MessageBox.Show(message, "Chasse Aquatique Pac-Requin");

        }
    }
}
