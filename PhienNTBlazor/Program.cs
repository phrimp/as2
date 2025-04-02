using PhienNTBlazor.Components;
using Microsoft.EntityFrameworkCore;
using Repository;
using Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddQuickGridEntityFrameworkAdapter();

builder.Services.AddDbContextFactory<NewsSystemContext>(options => {
    var connectionString = builder.Configuration.GetConnectionString("NewsSystemConnection");
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<INewsArticleRepo, NewsArticleRepo>();
builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<ITagRepo, TagRepo>();
builder.Services.AddScoped<IAccountRepo, AccountRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();