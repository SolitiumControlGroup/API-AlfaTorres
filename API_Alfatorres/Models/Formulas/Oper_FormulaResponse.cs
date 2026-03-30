using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_AlfaTorres.Models.Formulas
{
    public class Oper_FormulaResponse
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

        [Column("Operacion")]
        public string operacion { get; set; }

        [Column("DescripcionOperacion")]
        public string descripcionOperacion { get; set; }
    }
}
