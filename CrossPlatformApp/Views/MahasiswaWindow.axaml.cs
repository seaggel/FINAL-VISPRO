using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CrossPlatformApp.Models;
using CrossPlatformApp.Services;
using CrossPlatformApp.ViewModels;
using System.Linq;

namespace CrossPlatformApp.Views;

public partial class MahasiswaWindow : Window
{
    private static readonly string[] SessionStatuses = { "Dijadwalkan", "Berlangsung", "Selesai", "Dibatalkan" };
    private MainViewModel? viewModel;

    public MahasiswaWindow()
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
            BindSessionStatuses();
            UpdateProfile();
            BindSessionTargets();
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
            BindSessionStatuses();
            UpdateProfile();
            BindSessionTargets();
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

    private void MySessionsListBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        ExecuteSafely("Gagal memilih sesi.", () =>
        {
            if (GetListBox("MySessionsListBox")?.SelectedItem is SesiKonseling selected)
            {
                SetSelectedSessionFields(selected);
                return;
            }

            ClearSelectedSessionFields();
        });
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

    private void CreateSessionButton_Click(object? sender, RoutedEventArgs e)
    {
        if (viewModel == null)
        {
            SetStatus("Gagal memuat data.");
            return;
        }

        var konselor = GetComboBox("NewSessionKonselorComboBox")?.SelectedItem as Konselor;
        var topik = Text("NewSessionTopikTextBox");

        if (konselor == null || string.IsNullOrWhiteSpace(topik))
        {
            SetStatus("Isi data.");
            return;
        }

        try
        {
            viewModel.CreateSessionForCurrentMahasiswa(konselor.Id, topik);
            viewModel.ReloadData();
            UpdateProfile();
            BindSessionTargets();
            BindLists();
            UpdateSummary();
            ClearCreateSessionForm();
            SetStatus("Sesi dibuat.");
        }
        catch (System.Exception)
        {
            SetStatus("Gagal membuat sesi.");
        }
    }

    private void UpdateSessionButton_Click(object? sender, RoutedEventArgs e)
    {
        if (viewModel == null)
        {
            SetStatus("Gagal memuat data.");
            return;
        }

        if (GetListBox("MySessionsListBox")?.SelectedItem is not SesiKonseling selected)
        {
            SetStatus("Pilih data.");
            return;
        }

        var newTopik = Text("SelectedSessionTopikTextBox");
        var selectedDate = GetDatePicker("SelectedSessionDatePicker")?.SelectedDate;
        var selectedStartTime = GetTimePicker("SelectedSessionMulaiTimePicker")?.SelectedTime;
        var selectedEndTime = GetTimePicker("SelectedSessionSelesaiTimePicker")?.SelectedTime;
        var selectedStatus = GetComboBox("SelectedSessionStatusComboBox")?.SelectedItem as string;

        if (!selectedDate.HasValue || !selectedStartTime.HasValue || !selectedEndTime.HasValue || string.IsNullOrWhiteSpace(selectedStatus))
        {
            SetStatus("Isi data.");
            return;
        }

        if (string.IsNullOrWhiteSpace(newTopik))
        {
            SetStatus("Isi topik.");
            return;
        }

        try
        {
            viewModel.UpdateSesi(
                selected,
                selected.KodeSesi,
                selected.MahasiswaId,
                selected.KonselorId,
                selected.KategoriMasalahId,
                DateOnly.FromDateTime(selectedDate.Value.DateTime),
                TimeOnly.FromTimeSpan(selectedStartTime.Value),
                TimeOnly.FromTimeSpan(selectedEndTime.Value),
                selectedStatus,
                newTopik,
                selected.Catatan,
                selected.TindakLanjut);
            viewModel.ReloadData();
            BindSessionStatuses();
            UpdateProfile();
            BindSessionTargets();
            BindLists();
            UpdateSummary();
            SetStatus("Sesi diubah.");
        }
        catch (System.Exception)
        {
            SetStatus("Gagal mengubah sesi.");
        }
    }

    private void DeleteSessionButton_Click(object? sender, RoutedEventArgs e)
    {
        if (viewModel == null)
        {
            SetStatus("Gagal memuat data.");
            return;
        }

        if (GetListBox("MySessionsListBox")?.SelectedItem is not SesiKonseling selected)
        {
            SetStatus("Pilih data.");
            return;
        }

        try
        {
            viewModel.DeleteSesi(selected);
            viewModel.ReloadData();
            BindSessionStatuses();
            UpdateProfile();
            BindSessionTargets();
            BindLists();
            UpdateSummary();
            ClearSelectedSessionFields();
            SetStatus("Sesi dihapus.");
        }
        catch (System.Exception)
        {
            SetStatus("Gagal menghapus sesi.");
        }
    }

    private void UpdateProfile()
    {
        if (viewModel == null)
        {
            SetTextBox("ProfileInfo", "Pilih data.");
            return;
        }

        var mahasiswaId = AuthSession.CurrentUser?.MahasiswaId;
        var mahasiswa = mahasiswaId.HasValue
            ? viewModel.MahasiswaList.FirstOrDefault(item => item.Id == mahasiswaId.Value)
            : null;

        if (mahasiswa == null)
        {
            SetTextBox("ProfileInfo", "Profil tidak ditemukan.");
            return;
        }

        SetTextBox("ProfileInfo", $"NIM: {mahasiswa.Nim}\nNama: {mahasiswa.Nama}\nProgram: {mahasiswa.ProgramStudi}\nAngkatan: {mahasiswa.Angkatan}\nEmail: {mahasiswa.Email}\nHP: {mahasiswa.NoHp}");
    }

    private void BindLists()
    {
        if (viewModel == null) return;

        var mahasiswaId = AuthSession.CurrentUser?.MahasiswaId;
        var sessions = mahasiswaId.HasValue
            ? viewModel.SesiList.Where(item => item.MahasiswaId == mahasiswaId.Value).ToList()
            : viewModel.SesiList.ToList();

        var sessionListBox = GetListBox("MySessionsListBox");
        if (sessionListBox != null)
        {
            sessionListBox.ItemsSource = sessions;
            if (sessionListBox.SelectedIndex < 0 && sessions.Count > 0)
                sessionListBox.SelectedIndex = 0;
        }
    }

    private void BindSessionTargets()
    {
        if (viewModel == null) return;

        var konselorComboBox = GetComboBox("NewSessionKonselorComboBox");
        if (konselorComboBox != null)
        {
            konselorComboBox.ItemsSource = viewModel.KonselorList;
            if (konselorComboBox.SelectedIndex < 0 && viewModel.KonselorList.Count > 0)
                konselorComboBox.SelectedIndex = 0;
        }
    }

    private void UpdateSummary()
    {
        if (viewModel == null) return;

        var mahasiswaId = AuthSession.CurrentUser?.MahasiswaId;
        var sessionCount = mahasiswaId.HasValue
            ? viewModel.SesiList.Count(item => item.MahasiswaId == mahasiswaId.Value)
            : 0;

        SetTextBlock("ProfileCountText", mahasiswaId.HasValue ? "1" : "0");
        SetTextBlock("SesiSayaCountText", sessionCount.ToString());
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

    private string BuildSessionInfo(SesiKonseling sesi)
    {
        var konselor = viewModel?.KonselorList.FirstOrDefault(item => item.Id == sesi.KonselorId);
        return $"Kode: {sesi.KodeSesi}\nTanggal: {sesi.TanggalSesi:yyyy-MM-dd}\nWaktu: {FormatTime(sesi.WaktuMulai)} - {FormatTime(sesi.WaktuSelesai)}\nKonselor: {(konselor != null ? konselor.Nama : sesi.KonselorId.ToString())}\nStatus: {sesi.Status}\nTopik: {sesi.Topik}\nCatatan: {sesi.Catatan}\nTindak lanjut: {sesi.TindakLanjut}";
    }

    private void SetStatus(string message)
    {
        SetTextBlock("StatusText", message);
    }

    private void ClearCreateSessionForm()
    {
        SetTextBox("NewSessionTopikTextBox", string.Empty);
    }

    private void BindSessionStatuses()
    {
        var statusComboBox = GetComboBox("SelectedSessionStatusComboBox");
        if (statusComboBox != null)
        {
            statusComboBox.ItemsSource = SessionStatuses;
        }
    }

    private void SetSelectedSessionFields(SesiKonseling selected)
    {
        SetTextBox("SelectedSessionInfo", BuildSessionInfo(selected));
        SetDatePicker("SelectedSessionDatePicker", selected.TanggalSesi);
        SetTimePicker("SelectedSessionMulaiTimePicker", selected.WaktuMulai);
        SetTimePicker("SelectedSessionSelesaiTimePicker", selected.WaktuSelesai);
        SetComboBox("SelectedSessionStatusComboBox", selected.Status);
        SetTextBox("SelectedSessionTopikTextBox", selected.Topik);
    }

    private void ClearSelectedSessionFields()
    {
        SetTextBox("SelectedSessionInfo", "Pilih data.");
        SetDatePicker("SelectedSessionDatePicker", null);
        SetTimePicker("SelectedSessionMulaiTimePicker", null);
        SetTimePicker("SelectedSessionSelesaiTimePicker", null);
        SetComboBox("SelectedSessionStatusComboBox", null);
        SetTextBox("SelectedSessionTopikTextBox", string.Empty);
    }

    private void SetTextBlock(string name, string? value)
    {
        var control = this.FindControl<TextBlock>(name);
        if (control != null)
            control.Text = value ?? string.Empty;
    }

    private void SetTextBox(string name, string? value)
    {
        var control = this.FindControl<TextBox>(name);
        if (control != null)
            control.Text = value ?? string.Empty;
    }

    private void SetComboBox(string name, object? value)
    {
        var control = this.FindControl<ComboBox>(name);
        if (control != null)
            control.SelectedItem = value;
    }

    private void SetDatePicker(string name, DateOnly? value)
    {
        var control = this.FindControl<DatePicker>(name);
        if (control != null)
            control.SelectedDate = value.HasValue ? new DateTimeOffset(value.Value.ToDateTime(TimeOnly.MinValue)) : null;
    }

    private void SetTimePicker(string name, TimeOnly? value)
    {
        var control = this.FindControl<TimePicker>(name);
        if (control != null)
            control.SelectedTime = value.HasValue ? value.Value.ToTimeSpan() : null;
    }

    private static string FormatTime(TimeOnly? value)
    {
        return value.HasValue ? value.Value.ToString("HH:mm") : "-";
    }

    private string Text(string name) => this.FindControl<TextBox>(name)?.Text?.Trim() ?? string.Empty;

    private DatePicker? GetDatePicker(string name) => this.FindControl<DatePicker>(name);

    private TimePicker? GetTimePicker(string name) => this.FindControl<TimePicker>(name);

    private ComboBox? GetComboBox(string name) => this.FindControl<ComboBox>(name);

    private ListBox? GetListBox(string name) => this.FindControl<ListBox>(name);
}
