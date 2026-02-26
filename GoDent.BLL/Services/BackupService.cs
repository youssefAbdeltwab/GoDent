using GoDent.BLL.Service.Abstraction;
using Microsoft.Data.Sqlite;

namespace GoDent.BLL.Services
{
    public class BackupService : IBackupService
    {
        private readonly string _dbPath;
        private readonly string _backupDir;

        public BackupService(string dbPath, string backupDir)
        {
            _dbPath = dbPath;
            _backupDir = backupDir;

            if (!Directory.Exists(_backupDir))
                Directory.CreateDirectory(_backupDir);
        }

        public async Task<string> CreateBackupAsync()
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            var backupFileName = $"GoDent_Backup_{timestamp}.db";
            var backupPath = Path.Combine(_backupDir, backupFileName);

            // Use SQLite Online Backup API for a safe, consistent backup
            using var sourceConn = new SqliteConnection($"Data Source={_dbPath}");
            using var destConn = new SqliteConnection($"Data Source={backupPath}");

            await sourceConn.OpenAsync();
            await destConn.OpenAsync();

            sourceConn.BackupDatabase(destConn);

            await sourceConn.CloseAsync();
            await destConn.CloseAsync();

            // Cleanup old backups (keep last 10)
            CleanupOldBackups(10);

            return backupFileName;
        }

        public List<BackupFileInfo> GetAllBackups()
        {
            if (!Directory.Exists(_backupDir))
                return new List<BackupFileInfo>();

            return Directory.GetFiles(_backupDir, "GoDent_Backup_*.db")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTime)
                .Select(f => new BackupFileInfo
                {
                    FileName = f.Name,
                    FullPath = f.FullName,
                    SizeInBytes = f.Length,
                    CreatedAt = f.CreationTime
                })
                .ToList();
        }

        public string? GetBackupPath(string fileName)
        {
            // Prevent path traversal attacks
            var safeName = Path.GetFileName(fileName);
            var path = Path.Combine(_backupDir, safeName);

            return File.Exists(path) ? path : null;
        }

        public bool DeleteBackup(string fileName)
        {
            var path = GetBackupPath(fileName);
            if (path == null) return false;

            File.Delete(path);
            return true;
        }

        public void CleanupOldBackups(int keepCount = 10)
        {
            var backups = GetAllBackups();
            var toDelete = backups.Skip(keepCount);

            foreach (var backup in toDelete)
            {
                try { File.Delete(backup.FullPath); }
                catch { /* ignore cleanup errors */ }
            }
        }

        public async Task RestoreFromBackupAsync(string fileName)
        {
            var backupPath = GetBackupPath(fileName)
                ?? throw new FileNotFoundException($"Backup file not found: {fileName}");

            // Safety backup before restoring
            await CreateBackupAsync();

            // Restore using SQLite Backup API (backup file → live database)
            using var sourceConn = new SqliteConnection($"Data Source={backupPath}");
            using var destConn = new SqliteConnection($"Data Source={_dbPath}");

            await sourceConn.OpenAsync();
            await destConn.OpenAsync();

            sourceConn.BackupDatabase(destConn);

            await sourceConn.CloseAsync();
            await destConn.CloseAsync();
        }

        public async Task RestoreFromUploadAsync(Stream uploadStream)
        {
            // Save uploaded file to a temp location first
            var tempPath = Path.Combine(_backupDir, $"upload_temp_{Guid.NewGuid()}.db");
            try
            {
                using (var fileStream = new FileStream(tempPath, FileMode.Create))
                {
                    await uploadStream.CopyToAsync(fileStream);
                }

                // Validate it's a real SQLite database
                try
                {
                    using var testConn = new SqliteConnection($"Data Source={tempPath}");
                    await testConn.OpenAsync();
                    using var cmd = testConn.CreateCommand();
                    cmd.CommandText = "SELECT count(*) FROM sqlite_master;";
                    await cmd.ExecuteScalarAsync();
                    await testConn.CloseAsync();
                }
                catch
                {
                    throw new InvalidOperationException("الملف المرفوع ليس قاعدة بيانات SQLite صالحة");
                }

                // Safety backup before restoring
                await CreateBackupAsync();

                // Restore using SQLite Backup API
                using var sourceConn = new SqliteConnection($"Data Source={tempPath}");
                using var destConn = new SqliteConnection($"Data Source={_dbPath}");

                await sourceConn.OpenAsync();
                await destConn.OpenAsync();

                sourceConn.BackupDatabase(destConn);

                await sourceConn.CloseAsync();
                await destConn.CloseAsync();
            }
            finally
            {
                // Always clean up temp file
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
        }
    }
}
