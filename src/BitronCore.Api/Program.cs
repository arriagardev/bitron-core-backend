using BitronCore.Api.Hubs;
using BitronCore.Application.Interfaces;
using BitronCore.Application.Services;
using BitronCore.Infrastructure.Mqtt;
using BitronCore.Infrastructure.Persistence;
using BitronCore.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Persistencia ───────────────────────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IInspeccionRepository, InspeccionRepository>();

// ── Aplicación ────────────────────────────────────────────────────────────────────────────────────
builder.Services.AddScoped<InspeccionService>();

// ── MQTT ───────────────────────────────────────────────────────────────────────────────────────
builder.Services.Configure<MqttOptions>(builder.Configuration.GetSection(MqttOptions.SectionName));
builder.Services.AddHostedService<MqttBackgroundService>();

// ── SignalR ──────────────────────────────────────────────────────────────────────────────────────
builder.Services.AddSignalR();
builder.Services.AddScoped<IInspeccionNotifier, SignalRInspeccionNotifier>();

// ── API ───────────────────────────────────────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
        opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Bitron Core API", Version = "v1" });
});

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// ── Esquema de base de datos ────────────────────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

// ── Middleware ────────────────────────────────────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bitron Core API v1"));

app.UseCors();
app.MapControllers();
app.MapHub<InspeccionHub>("/hubs/inspeccion");

app.Run();
