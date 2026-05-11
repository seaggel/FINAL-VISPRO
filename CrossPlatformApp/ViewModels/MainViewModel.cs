using System.Collections.ObjectModel;
using System.Linq;
using CrossPlatformApp.Models;
using CrossPlatformApp.Services;

namespace CrossPlatformApp.ViewModels;

public sealed class MainViewModel : BaseViewModel
{
    private readonly ICounselingRepository repository;
    private string statusMessage = string.Empty;

    public MainViewModel()
    {
        try
        {
            repository = RepositoryFactory.Create();
            if (ReloadData())
            {
                statusMessage = "✓ MySQL terhubung sukses. Sistem siap digunakan.";
            }
        }
        catch (System.Exception ex)
        {
            repository = NullCounselingRepository.Instance;
            StatusMessage = "Database tidak siap: " + GetShortMessage(ex);
            ReloadData();
        }
    }

    public ObservableCollection<Mahasiswa> MahasiswaList { get; } = new();
    public ObservableCollection<Konselor> KonselorList { get; } = new();
    public ObservableCollection<SesiKonseling> SesiList { get; } = new();
    public ObservableCollection<KategoriMasalah> KategoriList { get; } = new();

    public int MahasiswaCount => MahasiswaList.Count;
    public int KonselorCount => KonselorList.Count;
    public int SesiCount => SesiList.Count;
    public int FollowUpCount => SesiList.Count(item => !string.Equals(item.Status, "Selesai", StringComparison.OrdinalIgnoreCase));

    public string StatusMessage
    {
        get => statusMessage;
        private set
        {
            if (statusMessage != value)
            {
                statusMessage = value;
                OnPropertyChanged();
            }
        }
    }

    public bool ReloadData()
    {
        try
        {
            ReplaceCollection(MahasiswaList, repository.GetMahasiswa());
            ReplaceCollection(KonselorList, repository.GetKonselor());
            ReplaceCollection(SesiList, repository.GetSesiKonseling());
            ReplaceCollection(KategoriList, repository.GetKategoriMasalah());
            RaiseCounts();
            return true;
        }
        catch (System.Exception ex)
        {
            StatusMessage = "Gagal memuat data: " + GetShortMessage(ex);
            return false;
        }
    }

    public void AddMahasiswa(string nim, string nama, string programStudi, int? angkatan = null, string? jenisKelamin = null, string? noHp = null, string? email = null, string? alamat = null)
    {
        TryWrite("Mahasiswa", $"Mahasiswa {nama} berhasil ditambahkan.", () =>
        {
            repository.AddMahasiswa(new Mahasiswa
            {
                Nim = nim,
                Nama = nama,
                ProgramStudi = programStudi,
                Angkatan = angkatan,
                JenisKelamin = jenisKelamin,
                NoHp = noHp,
                Email = email,
                Alamat = alamat
            });
        });
    }

    public void UpdateMahasiswa(Mahasiswa selected, string nim, string nama, string programStudi, int? angkatan = null, string? jenisKelamin = null, string? noHp = null, string? email = null, string? alamat = null)
    {
        TryWrite("Mahasiswa", $"Mahasiswa {nama} berhasil diperbarui.", () =>
        {
            selected.Nim = nim;
            selected.Nama = nama;
            selected.ProgramStudi = programStudi;
            selected.Angkatan = angkatan;
            selected.JenisKelamin = jenisKelamin;
            selected.NoHp = noHp;
            selected.Email = email;
            selected.Alamat = alamat;
            repository.UpdateMahasiswa(selected);
        });
    }

    public void DeleteMahasiswa(Mahasiswa selected)
    {
        TryWrite("Mahasiswa", $"Mahasiswa {selected.Nama} berhasil dihapus.", () => repository.DeleteMahasiswa(selected.Id));
    }

    public void AddKonselor(string kodeKonselor, string nama, string jabatan, string? noHp = null, string? email = null, string? bidangKeahlian = null, bool aktif = true)
    {
        TryWrite("Konselor", $"Konselor {nama} berhasil ditambahkan.", () =>
        {
            repository.AddKonselor(new Konselor
            {
                KodeKonselor = kodeKonselor,
                Nama = nama,
                Jabatan = jabatan,
                NoHp = noHp,
                Email = email,
                BidangKeahlian = bidangKeahlian,
                Aktif = aktif
            });
        });
    }

    public void UpdateKonselor(Konselor selected, string kodeKonselor, string nama, string jabatan, string? noHp = null, string? email = null, string? bidangKeahlian = null, bool aktif = true)
    {
        TryWrite("Konselor", $"Konselor {nama} berhasil diperbarui.", () =>
        {
            selected.KodeKonselor = kodeKonselor;
            selected.Nama = nama;
            selected.Jabatan = jabatan;
            selected.NoHp = noHp;
            selected.Email = email;
            selected.BidangKeahlian = bidangKeahlian;
            selected.Aktif = aktif;
            repository.UpdateKonselor(selected);
        });
    }

    public void DeleteKonselor(Konselor selected)
    {
        TryWrite("Konselor", $"Konselor {selected.Nama} berhasil dihapus.", () => repository.DeleteKonselor(selected.Id));
    }

    public void AddSesi(string kodeSesi, int mahasiswaId, int konselorId, int? kategoriMasalahId, DateOnly tanggalSesi, TimeOnly? waktuMulai, TimeOnly? waktuSelesai, string status, string topik, string? catatan = null, string? tindakLanjut = null)
    {
        TryWrite("Sesi", $"Sesi {kodeSesi} berhasil ditambahkan.", () =>
        {
            repository.AddSesi(new SesiKonseling
            {
                KodeSesi = kodeSesi,
                MahasiswaId = mahasiswaId,
                KonselorId = konselorId,
                KategoriMasalahId = kategoriMasalahId,
                TanggalSesi = tanggalSesi,
                WaktuMulai = waktuMulai,
                WaktuSelesai = waktuSelesai,
                Status = status,
                Topik = topik,
                Catatan = catatan,
                TindakLanjut = tindakLanjut
            });
        });
    }

    public void AddSesi(string mahasiswa, string konselor, string topik)
    {
        var nextNumber = SesiList.Count + 1;
        AddSesi($"S-2026-{nextNumber:0000}", 1, 1, 1, DateOnly.FromDateTime(DateTime.Now), new TimeOnly(9, 0), new TimeOnly(9, 30), "Dijadwalkan", topik, $"Mahasiswa: {mahasiswa}, Konselor: {konselor}", "Belum diisi");
    }

    public void UpdateSesi(SesiKonseling selected, string kodeSesi, int mahasiswaId, int konselorId, int? kategoriMasalahId, DateOnly tanggalSesi, TimeOnly? waktuMulai, TimeOnly? waktuSelesai, string status, string topik, string? catatan = null, string? tindakLanjut = null)
    {
        TryWrite("Sesi", $"Sesi {kodeSesi} berhasil diperbarui.", () =>
        {
            selected.KodeSesi = kodeSesi;
            selected.MahasiswaId = mahasiswaId;
            selected.KonselorId = konselorId;
            selected.KategoriMasalahId = kategoriMasalahId;
            selected.TanggalSesi = tanggalSesi;
            selected.WaktuMulai = waktuMulai;
            selected.WaktuSelesai = waktuSelesai;
            selected.Status = status;
            selected.Topik = topik;
            selected.Catatan = catatan;
            selected.TindakLanjut = tindakLanjut;
            repository.UpdateSesi(selected);
        });
    }

    public void DeleteSesi(SesiKonseling selected)
    {
        TryWrite("Sesi", $"Sesi {selected.KodeSesi} berhasil dihapus.", () => repository.DeleteSesi(selected.Id));
    }

    public void CreateSessionForCurrentMahasiswa(int konselorId, string topik)
    {
        var mahasiswaId = CrossPlatformApp.Services.AuthSession.CurrentUser?.MahasiswaId;
        if (!mahasiswaId.HasValue)
        {
            throw new System.InvalidOperationException("Sesi mahasiswa tidak tersedia.");
        }

        AddSesi(CreateSessionCode(), mahasiswaId.Value, konselorId, null, DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now.AddMinutes(30)), "Dijadwalkan", topik);
    }

    public void CreateSessionForCurrentKonselor(int mahasiswaId, string topik)
    {
        var konselorId = CrossPlatformApp.Services.AuthSession.CurrentUser?.KonselorId;
        if (!konselorId.HasValue)
        {
            throw new System.InvalidOperationException("Sesi konselor tidak tersedia.");
        }

        AddSesi(CreateSessionCode(), mahasiswaId, konselorId.Value, null, DateOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now), TimeOnly.FromDateTime(DateTime.Now.AddMinutes(30)), "Dijadwalkan", topik);
    }

    public void AddDemoMahasiswa()
    {
        var nextNumber = MahasiswaList.Count + 1;
        AddMahasiswa($"2311{nextNumber:00}", $"Demo Mahasiswa {nextNumber}", "Sistem Informasi");
    }

    public void AddDemoSesi()
    {
        var nextNumber = SesiList.Count + 1;

        // Ensure at least one mahasiswa exists
        if (MahasiswaList.Count == 0)
        {
            AddDemoMahasiswa();
        }

        // Ensure at least one konselor exists
        if (KonselorList.Count == 0)
        {
            AddKonselor($"K-{nextNumber:000}", $"Demo Konselor {nextNumber}", "Konselor");
        }

        // Use the actual IDs from the repository (take the most recently added entries)
        var mahasiswaId = MahasiswaList.Last().Id;
        var konselorId = KonselorList.Last().Id;

        try
        {
            AddSesi(CreateSessionCode(), mahasiswaId, konselorId, 1, DateOnly.FromDateTime(DateTime.Now), new TimeOnly(9, 0), new TimeOnly(9, 30), "Dijadwalkan", $"Sesi demo ke-{SesiList.Count + 1}", "Data demo", "Belum diisi");
        }
        catch (System.Exception ex)
        {
            // Surface friendly message to UI
            StatusMessage = "Gagal membuat sesi demo: " + ex.Message;
        }
    }

    private static string CreateSessionCode()
    {
        return $"S-{DateTime.Now:yyyyMMddHHmmssfff}";
    }

    public void SetStatus(string message)
    {
        StatusMessage = message;
    }

    public void ClearStatus()
    {
        StatusMessage = "Sistem siap digunakan. Pilih tab untuk mengelola data mahasiswa, konselor, dan sesi.";
    }

    private void TryWrite(string entityName, string successMessage, Action action)
    {
        try
        {
            action();
            if (ReloadData())
            {
                StatusMessage = successMessage;
            }
        }
        catch (System.Exception ex)
        {
            StatusMessage = $"Gagal {entityName.ToLowerInvariant()}: " + GetShortMessage(ex);
        }
    }

    private static string GetShortMessage(System.Exception ex)
    {
        var message = ex.GetBaseException().Message;
        return string.IsNullOrWhiteSpace(message) ? "Terjadi kesalahan." : message;
    }

    private void RaiseCounts()
    {
        OnPropertyChanged(nameof(MahasiswaCount));
        OnPropertyChanged(nameof(KonselorCount));
        OnPropertyChanged(nameof(SesiCount));
        OnPropertyChanged(nameof(FollowUpCount));
    }

    private static void ReplaceCollection<T>(ObservableCollection<T> target, IEnumerable<T> source)
    {
        target.Clear();
        foreach (var item in source)
        {
            target.Add(item);
        }
    }
}
