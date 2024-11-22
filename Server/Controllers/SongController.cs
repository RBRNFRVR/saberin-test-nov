using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using music_manager_starter.Data;
using music_manager_starter.Data.Models;
using System;
using music_manager_starter.Data.Migrations;

namespace music_manager_starter.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongsController : ControllerBase
    {
        private readonly DataDbContext _context;

        public SongsController(DataDbContext context)
        {
            _context = context;
        }

  
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Song>>> GetSongs()
        {
            return await _context.Songs.ToListAsync();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Song>>> SearchSongs([FromQuery] string query)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(query))
                {
                    return Ok(await _context.Songs.ToListAsync());
                }
                
                var filteredSongs = await _context.Songs
                    .Where(s => EF.Functions.Like(s.Title, $"%{query}%") ||
                                EF.Functions.Like(s.Artist, $"%{query}%") ||
                                EF.Functions.Like(s.Album, $"%{query}%"))
                    .ToListAsync();
                
                Console.WriteLine($"Found {filteredSongs.Count} songs matching query: {query}");

                if (!filteredSongs.Any())
                {
                    return NotFound("No songs found.");
                }

                return Ok(filteredSongs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchSongs: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }        

        [HttpPost]
        public async Task<ActionResult<Song>> PostSong( [FromForm] Song song,[FromForm] IFormFile? AlbumCover)
        {
            if (song == null)
            {
                return BadRequest("Song cannot be null.");
            }

            if (AlbumCover != null && AlbumCover.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await AlbumCover.CopyToAsync(memoryStream);
                    song.AlbumCover = memoryStream.ToArray();
                }

            }
            else
            {
                song.AlbumCover = null;
            }

            if (string.IsNullOrEmpty(song.ReleaseDate))
            {
                song.ReleaseDate = null;
            }


            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            return Ok();
        }
        
    }
}
