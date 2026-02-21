using GoDent.BLL.Service.Abstraction;
using GoDent.DAL.Entities;
using Microsoft.Extensions.Configuration;

namespace GoDent.BLL.Services
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly IConfiguration _configuration;

        public WhatsAppService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateShortageAlertUrl(Tool tool)
        {
            string? doctorPhone = _configuration["WhatsApp:DoctorPhone"];
            // Build Arabic message
            string message = $@" تنبيه نقص مخزون

            الأداة: {tool.Name}
            الكمية الحالية: {tool.Quantity}
            الحد الأدنى: {tool.MinQuantity}

            يرجى إعادة الطلب";

            // URL encode the message
            var encodedMessage = Uri.EscapeDataString(message);

            // Generate wa.me URL
            return $"https://wa.me/{doctorPhone}?text={encodedMessage}";
        }

        public string GeneratePostExtractionInstructionsUrl(Patient patient)
        {
            // Format patient phone: 01012345678 -> 2001012345678
            var formattedPhone = FormatEgyptianPhone(patient.PhoneNumber);

            // Build Arabic message with post-extraction instructions
            string message = $@"مرحباً {patient.FullName}

            تعليمات ما بعد خلع الأسنان:

            ✅ تجنب المشروبات الساخنة لمدة 24 ساعة
            ✅ لا تغسل فمك في اليوم الأول
            ✅ تناول الأدوية الموصوفة بانتظام
            ✅ ضع كمادات باردة في حالة التورم
            ✅ احصل على قسط من الراحة لمدة 24 ساعة
            ⚠️ اتصل بنا فوراً في حالة النزيف الشديد

            عيادة GoDent
            نتمنى لك الشفاء العاجل 🌸";

            var encodedMessage = Uri.EscapeDataString(message);
            return $"https://wa.me/{formattedPhone}?text={encodedMessage}";
        }

        public string GenerateDebtReminderUrl(Patient patient, decimal debtAmount)
        {
            // Format patient phone
            var formattedPhone = FormatEgyptianPhone(patient.PhoneNumber);

            // Build Arabic message for debt reminder
            string message = $@"مرحباً {patient.FullName}

            نأمل أن تكون بخير 🌸

            نود تذكيرك بأن لديك رصيد مستحق:
            💰 المبلغ: {debtAmount:N2} جنيه

            يرجى التواصل معنا لترتيب الدفع

            عيادة GoDent
            شكراً لتعاونك";

            var encodedMessage = Uri.EscapeDataString(message);
            return $"https://wa.me/{formattedPhone}?text={encodedMessage}";
        }

        public string GenerateTodayAppointmentReminderUrl(Appointment appointment)
        {
            // Format patient phone
            var formattedPhone = FormatEgyptianPhone(appointment.Patient.PhoneNumber);

            // Build Arabic message for today's appointment
            string message = $@"مرحباً {appointment.Patient.FullName}

            تذكير بموعدك اليوم 📅

            🕐 الوقت: {appointment.StartTime?.ToString(@"hh\:mm")} 
            📍 عيادة GoDent

            نرجو الحضور في الموعد المحدد
            في انتظارك 🌸";

            var encodedMessage = Uri.EscapeDataString(message);
            return $"https://wa.me/{formattedPhone}?text={encodedMessage}";
        }

        public string GenerateTomorrowAppointmentReminderUrl(Appointment appointment)
        {
            // Format patient phone
            var formattedPhone = FormatEgyptianPhone(appointment.Patient.PhoneNumber);

            // Build Arabic message for tomorrow's appointment
            string message = $@"مرحباً {appointment.Patient.FullName}

            تذكير بموعدك غداً 📅

            📆 التاريخ: {appointment.AppointmentDate:yyyy-MM-dd}
            🕐 الوقت: {appointment.StartTime?.ToString(@"hh\:mm")}
            📍 عيادة GoDent

            نتطلع لرؤيتك 🌸";

            var encodedMessage = Uri.EscapeDataString(message);
            return $"https://wa.me/{formattedPhone}?text={encodedMessage}";
        }

        // Helper method to format Egyptian phone numbers
        private string FormatEgyptianPhone(string phoneNumber)
        {
            // Remove any spaces or dashes
            phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "");
            
            // If starts with 0, remove it and add country code 20
            if (phoneNumber.StartsWith("0"))
            {
                return "20" + phoneNumber.Substring(1);
            }
            
            // If already has country code, return as is
            if (phoneNumber.StartsWith("20"))
            {
                return phoneNumber;
            }
            
            // Otherwise, assume it needs country code
            return "20" + phoneNumber;
        }
    }
}
