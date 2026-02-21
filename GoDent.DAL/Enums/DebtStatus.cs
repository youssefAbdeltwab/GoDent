using System.ComponentModel.DataAnnotations;

namespace GoDent.DAL.Enums
{
    public enum DebtStatus
    {
        [Display(Name = "معلق")]
        Pending = 0,

        [Display(Name = "مدفوع")]
        Paid = 1,

        [Display(Name = "متأخر")]
        Overdue = 2
    }
}
