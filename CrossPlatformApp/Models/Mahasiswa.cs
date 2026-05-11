namespace CrossPlatformApp.Models;

public sealed class Mahasiswa
{
    public int Id { get; set; }
    public string Nim { get; set; } = string.Empty;
    public string Nama { get; set; } = string.Empty;
    public string ProgramStudi { get; set; } = string.Empty;
    public int? Angkatan { get; set; }
    public string? JenisKelamin { get; set; }
    public string? NoHp { get; set; }
    public string? Email { get; set; }
    public string? Alamat { get; set; }

    public override string ToString() => $"{Nim} | {Nama} | {ProgramStudi}";
}
