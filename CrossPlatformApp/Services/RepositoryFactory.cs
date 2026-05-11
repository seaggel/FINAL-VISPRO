using MySqlConnector;

namespace CrossPlatformApp.Services;

public static class RepositoryFactory
{
    public static ICounselingRepository Create()
    {
        var connectionString = Environment.GetEnvironmentVariable("KONSELING_DB_CONNECTION")
            ?? "Server=127.0.0.1;Port=3306;Database=sistem_konseling_mahasiswa;User ID=root;Password=;SslMode=None;";
        return new MySqlCounselingRepository(connectionString);
    }
}
