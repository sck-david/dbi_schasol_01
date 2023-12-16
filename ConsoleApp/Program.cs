using System;
using MongoDB.Driver;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;

namespace dbi_projekt_2023
{
    class Program
    {
        static int Main(string[] args)
        {
            var fdb = fbDatabase.FromConnectionString("mongodb://localhost:27017", logging: true);
            try
            {
                fdb.Seed(10);
            }
            catch (TimeoutException)
            {
                Console.Error.WriteLine("Die Datenbank ist nicht erreichbar. Läuft der Container?");
                return 1;
            }
            catch (MongoAuthenticationException)
            {
                Console.Error.WriteLine("Mit dem Benutzer root (Passwort 1234) konnte keine Verbindung aufgebaut werden.");
                return 2;
            }

            var options = new System.Text.Json.JsonSerializerOptions()
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),  // Umlaute als Umlaut ausgeben (nicht als \uxxxx)
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            };
            options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

            // *****************************************************************************************
            {
                PrintHeader("Alle Vornamen der Spieler");
                var result = fdb.Players.AsQueryable().Where(p => p.firstname!=null);
                Console.WriteLine(string.Join(", ", result.ToList().OrderBy(r => r.Id).Select(r => r.firstname)));
            };

            // *****************************************************************************************
            {
                PrintHeader("Alle Spieler eines clubs");
                var result = fdb.Clubs.AsQueryable().Where(p => p.players != null && p.currentLeagueId>3).ToList();
                if (result.Count > 0)
                {
                    foreach (var club in result)
                    {
                        Console.WriteLine("Clubname:         " + club.clubName);
                        Console.WriteLine("Players from this club:       " +
                            string.Join(", ", club.players.ToList().OrderBy(r => r.Id).Select(r => r.lastname)));
                    }
                }              
            }

            return 0;
        }
        static void PrintHeader(string text)
        {
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(Environment.NewLine + text);
            Console.ForegroundColor = color;
        }

        
    }
}
