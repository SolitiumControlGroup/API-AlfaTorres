using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_AlfaTorres.Data;
using API_AlfaTorres.Models;
using API_AlfaTorres.Repositories;
using API_AlfaTorres.Filters;

namespace API_AlfaTorres.Controllers
{
    [ApiController]
    [Route("api/articulos")]
    [ApiKeyAuth]
    public class ArticulosController : Controller
    {
        private readonly ArticulosRepository _articulosRepository;
        private readonly IConfiguration _configuration;

        public ArticulosController(ArticulosRepository articulosRepository, IConfiguration configuration)
        {
            _articulosRepository = articulosRepository;
            _configuration = configuration;
        }

        [HttpGet]
        [Produces("application/json")]
        [Route("")]
        public async Task<ActionResult<ApiResponse>> GetAllArticles()
        {
            var articulos = await _articulosRepository.GetAllArticulosAsync();

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Lista de artículos obtenida correctamente.",
                Result = articulos
            });
        }

        [HttpGet("by-id/{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<ApiResponse>> GetArticleById(string id)
        {
            var articulo = await _articulosRepository.GetArticuloByIdAsync(id);
            if (articulo == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = $"No se encontró el artículo con código {id}."
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Artículo encontrado.",
                Result = articulo
            });
        }

        /// <summary>
        /// Obtiene modelos de una marca específica con filtros opcionales de posición
        /// </summary>
        /// <param name="codigoempresa">Marca (obligatorio)</param>
        /// <param name="codigoArticulo">Filtro opcional por códigoartículo </param>
        /// <param name="descripcionArticulo">Filtro opcional por descripción del artículo</param>
        [HttpGet("Articulo")]
        [ProducesResponseType(typeof(List<Articulo>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<Articulo>>> GetModels(
          [FromQuery] short codigoempresa,
          [FromQuery] string? codigoArticulo = null,
          [FromQuery] string? descripcionArticulo = null)
        {
            // Validar que la empresa sea obligatoria
            if (codigoempresa == 0)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "El parámetro codigoempresa es obligatorio"
                });
            }

            List<Articulo> articulos = new List<Articulo>();
            articulos = await _articulosRepository.GetArticulosByFiltersAsync(codigoempresa,codigoArticulo, descripcionArticulo);
               

            if (_articulosRepository == null || !articulos.Any())
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = "No se encontraron articulos con los filtros especificados"
                });
            }

            return Ok(articulos);
        }


        [HttpGet("by-empresa/{codigoempresa}")]
        [Produces("application/json")]
        public async Task<ActionResult<ApiResponse>> GetArticlesByEmpresa(short codigoempresa)
        {
            var articulos = await _articulosRepository.GetArticulosByEmpresaAsync(codigoempresa);
            if (articulos == null)
            {
                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = $"No se encontraron artículos materiales en la empresa {codigoempresa}."
                });
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = "Lista de artículos obtenida correctamente.",
                Result = articulos
            });
        }

        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<ApiResponse>> CreateArticle([FromBody] ArticuloRequest nuevoArticulo)
        {
            if (nuevoArticulo == null)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "El artículo no puede ser nulo."
                });
            }
            //Reparar
            try
            {
                ApiResponse respuesta = await _articulosRepository.CreateArticuloAsync(nuevoArticulo);

                return respuesta.Success
                    ? Ok(respuesta)
                    : StatusCode(500, respuesta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = $"Error al crear el artículo: {ex.Message}"
                });
            }
        }
    }
}
