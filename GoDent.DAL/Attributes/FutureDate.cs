using System;
using System.ComponentModel.DataAnnotations;

public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // Allow empty values (use [Required] if needed)
        if (value == null)
            return ValidationResult.Success;

        if (value is not DateTime date)
            return new ValidationResult("قيمة التاريخ غير صحيحة");

        // Compare only dates, ignore time
        if (date.Date < DateTime.Today)
            return new ValidationResult(ErrorMessage ?? "التاريخ يجب أن يكون اليوم أو تاريخ مستقبلي");

        return ValidationResult.Success;
    }
}