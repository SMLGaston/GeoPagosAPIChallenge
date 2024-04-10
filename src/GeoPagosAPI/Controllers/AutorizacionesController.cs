using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GeoPagosAPI.Models;
using GeoPagosAPI.DTO;
using NuGet.Protocol.Plugins;
using System.IO;
using GeoPagosAPI.Models.Enum;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http;

namespace GeoPagosAPI.Controllers
{
    [Route("api/Autorizaciones")]
    [ApiController]
    public class AutorizacionesController : ControllerBase
    {
        private readonly APIDbContext _context;
        private IConfiguration _configuracion;
        private readonly IHttpClientFactory _factory;

        public AutorizacionesController(APIDbContext context, IConfiguration iConf,IHttpClientFactory factory)
        {
            _context = context;
            _configuracion = iConf;
            _factory = factory;
        }

        [HttpGet("Reporte")]
        public async Task<ActionResult<IEnumerable<Autorizacion>>> AutorizacionesReporte()
        {
            return await _context.Autorizaciones.ToListAsync();
        }

        [HttpGet("ReporteAprobadas")]
        public async Task<ActionResult<IEnumerable<TablaAprobada>>> AutorizacionesAprobadasReporte()
        {
            return await _context.TablaAprobadas.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Autorizacion>> GetAutorizacion(long id)
        {
            var autorizacion = await _context.Autorizaciones.FindAsync(id);

            if (autorizacion == null)
            {
                return NotFound();
            }

            return autorizacion;
        }

        [HttpPost]
        public async Task<ActionResult<Autorizacion>> PostAutorizacion(AutorizacionDTO autorizacionDTO)
        {
            if (Validacion(autorizacionDTO))
            {
                return BadRequest();
            }

            try
            {
                // Valido la Autorización por Servicio Externo
                autorizacionDTO.Estado = Task.Run(() => GetProcesamiento(autorizacionDTO.Monto)).Result;

                // Guardado de la Autorización
                Autorizacion autorizacion = Task.Run(() => Mapeo(autorizacionDTO)).Result;
                _context.Autorizaciones.Add(autorizacion);
                _context.SaveChanges();

                //si es aprobacion generar registro
                if (autorizacion.Estado == EstadoAutorizacion.Aprobada)
                {
                    TablaAprobada autorizacionAprobada = await Task.Run(() => MapeoAprobada(autorizacion));
                    _context.TablaAprobadas.Add(autorizacionAprobada);
                    await _context.SaveChangesAsync();
                }

                // si es reversa
                if (autorizacion.TipoCliente == TipoCliente.SolicitudyConfirmacion)
                {
                    TablaReversa autorizacionReversa = await Task.Run(() => MapeoReversa(autorizacion));
                    _context.TablaReversas.Add(autorizacionReversa);
                    await _context.SaveChangesAsync();
                }

                return CreatedAtAction("GetAutorizacion", new { id = autorizacion.Id }, autorizacion);
            }
            catch
            {
                return BadRequest();
            }

        }

        private bool Validacion (AutorizacionDTO autorizacionDTO)
        {
            if ((autorizacionDTO.ClienteId == 0) ||
                (autorizacionDTO.TipoCliente == 0) ||
                (autorizacionDTO.TipoAutorizacion == 0) ||
                (autorizacionDTO.Monto == null))
                return true;
            else
                return false;

        }

        private async Task<Autorizacion> Mapeo(AutorizacionDTO autorizacionDTO)
        {
            return new Autorizacion()
            {
                Fecha = DateTime.Now,
                ClienteId = autorizacionDTO.ClienteId,
                TipoCliente = autorizacionDTO.TipoCliente,
                TipoAutorizacion = autorizacionDTO.TipoAutorizacion,
                Monto = autorizacionDTO.Monto,
                Estado = autorizacionDTO.Estado.Value
            };
        }

        private async Task<TablaAprobada> MapeoAprobada(Autorizacion autorizacion)
        {
            return new TablaAprobada()
            {
                AutorizacionId = autorizacion.Id,
                Fecha = autorizacion.Fecha,
                ClienteId = autorizacion.ClienteId,
                Monto = autorizacion.Monto,
            };
        }

        private async Task<TablaReversa> MapeoReversa(Autorizacion autorizacion)
        {
            return new TablaReversa()
            {
                AutorizacionId = autorizacion.Id,
                TimeStamp = DateTime.Now,
                Activo = true
            };
        }

        public async Task<EstadoAutorizacion> GetProcesamiento(decimal monto)
        {
            HttpClient cliente = _factory.CreateClient();
            string path = _configuracion.GetValue<string>("Configuracion:ProcesadorUrl");
            cliente.BaseAddress = new Uri(path);
            cliente.DefaultRequestHeaders.Accept.Clear();
            cliente.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = await cliente.GetAsync(monto.ToString());

                if (response.IsSuccessStatusCode)
                {
                    if(response.Content.ReadAsStringAsync().Result == "true")
                        return EstadoAutorizacion.Aprobada;
                    else
                        return EstadoAutorizacion.Rechazada;
                }
                else
                    return EstadoAutorizacion.Rechazada;

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAutorizacion(long id)
        {
            var autorizacion = await _context.Autorizaciones.FindAsync(id);
            if (autorizacion == null)
            {
                return NotFound();
            }

            _context.Autorizaciones.Remove(autorizacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AutorizacionExists(long id)
        {
            return _context.Autorizaciones.Any(e => e.Id == id);
        }
    }
}
