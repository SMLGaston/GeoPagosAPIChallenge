using GeoPagosAPI.Models.Enum;
using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeoPagosAPI.Models
{
    public class TablaReversa
    {
        public long Id { get; set; }

        public long AutorizacionId { get; set; }

        public DateTime TimeStamp { get; set; }

        public bool Activo { get; set; }
    }
}
