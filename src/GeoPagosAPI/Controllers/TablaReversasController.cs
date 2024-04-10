using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GeoPagosAPI.Models;

namespace GeoPagosAPI.Controllers
{
    [Route("api/TablaReversas")]
    [ApiController]
    public class TablaReversasController : ControllerBase
    {
        private readonly APIDbContext _context;

        public TablaReversasController(APIDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TablaReversa>>> GetTablaReversas()
        {
            return await _context.TablaReversas.ToListAsync();
        }

        [HttpPut("Confirmar/{id}")]
        public async Task<IActionResult> PutTablaReversa(long id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            try
            {
                TablaReversa tablaReversa = _context.TablaReversas.FirstOrDefault(x => x.AutorizacionId == id);
                if (tablaReversa != null)
                {
                    tablaReversa.Activo = false;
                    _context.Entry(tablaReversa).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TablaReversaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TablaReversas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TablaReversa>> PostTablaReversa(TablaReversa tablaReversa)
        {
            _context.TablaReversas.Add(tablaReversa);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTablaReversa", new { id = tablaReversa.Id }, tablaReversa);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTablaReversa(long id)
        {
            var tablaReversa = await _context.TablaReversas.FindAsync(id);
            if (tablaReversa == null)
            {
                return NotFound();
            }

            _context.TablaReversas.Remove(tablaReversa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TablaReversaExists(long id)
        {
            return _context.TablaReversas.Any(e => e.Id == id);
        }
    }
}
