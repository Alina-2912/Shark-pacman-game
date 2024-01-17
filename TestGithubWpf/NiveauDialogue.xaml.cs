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
                MainWindow mw = new MainWindow();
                mw.Show();
                this.Close();
                mediaElement.Close();
            }
            if (ComboBoxItem2.IsSelected)
            {
                Normale mw = new Normale();
                mw.Show();
                this.Close();
                mediaElement.Close();
            }
            if (ComboBoxItem3.IsSelected)
            {
                Difficile mw = new Difficile();
                mw.Show();
                this.Close();
                mediaElement.Close();
            }
            
        }
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
