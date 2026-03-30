using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_AlfaTorres.Models.Formulas
{
    public class Oper_FormulaRequest
    {
        //TMPIME
        [Column("CodigoEmpresa")]
        public short codigoEmpresa { get; set; }

        [Column("Formula")]
        public short formula { get; set; }

        [MaxLength(20)]
        [Column("CodigoArticulo")]
        public string codigoArticulo { get; set; }

        [Column("Orden")]
        public short orden { get; set; }

        [Column("Operacion")]
        public string operacion { get; set; }

        [Column("DescripcionOperacion")]
        public string descripcionOperacion { get; set; }

        [JsonIgnore]
        [Column("IdProcesoIME")]
        public Guid IdProcesoIME { get; set; }
    }
}
