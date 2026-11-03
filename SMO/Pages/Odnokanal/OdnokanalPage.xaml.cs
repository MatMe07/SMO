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

namespace SMO.Pages.Odnokanal
{
    /// <summary>
    /// Логика взаимодействия для OdnokanalPage.xaml
    /// </summary>
    public partial class OdnokanalPage : Page
    {
        private string _type_SMO;
        public OdnokanalPage(string type_SMO)
        {
            _type_SMO = type_SMO;
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
