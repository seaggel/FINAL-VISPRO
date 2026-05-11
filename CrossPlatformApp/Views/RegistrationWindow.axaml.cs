using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CrossPlatformApp.Models;
using CrossPlatformApp.Services;

namespace CrossPlatformApp.Views;

public partial class RegistrationWindow : Window
{
    public RegistrationWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void RegisterButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var nim = Text("RegNim");
            var nama = Text("RegNama");
            var programStudi = Text("RegProgramStudi");
            var username = Text("RegUsername");
            var password = this.FindControl<TextBox>("RegPassword")?.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(nim) || string.IsNullOrWhiteSpace(nama) || string.IsNullOrWhiteSpace(programStudi) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                SetStatus("Isi data.");
                return;
            }

            var repo = RepositoryFactory.Create();
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            var created = repo.RegisterMahasiswaAccount(new Mahasiswa
            {
                Nim = nim,
                Nama = nama,
                ProgramStudi = programStudi
            }, username, hash);

            if (created)
            {
                SetStatus("Akun dibuat.", true);
                Close();
            }
            else
            {
                SetStatus("Gagal.");
            }
        }
        catch (System.Exception)
        {
            SetStatus("Gagal.");
        }
    }

    private string Text(string name) => this.FindControl<TextBox>(name)?.Text?.Trim() ?? string.Empty;

    private void SetStatus(string message, bool success = false)
    {
        var status = this.FindControl<TextBlock>("RegStatus");
        if (status != null)
        {
            status.Foreground = success ? Avalonia.Media.Brushes.Green : Avalonia.Media.Brushes.Red;
            status.Text = message;
        }
    }
}
