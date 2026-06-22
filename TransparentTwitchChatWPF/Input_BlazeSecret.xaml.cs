using System.Windows;

namespace TransparentTwitchChatWPF;

public partial class Input_BlazeSecret : Window
{
    public string Secret { get; private set; } = string.Empty;

    public Input_BlazeSecret()
    {
        InitializeComponent();
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
        pbSecret.Focus();
    }

    private void OKButton_Click(object sender, RoutedEventArgs e)
    {
        Secret = pbSecret.Password;
        DialogResult = true;
    }
}
