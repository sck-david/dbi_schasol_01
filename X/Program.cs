using ConsoleApp;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.Serialization;

//using var db = new FDBContext();
//// Delete + Create DB
//db.Database.EnsureDeleted();
//db.Database.EnsureCreated();
////db.Database.ExecuteSqlRaw(...)

//string connection = "Username=postgres;Password=postgres;Server=localhost;Port=5432;Database=fussball";
//var dbContextOptions = new DbContextOptionsBuilder<FDBContext>()
//    .UseNpgsql(connection)
//        .EnableSensitiveDataLogging()
//        .LogTo(Console.WriteLine, LogLevel.Information).Options
//                ;

var dbContextOptions = new DbContextOptionsBuilder<FDBContext>()
    .UseSqlite(@$"DataSource=fussball.db")
        .EnableSensitiveDataLogging()
        .LogTo(Console.WriteLine, LogLevel.Information).Options
                ;

using var db = new FDBContext(dbContextOptions);
db.Database.EnsureDeleted();
db.Database.EnsureCreated();

//await db.SeedAsync();
await db.SeedAsync();

var erg2 = db.Clubs.Include(x => x.Players).ToList();
foreach (var item in erg2)
{
    Console.WriteLine(item.ClubName);
    Console.WriteLine($"Club id: {item.Id}");
    foreach (var i2 in item.Players)
    {
        Console.WriteLine($"Player id: {i2.Id}");
        Console.WriteLine("------");
    }
    Console.WriteLine("====================================");
}