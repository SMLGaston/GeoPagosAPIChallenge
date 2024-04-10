using GeoPagosAPI.Models.Enum;

namespace GeoPagosAPI.Models
{
    public class Autorizacion
    {
        public long Id { get; set; }
        public DateTime Fecha { get; set; }
        public long ClienteId { get; set; }
        public TipoCliente TipoCliente { get; set; }
        public TipoAutorizacion TipoAutorizacion { get; set; }
        public decimal Monto { get; set; }
        public EstadoAutorizacion Estado { get; set; }
    }
}
