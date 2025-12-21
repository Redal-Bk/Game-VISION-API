using Game_Vision.AppExtention;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddAuthExtention(builder.Configuration);
builder.Services.AddHttpContextAccessor();


builder.Host.UseGameVisionLogging();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
// app.UseGameVisionRequestLogging(); // اگر لازم داشتی آنکامنت کن
app.UseStaticFiles(); // یا app.MapStaticAssets() اگر اکستنشن خودت داری

app.UseRouting();

app.UseAuthentication();  // حتماً قبل از UseAuthorization
app.UseAuthorization();

app.MapStaticAssets(); // اگر اکستنشن جداگانه داری

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();