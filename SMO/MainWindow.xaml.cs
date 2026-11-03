using SMO.Pages.Mnogokanal;
using SMO.Pages.Odnokanal;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SMO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string tag_ = (sender as MenuItem).Tag.ToString() ?? "" ;
            mainFrame.Navigate(new OdnokanalPage());
        }

        private void MenuItemOtkaz_Click(object sender, RoutedEventArgs e)
        {
            string tag_ = (sender as MenuItem).Tag.ToString() ?? "";
            mainFrame.Navigate(new MnogokanalPage());
        }
    }
}