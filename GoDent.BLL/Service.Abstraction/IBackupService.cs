namespace GoDent.BLL.Service.Abstraction
{
    public interface IBackupService
    {
        /// <summary>Creates a timestamped backup of the SQLite database file.</summary>
        Task<string> CreateBackupAsync();

        /// <summary>Returns info about all existing backup files, newest first.</summary>
        List<BackupFileInfo> GetAllBackups();

        /// <summary>Returns the full path for a backup by file name.</summary>
        string? GetBackupPath(string fileName);

        /// <summary>Deletes a specific backup file.</summary>
        bool DeleteBackup(string fileName);

        /// <summary>Removes old backups, keeping only the most recent N.</summary>
        void CleanupOldBackups(int keepCount = 10);

        /// <summary>Restores the database from an existing backup file. Creates a safety backup first.</summary>
        Task RestoreFromBackupAsync(string fileName);

        /// <summary>Restores the database from an uploaded .db file. Creates a safety backup first.</summary>
        Task RestoreFromUploadAsync(Stream uploadStream);
    }

    public class BackupFileInfo
    {
        public string FileName { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public long SizeInBytes { get; set; }
        public DateTime CreatedAt { get; set; }

        public string SizeDisplay => SizeInBytes switch
        {
            < 1024 => $"{SizeInBytes} B",
            < 1024 * 1024 => $"{SizeInBytes / 1024.0:F1} KB",
            _ => $"{SizeInBytes / (1024.0 * 1024.0):F2} MB"
        };
    }
}
