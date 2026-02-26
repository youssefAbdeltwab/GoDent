using GoDent.BLL.Service.Abstraction;
using Microsoft.AspNetCore.Mvc;

namespace GoDent.Controllers
{
    public class BackupController : Controller
    {
        private readonly IBackupService _backupService;

        public BackupController(IBackupService backupService)
        {
            _backupService = backupService;
        }

        // GET: Backup
        public IActionResult Index()
        {
            var backups = _backupService.GetAllBackups();
            return View(backups);
        }

        // POST: Backup/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            try
            {
                var fileName = await _backupService.CreateBackupAsync();
                TempData["Success"] = $"تم إنشاء النسخة الاحتياطية بنجاح: {fileName}";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"فشل إنشاء النسخة الاحتياطية: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Backup/Download?fileName=xxx
        public IActionResult Download(string fileName)
        {
            var path = _backupService.GetBackupPath(fileName);
            if (path == null)
                return NotFound();

            var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return File(stream, "application/octet-stream", fileName);
        }

        // POST: Backup/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string fileName)
        {
            if (_backupService.DeleteBackup(fileName))
            {
                TempData["Success"] = "تم حذف النسخة الاحتياطية بنجاح";
            }
            else
            {
                TempData["Error"] = "لم يتم العثور على النسخة الاحتياطية";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Backup/Restore?fileName=xxx (restore from existing backup)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(string fileName)
        {
            try
            {
                await _backupService.RestoreFromBackupAsync(fileName);
                TempData["Success"] = $"تم استعادة قاعدة البيانات بنجاح من: {fileName}. تم إنشاء نسخة أمان قبل الاستعادة.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"فشل استعادة قاعدة البيانات: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Backup/Upload (restore from uploaded .db file)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile backupFile)
        {
            if (backupFile == null || backupFile.Length == 0)
            {
                TempData["Error"] = "يرجى اختيار ملف قاعدة بيانات";
                return RedirectToAction(nameof(Index));
            }

            if (!backupFile.FileName.EndsWith(".db", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "يجب أن يكون الملف بصيغة .db";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var stream = backupFile.OpenReadStream();
                await _backupService.RestoreFromUploadAsync(stream);
                TempData["Success"] = "تم استعادة قاعدة البيانات من الملف المرفوع بنجاح. تم إنشاء نسخة أمان قبل الاستعادة.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"فشل استعادة قاعدة البيانات: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
