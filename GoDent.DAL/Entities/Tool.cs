using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoDent.DAL.Entities
{
    /// <summary>
    /// Represents a dental instrument or equipment in the clinic inventory.
    /// Tracks quantity and alerts when stock is low (Quantity <= MinQuantity).
    /// </summary>
    public class Tool
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الأداة مطلوب")]
        [MaxLength(200, ErrorMessage = "اسم الأداة لا يمكن أن يتجاوز 200 حرف")]
        [Display(Name = "اسم الأداة")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "الوصف لا يمكن أن يتجاوز 1000 حرف")]
        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(0, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون صفر أو أكثر")]
        [Display(Name = "الكمية")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "الحد الأدنى للكمية مطلوب")]
        [Range(0, int.MaxValue, ErrorMessage = "الحد الأدنى يجب أن يكون صفر أو أكثر")]
        [Display(Name = "الحد الأدنى للكمية")]
        public int MinQuantity { get; set; }

        [Required(ErrorMessage = "السعر مطلوب")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "السعر يجب أن يكون صفر أو أكثر")]
        [Display(Name = "السعر")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "يجب اختيار القسم")]
        [Display(Name = "القسم")]
        public int DepartmentId { get; set; }

        [Display(Name = "تاريخ الإضافة")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ── Navigation Properties ──
        public virtual Department Department { get; set; } = null!;
    }
}
