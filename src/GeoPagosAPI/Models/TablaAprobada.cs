using GeoPagosAPI.Models.Enum;

namespace GeoPagosAPI.Models
{
    public class TablaAprobada
    {
        public long Id { get; set; } 
        public long AutorizacionId { get; set; }
        public DateTime Fecha { get; set; }
        public long ClienteId { get; set; }
        public decimal Monto { get; set; }
    }
}
