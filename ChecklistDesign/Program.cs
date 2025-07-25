using StudentFilesFrontend.Services;

var builder = WebApplication.CreateBuilder(args);

// ✅ Register Razor Pages
builder.Services.AddRazorPages();

// ✅ Register session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Register HTTP clients for your services
builder.Services.AddHttpClient<StudentfilesService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7116/"); // ✅ Replace with your actual API base URL
});

// ✅ Register other services (no duplicate AddHttpClient calls needed for these)

builder.Services.AddScoped<CampusService>();
builder.Services.AddScoped<SchoolyearService>();

// ✅ Optional: general HttpClient
builder.Services.AddHttpClient(); // OK to keep if used elsewhere

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
app.UseSession(); // ✅ Session before Authorization
app.UseAuthorization();

// ✅ Default route to Login page
app.MapGet("/", context =>
{
    context.Response.Redirect("/Login");
    return Task.CompletedTask;
});

app.MapRazorPages();
app.Run();
