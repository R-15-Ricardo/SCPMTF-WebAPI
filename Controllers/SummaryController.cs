using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using scpmtf_webapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace scpmtf_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SummaryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SummaryController(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        [HttpPost]
        public async Task<ActionResult<ScpGPT3Summary>> Post(ScpGPT3Summary newItem)
        {
            _context.Summaries.Add(newItem);
            await _context.SaveChangesAsync();

            return newItem;
        }

        [HttpGet]
        public async Task<ActionResult<List<ScpGPT3Summary>>> GetAll()
        {
            var response = await _context.Summaries.ToListAsync();
            if (response == null)
                return NotFound();
            else
                return response;
        }
        [HttpGet("scp-{id}")]
        public async Task<ActionResult<ScpGPT3Summary>> GetItem(int id)
        {
            var mtfsummary = await _context.Summaries.AsQueryable().Where(item => item.ItemNo == id).AsNoTracking().ToListAsync();

            if (mtfsummary.Count >= 1) return mtfsummary.First();

            var scpitem = await _context.SCPs.AsQueryable().Where(item => item.ItemNo == ((id < 100) ? id.ToString("D3") : id.ToString())).AsNoTracking().ToListAsync();
            if (scpitem.Count < 1) return NotFound();

            var generator = new Scrapper.gpt3generator();
            var generatedSummary = await generator.autoGetSummary(scpitem.First());

            if (generatedSummary == null)
                return NotFound();

            return generatedSummary;
        }
    }
}
