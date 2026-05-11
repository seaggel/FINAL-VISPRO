using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CrossPlatformApp.Views;
using CrossPlatformApp.Services;

namespace CrossPlatformApp.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        AuthSession.SignOut();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void LoginButton_Click(object? sender, RoutedEventArgs e)
    {
        var username = this.FindControl<TextBox>("UsernameBox")?.Text?.Trim() ?? string.Empty;
        var password = this.FindControl<TextBox>("PasswordBox")?.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            SetStatus("Isi data.");
            return;
        }

        try
        {
            var repo = RepositoryFactory.Create();
            var result = repo.AuthenticateUser(username, password);
            if (!result.Success || string.IsNullOrWhiteSpace(result.Role))
            {
                SetStatus("Gagal login.");
                return;
            }

            var role = result.Role;
            AuthSession.SignIn(new AuthenticatedUser(username, role, result.MahasiswaId, result.KonselorId));

            Window nextWindow = role switch
            {
                "Admin" => new AdminWindow(),
                "Konselor" => new KonselorWindow(),
                _ => new MahasiswaWindow()
            };

            nextWindow.Show();
            Close();
        }
        catch (System.Exception)
        {
            SetStatus("Gagal login.");
        }
    }

    private void RegisterButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var reg = new RegistrationWindow();
            reg.ShowDialog(this);
        }
        catch (System.Exception)
        {
            SetStatus("Gagal membuka daftar.");
        }
    }

    private void SetStatus(string message)
    {
        var statusText = this.FindControl<TextBlock>("StatusText");
        if (statusText != null)
        {
            statusText.Text = message;
        }
    }
}
