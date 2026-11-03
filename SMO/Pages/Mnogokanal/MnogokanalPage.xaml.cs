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

namespace SMO.Pages.Mnogokanal
{
    /// <summary>
    /// Логика взаимодействия для MnogokanalPage.xaml
    /// </summary>
    public partial class MnogokanalPage : Page
    {
        private string _type_SMO;
        public MnogokanalPage(string type_SMO)
        {
            _type_SMO = type_SMO;
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
