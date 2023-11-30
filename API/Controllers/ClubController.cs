
using API.Controllers.DTO;
using ConsoleApp;
using dbi_projekt_2023.ConsoleApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Reflection.Metadata;

namespace API.Controllers
{
    [ApiController]
    [Route("")]
    public class ClubController : ControllerBase
    {
        private readonly ILogger<ClubController> _logger;
        private readonly FDBContext _context;

        public ClubController(ILogger<ClubController> logger, FDBContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Route("getC")]
        [HttpGet]
        public IEnumerable<Club> GetClubs()
        {
            var result = _context.Clubs.Include(x =>
            x.Players);
            return result;
        }

        [HttpGet("getClub/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ClubDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetClubByIdAsync(int id)
        {
            //var result = await _context.GetByIdAsync(id);
            //IFmService _context1 = new fmService();
            var result = _context.Clubs
                .Include(x => x.Players)
                .FirstOrDefault(x => x.Id == id);

            if (result is null)
            {
                return NotFound();
            }

            ClubDTO dto = new ClubDTO(result.Id, result.ClubName, result.Players.Count());

            return Ok(dto);
        }

        [HttpGet("getPlayer/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PlayerDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPlayerByIdAsync(int id)
        {
            var result = _context.Players.FirstOrDefault(x => x.Id == id); ;

            if (result is null)
            {
                return NotFound();
            }

            PlayerDTO dto = new PlayerDTO(result.Id, result.firstname, result.lastname, result.gebDat);

            return Ok(dto);
        }


        [HttpGet("getPlayersFromClub/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IEnumerable<PlayerDTO> GetVolleyBallPlayers(int id)
        {
            List<PlayerDTO> ret = new List<PlayerDTO>();

            var result = _context.Clubs
                .Include(x => x.Players)
                .FirstOrDefault(x => x.Id == id);

            result.Players.ForEach(y =>
            {
                PlayerDTO pd = new PlayerDTO(y.Id, y.firstname, y.lastname, y.gebDat);
                ret.Add(pd);
            });
            return ret;
        }

        [HttpPost("fromForm")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<Player> PostFromForm([FromForm] PlayerDTO play)
        {
            PostPlayer(play);
            return Ok(play);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult PostPlayer(PlayerDTO vbp)
        {
            Player p1 = new Player();
            p1.firstname = vbp.Firstname;
            p1.lastname = vbp.Lastname;
            p1.gebDat = vbp.gebDat;
            _context.Players.Add(p1);
            _context.SaveChanges();
            return Ok(p1);
        }




        [HttpPut("/put{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Player> Put(int id, PlayerDTO vbp22)
        {
            try
            {
                Player p1 = new Player();
                p1.firstname = vbp22.Firstname;
                p1.lastname = vbp22.Lastname;
                p1.gebDat = vbp22.gebDat;
                if (id != vbp22.id) { return BadRequest(); }
                Player? found = _context.Players.SingleOrDefault(p => p.Id == id);
                if (found == null) { return NotFound(); }

                found.firstname = p1.firstname;
                found.lastname = p1.lastname;
                found.gender = p1.gender;
                found.gebDat = p1.gebDat;
                _context.SaveChanges();
                return NoContent();
            }
            catch
            {
                throw;
            }
        }






    }
}
