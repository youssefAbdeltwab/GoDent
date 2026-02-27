using GoDent.BLL.Service.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace GoDent.Controllers
{
    public class BackupController : Controller
    {
        private readonly IBackupService _backupService;
        private readonly IWebHostEnvironment _environment;

        public BackupController(IBackupService backupService, IWebHostEnvironment environment)
        {
            _backupService = backupService;
            _environment = environment;
        }

        // GET: Backup
        public async Task<IActionResult> Index()
        {
            var backups = await _backupService.GetAllBackupsAsync();
            
            // Get file info for each backup
            var backupDir = Path.Combine(_environment.ContentRootPath, "Backups");
            var backupInfos = backups.Select(fileName =>
            {
                var filePath = Path.Combine(backupDir, fileName);
                var fileInfo = new FileInfo(filePath);
                return new
                {
                    FileName = fileName,
                    Size = fileInfo.Length,
                    CreatedAt = fileInfo.CreationTime
                };
            }).ToList();

            ViewBag.BackupInfos = backupInfos;
            return View(backups);
        }

        // POST: Backup/CreateBackup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBackup()
        {
            try
            {
                var backupFileName = await _backupService.CreateBackupAsync();
                TempData["Success"] = $"تم إنشاء النسخة الاحتياطية بنجاح: {backupFileName}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"خطأ في إنشاء النسخة الاحتياطية: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Backup/Download?fileName=xxx
        public async Task<IActionResult> Download(string fileName)
        {
            try
            {
                var fileBytes = await _backupService.DownloadBackupAsync(fileName);
                return File(fileBytes, "application/x-sqlite3", fileName);
            }
            catch (FileNotFoundException)
            {
                TempData["Error"] = "ملف النسخة الاحتياطية غير موجود";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Backup/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string fileName)
        {
            try
            {
                await _backupService.DeleteBackupAsync(fileName);
                TempData["Success"] = "تم حذف النسخة الاحتياطية بنجاح";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"خطأ في حذف النسخة الاحتياطية: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Backup/RestoreFromList
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreFromList(string fileName)
        {
            try
            {
                await _backupService.RestoreFromBackupAsync(fileName);
                TempData["Success"] = "تم استعادة النسخة الاحتياطية بنجاح. يُرجى إعادة تشغيل التطبيق لتطبيق التغييرات.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"خطأ في استعادة النسخة الاحتياطية: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Backup/RestoreFromUpload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreFromUpload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "يرجى اختيار ملف للاستعادة";
                return RedirectToAction(nameof(Index));
            }

            // Validate file extension
            if (!file.FileName.EndsWith(".db", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "يجب أن يكون الملف بصيغة .db";
                return RedirectToAction(nameof(Index));
            }

            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".db");

            try
            {
                // Save uploaded file to temp location using FileStream
                using (var fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(fs);
                }

                // Restore from temp file
                await _backupService.RestoreFromUploadAsync(tempPath);
                TempData["Success"] = "تم استعادة قاعدة البيانات من الملف المرفوع بنجاح. يُرجى إعادة تشغيل التطبيق لتطبيق التغييرات.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = $"الملف غير صالح: {ex.Message}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"خطأ في استعادة النسخة الاحتياطية: {ex.Message}";
            }
            finally
            {
                // Clean up temp file
                if (System.IO.File.Exists(tempPath))
                {
                    try { System.IO.File.Delete(tempPath); } catch { }
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
