using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using ConsoleApp;
using ConsoleApp.Domain.Entities;
using dbi_projekt_2023;
using dbi_projekt_2023.ConsoleApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace X
{ 

  
        public class Services 
        {
            public class Diff
            {
                public int Anz { get; set; }
                public long Sql { get; set; }
                public long Mongo { get; set; }
                public long Differenz
                {
                    get
                    {
                        return Math.Abs(Sql - Mongo);
                    }
                }
                public Diff(int anz, long sql, long mongo)
                {
                    Anz = anz;
                    Sql = sql;
                    Mongo = mongo;
                }
            }
            public FDBContext fDBContext { get; set; }
            public fbDatabase FDBMongoContext { get; set; }

            public Services(FDBContext fDBContextq, fbDatabase FDBMongoContextq)
            {
                fDBContext = fDBContextq;
                FDBMongoContext = FDBMongoContextq;
                //CreateAndInsertPostgresTimer(1);
                //CreateAndInsertMongoTimer(true, 1);
            }

            //Postgres Create
            public long CreateAndInsertPostgresTimer(int anz)
            {
                fDBContext.deleteDB();
                Stopwatch timer = new();
                timer.Start();

                fDBContext.Seed(anz);

                timer.Stop();
                return timer.ElapsedMilliseconds;
            }

            //Postgres Read
            public long ReadAllClubs()
            {
                long timer = 0;
                (long temp, List<Club> templist) = ReadClubsNoFilter();
                timer += temp;
                (temp, templist) = ReadClubsWithFilter();
                timer += temp;
                (temp, templist) = ReadClubsWithFilterAndProjektion();
                timer += temp;
                (temp, templist) = ReadClubsWithFilterProjektionAndSorting();
                timer += temp;
                (temp, int count) = ReadClubsWithAggregation();
                timer += temp;
                return timer;
            }

            public (long, List<Club>) ReadClubsNoFilter()
            {
                //CreateAndInsertPostgresTimer(anz);
                Stopwatch timer = new();

                timer.Start();
                var personen = fDBContext.Clubs.Include(x => x.Players).ToList();
                timer.Stop();

                return (timer.ElapsedMilliseconds, personen);
            }

            public (long, List<Club>) ReadClubsWithFilter()
            {
                //CreateAndInsertPostgresTimer(anz);
                Stopwatch timer = new();

                timer.Start();
                var clubs = fDBContext.Clubs.Include(x => x.Players).ToList().FindAll(x => x.currentLeagueId > 2);
                timer.Stop();

                return (timer.ElapsedMilliseconds, clubs);
            }

            public (long, List<Club>) ReadClubsWithFilterAndProjektion()
            {
                //CreateAndInsertPostgresTimer(anz);
                var clubs = fDBContext.Clubs.Include(x => x.Players).ToList();
                Stopwatch timer = new();

                timer.Start();
                var query = (from c in clubs.AsEnumerable()
                             where c.currentLeagueId > 2
                             select new { c.Id, c.ClubName });
                timer.Stop();

                clubs = new();

                query.ToList().ForEach(club =>
                {
                    Club c = new Club();
                    c.Id = club.Id;
                    c.ClubName = club.ClubName;
                    clubs.Add(c);
                });

                return (timer.ElapsedMilliseconds, clubs);
            }

        public (long, List<Club>) ReadClubsWithFilterProjektionAndSorting()
        {
            //CreateAndInsertPostgresTimer(anz);
            var clubs = fDBContext.Clubs.Include(x => x.Players).ToList();
            Stopwatch timer = new();
            timer.Start();

            var personenFilterProjektionSorting =
            (from club in clubs.AsEnumerable()
             where club.currentLeagueId < 5
             orderby club.ClubName
             select new { club.Id, club.ClubName });

            timer.Stop();

            clubs = new();

            personenFilterProjektionSorting.ToList().ForEach(club =>
            {
                Club c = new Club();
                c.Id = club.Id;
                c.ClubName = club.ClubName;
                clubs.Add(c);
            });

                    return (timer.ElapsedMilliseconds, clubs);
            }

            public (long, int) ReadClubsWithAggregation()
            {
                //CreateAndInsertPostgresTimer(anz);
                var clubs = fDBContext.Clubs.Include(x => x.Players).ToList();
                Stopwatch timer = new();
                timer.Start();

            var personenAggregate = fDBContext.Clubs
            .Include(X => X.Players)
            .ToList();


                int maxLeagueId = personenAggregate.Any() ? personenAggregate.Max(x => x.currentLeagueId) : 0;

            timer.Stop();
                return (timer.ElapsedMilliseconds, maxLeagueId);
            }

            public List<Player> GetPlayersPerClub(int id)
            {
                Club c = fDBContext.Clubs.Include(x => x.Players).ToList().FirstOrDefault(x => x.Id == id);
                if (c == null)
                    return new List<Player>();
                else
                    return c.Players.ToList();

            }

            //Postgres Update
            public long UpdatePostgresTimer()
            {
                //CreateAndInsertPostgresTimer(anz);
                var clubs = fDBContext.Clubs.Include(x => x.Players).ToList();
                var players = fDBContext.Players.Include(x => x.address).ToList();

                Stopwatch timer = new();
                timer.Start();
                foreach (var c in clubs)
                {
                    c.ClubName = c.ClubName + "Test";
                }
                foreach (var p in players)
                {
                    p.firstname = p.firstname + "Test";
                }
                fDBContext.SaveChanges();

                timer.Stop();
                return timer.ElapsedMilliseconds;
            }

            //Postgres Delete
            public long DeletePostgresTimer()
            {
            //CreateAndInsertPostgresTimer(anz);
            var clubs = fDBContext.Clubs.Include(x => x.Players).ToList();
            var players = fDBContext.Players.Include(x => x.address).ToList();
            Stopwatch timer = new();
                timer.Start();

            fDBContext.Clubs.RemoveRange(clubs);
            fDBContext.Players.RemoveRange(players);

            fDBContext.SaveChanges();

                timer.Stop();
                return timer.ElapsedMilliseconds;
            }

            //Postgres Geraet Add
            public long AddPlayerPostgresTimer(Player player)
            {
                Stopwatch timer = new();
                timer.Start();
                fDBContext.Players.Add(player);
                fDBContext.SaveChanges();
                timer.Stop();
                return timer.ElapsedMilliseconds;
            }

            //Mongo
            public List<Player> GetPlayerPerClubMongo(string id)
            {
                if (id == null)
                    return new List<Player>();
                else
                {
                var players = new List<Player>();
                var club = FDBMongoContext.Clubs.AsQueryable().Where(p => p.players != null && p.Id > new ObjectId(id)).ToList();
                if (club.Count > 0)
                {
                    foreach (var c in club)
                    {
                        players.AddRange((IEnumerable<Player>)c.players);
                    }
                }
                return players;
            }
            }
            //Mongo Create
            public long CreateAndInsertMongoTimer(bool withIndex, int anz)
            {
                FDBMongoContext.DeleteDb();
                Stopwatch timer = new();
                timer.Start();

                if (withIndex)
                FDBMongoContext.Seed(anz);
                else
                FDBMongoContext.Seed(anz);

                timer.Stop();
                return timer.ElapsedMilliseconds;
            }

            //Mongo Read
            public long ReadMongoAllMethodes()
            {
                long timer = 0;
                (long temp, List<fbDatabase.Club> templist) = ReadMongoClubsNoFilter();
                timer += temp;
                (temp, templist) = ReadMongoClubsWithFilter();
                timer += temp;
                (temp, templist) = ReadMongoClubsWithFilterAndProjection();
                timer += temp;
                (temp, templist) = ReadMongoTimerWithFilterProjektionAndSorting();
                timer += temp;
                (temp, int players) = ReadMongoClubsWithAggregation();
                timer += temp;
                return timer;
            }

        //public (long, List<Club>) ReadMongoClubsNoFilter()
        //{
        //    //CreateAndInsertMongoTimer(false, anz);
        //    Stopwatch timer = new();
        //    timer.Start();
        //    List<Club> clubs = new List<Club>();
        //    FDBMongoContext.Clubs.AsQueryable().ToList()
        //    .ForEach(c => clubs.Add(new Club(
        //        c.clubName, 
        //        new Address(c.BillingAddress.Street,c.BillingAddress.StreetNr, c.BillingAddress.Zip, c.BillingAddress.City, c.BillingAddress.State), 
        //        c.currentLeagueId
        //        )));
        //    timer.Stop();
        //    return (timer.ElapsedMilliseconds, clubs);
        //}

        public (long, List<fbDatabase.Club>) ReadMongoClubsNoFilter()
        {
            //CreateAndInsertMongoTimer(false, anz);
            Stopwatch timer = new();
            timer.Start();
            List<fbDatabase.Club> clubs = FDBMongoContext.Clubs.AsQueryable().ToList();
            timer.Stop();
            return (timer.ElapsedMilliseconds, clubs);
        }

        public (long, List<fbDatabase.Club>) ReadMongoClubsWithFilter()
            {
                //CreateAndInsertMongoTimer(false, anz);
                Stopwatch timer = new();
                timer.Start();
                List<fbDatabase.Club> clubs = FDBMongoContext.Clubs.Find(x => x.currentLeagueId > 2).ToList();
                timer.Stop();
                return (timer.ElapsedMilliseconds, clubs);
            }

            public (long, List<fbDatabase.Club>) ReadMongoClubsWithFilterAndProjection()
            {
                //CreateAndInsertMongoTimer(false, anz);
                List< fbDatabase.Club> clubs = new();
                Stopwatch timer = new();
                timer.Start();
                var clubsFilterProjektion = FDBMongoContext.Clubs
                    .Find(x => x.currentLeagueId > 2)
                    .Project(x => new { x.Id, x.clubName, x.BillingAddress, x.currentLeagueId })
                    .ToList();
                timer.Stop();
                clubsFilterProjektion.ToList().ForEach(club =>
                {
                    fbDatabase.Club c = new(club.clubName, club.BillingAddress, club.currentLeagueId);
                    clubs.Add(c);
                });
                return (timer.ElapsedMilliseconds, clubs);
            }

            public (long, List<fbDatabase.Club>) ReadMongoTimerWithFilterProjektionAndSorting()
            {
                //CreateAndInsertMongoTimer(false, anz);
                List< fbDatabase.Club> clubs = new();
                Stopwatch timer = new();
                timer.Start();
                var clubsFilterProjektionSorting = FDBMongoContext.Clubs
                    .Find(x => x.currentLeagueId > 2)
                    .Project(x => new { x.Id, x.clubName, x.BillingAddress, x.currentLeagueId })
                    .SortBy(x => x.clubName)
                    .ToList();
                timer.Stop();
                clubsFilterProjektionSorting.ToList().ForEach(club =>
                {
                    fbDatabase.Club c = new(club.clubName, club.BillingAddress, club.currentLeagueId);
                    clubs.Add(c);
                });
                return (timer.ElapsedMilliseconds, clubs);
            }

            public (long, int) ReadMongoClubsWithAggregation()
            {
                //CreateAndInsertMongoTimer(false, anz);
                Stopwatch timer = new();
                timer.Start();
                var clubAggregation = FDBMongoContext.Clubs.Aggregate()
                    .Group(x => x.currentLeagueId, g =>
                            new
                            {
                                currentLeagueId = g.Max(a => a.currentLeagueId)
                            }).ToList()[0];
                timer.Stop();
                return (timer.ElapsedMilliseconds, clubAggregation.currentLeagueId);
            }

            //Mongo Update
            public long UpdateMongoTimer()
            {
                //CreateAndInsertMongoTimer(withIndex, anz);
                Stopwatch timer = new();
                timer.Start();

                var updateClub = Builders<fbDatabase.Club>.Update
                .Set(club => club.clubName, "Test");

                var clubs = FDBMongoContext.Clubs.UpdateMany(Builders<fbDatabase.Club>.Filter.Where(x => true), updateClub);

                var updatePlayer = Builders<fbDatabase.Player>.Update
                .Set(player => player.firstname, "Test");

                var players = FDBMongoContext.Players.UpdateMany(Builders<fbDatabase.Player>.Filter.Where(x => true), updatePlayer);

                timer.Stop();
                return timer.ElapsedMilliseconds;
            }

            //Mongo Delete
            public long DeleteMongoTimer()
            {
                //CreateAndInsertMongoTimer(withIndex, anz);
                Stopwatch timer = new();
                timer.Start();

            FDBMongoContext.Clubs.DeleteMany(Builders<fbDatabase.Club>.Filter.Where(x => true));
            FDBMongoContext.Players.DeleteMany(Builders<fbDatabase.Player>.Filter.Where(x => true));

                timer.Stop();
                return timer.ElapsedMilliseconds;
            }

            //Mongo Geraet Add
            public long AddPlayerMongoTimer(fbDatabase.Player player)
            {
                Stopwatch timer = new();
                timer.Start();
            FDBMongoContext.Players.InsertOne(player);
                timer.Stop();
                return timer.ElapsedMilliseconds;
            }
        }
    }


