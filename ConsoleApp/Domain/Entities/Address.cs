using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Domain.Entities
{
    [Owned]
    public class Address
    {
        public Address() { }

        public Address(string street, string streetN, int zip, string city, string state)
        {
            Street = street;
            StreetN = streetN;
            Zip = zip;
            City = city;
            State = state;
        }

        public string Street { get; set; }
        public string StreetN { get; set; }
        public int Zip { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }
}
