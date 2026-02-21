using Microsoft.EntityFrameworkCore;
using GoDent.DAL.Data;
using GoDent.BLL.Services;
using GoDent.BLL.Service.Abstraction;

namespace GoDent
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ── Register AppDbContext with SQLite ──
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ── Register BLL Services ──
            builder.Services.AddScoped<IPatientService, PatientService>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<IToolService, ToolService>();
            builder.Services.AddScoped<IWhatsAppService, WhatsAppService>();
            builder.Services.AddScoped<IVisitService, VisitService>();
            builder.Services.AddScoped<ITreatmentService, TreatmentService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IExpenseService, ExpenseService>();
            builder.Services.AddScoped<IClinicDebtService, ClinicDebtService>();
            builder.Services.AddScoped<IFinanceDashboardService, FinanceDashboardService>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
