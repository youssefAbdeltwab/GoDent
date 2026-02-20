namespace GoDent.DAL.Enums
{
    /// <summary>
    /// Categories for clinic expenses. This enables grouping in reports.
    /// We pre-define the most common categories for a dental clinic.
    /// </summary>
    public enum ExpenseCategory
    {
        Rent = 0,
        Materials = 1,
        Utilities = 2,
        Salary = 3,
        Equipment = 4,
        Maintenance = 5,
        Other = 6
    }
}
