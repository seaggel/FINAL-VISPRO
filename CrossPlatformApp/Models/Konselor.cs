namespace CrossPlatformApp.Models;

public sealed class Konselor
{
    public int Id { get; set; }
    public string KodeKonselor { get; set; } = string.Empty;
    public string Nama { get; set; } = string.Empty;
    public string Jabatan { get; set; } = string.Empty;
    public string? NoHp { get; set; }
    public string? Email { get; set; }
    public string? BidangKeahlian { get; set; }
    public bool Aktif { get; set; } = true;

    public override string ToString() => $"{KodeKonselor} | {Nama} | {Jabatan}";
}
