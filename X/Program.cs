using ConsoleApp;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Runtime.Serialization;
using ConsoleTables;

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MongoDB.Driver;
using System.Diagnostics;
using dbi_projekt_2023;
using X;
using static X.Services;

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

//var dbContextOptions = new DbContextOptionsBuilder<FDBContext>()
//    .UseSqlite(@$"DataSource=fussball.db")
//        .EnableSensitiveDataLogging()
//        .LogTo(Console.WriteLine, LogLevel.Information).Options
//                ;

//using var db = new FDBContext(dbContextOptions);
//db.Database.EnsureDeleted();
//db.Database.EnsureCreated();

////await db.SeedAsync();
//await db.SeedAsync();

//var erg2 = db.Clubs.Include(x => x.Players).ToList();
//foreach (var item in erg2)
//{
//    Console.WriteLine(item.ClubName);
//    Console.WriteLine($"Club id: {item.Id}");
//    foreach (var i2 in item.Players)
//    {
//        Console.WriteLine($"Player id: {i2.Id}");
//        Console.WriteLine("------");
//    }
//    Console.WriteLine("====================================");
//}

FDBContext fDBContext = new FDBContext(new DbContextOptionsBuilder<FDBContext>()
.UseSqlite(@$"DataSource=fussball.db")
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information)
                .Options
                );

fDBContext.Database.EnsureDeleted();
fDBContext.Database.EnsureCreated();

fbDatabase FDBMongoContext = fbDatabase.FromConnectionString("mongodb://localhost:27017", logging: false);
FDBMongoContext.DeleteDb();

Services service = new Services(fDBContext, FDBMongoContext);

Console.WriteLine("Benchmark Test:");

int[] anzarray = { 100, 1000, 100000 };
foreach (int anz in anzarray)
{
    Console.WriteLine($"{anz}: ");
    ConsoleTable mongo = new("", "SQL", "Mongo");
    mongo.AddRow("CREATE", service.CreateAndInsertPostgresTimer() + "ms", service.CreateAndInsertMongoTimer(false, anz) + "ms");
    mongo.AddRow("READ", service.ReadAllClubs() + "ms", service.ReadMongoAllMethodes() + "ms");
    mongo.AddRow("UPDATE", service.UpdatePostgresTimer() + "ms", service.UpdateMongoTimer() + "ms");
    mongo.AddRow("DELETE", service.DeletePostgresTimer() + "ms", service.DeleteMongoTimer() + "ms");
    Console.WriteLine(mongo);
}

Console.WriteLine("Vergleich ohne und mit Aggregation");
service.CreateAndInsertPostgresTimer();
service.CreateAndInsertMongoTimer(false, 1000);
ConsoleTable agg = new("", "SQL", "Mongo");
agg.AddRow("ohne", service.ReadClubsNoFilter().Item1 + "ms", service.ReadMongoClubsNoFilter().Item1 + "ms");
agg.AddRow("mit", service.ReadClubsWithAggregation().Item1 + "ms", service.ReadMongoClubsWithAggregation().Item1 + "ms");
Console.WriteLine(agg);

Console.WriteLine("Vergleich der Laufzeiten beim Setzen eines Index auf die Mongo-Struktur");
ConsoleTable index = new("", "CREATE", "READ", "UPDATE", "DELETE");
index.AddRow("ohne Index", service.CreateAndInsertMongoTimer(false, 1000) + "ms", service.ReadMongoAllMethodes() + "ms", service.UpdateMongoTimer() + "ms", service.DeleteMongoTimer() + "ms");
index.AddRow("mit Index", service.CreateAndInsertMongoTimer(true, 1000) + "ms", service.ReadMongoAllMethodes() + "ms", service.UpdateMongoTimer() + "ms", service.DeleteMongoTimer() + "ms");
Console.WriteLine(index);

int min = 100;
int steps = 100;
int max = 1000;

Console.WriteLine("Maximale Differenz in der Zeit");
ConsoleTable diffTable = new("", "Anz", "SQL", "Mongo");
List<Diff> diffCreate = new();
List<Diff> diffRead = new();
List<Diff> diffUpdate = new();
List<Diff> diffDelete = new();
//List<Task<long>> postgresTasks = new List<Task<long>>();
//for (int i = min; i < max; i += steps)
//{
//    postgresTasks.Add(service.CreateAndInsertPostgresTimer());
//}
//await Task.WhenAll(postgresTasks);

//List<long> postgresResults = postgresTasks.Select(task => task.Result).ToList();

for (int i = min; i < max; i += steps)
{ 
    diffCreate.Add(new Diff(i, service.CreateAndInsertPostgresTimer(), service.CreateAndInsertMongoTimer(false, i)));
    diffRead.Add(new Diff(i, service.ReadAllClubs(), service.ReadMongoAllMethodes()));
    diffUpdate.Add(new Diff(i, service.UpdatePostgresTimer(), service.UpdateMongoTimer()));
    diffDelete.Add(new Diff(i, service.DeletePostgresTimer(), service.DeleteMongoTimer()));
}

Diff MaxDiffCreate = diffCreate.OrderByDescending(x => x.Differenz).First();
Diff MaxDiffRead = diffRead.OrderByDescending(x => x.Differenz).First();
Diff MaxDiffUpdate = diffUpdate.OrderByDescending(x => x.Differenz).First();
Diff MaxDiffDelete = diffDelete.OrderByDescending(x => x.Differenz).First();

diffTable.AddRow("Create", MaxDiffCreate.Anz, MaxDiffCreate.Sql, MaxDiffCreate.Mongo);
diffTable.AddRow("Read", MaxDiffRead.Anz, MaxDiffRead.Sql, MaxDiffRead.Mongo);
diffTable.AddRow("Update", MaxDiffUpdate.Anz, MaxDiffUpdate.Sql, MaxDiffUpdate.Mongo);
diffTable.AddRow("Delete", MaxDiffDelete.Anz, MaxDiffDelete.Sql, MaxDiffDelete.Mongo);
Console.WriteLine(diffTable);