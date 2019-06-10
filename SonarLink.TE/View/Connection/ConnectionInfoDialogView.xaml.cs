using Microsoft.VisualStudio.PlatformUI;
using System.Windows;
using System.Windows.Controls;

namespace SonarLink.TE.View
{

    /// <summary>
    /// Interaction logic for ConnectionInfoDialogView.xaml
    /// </summary>
    public partial class ConnectionInfoDialogView : DialogWindow
    {
        public ConnectionInfoDialogView()
        {
            InitializeComponent();
        }

        private void ConnectButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void PasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Reference: https://stackoverflow.com/a/25001115

            if (DataContext != null)
            {
                ((dynamic)DataContext).Password = ((PasswordBox)sender).Password;
            }
        }
    }

}
