using GeoPagosAPI.DTO;
using GeoPagosAPI.Models;
using GeoPagosAPI.Models.Enum;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GeoPagosAPI.Background_Services;

public class GeneradorReversa : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private IConfiguration _configuracion;
    public GeneradorReversa(IServiceProvider serviceProvider, IConfiguration iConf)
    {
        _serviceProvider = serviceProvider;
        _configuracion = iConf;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(15));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                BuscarAutorizaciones();
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void BuscarAutorizaciones()
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<APIDbContext>();
        int segundos = _configuracion.GetValue<int>("Configuracion:TiempoConfirmacion");
        var ahora = DateTime.Now;

        foreach(var item in dbContext.TablaReversas.Where(x => x.Activo))
        {
            if (item.TimeStamp.AddSeconds(segundos) < ahora)
            {
                item.Activo = false;    
                Autorizacion autorizacionAnulada = dbContext.Autorizaciones.FirstOrDefault(x => x.Id == item.AutorizacionId);
                Autorizacion autorizacion = Task.Run(() => GenerarAuthReversa(autorizacionAnulada)).Result;
                dbContext.Autorizaciones.Add(autorizacion);
                TablaAprobada autorizacionAprobada = Task.Run(() => MapeoAprobadaReversa(autorizacion)).Result; ;
                dbContext.TablaAprobadas.Add(autorizacionAprobada);
            }
        }

        foreach(var itemAborrar in dbContext.TablaReversas.Where(x => x.Activo == false))
        {
            dbContext.TablaReversas.Remove(itemAborrar);
        }

        dbContext.SaveChanges();
    }

    private async Task<Autorizacion> GenerarAuthReversa(Autorizacion autorizacionAnulada)
    {

        return new Autorizacion()
        {
            Fecha = DateTime.Now,
            ClienteId = autorizacionAnulada.ClienteId,
            TipoCliente = autorizacionAnulada.TipoCliente,
            TipoAutorizacion = TipoAutorizacion.Reversa,
            Monto = autorizacionAnulada.Monto*(-1),
            Estado = EstadoAutorizacion.Aprobada
        };
    }

    private async Task<TablaAprobada> MapeoAprobadaReversa(Autorizacion autorizacion)
    {
        return new TablaAprobada()
        {
            AutorizacionId = autorizacion.Id,
            Fecha = autorizacion.Fecha,
            ClienteId = autorizacion.ClienteId,
            Monto = autorizacion.Monto,
        };
    }
}
