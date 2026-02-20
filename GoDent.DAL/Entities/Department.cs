using System.ComponentModel.DataAnnotations;

namespace GoDent.DAL.Entities
{
    /// <summary>
    /// Represents a department in the dental clinic.
    /// Departments are static data seeded in the database (Surgery, Radiology, etc.)
    /// </summary>
    public class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم القسم مطلوب")]
        [MaxLength(100, ErrorMessage = "اسم القسم لا يمكن أن يتجاوز 100 حرف")]
        [Display(Name = "اسم القسم")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "الوصف لا يمكن أن يتجاوز 500 حرف")]
        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ── Navigation Properties ──
        public virtual ICollection<Tool> Tools { get; set; } = new List<Tool>();
    }
}
