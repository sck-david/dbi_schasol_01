using dbi_projekt_2023.ConsoleApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using ConsoleApp.Domain.Entities;
using System.Runtime.Intrinsics.X86;

namespace ConsoleApp
{
    public class FDBContext : DbContext
    {

        public FDBContext() : this(
            new DbContextOptionsBuilder<FDBContext>()
            .UseNpgsql(
                "Server = schasol.postgres.database.azure.com; Database=postgres;Port=5432;User Id = raphi; Password=schasol123!; Ssl Mode = Require;"
                )
            .Options)
        {
        }

        public FDBContext(DbContextOptions<FDBContext> options) : base(options)
        {
        }


        public DbSet<Club> Clubs => Set<Club>();
        public DbSet<Player> Players => Set<Player>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public async Task<int> deleteDB()
        {
            await Database.EnsureDeletedAsync();
            await Database.EnsureCreatedAsync();
            return 1;
        }
        public void Seed(int anz)
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();

            Randomizer.Seed = new Random(944);
            var faker = new Faker("de");

            var states = new Dictionary<string, (int zip, string name)[]>()
            {
                {"W", new (int, string)[] {(1010, "Wien"), (1020, "Wien"), (1030, "Wien"), (1040, "Wien") } },
                {"N", new (int, string)[] {(2500, "Baden"), (3100, "Sankt Pölten"), (3300, "Amstetten") } },
                {"B", new (int, string)[] { (7000, "Eisenstadt"), (7210, "Mattersburg"), (7100, "Neusiedl am See") } }
            };


            var players = new Faker<Player>("de").CustomInstantiator(f =>
            {
                var addresses = new Faker<Address>("de").CustomInstantiator(f =>
                {
                    var state = f.Random.ListItem(states.Keys.ToList());
                    var city = f.Random.ListItem(states[state]);
                    return new Address(
                        street: f.Address.StreetName(), 
                        streetN: f.Address.BuildingNumber(),
                        zip: city.zip, 
                        city: city.name,
                        state: state);
                })
                .Generate(f.Random.Int(1, 3))
                .ToList();

                return new Player(

                    firstname: f.Person.FirstName,
                    lastname: f.Person.LastName,
                    gebDat: f.Person.DateOfBirth.ToUniversalTime(),
                    gender: 0,
                    address: addresses[0],
                    marketvalue: f.Random.Decimal(0, 100000000)
                );

            })
            .Generate(anz*14)
            .ToList();
            Players.AddRange(players);

            SaveChanges();


            var clubs = new Faker<Club>("de").CustomInstantiator(f =>
            {
                var addresses = new Faker<Address>("de").CustomInstantiator(f =>
                {
                    var state = f.Random.ListItem(states.Keys.ToList());
                    var city = f.Random.ListItem(states[state]);
                    return new Address(
                        street: f.Address.StreetName(),
                        streetN: f.Address.BuildingNumber(),
                        zip: city.zip,
                        city: city.name,
                        state: state);
                }).Generate(f.Random.Int(1, 3))
                .ToList();

                var c = new Club(
                    clubName: f.Address.City(),
                    billingAddress: addresses[0],
                    currentLeagueId: f.Random.Int(1, 4)
                    );

                int r = 14;
                var l = new List<Player>(players);
                for (int i = 0; i < r; i++)
                {
                    var t = f.PickRandom<Player>(l);
                    c.Players.Add(t);
                    l.Remove(t);
                }



                return c;
            })
            .Generate(anz)
            .ToList();
            Clubs.AddRangeAsync(clubs);

            SaveChanges();
        }

    }
}
