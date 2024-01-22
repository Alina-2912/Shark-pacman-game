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
            Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "sound/blackswan.wav");
            mediaElement.Source = uri;
            mediaElement.Play();
        }
        public string ChoixNiveauComboBox() 
        {
            string comboboxFonctionnement = ComboBoxChoisirNiveau.SelectedValue.ToString();
            return comboboxFonctionnement;
            
        }
        private void BouttonChoisirNiveau_Click(object sender, RoutedEventArgs e)
        {
          if (premierNiveau.IsSelected)
            {
                this.Hide();
                niveauFacile.Show();
                niveauFacile.Owner = this;
                Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "sound/gogo2.wav");
                mediaElement.Source = uri;
                mediaElement.Play();
            }
            if (deuxiemeNiveau.IsSelected)
            {
                this.Hide();
                niveauNormal.Show();
                niveauNormal.Owner = this;
                Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "sound/idol.wav");
                mediaElement.Source = uri;
                mediaElement.Play();
            }
            if (troisiemeNiveau.IsSelected)
            {
                this.Hide();
                niveauDifficile.Show();
                niveauDifficile.Owner = this;
                Uri uri = new Uri(AppDomain.CurrentDomain.BaseDirectory + "sound/dionysus.wav");
                mediaElement.Source = uri;
                mediaElement.Play();
            }

        }
        private void BouttonAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            App.Current.Shutdown();
        }
    }
}
