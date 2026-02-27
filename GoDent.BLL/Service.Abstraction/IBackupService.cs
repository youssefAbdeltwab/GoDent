namespace GoDent.BLL.Service.Abstraction
{
    public interface IBackupService
    {
        Task<string> CreateBackupAsync();
        Task<IEnumerable<string>> GetAllBackupsAsync();
        Task<byte[]> DownloadBackupAsync(string backupFileName);
        Task DeleteBackupAsync(string backupFileName);
        Task CleanupOldBackupsAsync(int keepCount = 10);
        Task RestoreFromBackupAsync(string backupFileName);
        Task RestoreFromUploadAsync(string uploadedFilePath);
    }
}
