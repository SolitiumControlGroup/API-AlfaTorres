using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_AlfaTorres.Models
{
    public class Articulo
    {
        [Key]
        [Column("CodigoEmpresa")]
        public short codigoempresa { get; set; }
        [Key]
        [MaxLength(20)]
        [Column("CodigoArticulo")]
        public string codigoArticulo { get; set; }

        [MaxLength(50)]
        [Column("DescripcionArticulo")]
        public string descripcionArticulo { get; set; }
        [MaxLength(40)]
        [Column("Descripcion2Articulo")]
        public string descripcion2Articulo { get; set; }
    }
}
