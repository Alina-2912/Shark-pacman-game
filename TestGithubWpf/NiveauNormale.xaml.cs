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
        public class DoubleAnimationUsingPathExample : Page
        {

            public DoubleAnimationUsingPathExample()
            {

                // Create a NameScope for the page so that
                // we can use Storyboards.
                NameScope.SetNameScope(this, new NameScope());

                // Create a rectangle.
                Rectangle aRectangle = new Rectangle();
                aRectangle.Width = 30;
                aRectangle.Height = 30;
                aRectangle.Fill = Brushes.Blue;

                // Create a transform. This transform
                // will be used to move the rectangle.
                TranslateTransform animatedTranslateTransform =
                    new TranslateTransform();

                // Register the transform's name with the page
                // so that they it be targeted by a Storyboard.
                this.RegisterName("AnimatedTranslateTransform", animatedTranslateTransform);

                aRectangle.RenderTransform = animatedTranslateTransform;

                // Create a Canvas to contain the rectangle
                // and add it to the page.
                Canvas mainPanel = new Canvas();
                mainPanel.Width = 400;
                mainPanel.Height = 400;
                mainPanel.Children.Add(aRectangle);
                this.Content = mainPanel;

                // Create the animation path.
                PathGeometry animationPath = new PathGeometry();
                PathFigure pFigure = new PathFigure();
                pFigure.StartPoint = new Point(10, 100);
                PolyBezierSegment pBezierSegment = new PolyBezierSegment();
                pBezierSegment.Points.Add(new Point(35, 0));
                pBezierSegment.Points.Add(new Point(135, 0));
                pBezierSegment.Points.Add(new Point(160, 100));
                pBezierSegment.Points.Add(new Point(180, 190));
                pBezierSegment.Points.Add(new Point(285, 200));
                pBezierSegment.Points.Add(new Point(310, 100));
                pFigure.Segments.Add(pBezierSegment);
                animationPath.Figures.Add(pFigure);

                // Freeze the PathGeometry for performance benefits.
                animationPath.Freeze();

                // Create a DoubleAnimationUsingPath to move the
                // rectangle horizontally along the path by animating
                // its TranslateTransform.
                DoubleAnimationUsingPath translateXAnimation =
                    new DoubleAnimationUsingPath();
                translateXAnimation.PathGeometry = animationPath;
                translateXAnimation.Duration = TimeSpan.FromSeconds(5);

                // Set the Source property to X. This makes
                // the animation generate horizontal offset values from
                // the path information.
                translateXAnimation.Source = PathAnimationSource.X;

                // Set the animation to target the X property
                // of the TranslateTransform named "AnimatedTranslateTransform".
                Storyboard.SetTargetName(translateXAnimation, "AnimatedTranslateTransform");
                Storyboard.SetTargetProperty(translateXAnimation,
                    new PropertyPath(TranslateTransform.XProperty));

                // Create a DoubleAnimationUsingPath to move the
                // rectangle vertically along the path by animating
                // its TranslateTransform.
                DoubleAnimationUsingPath translateYAnimation =
                    new DoubleAnimationUsingPath();
                translateYAnimation.PathGeometry = animationPath;
                translateYAnimation.Duration = TimeSpan.FromSeconds(5);

                // Set the Source property to Y. This makes
                // the animation generate vertical offset values from
                // the path information.
                translateYAnimation.Source = PathAnimationSource.Y;

                // Set the animation to target the Y property
                // of the TranslateTransform named "AnimatedTranslateTransform".
                Storyboard.SetTargetName(translateYAnimation, "AnimatedTranslateTransform");
                Storyboard.SetTargetProperty(translateYAnimation,
                    new PropertyPath(TranslateTransform.YProperty));

                // Create a Storyboard to contain and apply the animations.
                Storyboard pathAnimationStoryboard = new Storyboard();
                pathAnimationStoryboard.RepeatBehavior = RepeatBehavior.Forever;
                pathAnimationStoryboard.Children.Add(translateXAnimation);
                pathAnimationStoryboard.Children.Add(translateYAnimation);

                // Start the animations when the rectangle is loaded.
                aRectangle.Loaded += delegate (object sender, RoutedEventArgs e)
                {
                    // Start the storyboard.
                    pathAnimationStoryboard.Begin(this);
                };
            }
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

            Canvas.SetLeft(requin, 50);
            Canvas.SetTop(requin, 104);

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
                requinHitBox = new Rect(Canvas.GetLeft(requin), Canvas.GetTop(requin), requin.Width, requin.Height);

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
            /*foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if (x.Name.ToString() == "orangePieuvre")
                {
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - 10);

                    if (Canvas.GetLeft(x) < 50)
                    {
                        Canvas.SetLeft(orangePieuvre, 400);
                        Canvas.SetTop(orangePieuvre, 120);
                        Canvas.SetRight(x, Canvas.GetRight(x) + 400);
                    }
                }
            }
            /*foreach (var x in MyCanvas.Children.OfType<Rectangle>())
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
                    actuellePieuvrePas--;
                    if (actuellePieuvrePas < 1)
                    {
                        actuellePieuvrePas = mouvementPieuvre;
                        vitesseEnnemie = -vitesseEnnemie;
                    }
                }

            }
            //////////////////////////////
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                /*if (x is Rectangle && x.Tag == "pieuvre")
                {
                    //move zombie towards the player picture box
                    if (violetPieuvre.Left > requin.Left)
                    {
                        ((PictureBox)x).Left -= zombieSpeed; // move zombie towards the left of the player
                        ((PictureBox)x).Image = Properties.Resources.zleft; // change the zombie image to the left
                    }
                    if (((PictureBox)x).Top > player.Top)
                    {
                        ((PictureBox)x).Top -= zombieSpeed; // move zombie upwards towards the players top
                        ((PictureBox)x).Image = Properties.Resources.zup; // change the zombie picture to the top pointing image
                    }
                    if (((PictureBox)x).Left < player.Left)
                    {
                        ((PictureBox)x).Left += zombieSpeed; // move zombie towards the right of the player
                        ((PictureBox)x).Image = Properties.Resources.zright; // change the image to the right image
                    }
                    if (((PictureBox)x).Top < player.Top)
                    {
                        ((PictureBox)x).Top += zombieSpeed; // move the zombie towards the bottom of the player
                        ((PictureBox)x).Image = Properties.Resources.zdown; // change the image to the down zombie
                    }
                }
            }*/
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
        private void JeuTermine(string message)
        {
            gameTimer.Stop();
            MessageBox.Show(message, "Chasse Aquatique Pac-Requin");
        }
    }
}
