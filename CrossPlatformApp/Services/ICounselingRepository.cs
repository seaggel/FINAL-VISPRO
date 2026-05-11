using CrossPlatformApp.Models;

namespace CrossPlatformApp.Services;

public interface ICounselingRepository
{
    IReadOnlyList<Mahasiswa> GetMahasiswa();
    IReadOnlyList<Konselor> GetKonselor();
    IReadOnlyList<KategoriMasalah> GetKategoriMasalah();
    IReadOnlyList<SesiKonseling> GetSesiKonseling();

    Mahasiswa AddMahasiswa(Mahasiswa mahasiswa);
    bool UpdateMahasiswa(Mahasiswa mahasiswa);
    bool DeleteMahasiswa(int id);

    Konselor AddKonselor(Konselor konselor);
    bool UpdateKonselor(Konselor konselor);
    bool DeleteKonselor(int id);

    SesiKonseling AddSesi(SesiKonseling sesi);
    bool UpdateSesi(SesiKonseling sesi);
    bool DeleteSesi(int id);

    // Authentication/authorization
    bool RegisterMahasiswaAccount(Mahasiswa mahasiswa, string username, string passwordHash);
    bool CreateUser(string username, string passwordHash, string role, int? mahasiswaId, int? konselorId);
    (bool Success, string? Role, int? MahasiswaId, int? KonselorId) AuthenticateUser(string username, string password);
}
