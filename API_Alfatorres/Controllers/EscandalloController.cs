using API_AlfaTorres.Filters;

using API_AlfaTorres.Models;
using API_AlfaTorres.Models.Formulas;
using API_AlfaTorres.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API_AlfaTorres.Controllers
{
   // public class EscandalloController
    //{
        [ApiController]
        [Route("api/escandallo")]
        [ApiKeyAuth]

        public class EscandallosController : Controller
        {
            //inyección de dependencias
            //Manejador de toda la lógica de obtención y  creación de escandallos.
            private readonly EscandalloRespository _escandalloRepository;
            //Manejador de configuración para obtener la cadena de conexión a la base de datos.
            private readonly IConfiguration _configuration;
             //he de poder comprobar si los articulos que forman el escandallo existen
            private readonly ArticulosRepository _articulosRepository;

        public EscandallosController(EscandalloRespository escandalloRepository, IConfiguration configuration, ArticulosRepository articulosRepository)
            {
                _escandalloRepository = escandalloRepository;
                _configuration = configuration;
                _articulosRepository = articulosRepository;
            }

            //obtener todas las formulas
            [HttpGet]
            [Produces("application/json")] //devuelve in json
            [Route("")]
            public async Task<ActionResult<ApiResponse>> GetAllFormulas()
            {
                //Consulta a base de datos mediante el repositorio.
                var escandallos = await _escandalloRepository.GetAllFormulasAsync();
                //pk no compreba si retorna null?
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Lista de fórmulas obtenida correctamente.",
                    Result = escandallos
                });
            }

            //obtener formula por ID
            [HttpGet("by-id/{codigoempresa}/formula/{idFormula}/articulo/{Codigoarticulo}")]
            [Produces("application/json")]
            public async Task<ActionResult<ApiResponse>> GetFormulaById(short codigoempresa,short idFormula, string Codigoarticulo)
            {
            var escandallo = await _escandalloRepository.GetFormulaByIdAsync(codigoempresa,idFormula, Codigoarticulo);
                if (escandallo == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = $"No se encontró la fórmula con ID {idFormula}."
                    });
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Fórmula encontrada.",
                    Result = escandallo
                });
            }

            //OBTENER FORMULAS POR EMPRESA
            [HttpGet("by-empresa/{codigoempresa}")]
            [Produces("application/json")]
            public async Task<ActionResult<ApiResponse>> GetFormulasByEmpresa(short codigoempresa)
            {
                var escandallos = await _escandalloRepository.GetFormulasByEmpresaAsync(codigoempresa);
                if (escandallos == null || escandallos.Count == 0)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = $"No se encontraron fórmulas para la empresa con código {codigoempresa}."
                    });
                }
                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Fórmulas encontradas para la empresa.",
                    Result = escandallos
                });
            }
        [HttpGet("materiales/{codigoempresa}")]
        [Produces("application/json")] 
        public async Task<ActionResult<ApiResponse>> GetMaterialesByEmpresa(short codigoempresa)
        {
            //Consulta a base de datos mediante el repositorio.
            var materiales = await _escandalloRepository.GetMaterialesByEmpresa(codigoempresa);
            //pk no compreba si retorna null?
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Lista de fórmulas obtenida correctamente.",
                Result = materiales
            });
        }
        //OPERACIONES
        /// <summary>.
        /// Obtiene modelos de una marca específica con filtros opcionales de posición
        /// </summary>
        /// <param name="codigoempresa">Marca (obligatorio)</param>
        /// <param name="operacion">Filtro opcional por operación </param>
        [HttpGet("Operacion")]
        [ProducesResponseType(typeof(List<OperacionesResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> GetOperaciones(short codigoempresa, string operacion = null)
        {
            var operaciones = await _escandalloRepository.GetOperaciones(codigoempresa, operacion);
            if (operaciones == null || operaciones.Count == 0)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = $"No se encontraron operaciones para la empresa con código {codigoempresa}."
                });
            }
            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Operaciones encontradas para la empresa.",
                Result = operaciones
            });
        }

        //crear una formula nueva
        [HttpPost]
            [Produces("application/json")]
            public async Task<ActionResult<ApiResponse>> CreateFormula([FromBody] FormulaRequest nevaFormula)
            {
                if (nevaFormula == null)
                {
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = "La solicitud no puede estar vacía."
                    });
                }
                try
                {
                     if (nevaFormula.codigoEmpresa != 9997 ) {
                         return NotFound(new ApiResponse
                         {
                             Success = false,
                             Message = "Estamos en fase de prueba, solo se pueden insertar fórmulas a la empresa 9997."
                         });
                }
                    var articulo = await _articulosRepository.GetArticuloByIdAsync(nevaFormula.codigoArticulo);
                    if (articulo == null)
                    {
                        return NotFound(new ApiResponse
                        {
                            Success = false,
                            Message = $"No se ha generado el escandallo devido a que el artículo con código {nevaFormula.codigoArticulo}" +
                            $" no existe en la base de datos."
                        });
                    }
                    if (nevaFormula.TmpIME_Materiales != null)
                    {
                        foreach (var material in nevaFormula.TmpIME_Materiales)
                        {
                            var articuloMaterial = await _articulosRepository.GetArticuloByIdAsync(material.articuloComponente);
                            if (articuloMaterial == null)
                            {
                                return NotFound(new ApiResponse
                                {
                                    Success = false,
                                    Message = $"No se ha generado el escandallo devido a que el artículo material con código {material.articuloComponente}" +
                                    $" no existe en la base de datos."
                                });
                            }

                        }
                    }
                    
                    var result = await _escandalloRepository.CreateFormulaAsync(nevaFormula);
                        return Ok(new ApiResponse
                        {
                            Success = true,
                            Message = "Fórmula creada exitosamente.",
                            Result = result
                        });
                }
                catch (Exception ex)
                {
                    // Log the exception (not implemented here)
                    return StatusCode(500, new ApiResponse
                    {
                        Success = false,
                        Message = $"Ocurrió un error al crear la fórmula: {ex.Message}"
                    });
                }
            }
        }
   // }
}
