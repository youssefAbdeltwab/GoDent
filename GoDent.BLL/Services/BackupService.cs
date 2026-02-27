using GoDent.BLL.Service.Abstraction;
using Microsoft.Data.Sqlite;

namespace GoDent.BLL.Services
{
    /// <summary>
    /// Handles SQLite database backups using the Online Backup API.
    /// This is safer than simple file copy as it handles concurrent access gracefully.
    /// </summary>
    public class BackupService : IBackupService
    {
        private readonly string _databasePath;
        private readonly string _backupDirectory;

        public BackupService(string databasePath, string backupDirectory)
        {
            _databasePath = databasePath;
            _backupDirectory = backupDirectory;

            if (!Directory.Exists(_backupDirectory))
                Directory.CreateDirectory(_backupDirectory);
        }

        public async Task<string> CreateBackupAsync()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupFileName = $"GoDent_Backup_{timestamp}.db";
            var backupPath = Path.Combine(_backupDirectory, backupFileName);

            // Use SQLite Online Backup API for safe backup
            using (var sourceConnection = new SqliteConnection($"Data Source={_databasePath}"))
            using (var backupConnection = new SqliteConnection($"Data Source={backupPath}"))
            {
                await sourceConnection.OpenAsync();
                await backupConnection.OpenAsync();

                sourceConnection.BackupDatabase(backupConnection);
            }

            // Cleanup old backups (keep last 10)
            await CleanupOldBackupsAsync(10);

            return backupFileName;
        }

        public async Task<IEnumerable<string>> GetAllBackupsAsync()
        {
            return await Task.Run(() =>
            {
                if (!Directory.Exists(_backupDirectory))
                    return Enumerable.Empty<string>();

                return Directory.GetFiles(_backupDirectory, "*.db")
                    .Select(Path.GetFileName)
                    .Where(f => f != null)
                    .Cast<string>()
                    .OrderByDescending(f => f)
                    .ToList();
            });
        }

        public async Task<byte[]> DownloadBackupAsync(string backupFileName)
        {
            var backupPath = Path.Combine(_backupDirectory, backupFileName);
            
            if (!File.Exists(backupPath))
                throw new FileNotFoundException("Backup file not found");

            // Use FileStream with FileShare.ReadWrite to avoid locking issues
            using (var fs = new FileStream(backupPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var buffer = new byte[fs.Length];
                await fs.ReadAsync(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        public async Task DeleteBackupAsync(string backupFileName)
        {
            var backupPath = Path.Combine(_backupDirectory, backupFileName);
            
            await Task.Run(() =>
            {
                if (File.Exists(backupPath))
                    File.Delete(backupPath);
            });
        }

        public async Task CleanupOldBackupsAsync(int keepCount = 10)
        {
            await Task.Run(() =>
            {
                if (!Directory.Exists(_backupDirectory))
                    return;

                var backups = Directory.GetFiles(_backupDirectory, "*.db")
                    .OrderByDescending(f => new FileInfo(f).CreationTime)
                    .ToList();

                // Delete old backups beyond keepCount
                foreach (var backup in backups.Skip(keepCount))
                {
                    try
                    {
                        File.Delete(backup);
                    }
                    catch
                    {
                        // Ignore errors during cleanup
                    }
                }
            });
        }

        public async Task RestoreFromBackupAsync(string backupFileName)
        {
            var backupPath = Path.Combine(_backupDirectory, backupFileName);
            
            if (!File.Exists(backupPath))
                throw new FileNotFoundException("Backup file not found");

            // Create safety backup before restore
            var safetyBackup = $"Safety_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.db";
            var safetyPath = Path.Combine(_backupDirectory, safetyBackup);
            
            using (var sourceConnection = new SqliteConnection($"Data Source={_databasePath}"))
            using (var safetyConnection = new SqliteConnection($"Data Source={safetyPath}"))
            {
                await sourceConnection.OpenAsync();
                await safetyConnection.OpenAsync();
                sourceConnection.BackupDatabase(safetyConnection);
            }

            // Restore from backup
            await Task.Run(() =>
            {
                File.Copy(backupPath, _databasePath, overwrite: true);
            });
        }

        public async Task RestoreFromUploadAsync(string uploadedFilePath)
        {
            if (!File.Exists(uploadedFilePath))
                throw new FileNotFoundException("Uploaded file not found");

            // Validate it's a SQLite database
            try
            {
                using (var testConnection = new SqliteConnection($"Data Source={uploadedFilePath}"))
                {
                    await testConnection.OpenAsync();
                    // Simple validation query
                    using var cmd = testConnection.CreateCommand();
                    cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' LIMIT 1";
                    await cmd.ExecuteScalarAsync();
                }
            }
            catch
            {
                throw new InvalidOperationException("Invalid SQLite database file");
            }

            // Create safety backup before restore
            var safetyBackup = $"Safety_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.db";
            var safetyPath = Path.Combine(_backupDirectory, safetyBackup);
            
            using (var sourceConnection = new SqliteConnection($"Data Source={_databasePath}"))
            using (var safetyConnection = new SqliteConnection($"Data Source={safetyPath}"))
            {
                await sourceConnection.OpenAsync();
                await safetyConnection.OpenAsync();
                sourceConnection.BackupDatabase(safetyConnection);
            }

            // Restore from uploaded file
            await Task.Run(() =>
            {
                File.Copy(uploadedFilePath, _databasePath, overwrite: true);
            });
        }
    }
}
