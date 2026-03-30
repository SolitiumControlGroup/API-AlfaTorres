using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_AlfaTorres.Models.Formulas
{
    public class FormulaResponse

    {
        [Key]
        [Column ("CodigoEmpresa")]
        public short codigoEmpresa { get; set;}
        
        [Key]
        [Column("Formula")]
        public short formula { get; set;}

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

        [Column("NivelCompuesto")]
        public short nivelCompuesto { get; set; }

        [Column("UnidadesEscandallo")]
        public decimal unidadesEscandallo { get; set; }

        //viculas las formula
        public ICollection<Mat_FormulaResponse> Materiales { get; set; } = new List<Mat_FormulaResponse>();
        public ICollection<Oper_FormulaResponse> Operaciones { get; set; } = new List<Oper_FormulaResponse>();
   
    }
}
