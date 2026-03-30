using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace API_AlfaTorres.Models.Formulas
{   
    //Tmpime
    public class FormulaRequest
    {

        [Column("CodigoEmpresa")]
        public short codigoEmpresa { get; set; }

        [Column("Formula")]
        public short formula { get; set; }

        [MaxLength(20)]
        [Column("CodigoArticulo")]
        public string codigoArticulo { get; set; }

        [MaxLength(50)]
        [Column("DescripcionArticulo")]
        public string descripcionArticulo { get; set; }

        [MaxLength(40)]
        [Column("Descripcion2Articulo")]
        public string descripcion2Articulo { get; set; }

        [Column("NivelCompuesto")]
        public short nivelCompuesto { get; set; }

        [Column("UnidadesEscandallo")]
        public decimal UnidadesEscandallo { get; set; }

        [JsonIgnore]
        [Column("IdProcesoIME")]
        public Guid IdProcesoIME { get; set; }

 
        //viculas las tmpime
        public ICollection<Mat_FormulaRequest> TmpIME_Materiales { get; set; } = new List<Mat_FormulaRequest>();
        public ICollection<Oper_FormulaRequest> TmpIME_Operaciones { get; set; } = new List<Oper_FormulaRequest>();
    }
}
