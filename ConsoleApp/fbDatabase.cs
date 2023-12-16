using Bogus;
using Bogus.DataSets;
using DnsClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using static Bogus.DataSets.Name;

namespace dbi_projekt_2023
{
    public class fbDatabase
    {
        // Value Objects
        public record Address(string Street, string StreetNr, int Zip, string City, string State);
        
        // Collections
        public record Player(string firstname, string lastname, DateTime gebDat,
            [property:BsonRepresentation(BsonType.Decimal128, AllowTruncation = true)]
        decimal marketvalue,
            int gender, Address address, ObjectId Id = default
        );
        public record Club(string clubName, Address BillingAddress, int currentLeagueId, ObjectId Id = default)
        {
            public List<Player> players { get; private set; } = new();
        }
       
        public MongoClient Client { get; }
        public IMongoDatabase Db { get; }
        public IMongoCollection<Player> Players => Db.GetCollection<Player>("players");
        public IMongoCollection<Club> Clubs => Db.GetCollection<Club>("clubs");

        /// <summary>
        /// Konfiguriert die Datenbank für einen Connection string.
        /// </summary>
        public static fbDatabase FromConnectionString(string connectionString, bool logging = false)
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
            if (logging)
            {
                settings.ClusterConfigurator = cb =>
                {
                    cb.Subscribe<CommandStartedEvent>(e =>
                    {
                        // Bei update Statements geben wir die Anweisung aus, wie wir sie in
                        // der Shell eingeben könnten.
                        if (e.Command.TryGetValue("updates", out var updateCmd))
                        {
                            var collection = e.Command.GetValue("update");
                            var isUpdateOne = updateCmd[0]["q"].AsBsonDocument.Contains("_id");
                            foreach (var cmd in updateCmd.AsBsonArray)
                            {
                                Console.WriteLine($"db.getCollection(\"{collection}\").{(isUpdateOne ? "updateOne" : "updateMany")}({updateCmd[0]["q"]}, {updateCmd[0]["u"]})");
                            }
                        }
                        // Bei aggregate Statements geben wir die Anweisung aus, wie wir sie in
                        // der Shell eingeben könnten.
                        if (e.Command.TryGetValue("aggregate", out var aggregateCmd))
                        {
                            var collection = aggregateCmd.AsString;
                            Console.WriteLine($"db.getCollection(\"{collection}\").aggregate({e.Command["pipeline"]})");
                        }

                        // Bei Filter Statements geben wir die find Anweisung aus.
                        if (e.Command.TryGetValue("find", out var findCmd))
                        {
                            var collection = findCmd.AsString;
                            Console.WriteLine($"db.getCollection(\"{collection}\").find({e.Command["filter"]})");
                        }
                    });
                };
            }
            var client = new MongoClient(settings);
            var db = client.GetDatabase("fussball");
            // LowerCase property names.
            var conventions = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreIfNullConvention(ignoreIfNull: true)
            };
            ConventionRegistry.Register(nameof(CamelCaseElementNameConvention), conventions, _ => true);
            return new fbDatabase(client, db);
        }

        private fbDatabase(MongoClient client, IMongoDatabase db)
        {
            Client = client;
            Db = db;
        }

        public void DeleteDb()
        {
            Db.DropCollection("players");
            Db.DropCollection("clubs");
        }

        /// <summary>
        /// Löscht die Datenbank und befüllt sie mit Musterdaten.
        /// </summary>
        public void Seed()
        {
            Db.DropCollection("players");
            Db.DropCollection("clubs");

            Randomizer.Seed = new Random(944);
            var faker = new Faker("de");

            var states = new Dictionary<string, (int zip, string name)[]>()
            {
                {"W", new (int, string)[] {(1010, "Wien"), (1020, "Wien"), (1030, "Wien"), (1040, "Wien") } },
                {"N", new (int, string)[] {(2500, "Baden"), (3100, "Sankt Pölten"), (3300, "Amstetten") } },
                {"B", new (int, string)[] { (7000, "Eisenstadt"), (7210, "Mattersburg"), (7100, "Neusiedl am See") } }
            };


            int counter = 1;
            var players = new Faker<Player>("de").CustomInstantiator(f =>
            {
            var addresses = new Faker<Address>("de").CustomInstantiator(f =>
            {
                var state = f.Random.ListItem(states.Keys.ToList());
                var city = f.Random.ListItem(states[state]);
                return new Address(
                    Street: f.Address.StreetName(), StreetNr: f.Address.BuildingNumber(),
                    Zip: city.zip, City: city.name,
                    State: state);
            })
            .Generate(f.Random.Int(1, 3))
            .ToList();

                return new Player(

                    firstname: f.Person.FirstName,
                    lastname: f.Person.LastName,
                    gebDat: f.Person.DateOfBirth.ToUniversalTime(),
                    gender: 0,
                    Id: new ObjectId(counter++.ToString("x24")),
                    address: addresses[0],
                    marketvalue: f.Random.Decimal(0, 100000000)
                ) ;

            })
            .Generate(50)
            .ToList();
            Players.InsertMany(players);

            counter = 1;
            var clubs = new Faker<Club>("de").CustomInstantiator(f =>
            {
                var addresses = new Faker<Address>("de").CustomInstantiator(f =>
                {
                    var state = f.Random.ListItem(states.Keys.ToList());
                    var city = f.Random.ListItem(states[state]);
                    return new Address(
                        Street: f.Address.StreetName(), StreetNr: f.Address.BuildingNumber(),
                        Zip: city.zip, City: city.name,
                        State: state);
                }).Generate(f.Random.Int(1, 3))
                .ToList();

                var c = new Club(
                    clubName: f.Address.City(),
                    BillingAddress: addresses[0],
                    currentLeagueId: f.Random.Int(1, 4),
                    Id: new ObjectId(counter++.ToString("x24"))
                    );

                int r = 14;
                var l = new List<Player>(players);
                var neueSpieler = new List<Player>();
                for (int i = 0; i < r; i++)
                {
                    var t = f.PickRandom<Player>(l);
                    c.players.Add(t);
                    l.Remove(t);
                }

                
                
                return c;
            })
            .Generate(10)
            .ToList();
            Clubs.InsertMany(clubs);

            
        }
    }
}
