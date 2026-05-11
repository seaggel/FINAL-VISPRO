using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CrossPlatformApp.Models;
using CrossPlatformApp.Services;
using CrossPlatformApp.ViewModels;
using System.Linq;

namespace CrossPlatformApp.Views;

public partial class AdminWindow : Window
{
    private MainViewModel? viewModel;

    public AdminWindow()
    {
        InitializeComponent();
        LoadData();
        UpdateSessionText();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void LoadData()
    {
        try
        {
            viewModel = new MainViewModel();
            DataContext = viewModel;
            BindLists();
            UpdateSummary();
            SetStatus(viewModel.StatusMessage);
        }
        catch (System.Exception)
        {
            SetStatus("Gagal memuat data.");
        }
    }

    private void Refresh_Click(object? sender, RoutedEventArgs e)
    {
        ExecuteSafely("Gagal memuat data.", () =>
        {
            if (viewModel == null)
            {
                LoadData();
                UpdateSessionText();
                return;
            }

            viewModel.ReloadData();
            BindLists();
            UpdateSummary();
            SetStatus("Data dimuat.");
        });
    }

    private void Logout_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            AuthSession.SignOut();
            new LoginWindow().Show();
            Close();
        }
        catch (System.Exception)
        {
            SetStatus("Gagal membuka login.");
        }
    }

    private void AdminAddStudentButton_Click(object? sender, RoutedEventArgs e)
    {
        ExecuteSafely("Gagal menambah mahasiswa.", () =>
        {
            if (viewModel == null) return;

            var nim = Text("AdminStudentNimTextBox");
            var nama = Text("AdminStudentNamaTextBox");
            var program = Text("AdminStudentProgramTextBox");

            if (string.IsNullOrWhiteSpace(nim) || string.IsNullOrWhiteSpace(nama) || string.IsNullOrWhiteSpace(program))
            {
                SetStatus("Isi data mahasiswa.");
                return;
            }

            viewModel.AddMahasiswa(nim, nama, program);
            BindLists();
            UpdateSummary();
            ClearStudentForm();
            SetStatus(viewModel.StatusMessage);
        });
    }

    private void AdminUpdateStudentButton_Click(object? sender, RoutedEventArgs e)
    {
        ExecuteSafely("Gagal mengubah mahasiswa.", () =>
        {
            if (viewModel == null) return;

            var selected = GetListBox("MahasiswaListBox")?.SelectedItem as Mahasiswa;
            if (selected == null)
            {
                SetStatus("Pilih mahasiswa.");
                return;
            }

            viewModel.UpdateMahasiswa(selected, Text("AdminStudentNimTextBox"), Text("AdminStudentNamaTextBox"), Text("AdminStudentProgramTextBox"));
            BindLists();
            UpdateSummary();
            SetStatus(viewModel.StatusMessage);
        });
    }

    private void AdminDeleteStudentButton_Click(object? sender, RoutedEventArgs e)
    {
        ExecuteSafely("Gagal menghapus mahasiswa.", () =>
        {
            if (viewModel == null) return;

            var selected = GetListBox("MahasiswaListBox")?.SelectedItem as Mahasiswa;
            if (selected == null)
            {
                SetStatus("Pilih mahasiswa.");
                return;
            }

            viewModel.DeleteMahasiswa(selected);
            BindLists();
            UpdateSummary();
            ClearStudentForm();
            SetStatus(viewModel.StatusMessage);
        });
    }

    private void AdminAddCounselorButton_Click(object? sender, RoutedEventArgs e)
    {
        ExecuteSafely("Gagal menambah konselor.", () =>
        {
            if (viewModel == null) return;

            var kode = Text("AdminCounselorKodeTextBox");
            var nama = Text("AdminCounselorNamaTextBox");
            var jabatan = Text("AdminCounselorJabatanTextBox");

            if (string.IsNullOrWhiteSpace(kode) || string.IsNullOrWhiteSpace(nama) || string.IsNullOrWhiteSpace(jabatan))
            {
                SetStatus("Isi data konselor.");
                return;
            }

            viewModel.AddKonselor(kode, nama, jabatan);
            BindLists();
            UpdateSummary();
            ClearCounselorForm();
            SetStatus(viewModel.StatusMessage);
        });
    }

    private void AdminUpdateCounselorButton_Click(object? sender, RoutedEventArgs e)
    {
        ExecuteSafely("Gagal mengubah konselor.", () =>
        {
            if (viewModel == null) return;

            var selected = GetListBox("KonselorListBox")?.SelectedItem as Konselor;
            if (selected == null)
            {
                SetStatus("Pilih konselor.");
                return;
            }

            viewModel.UpdateKonselor(selected, Text("AdminCounselorKodeTextBox"), Text("AdminCounselorNamaTextBox"), Text("AdminCounselorJabatanTextBox"));
            BindLists();
            UpdateSummary();
            SetStatus(viewModel.StatusMessage);
        });
    }

    private void AdminDeleteCounselorButton_Click(object? sender, RoutedEventArgs e)
    {
        ExecuteSafely("Gagal menghapus konselor.", () =>
        {
            if (viewModel == null) return;

            var selected = GetListBox("KonselorListBox")?.SelectedItem as Konselor;
            if (selected == null)
            {
                SetStatus("Pilih konselor.");
                return;
            }

            viewModel.DeleteKonselor(selected);
            BindLists();
            UpdateSummary();
            ClearCounselorForm();
            SetStatus(viewModel.StatusMessage);
        });
    }

    private void MahasiswaListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        ExecuteSafely("Gagal memilih mahasiswa.", () =>
        {
            if (GetListBox("MahasiswaListBox")?.SelectedItem is Mahasiswa selected)
            {
                SetTextBox("AdminStudentNimTextBox", selected.Nim);
                SetTextBox("AdminStudentNamaTextBox", selected.Nama);
                SetTextBox("AdminStudentProgramTextBox", selected.ProgramStudi);
                SetTextBlock("SelectedStudentInfo", $"NIM: {selected.Nim}\nNama: {selected.Nama}\nProgram: {selected.ProgramStudi}\nAngkatan: {selected.Angkatan}");
                return;
            }

            SetTextBlock("SelectedStudentInfo", "Pilih data.");
        });
    }

    private void KonselorListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        ExecuteSafely("Gagal memilih konselor.", () =>
        {
            if (GetListBox("KonselorListBox")?.SelectedItem is Konselor selected)
            {
                SetTextBox("AdminCounselorKodeTextBox", selected.KodeKonselor);
                SetTextBox("AdminCounselorNamaTextBox", selected.Nama);
                SetTextBox("AdminCounselorJabatanTextBox", selected.Jabatan);
                SetTextBlock("SelectedCounselorInfo", $"Kode: {selected.KodeKonselor}\nNama: {selected.Nama}\nJabatan: {selected.Jabatan}\nBidang: {selected.BidangKeahlian}\nAktif: {selected.Aktif}");
                return;
            }

            SetTextBlock("SelectedCounselorInfo", "Pilih data.");
        });
    }

    private void SesiListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        ExecuteSafely("Gagal memilih sesi.", () =>
        {
            if (GetListBox("SesiListBox")?.SelectedItem is SesiKonseling selected)
            {
                SetTextBlock("SelectedSessionInfo", BuildSessionInfo(selected));
                return;
            }

            SetTextBlock("SelectedSessionInfo", "Pilih data.");
        });
    }

    private void BindLists()
    {
        if (viewModel == null) return;

        var mahasiswaListBox = GetListBox("MahasiswaListBox");
        if (mahasiswaListBox != null)
        {
            mahasiswaListBox.ItemsSource = viewModel.MahasiswaList;
            if (mahasiswaListBox.SelectedIndex < 0 && viewModel.MahasiswaList.Count > 0)
                mahasiswaListBox.SelectedIndex = 0;
        }

        var konselorListBox = GetListBox("KonselorListBox");
        if (konselorListBox != null)
        {
            konselorListBox.ItemsSource = viewModel.KonselorList;
            if (konselorListBox.SelectedIndex < 0 && viewModel.KonselorList.Count > 0)
                konselorListBox.SelectedIndex = 0;
        }

        var sesiListBox = GetListBox("SesiListBox");
        if (sesiListBox != null)
        {
            sesiListBox.ItemsSource = viewModel.SesiList;
            if (sesiListBox.SelectedIndex < 0 && viewModel.SesiList.Count > 0)
                sesiListBox.SelectedIndex = 0;
        }
    }

    private void UpdateSummary()
    {
        if (viewModel == null) return;

        SetTextBlock("MahasiswaCountText", viewModel.MahasiswaCount.ToString());
        SetTextBlock("KonselorCountText", viewModel.KonselorCount.ToString());
        SetTextBlock("SesiCountText", viewModel.SesiCount.ToString());
    }

    private void UpdateSessionText()
    {
        var sessionText = this.FindControl<TextBlock>("SessionText");
        if (sessionText != null)
        {
            sessionText.Text = AuthSession.CurrentUser is { } user
                ? $"Session: {user.Username} ({user.Role})"
                : "Session: -";
        }
    }

    private void SetStatus(string message)
    {
        SetTextBlock("StatusText", message);
    }

    private string BuildSessionInfo(SesiKonseling sesi)
    {
        var mahasiswa = viewModel?.MahasiswaList.FirstOrDefault(item => item.Id == sesi.MahasiswaId);
        var konselor = viewModel?.KonselorList.FirstOrDefault(item => item.Id == sesi.KonselorId);
        return $"Kode: {sesi.KodeSesi}\nTanggal: {sesi.TanggalSesi:yyyy-MM-dd}\nMahasiswa: {(mahasiswa != null ? mahasiswa.Nama : sesi.MahasiswaId.ToString())}\nKonselor: {(konselor != null ? konselor.Nama : sesi.KonselorId.ToString())}\nStatus: {sesi.Status}\nTopik: {sesi.Topik}";
    }

    private void ClearStudentForm()
    {
        SetTextBox("AdminStudentNimTextBox", string.Empty);
        SetTextBox("AdminStudentNamaTextBox", string.Empty);
        SetTextBox("AdminStudentProgramTextBox", string.Empty);
        SetTextBox("SelectedStudentInfo", "Pilih data.");
    }

    private void ClearCounselorForm()
    {
        SetTextBox("AdminCounselorKodeTextBox", string.Empty);
        SetTextBox("AdminCounselorNamaTextBox", string.Empty);
        SetTextBox("AdminCounselorJabatanTextBox", string.Empty);
        SetTextBox("SelectedCounselorInfo", "Pilih data.");
    }

    private void SetTextBox(string name, string? value)
    {
        var control = this.FindControl<TextBox>(name);
        if (control != null)
            control.Text = value ?? string.Empty;
    }

    private void SetTextBlock(string name, string? value)
    {
        var control = this.FindControl<TextBlock>(name);
        if (control != null)
            control.Text = value ?? string.Empty;
    }

    private void ExecuteSafely(string fallbackMessage, System.Action action)
    {
        try
        {
            action();
        }
        catch (System.Exception ex)
        {
            SetStatus(fallbackMessage + " " + GetShortMessage(ex));
        }
    }

    private static string GetShortMessage(System.Exception ex)
    {
        var message = ex.GetBaseException().Message;
        return string.IsNullOrWhiteSpace(message) ? "Terjadi kesalahan." : message;
    }

    private string Text(string name) => this.FindControl<TextBox>(name)?.Text?.Trim() ?? string.Empty;

    private TextBox? GetTextBox(string name) => this.FindControl<TextBox>(name);

    private ListBox? GetListBox(string name) => this.FindControl<ListBox>(name);
}
