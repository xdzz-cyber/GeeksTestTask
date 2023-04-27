using GeeksTestTask.DLL;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>();
builder.Services.AddScoped<CustomInitializer>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

using (var scope = app.Services.CreateScope())
{
    var customInitializer = scope.ServiceProvider.GetRequiredService<CustomInitializer>();
    customInitializer.Initialize();
}

app.Run();
