 using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        int modePuissantCompteur = 200;
        int imageRequin = 1;
        int imageTorche = 1;
        bool modePuissant = false;
        bool jeu_termine = false;
        bool gagne = false;
        bool estJeuEnPause = false;
        List<Rectangle> dissolvantObjets = new List<Rectangle>();
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
            if (e.Key == Key.R && jeu_termine)
            {
                CommencerJeu();
            }

            if (e.Key == Key.Left)
            {
                vaGauche = true;
                //requin.RenderTransform = new RotateTransform(-180, requin.Width / 2, requin.Height / 2);
                requin.RenderTransformOrigin = new Point(0.5, 0.2);
                ScaleTransform flipTrans = new ScaleTransform();
                flipTrans.ScaleX = -1;
                requin.RenderTransform = flipTrans;
                vaDroite = false;
                vaEnHaut = false;
                vaEnBas = false;
            }
            if (e.Key == Key.Right)
            {
                vaDroite = true;
                requin.RenderTransform = new RotateTransform(0, requin.Width / 2, requin.Height / 2);
                vaGauche = false;
                vaEnHaut = false;
                vaEnBas = false;

            }
            if (e.Key == Key.Up)
            {
                if (!vaEnHaut && !vaEnBas)
                {
                    Canvas.SetLeft(requin, Canvas.GetLeft(requin) - 7);
                }
                vaEnHaut = true;
                requin.RenderTransform = new RotateTransform(-90, requin.Width / 2, requin.Height / 2);
                vaDroite = false;
                vaGauche = false;
                vaEnBas = false;


            }
            if (e.Key == Key.Down)
            {
                if (!vaEnHaut && !vaEnBas)
                {
                    Canvas.SetLeft(requin, Canvas.GetLeft(requin) - 7);
                }
                vaEnBas = true;
                requin.RenderTransform = new RotateTransform(90, requin.Width / 2, requin.Height / 2);
                vaDroite = false;
                vaGauche = false;
                vaEnHaut = false;
            }
        }
        private void ConfigurationJeu()
        {
            MyCanvas.Focus();
            gameTimer.Tick += BoucleJeu;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();
            actuellePieuvrePas = mouvementPieuvre;

            ImageBrush porte1 = new ImageBrush();
            porte1.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/porte.png"));
            porte.Fill = porte1;

            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "mur")
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
                //requinHitBox = new Rect(Canvas.GetLeft(requin), Canvas.GetTop(requin), requin.Width, requin.Height);
        }
        private void DeplacerRequin()
        {
            if (vaDroite && Canvas.GetLeft(requin) < Application.Current.MainWindow.Width - 60)
            {
                Canvas.SetLeft(requin, Canvas.GetLeft(requin) + vitesse);
            }
            if (vaGauche && Canvas.GetLeft(requin) > 20)
            {
                Canvas.SetLeft(requin, Canvas.GetLeft(requin) - vitesse);
            }
            if (vaEnHaut && Canvas.GetTop(requin) > 20)
            {
                Canvas.SetTop(requin, Canvas.GetTop(requin) - vitesse);
            }
            if (vaEnBas && Canvas.GetTop(requin) < Application.Current.MainWindow.Height - 60)
            {
                Canvas.SetTop(requin, Canvas.GetTop(requin) + vitesse);
            }
  
            requinHitBox = new Rect(Canvas.GetLeft(requin), Canvas.GetTop(requin), requin.Width, requin.Height);
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                if ((string)x.Tag == "bonus")
                {
                    if (requinHitBox.IntersectsWith(hitBox) && x.Visibility == Visibility.Visible)
                    {
                        modePuissant = true;
                        modePuissantCompteur = 200;
                        x.Visibility = Visibility.Hidden;
                    }
                }

                if ((string)x.Tag == "mur")
                {
                    if (requinHitBox.IntersectsWith(hitBox))
                    {
                        if (vaDroite)
                        {
                            Canvas.SetLeft(requin, Canvas.GetLeft(requin) - vitesse);
                            vaDroite = false;
                        }

                        if (vaGauche)
                        {
                            Canvas.SetLeft(requin, Canvas.GetLeft(requin) + vitesse);
                            vaGauche = false;
                        }

                        if (vaEnHaut)
                        {
                            Canvas.SetTop(requin, Canvas.GetTop(requin) + vitesse);
                            vaEnHaut = false;
                        }

                        if (vaEnBas)
                        {
                            Canvas.SetTop(requin, Canvas.GetTop(requin) - vitesse);
                            vaEnBas = false;
                        }
                    }
                }

            }
        }
        private void CommencerJeu()
        {
            actuellePieuvrePas = mouvementPieuvre;

            Canvas.SetLeft(requin, 22);
            Canvas.SetTop(requin, 544);

            gameTimer.Start();
            score = 0;
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if (x.Name.ToString() == "rosePieuvre")
                {
                    if (Canvas.GetTop(x) == 275)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemie);
                        if (Canvas.GetLeft(x) > 480)
                        {
                            Canvas.SetLeft(x, 478);

                        }
                    }
                    if (Canvas.GetLeft(x) == 478)
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) - vitesseEnnemie);
                        if (Canvas.GetTop(x) < 146)
                        {
                            Canvas.SetTop(x, 147);
                        }

                    }
                    if (Canvas.GetTop(x) == 147)
                    {
                        Canvas.SetLeft(x, 481);
                    }
                    if (Canvas.GetLeft(x) == 481)
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) + vitesseEnnemie);
                        if (Canvas.GetTop(x) > 276)
                        {
                            Canvas.SetLeft(x, 482);
                        }
                    }
                    if (Canvas.GetLeft(x) == 482)
                    {
                        Canvas.SetTop(x, 278);
                    }
                    if (Canvas.GetTop(x) == 278)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseEnnemie);
                        if (Canvas.GetLeft(x) < 20)
                        {
                            Canvas.SetTop(x, 275);
                        }
                    }

                }
                if (x.Name.ToString() == "orangePieuvre")
                {
                    if (Canvas.GetTop(x) == 37)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemie);
                        if (Canvas.GetLeft(x) > 735)
                        {
                            Canvas.SetLeft(x, 736);

                        }
                    }
                    if (Canvas.GetLeft(x) == 736)
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) + vitesseEnnemie);
                        if (Canvas.GetTop(x) > 540)
                        {
                            Canvas.SetTop(x, 533);
                        }

                    }
                    if (Canvas.GetTop(x) == 533)
                    {
                        Canvas.SetLeft(x, 737);
                    }
                    if (Canvas.GetLeft(x) == 737)
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) - vitesseEnnemie);
                        if (Canvas.GetTop(x) < 40)
                        {
                            Canvas.SetLeft(x, 738);
                        }
                    }
                    if (Canvas.GetLeft(x) == 738)
                    {
                        Canvas.SetTop(x, 39);
                    }
                    if (Canvas.GetTop(x) == 39)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseEnnemie);
                        if (Canvas.GetLeft(x) < 20)
                        {
                            Canvas.SetTop(x, 37);
                        }
                    }
                }
            }
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                Rect hitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                requinHitBox = new Rect(Canvas.GetLeft(requin), Canvas.GetTop(requin), requin.Width, requin.Height);

                if ((string)x.Tag == "meduses")
                {
                    if (x.Visibility == Visibility.Hidden)
                    {
                        x.Visibility = Visibility.Visible;
                    }
                }
                if ((string)x.Tag == "fin")
                {
                    if (requinHitBox.IntersectsWith(hitBox) && gagne == true)
                    {
                        x.Visibility = Visibility.Hidden;
                        JeuTermine("Vous avez gagne");
                    }
                }
                if ((string)x.Tag == "bonus" && x.Visibility == Visibility.Visible)
                {
                    x.Visibility = Visibility.Hidden;
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

        private void DeplacerPieuvre()
        {
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if (x.Name.ToString() == "rosePieuvre")
                {
                    if (Canvas.GetTop(x) == 275)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemie);
                        if (Canvas.GetLeft(x) > 480)
                        {
                            Canvas.SetLeft(x, 478);
                        }
                    }
                    if (Canvas.GetLeft(x) == 478)
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) - vitesseEnnemie);
                        if (Canvas.GetTop(x) < 146)
                        {
                            Canvas.SetTop(x, 147);
                        }
                    }
                    if (Canvas.GetTop(x) == 147)
                    {
                        Canvas.SetLeft(x, 481);
                    }
                    if (Canvas.GetLeft(x) == 481)
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) + vitesseEnnemie);
                        if (Canvas.GetTop(x) > 276)
                        {
                            Canvas.SetLeft(x, 482);
                        }
                    }
                    if (Canvas.GetLeft(x) == 482)
                    {
                        Canvas.SetTop(x, 278);
                    }
                    if (Canvas.GetTop(x) == 278)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseEnnemie);
                        if (Canvas.GetLeft(x) < 20)
                        {
                            Canvas.SetTop(x, 275);
                        }
                    }

                }
                if (x.Name.ToString() == "orangePieuvre")
                {
                    if (Canvas.GetTop(x) == 37)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemie);
                        if (Canvas.GetLeft(x) > 735)
                        {
                            Canvas.SetLeft(x, 736);

                        }
                    }
                    if (Canvas.GetLeft(x) == 736)
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) + vitesseEnnemie);
                        if (Canvas.GetTop(x) > 540)
                        {
                            Canvas.SetTop(x, 533);
                        }

                    }
                    if (Canvas.GetTop(x) == 533)
                    {
                        Canvas.SetLeft(x, 737); 
                    }
                    if (Canvas.GetLeft(x) == 737)
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) - vitesseEnnemie);
                        if (Canvas.GetTop(x) < 40)
                        {
                            Canvas.SetLeft(x, 738);
                        }
                    }
                    if (Canvas.GetLeft(x) == 738)
                    {
                        Canvas.SetTop(x, 39);
                    }
                    if (Canvas.GetTop(x) == 39)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseEnnemie);
                        if (Canvas.GetLeft(x) < 20)
                        {
                            Canvas.SetTop(x, 37);
                        }
                    }  
                }
                if (x.Name.ToString() == "violetPieuvre")
                {
                    if (Canvas.GetTop(x) == 146)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemie);
                        if (Canvas.GetLeft(x) > 625)
                        {
                            Canvas.SetLeft(x, 626);

                        }
                    }
                    if (Canvas.GetLeft(x) == 626)
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) + vitesseEnnemie);
                        if (Canvas.GetTop(x) > 420)
                        {
                            Canvas.SetTop(x, 419);
                        }

                    }
                    if (Canvas.GetTop(x) == 419)
                    {
                        Canvas.SetLeft(x, 627);
                    }
                    if (Canvas.GetLeft(x) == 627)
                    {
                        Canvas.SetTop(x, Canvas.GetTop(x) - vitesseEnnemie);
                        if (Canvas.GetTop(x) < 140)
                        {
                            Canvas.SetLeft(x, 628);
                        }
                    }
                    if (Canvas.GetLeft(x) == 628)
                    {
                        Canvas.SetTop(x, 139);
                    }
                    if (Canvas.GetTop(x) == 139)
                    {
                        Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseEnnemie);
                        if (Canvas.GetLeft(x) < 120)
                        {
                            Canvas.SetTop(x, 146);
                        }
                    }
                }
            }
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
                        JeuTermine("Vous avez gagne");
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
                }

            }

        }
        private void BoucleJeu(object sender, EventArgs e)
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
            requin.Fill = requinImage;
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
                    torcheImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/torche_1.png"));
                    break;
                case 4:
                case 5:
                case 6:
                    torcheImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/torche_2.png"));
                    break;
                case 7:
                case 8:
                case 9:
                    torcheImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/torche_3.png"));
                    break;
                case 10:
                case 11:
                case 12:
                    torcheImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/torche_4.png"));
                    break;
                case 13:
                case 14:
                case 15:
                    torcheImage.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/torche_1.png"));
                    break;
            }
            torche.Fill = torcheImage;
            imageTorche++;
            torche_1.Fill = torcheImage;
            if (imageTorche > 15)
            {
                imageTorche = 1;
            }

            if (score == 15)
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
            if (score == 35)
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
            if (score == 55)
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
