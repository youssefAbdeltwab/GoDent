using GoDent.BLL.Service.Abstraction;

namespace GoDent.Services
{
    /// <summary>
    /// Background service that automatically creates a database backup every 3 days.
    /// Also creates a backup on application startup for safety.
    /// </summary>
    public class BackupSchedulerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BackupSchedulerService> _logger;
        private static readonly TimeSpan BackupInterval = TimeSpan.FromDays(3);

        public BackupSchedulerService(IServiceProvider serviceProvider, ILogger<BackupSchedulerService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Backup on startup
            await PerformBackupAsync("Startup");

            // Then every 3 days
            using var timer = new PeriodicTimer(BackupInterval);
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await timer.WaitForNextTickAsync(stoppingToken);
                    await PerformBackupAsync("Scheduled");
                }
                catch (OperationCanceledException)
                {
                    // App is shutting down
                    break;
                }
            }
        }

        private async Task PerformBackupAsync(string trigger)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var backupService = scope.ServiceProvider.GetRequiredService<IBackupService>();
                var fileName = await backupService.CreateBackupAsync();
                _logger.LogInformation("[Backup] {Trigger} backup created: {FileName}", trigger, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Backup] {Trigger} backup failed", trigger);
            }
        }
    }
}
