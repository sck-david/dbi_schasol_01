using ConsoleApp;
using dbi_projekt_2023.ConsoleApp.Domain.Entities;
using dbi_projekt_2023.ConsoleApp.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbi_projekt_2023.ConsoleApp.Services
{
    public class fmServices : IFmService
    {
        private readonly FDBContext _context;

        public fmServices(FDBContext context)
        {
            _context = context;
        }


        public IEnumerable<Club?> GetAll()
        {
            return _context.Clubs.ToList();
        }

        public async Task<Club?> GetByIdAsync(int id)
        {
            var result = await _context.Clubs
                .Include(x => x.Players)
                .FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }
    }
}
