namespace GoDent.DAL.Enums
{
    /// <summary>
    /// Status of an individual tooth. Used in the Dental Chart (Teeth Map).
    /// Color-coding in the UI will map to these values:
    ///   Healthy=White, Caries=Red, Filled=Green, Extracted=Grey, etc.
    /// </summary>
    public enum ToothStatus
    {
        Healthy = 0,
        Caries = 1,
        Filled = 2,
        Extracted = 3,
        Crown = 4,
        Bridge = 5,
        RootCanal = 6,
        Implant = 7,
        Missing = 8
    }
}
