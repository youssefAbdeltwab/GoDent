using System.ComponentModel.DataAnnotations;

namespace GoDent.DAL.Enums
{
    public enum ExpenseCategory
    {
        [Display(Name = "إيجار")]
        Rent = 0,

        [Display(Name = "مواد طبية")]
        Materials = 1,

        [Display(Name = "مختبرات")]
        Labs = 2,

        [Display(Name = "رواتب")]
        Salaries = 3,

        [Display(Name = "مرافق (كهرباء/ماء)")]
        Utilities = 4,

        [Display(Name = "صيانة")]
        Maintenance = 5,

        [Display(Name = "تسويق")]
        Marketing = 6,

        [Display(Name = "معدات")]
        Equipment = 7,

        [Display(Name = "أخرى")]
        Other = 8
    }
}
