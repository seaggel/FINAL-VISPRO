using CrossPlatformApp.Models;
using MySqlConnector;

namespace CrossPlatformApp.Services;

public sealed class MySqlCounselingRepository : ICounselingRepository
{
    private readonly string connectionString;

    public MySqlCounselingRepository(string connectionString)
    {
        this.connectionString = connectionString;
        EnsureAuthTable();
    }

    private void EnsureAuthTable()
    {
        using var connection = CreateConnection();
        var sql = @"CREATE TABLE IF NOT EXISTS users (
  id INT AUTO_INCREMENT PRIMARY KEY,
  username VARCHAR(100) NOT NULL UNIQUE,
  password_hash VARCHAR(255) NOT NULL,
  role ENUM('Admin','Mahasiswa','Konselor') NOT NULL,
  mahasiswa_id INT NULL,
  konselor_id INT NULL,
  created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (mahasiswa_id) REFERENCES mahasiswa(id) ON DELETE SET NULL ON UPDATE CASCADE,
  FOREIGN KEY (konselor_id) REFERENCES konselor(id) ON DELETE SET NULL ON UPDATE CASCADE
);";
        using var cmd = new MySqlCommand(sql, connection);
        cmd.ExecuteNonQuery();
    }

    public IReadOnlyList<Mahasiswa> GetMahasiswa() => ReadMahasiswa();
    public IReadOnlyList<Konselor> GetKonselor() => ReadKonselor();
    public IReadOnlyList<KategoriMasalah> GetKategoriMasalah() => ReadKategoriMasalah();
    public IReadOnlyList<SesiKonseling> GetSesiKonseling() => ReadSesiKonseling();

    public Mahasiswa AddMahasiswa(Mahasiswa mahasiswa)
    {
        const string sql = @"INSERT INTO mahasiswa (nim, nama, program_studi, angkatan, jenis_kelamin, no_hp, email, alamat)
VALUES (@nim, @nama, @program_studi, @angkatan, @jenis_kelamin, @no_hp, @email, @alamat);";

        using var connection = CreateConnection();
        using var command = new MySqlCommand(sql, connection);
        AddMahasiswaParameters(command, mahasiswa);
        command.ExecuteNonQuery();
        mahasiswa.Id = (int)command.LastInsertedId;
        return mahasiswa;
    }

    public bool UpdateMahasiswa(Mahasiswa mahasiswa)
    {
        const string sql = @"UPDATE mahasiswa SET nim = @nim, nama = @nama, program_studi = @program_studi,
angkatan = @angkatan, jenis_kelamin = @jenis_kelamin, no_hp = @no_hp, email = @email, alamat = @alamat
WHERE id = @id;";

        using var connection = CreateConnection();
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", mahasiswa.Id);
        AddMahasiswaParameters(command, mahasiswa);
        return command.ExecuteNonQuery() > 0;
    }

    public bool DeleteMahasiswa(int id)
    {
        using var connection = CreateConnection();
        using var command = new MySqlCommand("DELETE FROM mahasiswa WHERE id = @id;", connection);
        command.Parameters.AddWithValue("@id", id);
        return command.ExecuteNonQuery() > 0;
    }

    public Konselor AddKonselor(Konselor konselor)
    {
        const string sql = @"INSERT INTO konselor (kode_konselor, nama, jabatan, no_hp, email, bidang_keahlian, aktif)
VALUES (@kode_konselor, @nama, @jabatan, @no_hp, @email, @bidang_keahlian, @aktif);";

        using var connection = CreateConnection();
        using var command = new MySqlCommand(sql, connection);
        AddKonselorParameters(command, konselor);
        command.ExecuteNonQuery();
        konselor.Id = (int)command.LastInsertedId;
        return konselor;
    }

    public bool UpdateKonselor(Konselor konselor)
    {
        const string sql = @"UPDATE konselor SET kode_konselor = @kode_konselor, nama = @nama, jabatan = @jabatan,
no_hp = @no_hp, email = @email, bidang_keahlian = @bidang_keahlian, aktif = @aktif
WHERE id = @id;";

        using var connection = CreateConnection();
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", konselor.Id);
        AddKonselorParameters(command, konselor);
        return command.ExecuteNonQuery() > 0;
    }

    public bool DeleteKonselor(int id)
    {
        using var connection = CreateConnection();
        using var command = new MySqlCommand("DELETE FROM konselor WHERE id = @id;", connection);
        command.Parameters.AddWithValue("@id", id);
        return command.ExecuteNonQuery() > 0;
    }

    public SesiKonseling AddSesi(SesiKonseling sesi)
    {
        const string sql = @"INSERT INTO sesi_konseling (kode_sesi, mahasiswa_id, konselor_id, kategori_masalah_id, tanggal_sesi, waktu_mulai, waktu_selesai, status, topik, catatan, tindak_lanjut)
VALUES (@kode_sesi, @mahasiswa_id, @konselor_id, @kategori_masalah_id, @tanggal_sesi, @waktu_mulai, @waktu_selesai, @status, @topik, @catatan, @tindak_lanjut);";

        using var connection = CreateConnection();
        using var command = new MySqlCommand(sql, connection);
        AddSesiParameters(command, sesi);
        command.ExecuteNonQuery();
        sesi.Id = (int)command.LastInsertedId;
        return sesi;
    }

    public bool UpdateSesi(SesiKonseling sesi)
    {
        const string sql = @"UPDATE sesi_konseling SET kode_sesi = @kode_sesi, mahasiswa_id = @mahasiswa_id,
konselor_id = @konselor_id, kategori_masalah_id = @kategori_masalah_id, tanggal_sesi = @tanggal_sesi,
waktu_mulai = @waktu_mulai, waktu_selesai = @waktu_selesai, status = @status, topik = @topik,
catatan = @catatan, tindak_lanjut = @tindak_lanjut
WHERE id = @id;";

        using var connection = CreateConnection();
        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@id", sesi.Id);
        AddSesiParameters(command, sesi);
        return command.ExecuteNonQuery() > 0;
    }

    public bool DeleteSesi(int id)
    {
        using var connection = CreateConnection();
        using var command = new MySqlCommand("DELETE FROM sesi_konseling WHERE id = @id;", connection);
        command.Parameters.AddWithValue("@id", id);
        return command.ExecuteNonQuery() > 0;
    }

    public bool RegisterMahasiswaAccount(Mahasiswa mahasiswa, string username, string passwordHash)
    {
        using var connection = CreateConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            const string mahasiswaSql = @"INSERT INTO mahasiswa (nim, nama, program_studi, angkatan, jenis_kelamin, no_hp, email, alamat)
VALUES (@nim, @nama, @program_studi, @angkatan, @jenis_kelamin, @no_hp, @email, @alamat);";

            using var mahasiswaCommand = new MySqlCommand(mahasiswaSql, connection, transaction);
            AddMahasiswaParameters(mahasiswaCommand, mahasiswa);
            mahasiswaCommand.ExecuteNonQuery();
            mahasiswa.Id = (int)mahasiswaCommand.LastInsertedId;

            const string userSql = @"INSERT INTO users (username, password_hash, role, mahasiswa_id, konselor_id)
VALUES (@username, @password_hash, 'Mahasiswa', @mahasiswa_id, NULL);";

            using var userCommand = new MySqlCommand(userSql, connection, transaction);
            userCommand.Parameters.AddWithValue("@username", username);
            userCommand.Parameters.AddWithValue("@password_hash", passwordHash);
            userCommand.Parameters.AddWithValue("@mahasiswa_id", mahasiswa.Id);
            userCommand.ExecuteNonQuery();

            transaction.Commit();
            return true;
        }
        catch
        {
            try
            {
                transaction.Rollback();
            }
            catch
            {
            }

            return false;
        }
    }

    private MySqlConnection CreateConnection()
    {
        var connection = new MySqlConnection(connectionString);
        connection.Open();
        return connection;
    }

    private static void AddMahasiswaParameters(MySqlCommand command, Mahasiswa mahasiswa)
    {
        command.Parameters.AddWithValue("@nim", mahasiswa.Nim);
        command.Parameters.AddWithValue("@nama", mahasiswa.Nama);
        command.Parameters.AddWithValue("@program_studi", mahasiswa.ProgramStudi);
        command.Parameters.AddWithValue("@angkatan", mahasiswa.Angkatan.HasValue ? mahasiswa.Angkatan.Value : DBNull.Value);
        command.Parameters.AddWithValue("@jenis_kelamin", string.IsNullOrWhiteSpace(mahasiswa.JenisKelamin) ? DBNull.Value : mahasiswa.JenisKelamin);
        command.Parameters.AddWithValue("@no_hp", string.IsNullOrWhiteSpace(mahasiswa.NoHp) ? DBNull.Value : mahasiswa.NoHp);
        command.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(mahasiswa.Email) ? DBNull.Value : mahasiswa.Email);
        command.Parameters.AddWithValue("@alamat", string.IsNullOrWhiteSpace(mahasiswa.Alamat) ? DBNull.Value : mahasiswa.Alamat);
    }

    private static void AddKonselorParameters(MySqlCommand command, Konselor konselor)
    {
        command.Parameters.AddWithValue("@kode_konselor", konselor.KodeKonselor);
        command.Parameters.AddWithValue("@nama", konselor.Nama);
        command.Parameters.AddWithValue("@jabatan", konselor.Jabatan);
        command.Parameters.AddWithValue("@no_hp", string.IsNullOrWhiteSpace(konselor.NoHp) ? DBNull.Value : konselor.NoHp);
        command.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(konselor.Email) ? DBNull.Value : konselor.Email);
        command.Parameters.AddWithValue("@bidang_keahlian", string.IsNullOrWhiteSpace(konselor.BidangKeahlian) ? DBNull.Value : konselor.BidangKeahlian);
        command.Parameters.AddWithValue("@aktif", konselor.Aktif);
    }

    private static void AddSesiParameters(MySqlCommand command, SesiKonseling sesi)
    {
        command.Parameters.AddWithValue("@kode_sesi", sesi.KodeSesi);
        command.Parameters.AddWithValue("@mahasiswa_id", sesi.MahasiswaId);
        command.Parameters.AddWithValue("@konselor_id", sesi.KonselorId);
        command.Parameters.AddWithValue("@kategori_masalah_id", sesi.KategoriMasalahId.HasValue ? sesi.KategoriMasalahId.Value : DBNull.Value);
        command.Parameters.AddWithValue("@tanggal_sesi", sesi.TanggalSesi.ToDateTime(TimeOnly.MinValue));
        command.Parameters.AddWithValue("@waktu_mulai", sesi.WaktuMulai.HasValue ? sesi.WaktuMulai.Value.ToTimeSpan() : DBNull.Value);
        command.Parameters.AddWithValue("@waktu_selesai", sesi.WaktuSelesai.HasValue ? sesi.WaktuSelesai.Value.ToTimeSpan() : DBNull.Value);
        command.Parameters.AddWithValue("@status", sesi.Status);
        command.Parameters.AddWithValue("@topik", sesi.Topik);
        command.Parameters.AddWithValue("@catatan", string.IsNullOrWhiteSpace(sesi.Catatan) ? DBNull.Value : sesi.Catatan);
        command.Parameters.AddWithValue("@tindak_lanjut", string.IsNullOrWhiteSpace(sesi.TindakLanjut) ? DBNull.Value : sesi.TindakLanjut);
    }

    // Authentication methods
    public bool CreateUser(string username, string passwordHash, string role, int? mahasiswaId, int? konselorId)
    {
        using var connection = CreateConnection();
        using var cmd = new MySqlCommand("INSERT INTO users (username, password_hash, role, mahasiswa_id, konselor_id) VALUES (@username, @password_hash, @role, @mahasiswa_id, @konselor_id);", connection);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@password_hash", passwordHash);
        cmd.Parameters.AddWithValue("@role", role);
        cmd.Parameters.AddWithValue("@mahasiswa_id", mahasiswaId.HasValue ? mahasiswaId.Value : DBNull.Value);
        cmd.Parameters.AddWithValue("@konselor_id", konselorId.HasValue ? konselorId.Value : DBNull.Value);
        try
        {
            return cmd.ExecuteNonQuery() > 0;
        }
        catch (MySqlException)
        {
            return false;
        }
    }

    public (bool Success, string? Role, int? MahasiswaId, int? KonselorId) AuthenticateUser(string username, string password)
    {
        using var connection = CreateConnection();
        using var cmd = new MySqlCommand("SELECT password_hash, role, mahasiswa_id, konselor_id FROM users WHERE username = @username LIMIT 1;", connection);
        cmd.Parameters.AddWithValue("@username", username);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return (false, null, null, null);
        var hash = reader.GetString(0);
        var role = reader.GetString(1);
        var mahasiswaId = reader.IsDBNull(2) ? null : (int?)reader.GetInt32(2);
        var konselorId = reader.IsDBNull(3) ? null : (int?)reader.GetInt32(3);
        var ok = BCrypt.Net.BCrypt.Verify(password ?? string.Empty, hash);
        return (ok, ok ? role : null, ok ? mahasiswaId : null, ok ? konselorId : null);
    }

    private List<Mahasiswa> ReadMahasiswa()
    {
        var result = new List<Mahasiswa>();
        using var connection = CreateConnection();
        using var command = new MySqlCommand("SELECT id, nim, nama, program_studi, angkatan, jenis_kelamin, no_hp, email, alamat FROM mahasiswa ORDER BY nama;", connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Mahasiswa
            {
                Id = reader.GetInt32(Ordinal(reader, "id")),
                Nim = reader.GetString(Ordinal(reader, "nim")),
                Nama = reader.GetString(Ordinal(reader, "nama")),
                ProgramStudi = reader.GetString(Ordinal(reader, "program_studi")),
                Angkatan = GetNullableInt32(reader, "angkatan"),
                JenisKelamin = GetNullableString(reader, "jenis_kelamin"),
                NoHp = GetNullableString(reader, "no_hp"),
                Email = GetNullableString(reader, "email"),
                Alamat = GetNullableString(reader, "alamat")
            });
        }

        return result;
    }

    private List<Konselor> ReadKonselor()
    {
        var result = new List<Konselor>();
        using var connection = CreateConnection();
        using var command = new MySqlCommand("SELECT id, kode_konselor, nama, jabatan, no_hp, email, bidang_keahlian, aktif FROM konselor ORDER BY nama;", connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new Konselor
            {
                Id = reader.GetInt32(Ordinal(reader, "id")),
                KodeKonselor = reader.GetString(Ordinal(reader, "kode_konselor")),
                Nama = reader.GetString(Ordinal(reader, "nama")),
                Jabatan = reader.GetString(Ordinal(reader, "jabatan")),
                NoHp = GetNullableString(reader, "no_hp"),
                Email = GetNullableString(reader, "email"),
                BidangKeahlian = GetNullableString(reader, "bidang_keahlian"),
                Aktif = reader.GetBoolean(Ordinal(reader, "aktif"))
            });
        }

        return result;
    }

    private List<KategoriMasalah> ReadKategoriMasalah()
    {
        var result = new List<KategoriMasalah>();
        using var connection = CreateConnection();
        using var command = new MySqlCommand("SELECT id, nama_kategori, deskripsi, aktif FROM kategori_masalah ORDER BY nama_kategori;", connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new KategoriMasalah
            {
                Id = reader.GetInt32(Ordinal(reader, "id")),
                NamaKategori = reader.GetString(Ordinal(reader, "nama_kategori")),
                Deskripsi = GetNullableString(reader, "deskripsi"),
                Aktif = reader.GetBoolean(Ordinal(reader, "aktif"))
            });
        }

        return result;
    }

    private List<SesiKonseling> ReadSesiKonseling()
    {
        var result = new List<SesiKonseling>();
        using var connection = CreateConnection();
        using var command = new MySqlCommand(@"SELECT id, kode_sesi, mahasiswa_id, konselor_id, kategori_masalah_id, tanggal_sesi, waktu_mulai, waktu_selesai, status, topik, catatan, tindak_lanjut FROM sesi_konseling ORDER BY tanggal_sesi DESC, id DESC;", connection);
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new SesiKonseling
            {
                Id = reader.GetInt32(Ordinal(reader, "id")),
                KodeSesi = reader.GetString(Ordinal(reader, "kode_sesi")),
                MahasiswaId = reader.GetInt32(Ordinal(reader, "mahasiswa_id")),
                KonselorId = reader.GetInt32(Ordinal(reader, "konselor_id")),
                KategoriMasalahId = GetNullableInt32(reader, "kategori_masalah_id"),
                TanggalSesi = DateOnly.FromDateTime(reader.GetDateTime(Ordinal(reader, "tanggal_sesi"))),
                WaktuMulai = GetNullableTimeOnly(reader, "waktu_mulai"),
                WaktuSelesai = GetNullableTimeOnly(reader, "waktu_selesai"),
                Status = reader.GetString(Ordinal(reader, "status")),
                Topik = reader.GetString(Ordinal(reader, "topik")),
                Catatan = GetNullableString(reader, "catatan"),
                TindakLanjut = GetNullableString(reader, "tindak_lanjut")
            });
        }

        return result;
    }

    private static int Ordinal(MySqlDataReader reader, string name) => reader.GetOrdinal(name);

    private static string? GetNullableString(MySqlDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
    }

    private static int? GetNullableInt32(MySqlDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? null : reader.GetInt32(ordinal);
    }

    private static TimeOnly? GetNullableTimeOnly(MySqlDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? null : TimeOnly.FromTimeSpan(reader.GetTimeSpan(ordinal));
    }
}
