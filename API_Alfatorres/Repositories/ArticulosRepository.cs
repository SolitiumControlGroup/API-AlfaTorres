using API_AlfaTorres.Data;
using API_AlfaTorres.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace API_AlfaTorres.Repositories
{
    public class ArticulosRepository
    {
        private readonly SageDbContext _context;
        private readonly ILogger<ArticulosRepository> _logger;


        public ArticulosRepository(SageDbContext context, IConfiguration configuration, ILogger<ArticulosRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<List<Articulo>> GetAllArticulosAsync()
        {
            return await _context.Articulos
                .ToListAsync();
        }

        public async Task<Articulo> GetArticuloByIdAsync(string id)
        {
            return await _context.Articulos
                .FirstOrDefaultAsync(c => c.codigoArticulo == id);
        }
        public async Task<List<Articulo>> GetArticulosByEmpresaAsync(short codigoempresa)
        {
            return await _context.Articulos
                .Where(c => c.codigoempresa == codigoempresa)
                .ToListAsync();
        }
        public async Task<ApiResponse> CreateArticuloAsync(ArticuloRequest articulo)
        {
            Guid guidProceso = Guid.NewGuid();
            articulo.IdProcesoIME = guidProceso;

            _logger.LogInformation("Iniciando creación de artículo. GuidProceso: {Guid}, ArticuloId: {ArticuloId}",
             guidProceso, articulo.codigoArticulo);
            _context.ArticuloRequests.Add(articulo);
            await _context.SaveChangesAsync();

            RegistrarImportacion(guidProceso, $"Importacion Articulo '{articulo.codigoArticulo}' desde API");

            int res = 0;
            string mensaje = string.Empty;
            int reintentos = 3, i = 0;

            while (i < reintentos)
            {
                try
                {
                    res = OEM.oem.Importar(guidProceso, $"Importacion Articulo '{articulo.codigoArticulo}' desde API");

                    break;
                }
                catch (Exception)
                {
                    i++;
                    if (i == reintentos)
                        throw;
                }
            }

            //switch (res)
            //{
            //    case 1:
            //        mensaje = "Importación correcta.";
            //        break;
            //    case 2:
            //        mensaje = "Importación terminada con avisos.";
            //        break;
            //    case 3:
            //        mensaje = "Importación ejecutada sin éxito por errores en datos de documento.";
            //        break;
            //    default:
            //        mensaje = "Error no controlado.";
            //        break;
            //}
            //return new ApiResponse
            //{
            //    Success = res == 1 || res == 2,
            //    Message = mensaje,
            //    Result = await GetArticuloByIdAsync(articulo.codigoArticulo)
            //};
            var respuesta = await GetArticuloByIdAsync(articulo.codigoArticulo);
            if (respuesta == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "No se ha generado el artículo debido a errores en los datos proporcionados o en el proceso de importación.",
                    Result = null
                };

            }
            return new ApiResponse
            {
                Success = true,
                Message = "Se ha generado el artículo correctamente",
                Result = respuesta
            };

        }
        public void RegistrarImportacion(Guid IdProcesoIME, string DescripcionProceso)
        {
            Iniciador_ime procesoIME = new Iniciador_ime
            {
                IdProcesoIME = IdProcesoIME,
                Descripcion = DescripcionProceso,
                EstadoIME = 0,
                sysUsuario = 999,
                sysUserName = "OEM"
            };
            _context.Iniciador_imes.Add(procesoIME);

        }
        public void ActualizarIniciador(Guid IdProcesoIME, int estado = 1)
        {

            _context.Iniciador_imes.Where(p => p.IdProcesoIME == IdProcesoIME).ToList().ForEach(p => p.EstadoIME = estado);
        }
        //obtener los atículos con filtros opcionales
        public async Task<List<Articulo>> GetArticulosByFiltersAsync(short codigoempresa, string codigoArticulo = null, string descripcionArticulo = null)
     
        {
            Articulo articulo = GetArticuloByIdAsync(codigoArticulo).Result;
      
            if (!string.IsNullOrWhiteSpace(codigoArticulo) && !string.IsNullOrWhiteSpace(descripcionArticulo) )
            {
                return await _context.Articulos.Where(i => i.codigoArticulo.Contains(codigoArticulo) && i.codigoempresa == codigoempresa && i.descripcionArticulo.Contains(descripcionArticulo)).ToListAsync();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(codigoArticulo)) {
                    return await _context.Articulos.Where(i => i.codigoArticulo.Contains(codigoArticulo) && i.codigoempresa == codigoempresa).ToListAsync();
                }
                if (!string.IsNullOrWhiteSpace(descripcionArticulo)) { 
                    return await _context.Articulos.Where(i => i.descripcionArticulo.Contains(descripcionArticulo) && i.codigoempresa == codigoempresa).ToListAsync();
                }
            }
              return await _context.Articulos.Where(i =>  i.codigoempresa == codigoempresa).ToListAsync();

        }
    }
}
