namespace CrossPlatformApp.Models;

public sealed class SesiKonseling
{
    public int Id { get; set; }
    public string KodeSesi { get; set; } = string.Empty;
    public int MahasiswaId { get; set; }
    public int KonselorId { get; set; }
    public int? KategoriMasalahId { get; set; }
    public DateOnly TanggalSesi { get; set; }
    public TimeOnly? WaktuMulai { get; set; }
    public TimeOnly? WaktuSelesai { get; set; }
    public string Status { get; set; } = "Dijadwalkan";
    public string Topik { get; set; } = string.Empty;
    public string? Catatan { get; set; }
    public string? TindakLanjut { get; set; }

    public override string ToString()
    {
        var tanggal = TanggalSesi.ToString("yyyy-MM-dd");
        return $"{KodeSesi} | {tanggal} | {Topik} | {Status}";
    }
}
