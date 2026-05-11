namespace CrossPlatformApp.Models;

public sealed class KategoriMasalah
{
    public int Id { get; set; }
    public string NamaKategori { get; set; } = string.Empty;
    public string? Deskripsi { get; set; }
    public bool Aktif { get; set; } = true;

    public override string ToString() => NamaKategori;
}
