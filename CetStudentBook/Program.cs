using CetStudentBook.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CetStudentBook
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<CetStudentBook.Data.ApplicationDbContext>();

                if (!db.Categories.Any())
                {
                    var book = new CetStudentBook.Models.Category { Name = "Book" };
                    var computer = new CetStudentBook.Models.Category { Name = "Computer" };
                    var novel = new CetStudentBook.Models.Category { Name = "Novel" };

                    db.Categories.AddRange(book, computer, novel);
                    db.SaveChanges();

                    db.Products.AddRange(
                        new CetStudentBook.Models.Product { Name = "Laptop", Price = 50000, CategoryId = computer.Id },
                        new CetStudentBook.Models.Product { Name = "Keyboard", Price = 1500, CategoryId = computer.Id },
                        new CetStudentBook.Models.Product { Name = "Novel A", Price = 250, CategoryId = novel.Id },
                        new CetStudentBook.Models.Product { Name = "Book B", Price = 300, CategoryId = book.Id }
                    );

                    db.SaveChanges();
                }
            }

            app.Run();
        }
    }
}
