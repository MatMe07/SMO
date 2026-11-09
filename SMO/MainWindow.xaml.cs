using SMO.Pages;
using System.Windows;
using System.Windows.Controls;
using Vichislenia;

namespace SMO
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem || menuItem.Tag is null)
                return;

            SmoType type = ParseType(menuItem.Tag.ToString() ?? "");
            mainFrame.Navigate(new SmoPage(type));
        }

        private static SmoType ParseType(string tag) => tag switch
        {
            "Odno_s_otkaz" => SmoType.SingleRefusals,
            "Odno_s_ogranich" => SmoType.SingleLimitedQueue,
            "Odno_s_neogra" => SmoType.SingleUnlimitedQueue,
            "Mnogo_s_otkaz" => SmoType.MultiRefusals,
            "Mnogo_s_ogranich" => SmoType.MultiLimitedQueue,
            "Mnogo_s_neogra" => SmoType.MultiUnlimitedQueue,
            _ => throw new ArgumentOutOfRangeException(nameof(tag), tag, "Неизвестный тип СМО")
        };
    }
}
