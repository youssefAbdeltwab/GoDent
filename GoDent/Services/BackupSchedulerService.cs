using GoDent.BLL.Service.Abstraction;

namespace GoDent.Services
{
    /// <summary>
    /// Background service that automatically creates database backups:
    /// - On application startup
    /// - Every 3 days
    /// </summary>
    public class BackupSchedulerService : BackgroundService
    {
        private readonly IBackupService _backupService;
        private readonly ILogger<BackupSchedulerService> _logger;

        public BackupSchedulerService(IBackupService backupService, ILogger<BackupSchedulerService> logger)
        {
            _backupService = backupService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                // Create backup on startup
                _logger.LogInformation("Creating startup backup...");
                await _backupService.CreateBackupAsync();
                _logger.LogInformation("Startup backup created successfully");

                // Schedule periodic backups every 3 days
                using var timer = new PeriodicTimer(TimeSpan.FromDays(3));

                while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
                {
                    try
                    {
                        _logger.LogInformation("Creating scheduled backup...");
                        await _backupService.CreateBackupAsync();
                        _logger.LogInformation("Scheduled backup created successfully");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error creating scheduled backup");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in backup scheduler service");
            }
        }
    }
}
