using PPDDocumentation.BusinessLogic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddSingleton<IMissionStatementService, MissionStatementService>();
builder.Services.AddSingleton<IGoalService, GoalService>();
builder.Services.AddSingleton<IActionService, ActionService>();
builder.Services.AddSingleton<IJobService, JobService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
