using ConsoleApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbi_projekt_2023.ConsoleApp.Domain.Entities
{
    public class Club
    {
        public Club(string? clubName, Address billingAddress, int currentLeagueId)
        {
            ClubName = clubName;
            BillingAddress = billingAddress;
            this.currentLeagueId = currentLeagueId;
            Players = new List<Player>();
        }

        public Club() { }
        public int Id { get; set; }
        public string? ClubName { get; set; }
        public Address BillingAddress { get; set; }
        public int currentLeagueId { get; set; }
        public List<Player>? Players { get; set; }
    }
}
