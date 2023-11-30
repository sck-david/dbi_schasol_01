using ConsoleApp;
using dbi_projekt_2023.ConsoleApp.Domain.Interfaces;
using dbi_projekt_2023.ConsoleApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IFmService, fmServices>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FDBContext>(c => {
    c
    .UseSqlite(@$"DataSource=fussball.db")
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information);
}
);

var dbContextOptions = new DbContextOptionsBuilder<FDBContext>()
    .UseSqlite(@$"DataSource=fussball.db")
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Information).Options
                ;


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using (var scope = app.Services.CreateScope())
    {
        using (var db = scope.ServiceProvider.GetService<FDBContext>())
        {
            if (db is null)
            {
                throw new Exception("No DB!");
            }
            // New DB!!!
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            await db.SeedAsync();
        }
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
