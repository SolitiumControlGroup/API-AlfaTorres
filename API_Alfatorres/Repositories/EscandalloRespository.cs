using API_AlfaTorres.Data;
using API_AlfaTorres.Models;
using API_AlfaTorres.Models.Formulas;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace API_AlfaTorres.Repositories
{
    public class EscandalloRespository
    {
        private readonly SageDbContext _context;
        private readonly ILogger<EscandalloRespository> _logger;

        public EscandalloRespository(SageDbContext context, IConfiguration configuration, ILogger<EscandalloRespository> logger)
        {
            _context = context;
            _logger = logger;
        }

        //obtener todas las operaciones de una empresa
        public async Task<List<OperacionesResponse>> GetOperaciones(short codigoempresa, string operacion = null )
        {
            if (!string.IsNullOrWhiteSpace(operacion))
            {
                return await _context.Operaciones.Where(c => c.codigoEmpresa == codigoempresa && c.operacion.Contains(operacion)).ToListAsync();
            }
                return await _context.Operaciones.Where(c => c.codigoEmpresa == codigoempresa).ToListAsync();
        }

        

        //obtener todos los artículos generando lista con objeto formula
        public async Task<List<FormulaResponse>> GetAllFormulasAsync()
        {
            return await _context.Formulas
                .Include(f => f.Materiales)  //incluir los materiales asociados a cada fórmula
                .Include(f => f.Operaciones)  //incluir las operaciones asociadas a cada fórmula
                .ToListAsync();
        }
        //obtener la formula mediante su id
        public async Task<FormulaResponse> GetFormulaByIdAsync(short empresa, short formula, string articulo)
        {
        
            return await _context.Formulas
                .Include(f => f.Materiales)
                .Include(f => f.Operaciones)
                .FirstOrDefaultAsync(c => c.codigoEmpresa== empresa && c.formula == formula && c.codigoArticulo == articulo);
        }
        //obtener formulas por empresa
        public async Task<List<FormulaResponse>> GetFormulasByEmpresaAsync(short codigoempresa)
        {
            return await _context.Formulas
                .Include(f => f.Materiales)
                //.Include(f => f.Operaciones)
                .Where(c => c.codigoEmpresa == codigoempresa)
                .ToListAsync();

        }
        public async Task<List<Mat_FormulaResponse>> GetMaterialesByEmpresa(short codigoempresa)
        {
            return await _context.Mat_FormulaResponse
                .ToListAsync();
        }
        //crear una formula nueva
        public async Task<ApiResponse> CreateFormulaAsync(FormulaRequest formula)
        {
            //Cada importación tienen un idproceso Diferente
            Guid guidProceso = Guid.NewGuid();
            formula.IdProcesoIME = guidProceso;

            //reguistara la importación en la tabla de log
            _logger.LogInformation("Iniciando creación de fórmula. GuidProceso: {Guid}, FormulaId: {FormulaId}",
             guidProceso, formula.formula);
           
            //funcion el registro de la importación en la tabla iniciador ime
            RegistrarImportacion(guidProceso, $"Importacion Formula '{formula.formula}' desde API");

            // Añadir la fórmula a la tabla
            _context.FormulaRequests.Add(formula);
            await _context.SaveChangesAsync();

            ////guardar los materiales asociados a la formula
            if (formula.TmpIME_Materiales != null)
            {
                foreach (var material in formula.TmpIME_Materiales)
                {
                    material.IdProcesoIME = guidProceso; // Asignar el mismo IdProcesoIME a los materiales
              
                }
                await _context.SaveChangesAsync();
            }
            ////guardar las operaciones asociados a la formula
            if (formula.TmpIME_Operaciones != null)
            {
                foreach (var operacio in formula.TmpIME_Operaciones)
                {
                    operacio.IdProcesoIME = guidProceso; // Asignar el mismo IdProcesoIME a los materiales

                }
                await _context.SaveChangesAsync();
            }

            string res = "";
            string mensaje = string.Empty;
            int reintentos = 3, i = 0; //Se permitirán hasta 3 reintentos
           //ejecutar el cálculo de sage que se encarga de importar las tmpime
            res = OEM.oem.EjecutarCalculo("CO_IME_ApiEscandallos", "Proceso", [guidProceso.ToString()]); //mantener datos, mantener errores, todos los módulos
            
            //si se ha importado la formula devuelve la formula creada
            var formulaCreada = await GetFormulaByIdAsync(formula.codigoEmpresa, formula.formula, formula.codigoArticulo);
           
            if (formulaCreada != null)
            {
                return new ApiResponse
                {
                    Success = true,
                    Message = mensaje,
                    Result = formulaCreada
                };
            }
            else
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "La fórmula se creó pero no se pudo importar correctamente. " + mensaje,
                    Result = null
                };
            }

        }

        //guardar el registro de la importación en la table iniciador ime, es posible que se tenga que apuntar a la tabla lsysTraceime
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

        //Actualiza el estado de todos los registros con ese Idporcesoime.    
        public void ActualizarIniciador(Guid IdProcesoIME, int estado = 1)
        {

            _context.Iniciador_imes.Where(p => p.IdProcesoIME == IdProcesoIME).ToList().ForEach(p => p.EstadoIME = estado);
        }
    }
}
