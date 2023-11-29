using ConsoleApp.Domain.Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbi_projekt_2023.ConsoleApp.Domain.Entities
{
    public class Player 
    {
        public Player() { }

        public Player(string firstname, string lastname, DateTime gebDat, decimal marketvalue, int gender, Address address)
        {
            this.firstname = firstname;
            this.lastname = lastname;
            this.gebDat = gebDat;
            this.marketvalue = marketvalue;
            this.gender = gender;
            this.address = address;
        }

        public string firstname { get; set; }
        public string lastname { get; set; }
        public DateTime gebDat { get; set; }
        public decimal marketvalue { get; set; }
        public int gender { get; set; }
        public Address address { get; set; }
        public int Id { get; set; }
    }
}
