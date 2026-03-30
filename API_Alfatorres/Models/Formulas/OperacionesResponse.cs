using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API_AlfaTorres.Models.Formulas
{
    public class OperacionesResponse
    {
        [Key]
        [Column("CodigoEmpresa")]
        public short codigoEmpresa { get; set; }
        [Key]
        [Column("Operacion")]
        public string operacion { get; set; }

        [Column("DescripcionOperacion")]
        public string descripcionOperacion { get; set; }
    }
}
