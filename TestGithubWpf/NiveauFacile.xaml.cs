using Microsoft.Win32;
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
        bool vaGauche, vaDroite, vaEnBas, vaEnHaut;
        int vitesse = 7;
        Rect requinHitBox;
        int vitesseEnnemie = 10;
        int mouvementPieuvre = 160;
        int actuellePieuvrePas;
        int score = 0;
        bool jeu_termine = false;
        bool estJeuEnPause = false;
        bool modePuissant = false;
        ImageBrush treasureImage = new ImageBrush();
        List<Rectangle> dissolvantObjets = new List<Rectangle>();
        ImageBrush requinImage = new ImageBrush();
        ImageBrush ennemieRose = new ImageBrush();
        ImageBrush ennemieViolet = new ImageBrush();
        ImageBrush ennemieOrange = new ImageBrush();
        int imageRequin = 1;
        int imagePieuvre1 = 1;
        int imagePieuvre2 = 1;
        int imagePieuvre3 = 1;
        int modePuissantCompteur = 200;

        public NiveauFacile()
        {
            InitializeComponent();
            ConfigurationJeu();
            gameTimer.Tick += BoucleJeu;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
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
        private void CommencerJeu()
        {
            actuellePieuvrePas = mouvementPieuvre;

            Canvas.SetLeft(pacman, 50);
            Canvas.SetTop(pacman, 104);

            Canvas.SetLeft(rosePieuvre, 173);
            Canvas.SetTop(rosePieuvre, 404);

            Canvas.SetLeft(violetPieuvre, 173);
            Canvas.SetTop(violetPieuvre, 29);

            Canvas.SetLeft(orangePieuvre, 658);
            Canvas.SetTop(orangePieuvre, 120);

            gameTimer.Start();
            score = 0;
            DeplacerPieuvre();


        }
        private void ConfigurationJeu()
        {
            dissolvantObjets.Clear();

            MyCanvas.Focus();
            gameTimer.Start();
            actuellePieuvrePas = mouvementPieuvre;

            ImageBrush corail1 = new ImageBrush();
            corail1.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/fish_seaweed.png"));
            corailMillieu.Fill = corail1;

            ImageBrush corail2 = new ImageBrush();
            corail2.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/corailleRose.png"));
            obstacle1.Fill = corail2;

            ImageBrush corail3 = new ImageBrush();
            corail3.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/creatureMer.png"));
            obstacle2.Fill = corail3;

            ImageBrush corail4 = new ImageBrush();
            corail4.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/vaguesaquatiques.jpg"));
            obstacle3.Fill = corail4;

            ImageBrush bulles_1 = new ImageBrush();
            bulles_1.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/bulles.png"));
            bulles1.Fill = bulles_1;

            ImageBrush bulles_2 = new ImageBrush();
            bulles_2.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/bulles.png"));
            bulles2.Fill = bulles_2;

            ImageBrush rochet = new ImageBrush();
            rochet.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/rochets.png"));
            rochets.Fill = rochet;

            ImageBrush rougecorail1 = new ImageBrush();
            rougecorail1.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/coral.jpg"));
            corail_rouge1.Fill = rougecorail1;

            ImageBrush rougecorail2 = new ImageBrush();
            rougecorail2.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/coral2.jpg"));
            corail_rouge2.Fill = rougecorail2;

            ImageBrush rougecorail3 = new ImageBrush();
            rougecorail3.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/coral4.jpg"));
            corail_rouge3.Fill = rougecorail3;

            ImageBrush vertecorail1 = new ImageBrush();
            vertecorail1.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/seaweed3.jpg"));
            corail_vert1.Fill = vertecorail1;

            ImageBrush vertecorail2 = new ImageBrush();
            vertecorail2.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/coral6.jpg"));
            corail_vert2.Fill = vertecorail2;

            ImageBrush jaunecorail = new ImageBrush();
            jaunecorail.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/coral3.jpg"));
            corail_jaune.Fill = jaunecorail;

            ImageBrush algue_vert = new ImageBrush();
            algue_vert.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/seaweed2.jpg"));
            algue1.Fill = algue_vert;
            algue2.Fill = algue_vert;
            algue3.Fill = algue_vert;
            algue4.Fill = algue_vert;

            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "poisson")
                {
                    ImageBrush nemo = new ImageBrush();
                    nemo.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/nemo.jpg"));
                    x.Fill = nemo;
                }
            }
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
                        modePuissant = true;
                        modePuissantCompteur=200;
                        x.Visibility = Visibility.Hidden;
                    }
                }
                if ((string)x.Tag == "mur")
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
        private void DeplacerPieuvre()
        {
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if (x.Name.ToString() == "rosePieuvre")
                {
                    if (Canvas.GetTop(x) == 404)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemie);
                        if (Canvas.GetLeft(x) > 700)
                        {
                            Canvas.SetTop(x, 405);
                        }
                    }
                    if (Canvas.GetTop(x) == 405)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseEnnemie);
                        if (Canvas.GetLeft(x) < 115)
                        {
                            Canvas.SetTop(x, 404);
                        }

                    }

                }
                if (x.Name.ToString() == "violetPieuvre")
                {
                    if (Canvas.GetTop(x) == 18)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemie);
                        if (Canvas.GetLeft(x) > 700)
                        {
                            Canvas.SetTop(x, 19);
                        }
                    }
                    if (Canvas.GetTop(x) == 19)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseEnnemie);
                        if (Canvas.GetLeft(x) < 115)
                        {
                            Canvas.SetTop(x, 18);
                        }

                    }

                }
                if (x.Name.ToString() == "orangePieuvre")
                {
                    if (Canvas.GetTop(x) == 120)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesse);
                        if (Canvas.GetLeft(x) < 115)
                        {
                            Canvas.SetTop(x, 119);
                        }
                    }
                    if (Canvas.GetTop(x) == 119)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesse);
                        if (Canvas.GetLeft(x) > 700)
                        {
                            Canvas.SetTop(x, 120);
                        }

                    }

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
                    if (requinHitBox.IntersectsWith(hitBox) && modePuissant == false)
                    {
                        gameTimer.Stop();
                        jeu_termine = true;
                    }
                    if (requinHitBox.IntersectsWith(hitBox) && modePuissant == true)
                    {
                        dissolvantObjets.Add(x);
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
        private void BoucleJeu(object sender, EventArgs e)
        {
            foreach (Rectangle y in dissolvantObjets)
            {
                MyCanvas.Children.Remove(y);
            }

            treasureImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "/images/treasurebox.jpg"));
            txtScore.Content = "Score: " + score + "\nCliquer P pour mettre le jeu en pause et C pour continuer";

            DeplacerRequin();
            DeplacerPieuvre();

            ////////////////////////////////////////////  ANIMATION REQUIN
            switch (imageRequin)
            {
                case 1:
                case 2:
                case 3:
                    requinImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/shark_chomp-ani1.png"));
                    break;
                case 4:
                case 5:
                case 6:
                    requinImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/shark_chomp-ani2.png"));
                    break;
                case 7:
                case 8:
                case 9:
                    requinImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/shark_chomp-ani3.png"));
                    break;
            }
            pacman.Fill = requinImage;
            imageRequin++;
            if (imageRequin > 9)
            {
                imageRequin = 1;
            }

            //////////////////////////////////////////  ANIMATION PIEUVRE ROSE
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

            //////////////////////////////////////////  ANIMATION PIEUVRE VIOLET
            switch (imagePieuvre2)
            {
                case 1:
                case 2:
                    ennemieViolet.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/pieuvreViolet01.png"));
                    break;
                case 3:
                case 4:
                    ennemieViolet.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/pieuvreViolet02.png"));
                    break;
                case 5:
                case 6:
                    ennemieViolet.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/pieuvreViolet03.png"));
                    break;
                case 7:
                case 8:
                    ennemieViolet.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/pieuvreViolet04.png"));
                    break;
                case 9:
                case 10:
                    ennemieViolet.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/pieuvreViolet05.png"));
                    break;
                case 11:
                case 12:
                    ennemieViolet.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/pieuvreViolet06.png"));
                    break;
            }
            violetPieuvre.Fill = ennemieViolet;
            imagePieuvre2++;
            if (imagePieuvre2 > 12)
            {
                imagePieuvre2 = 1;
            }

            //////////////////////////////////////////  ANIMATION PIEUVRE ORANGE
            switch(imagePieuvre3)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                    ennemieOrange.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/poulpeOrange01.png"));
                    break;
                case 7:
                case 8:
                    ennemieOrange.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/poulpeOrange02.png"));
                    break;
                case 9:
                case 10:
                    ennemieOrange.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/poulpeOrange03.png"));
                    break;
                case 11:
                case 12:
                    ennemieOrange.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/poulpeOrange04.png"));
                    break;
            }
                orangePieuvre.Fill = ennemieOrange;
                imagePieuvre3++;
                if (imagePieuvre3 > 12)
                {
                    imagePieuvre3 = 1;
                }

                /*******************************    ModePuissant    ******************************/
                if (modePuissant == true)
            {
                vitesse = 9;
                vitesseEnnemie = 2;
                modePuissantCompteur -= 1;
                if (modePuissantCompteur < 1)
                {
                    vitesse = 7;
                    vitesseEnnemie = 10;
                    modePuissant = false;
                }
            }
            //*******************************    BONUS    ******************************/
            if (score == 16)
            {
                for (int i = 0; i < 5; i++)
                {
                    Rectangle rec = new Rectangle()
                    {
                        Width = 30,
                        Height = 30,
                        Fill = treasureImage,
                        StrokeThickness = 2,
                        Tag = "bonus",
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
                        Fill = treasureImage,
                        StrokeThickness = 2,
                        Tag = "bonus",
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
                        Fill = treasureImage,
                        StrokeThickness = 2,
                        Tag = "bonus",
                    };
                    MyCanvas.Children.Add(rec);
                    Canvas.SetTop(rec, 50);
                    Canvas.SetLeft(rec, 389);
                }
            }
            if (score == 85)
            {
                JeuTermine("Vous avez gagné ! \nVous avez mangé tous les poissons !");
            }
            if (jeu_termine)
            {
                mediaElement.Close();
                txtScore.Content += "\n\n\nCliquer R \npour Réessayer";   
            }
        }
        private void JeuTermine(string message)
        {
            gameTimer.Stop();
            MessageBox.Show(message, "Chasse Aquatique Pac-Requin");
        }
    }
}

