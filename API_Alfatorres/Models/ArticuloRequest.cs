using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API_AlfaTorres.Models
{
    public class ArticuloRequest
    {
        [Column("CodigoEmpresa")]
        public short codigoempresa { get; set; }

        [MaxLength(20)]
        [Column("CodigoArticulo")]
        public string codigoArticulo { get; set; }

        [MaxLength(50)]
        [Column("DescripcionArticulo")]
        public string descripcionArticulo { get; set; }
        [MaxLength(40)]
        [Column("Descripcion2Articulo")]
        public string descripcion2Articulo { get; set; }
        [JsonIgnore]
        [Column("IdProcesoIME")]
        public Guid IdProcesoIME { get; set; }
    }
}
