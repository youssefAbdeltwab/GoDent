using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GoDent.DAL.Enums;

namespace GoDent.DAL.Entities
{
    /// <summary>
    /// The Patient entity is the central hub of the system.
    /// Every module (Appointments, Treatments, Payments) links back to a Patient.
    /// </summary>
    public class Patient
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المريض مطلوب")]
        [MaxLength(150, ErrorMessage = "الاسم لا يمكن أن يتجاوز 150 حرف")]
        [Display(Name = "اسم المريض")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        [MaxLength(20, ErrorMessage = "رقم الهاتف لا يمكن أن يتجاوز 20 رقم")]
        [Display(Name = "رقم الهاتف")]
        [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "رقم الهاتف غير صحيح")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "النوع مطلوب")]
        [Display(Name = "النوع")]
        public Gender Gender { get; set; }

        [Display(Name = "تاريخ الميلاد")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "تاريخ الميلاد مطلوب")]
        public DateOnly DateOfBirth { get; set; }

        [MaxLength(300, ErrorMessage = "العنوان لا يمكن أن يتجاوز 300 حرف")]
        [Display(Name = "العنوان")]
        public string? Address { get; set; }

        [MaxLength(1000, ErrorMessage = "التاريخ المرضي لا يمكن أن يتجاوز 1000 حرف")]
        [Display(Name = "التاريخ المرضي")]
        public string? MedicalHistory { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "تاريخ التسجيل")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "الرصيد المستحق")]
        public decimal CurrentDebt { get; set; } = 0;

        // ── Navigation Properties ──
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
        public virtual ICollection<Treatment> Treatments { get; set; } = new List<Treatment>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
