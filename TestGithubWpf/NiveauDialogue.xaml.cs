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

        private Facile niveauFacile = new Facile();
        private Normale niveauNormal = new Normale();
        private Difficile niveauDifficile = new Difficile();
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
               
                mediaElement.Close();
            }
            if (ComboBoxItem2.IsSelected)
            {
                this.Hide();
                niveauNormal.Show();
                niveauNormal.Owner = this;

                mediaElement.Close();
            }
            if (ComboBoxItem3.IsSelected)
            {
                this.Hide();
                niveauDifficile.Show();
                niveauDifficile.Owner = this;

                mediaElement.Close();
            }
            
        }
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            App.Current.Shutdown();
        }
    }
}
