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

namespace TestGithubWpf
{
    /// <summary>
    /// Логика взаимодействия для NiveauDialogue.xaml
    /// </summary>
    public partial class NiveauDialogue : Window
    {

        private NiveauFacile niveauFacile = new NiveauFacile();
        private NiveauNormale niveauNormal = new NiveauNormale();
        private NiveauDifficile niveauDifficile = new NiveauDifficile();
        public NiveauDialogue()
        {
            InitializeComponent();
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "sound/sea.wav");
            mediaElement.Source = uri;
            mediaElement.Play();
        }
        public string Combo_Box() 
        {
            string combo = ComboBox.SelectedValue.ToString();
            return combo;
            
        }
        private void Button1_Click(object sender, RoutedEventArgs e)
        {
          if (ComboBoxItem1.IsSelected)
            {
                this.Hide();
                niveauFacile.Show();
                niveauFacile.Owner = this;
                
            }
            if (ComboBoxItem2.IsSelected)
            {
                this.Hide();
                niveauNormal.Show();
                niveauNormal.Owner = this;
                Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "sound/idol.wav");
                mediaElement.Source = uri;
                mediaElement.Play();
            }
            if (ComboBoxItem3.IsSelected)
            {
                this.Hide();
                niveauDifficile.Show();
                niveauDifficile.Owner = this;
                Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "sound/dionysus.wav");
                mediaElement.Source = uri;
                mediaElement.Play();
            }

        }
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            App.Current.Shutdown();
        }

        private void ButtonMusique_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "sound/sea.wav");
            mediaElement.Source = uri;
            mediaElement.Play();
        }
    }
}
