using CrossPlatformApp.Models;

namespace CrossPlatformApp.Services;

public sealed class NullCounselingRepository : ICounselingRepository
{
    public static NullCounselingRepository Instance { get; } = new();

    private NullCounselingRepository()
    {
    }

    public IReadOnlyList<Mahasiswa> GetMahasiswa() => Array.Empty<Mahasiswa>();
    public IReadOnlyList<Konselor> GetKonselor() => Array.Empty<Konselor>();
    public IReadOnlyList<KategoriMasalah> GetKategoriMasalah() => Array.Empty<KategoriMasalah>();
    public IReadOnlyList<SesiKonseling> GetSesiKonseling() => Array.Empty<SesiKonseling>();

    public Mahasiswa AddMahasiswa(Mahasiswa mahasiswa) => mahasiswa;
    public bool UpdateMahasiswa(Mahasiswa mahasiswa) => false;
    public bool DeleteMahasiswa(int id) => false;

    public Konselor AddKonselor(Konselor konselor) => konselor;
    public bool UpdateKonselor(Konselor konselor) => false;
    public bool DeleteKonselor(int id) => false;

    public SesiKonseling AddSesi(SesiKonseling sesi) => sesi;
    public bool UpdateSesi(SesiKonseling sesi) => false;
    public bool DeleteSesi(int id) => false;

    public bool RegisterMahasiswaAccount(Mahasiswa mahasiswa, string username, string passwordHash) => false;
    public bool CreateUser(string username, string passwordHash, string role, int? mahasiswaId, int? konselorId) => false;
    public (bool Success, string? Role, int? MahasiswaId, int? KonselorId) AuthenticateUser(string username, string password) => (false, null, null, null);
}