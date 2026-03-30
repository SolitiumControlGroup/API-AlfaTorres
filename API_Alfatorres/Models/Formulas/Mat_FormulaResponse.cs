using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_AlfaTorres.Models.Formulas
{
    public class Mat_FormulaResponse
    {
        [Key]
        [Column("CodigoEmpresa")]
        public short codigoEmpresa { get; set; }

        [Key]
        [Column("Formula")]
        public short formula { get; set; }

        [Key]
        [MaxLength(20)]
        [Column("CodigoArticulo")]
        public string codigoArticulo { get; set; }

        [Key]
        [Column("Orden")]
        public short orden { get; set; }

        [Column("FormulaComponente")]
        public short formulaComponente { get; set; }

        [MaxLength(20)]
        [Column("ArticuloComponente")]
        public string articuloComponente { get; set; }

        [MaxLength(50)]
        [Column("DescripcionArticulo")]
        public string descripcionArticulo { get; set; }

        [MaxLength(40)]
        [Column("Descripcion2Articulo")]
        public string descripcion2Articulo { get; set; }


        [Column("NivelCompuesto")]
        public short nivelCompuesto { get; set; }

        [Column("Operacion")]
        public string operacion { get; set; }

        [Column("unidadesNecesarias")]
        public decimal unidadesNecesarias { get; set; }
    }
}
