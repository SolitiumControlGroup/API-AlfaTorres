using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using API_AlfaTorres.Data;
using API_AlfaTorres.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API_AlfaTorres
{
    public class TrazaImportacion
    {
        public string tipo_documento = string.Empty;
        public int documento = 0;
        public string sudocumento = string.Empty;
        public string codigo_cliente = string.Empty;
        public string cliente_final = string.Empty;
        public int CodigoError = 0;
        public List<Aviso> avisos;
        public class Aviso
        {
            public string status = string.Empty;
            public string mensaje = string.Empty;
        }
        public TrazaImportacion()
        {
            string mensaje = string.Empty;
            avisos = new List<Aviso>();
        }
    }

    public static class OEM
    {
        public static LcOEM oem;
    }
    public class LcOEM : IDisposable
    {
        private object? lcOEM;
        private Type? tipoObjeto;
        private object? lcOEMVarApli;
        private Type? tipoObjetoVarApli;
        private readonly IConfiguration _configuration;

        public LcOEM(IConfiguration configuration)
        {
            _configuration = configuration;
            InicializaOEM();
        }

        public LcOEM(short codigoempresa)
        {
            InicializaOEM(codigoempresa);
        }

        ~LcOEM()
        {
            FinalizaOEM();
        }

        public void Dispose()
        {
            FinalizaOEM();
        }

        public void InicializaOEM()
        {
            FinalizaOEM();

            tipoObjeto = Type.GetTypeFromProgID("LogicControlOEM.OEM_EjecutaOEM");
            lcOEM = Activator.CreateInstance(tipoObjeto);
            tipoObjetoVarApli = Type.GetTypeFromProgID("LogicControlClient.sysVarApli");
            lcOEMVarApli = Activator.CreateInstance(tipoObjetoVarApli);

            string[] argumentos = {
            _configuration["SqlServerSage:CompanyNumber"],
            _configuration["SqlServerSage:OEMUser"],
            _configuration["SqlServerSage:OEMPass"],
            "",
            _configuration["SqlServerSage:DataSource"],
            _configuration["SqlServerSage:InitialCatalog"]
        };

            var result = (string)tipoObjeto?.InvokeMember("InicializaOEM", BindingFlags.InvokeMethod, null, lcOEM, argumentos);
        }

        public void InicializaOEM(short codigoempresa)
        {
            FinalizaOEM();

            tipoObjeto = Type.GetTypeFromProgID("LogicControlOEM.OEM_EjecutaOEM");
            lcOEM = Activator.CreateInstance(tipoObjeto);
            tipoObjetoVarApli = Type.GetTypeFromProgID("LogicControlClient.sysVarApli");
            lcOEMVarApli = Activator.CreateInstance(tipoObjetoVarApli);

            string[] argumentos = {
            codigoempresa.ToString(),
            _configuration["SqlServerSage:OEMUser"],
            _configuration["SqlServerSage:OEMPass"],
            "",
            _configuration["SqlServerSage:DataSource"],
            _configuration["SqlServerSage:InitialCatalog"]
        };

            var result = (string)tipoObjeto?.InvokeMember("InicializaOEM", BindingFlags.InvokeMethod, null, lcOEM, argumentos);
        }

        public void FinalizaOEM()
        {
            if (lcOEM != null)
            {
                if (Marshal.FinalReleaseComObject(lcOEM) != 0)
                    Marshal.ReleaseComObject(lcOEM);
                lcOEM = null;
                tipoObjeto = null;
            }
            if (lcOEMVarApli != null)
            {
                if (Marshal.FinalReleaseComObject(lcOEMVarApli) != 0)
                    Marshal.ReleaseComObject(lcOEMVarApli);
                lcOEMVarApli = null;
                tipoObjetoVarApli = null;
            }
        }


        public string EjecutarCalculo(string Calculo, string Etiqueta, string[] param)
        {
           
            // Construir el string de parámetros
            string parametrosString = "Label:=" + Etiqueta + ",IdProcesoime:="+ param.FirstOrDefault();
         
            // Crear el array de argumentos
            object[] argumentos = new object[2];
            argumentos[0] = Calculo;
            argumentos[1] = parametrosString;

            try
            {
                // Invocar el método mediante reflexión
                string res = tipoObjeto.InvokeMember("EjecutaScript", BindingFlags.InvokeMethod, null, lcOEM, argumentos).ToString();

                if (string.IsNullOrEmpty(res))
                    return "false";
                else
                return res;
                     
            }
            catch (TargetInvocationException ex)
            {
                
                var innerException = ex.InnerException;

                
                Log.Error("Error al invocar EjecutaScript. Fecha y hora: {FechaHora}. Función: {Funcion}. Parámetros: {Parametros}. Excepción: {Excepcion}. Excepción interna: {ExcepcionInterna}",
                    DateTime.Now, Calculo, parametrosString, ex.Message, innerException?.Message);

                return "false";
            }
            catch (Exception ex)
            {
                
                Log.Error("Error inesperado al invocar EjecutaScript. Fecha y hora: {FechaHora}. Función: {Funcion}. Parámetros: {Parametros}. Excepción: {Excepcion}",
                    DateTime.Now, Calculo, parametrosString, ex.Message);

                return "false";
            }
        }

        public int Importar(Guid IdProcesoIME, string DescripcionProceso)
        {
            
            List<object> parametros = new List<object> { "LCCImExP.LcImExProceso", "ImportaIME", IdProcesoIME.ToString(), "1", "1", "4" }; //mantener datos, mantener errores, solo modulo gestión
            object[] argumentos = new object[parametros.Count];
            int res;

            for (int i = 0; i < parametros.Count; i++)
                argumentos[i] = parametros[i];
         
            string ret = tipoObjeto.InvokeMember("EjecutaOEM", BindingFlags.InvokeMethod, null, lcOEM, argumentos).ToString();
            if (string.IsNullOrEmpty(ret))
                return 0;

            res = int.Parse(ret);

            return res;
            
        }
       

        public int Procesa(string clase, string funcion, params string[] args)
        {
            List<object> parametros = new List<object> { clase, funcion };
            parametros.AddRange(args);
            object[] argumentos = parametros.ToArray();
            string ret = tipoObjeto.InvokeMember("EjecutaOEM", BindingFlags.InvokeMethod, null, lcOEM, argumentos).ToString();

            return string.IsNullOrEmpty(ret) ? 0 : int.Parse(ret);
        }
        public int GenerarPDF(string formato, string where, string order, string having, string fichero, (string, string)[] valores)
        {
            List<string> parametros = new List<string> { "LogicControlGenerP.GenFunctions", "ListadoEjecutaPDF", formato, where, order, having, fichero };
            object[] instanceArguments = new object[parametros.Count];
            for (int i = 0; i < parametros.Count; i++)
                instanceArguments[i] = parametros[i];

            foreach ((string, string) valor in valores)
                GrabaVarApli(valor.Item1, valor.Item2);

            string ret = tipoObjeto.InvokeMember("EjecutaOEM", BindingFlags.InvokeMethod, null, lcOEM, instanceArguments).ToString();

            if (string.IsNullOrEmpty(ret))
                return 0;
            else
                return int.Parse(ret);

        }
        public void GrabaVarApli(string campo, string valor)
        {
            object[] instanceArguments = { campo, valor };
            tipoObjetoVarApli.InvokeMember("lcValueVarApli", BindingFlags.SetProperty, null, lcOEMVarApli, instanceArguments);
        }
    }
 
}

            