using dbi_projekt_2023.ConsoleApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbi_projekt_2023.ConsoleApp.Domain.Interfaces
{
    public interface IFmService
    {
        Task<Club?> GetByIdAsync(int id);

        IEnumerable<Club?> GetAll();
    }
}
