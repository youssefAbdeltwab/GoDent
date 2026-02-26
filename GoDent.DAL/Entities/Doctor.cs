using System.ComponentModel.DataAnnotations;

namespace GoDent.DAL.Entities
{
    public class Doctor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الطبيب مطلوب")]
        [MaxLength(150, ErrorMessage = "الاسم لا يمكن أن يتجاوز 150 حرف")]
        [Display(Name = "اسم الطبيب")]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        [MaxLength(20, ErrorMessage = "رقم الهاتف لا يمكن أن يتجاوز 20 رقم")]
        [Display(Name = "رقم الهاتف")]
        public string? PhoneNumber { get; set; }

        [MaxLength(200, ErrorMessage = "التخصص لا يمكن أن يتجاوز 200 حرف")]
        [Display(Name = "التخصص")]
        public string? Specialty { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "تاريخ الإضافة")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ── Navigation ──
        public virtual ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
    }
}
