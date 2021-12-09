using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using scpmtf_webapi.Models;
using scpmtf_webapi.Scrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;


namespace scpmtf_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ItemsController(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        [HttpPost]
        public async Task<ActionResult<SCPObject>> Post(SCPObject newItem)
        {
            _context.SCPs.Add(newItem);
            await _context.SaveChangesAsync();

            return newItem;
        }

        [HttpGet]
        public async Task<ActionResult<List<SCPObject>>> GetAll()
        {
            var response = await _context.SCPs.ToListAsync();
            if (response == null)
                return NotFound();
            else
                return response;
        }
        [HttpGet("scp-{id}")]
        public async Task<ActionResult<SCPObject>> GetItem(int id)
        {
            var scpitem = await _context.SCPs.AsQueryable().Where(item => item.ItemNo == ((id < 100) ? id.ToString("D3") : id.ToString())).AsNoTracking().ToListAsync();

            if (scpitem.Count >= 1) return scpitem.First();

            var generator = new gpt3generator();

            var generatedScp = await generator.autoGetSCP(id);

            if (generatedScp == null)
                return NotFound();

            return await Post(generatedScp);     
        }
    }
}
