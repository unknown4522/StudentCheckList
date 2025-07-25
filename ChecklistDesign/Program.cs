using StudentFilesFrontend.Services;

var builder = WebApplication.CreateBuilder(args);

// ✅ Register Razor Pages
builder.Services.AddRazorPages();

// ✅ Register session services (BEFORE Build)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Register HTTP clients for your services
builder.Services.AddHttpClient<StudentfilesService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7116/");
});

////builder.Services.AddHttpClient<CampusService>(client =>
////{
////    client.BaseAddress = new Uri("https://localhost:7116/");
////});

////builder.Services.AddHttpClient<SchoolyearService>(client =>
////{
////    client.BaseAddress = new Uri("https://localhost:7116/");
//});
builder.Services.AddScoped<StudentfilesService>();
builder.Services.AddScoped<CampusService>();
builder.Services.AddScoped<SchoolyearService>();
// ✅ Optional: if you also want a general HttpClient
builder.Services.AddHttpClient(); // This is fine, but not required if not used

// ✅ Build the app
var app = builder.Build();

// ✅ Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // ✅ Session must come BEFORE UseAuthorization
app.UseAuthorization();

// ✅ Default route to Login page
app.MapGet("/", context =>
{
    context.Response.Redirect("/Login");
    return Task.CompletedTask;
});

app.MapRazorPages();
app.Run();
