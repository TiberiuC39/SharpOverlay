using System.Windows;
using System.Windows.Controls;

namespace Presentation.Models.Views
{
    public partial class GeneralSettingsView : UserControl
    {
        public GeneralSettingsView()
        {
            InitializeComponent();
        }

        public void WindowToggle_Checked(object sender, RoutedEventArgs e)
        {
            // Logic for when the Bar Spotter is turned ON
            // e.g., showing the external window
        }

        public void WindowToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            // Logic for when the Bar Spotter is turned OFF
            // e.g., hiding/closing the external window
        }
    }
}
