namespace GoDent.DAL.Enums
{
    /// <summary>
    /// Payment methods available in small clinics.
    /// Most transactions will be Cash in rural Egypt.
    /// </summary>
    public enum PaymentMethod
    {
        Cash = 0,
        Card = 1,
        InstaPay = 2,
        Other = 3
    }
}
