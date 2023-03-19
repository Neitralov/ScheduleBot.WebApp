var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<DataBaseProvider>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromDays(365);
        options.Cookie.MaxAge = TimeSpan.FromDays(365);
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

LoadConfigurationToEnvironment();

app.Run();

void LoadConfigurationToEnvironment()
{
    Environment.SetEnvironmentVariable("FirstCorpsSchedulePath", app.Configuration["FirstCorpsSchedulePath"]);
    Environment.SetEnvironmentVariable("SecondCorpsSchedulePath", app.Configuration["SecondCorpsSchedulePath"]);
    Environment.SetEnvironmentVariable("ThirdCorpsSchedulePath", app.Configuration["ThirdCorpsSchedulePath"]);
    Environment.SetEnvironmentVariable("FourthCorpsSchedulePath", app.Configuration["FourthCorpsSchedulePath"]);
}