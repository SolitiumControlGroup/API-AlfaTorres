using API_AlfaTorres.Models;
using API_AlfaTorres.Models.Formulas;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;


namespace API_AlfaTorres.Data
{
    public class SageDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public SageDbContext(DbContextOptions<SageDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Articulo> Articulos { get; set; }
        public DbSet<ArticuloRequest> ArticuloRequests { get; set; }
        //khadija
        public DbSet<FormulaResponse> Formulas { get; set; }
        public DbSet<FormulaRequest> FormulaRequests { get; set; }
        public DbSet<Mat_FormulaResponse> Mat_FormulaResponse { get; set; }
        public DbSet<Mat_FormulaRequest> Mat_FormulaRequests { get; set; }
        public DbSet<Oper_FormulaResponse> Oper_FormulaResponse { get; set; }
        public DbSet<Oper_FormulaRequest> Oper_FormulaRequests { get; set; }
        public DbSet<OperacionesResponse> Operaciones { get; set; }
        //--
        public DbSet<Iniciador_ime> Iniciador_imes { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Articulo>(entity =>
            {                              
                entity.ToView("VIS_CO_API_Articulos");
                entity.HasKey(e => new { e.codigoArticulo, e.codigoempresa });
                entity.Property(e => e.codigoempresa).HasColumnName("CodigoEmpresa");
                entity.Property(e => e.codigoArticulo).HasColumnName("CodigoArticulo");
                entity.Property(e => e.descripcionArticulo).HasColumnName("DescripcionArticulo");
                entity.Property(e => e.descripcion2Articulo).HasColumnName("Descripcion2Articulo");
            });
            modelBuilder.Entity<ArticuloRequest>(entity => 
            {
                entity.ToTable("TmpIME_Articulos");
                entity.HasKey(e => e.codigoArticulo);
                entity.Property(e => e.codigoempresa).HasColumnName("CodigoEmpresa");
                entity.Property(e => e.codigoArticulo).HasColumnName("CodigoArticulo");
                entity.Property(e => e.descripcionArticulo).HasColumnName("DescripcionArticulo");
                entity.Property(e => e.descripcion2Articulo).HasColumnName("Descripcion2Articulo");
                entity.Property(e => e.IdProcesoIME).HasColumnName("IdProcesoIME");
            });
            //khadija
            modelBuilder.Entity<FormulaResponse>(entity =>
            {
                entity.ToView("VIS_CO_API_Formula");
                entity.HasKey(e => new {e.codigoEmpresa, e.formula, e.codigoArticulo });
                entity.Property(e => e.codigoEmpresa).HasColumnName("CodigoEmpresa");
                entity.Property(e => e.formula).HasColumnName("Formula");
                entity.Property(e => e.codigoArticulo).HasColumnName("CodigoArticulo");
                entity.Property(e => e.descripcionArticulo).HasColumnName("DescripcionArticulo");
                entity.Property(e => e.descripcion2Articulo).HasColumnName("Descripcion2Articulo");
                entity.Property(e => e.nivelCompuesto).HasColumnName("NivelCompuesto");
                entity.Property(e => e.unidadesEscandallo).HasColumnName("UnidadesEscandallo");
                //vincular con materiales
                entity.HasMany(e => e.Materiales)
                  .WithOne()
                  .HasForeignKey(m => new { m.codigoEmpresa, m.formula, m.codigoArticulo });
                //vincular con operaciones
                entity.HasMany(e => e.Operaciones)
                  .WithOne()
                  .HasForeignKey(m => new { m.codigoEmpresa, m.formula, m.codigoArticulo });
            });
            modelBuilder.Entity<FormulaRequest> (entity =>
            {
                entity.ToTable("TmpIME_CO_Formulas");
                entity.HasKey(e => new { e.codigoEmpresa, e.formula, e.codigoArticulo });
                entity.Property(e => e.codigoEmpresa).HasColumnName("CodigoEmpresa");
                entity.Property(e => e.formula).HasColumnName("Formula");
                entity.Property(e => e.codigoArticulo).HasColumnName("CodigoArticulo");
                entity.Property(e => e.descripcionArticulo).HasColumnName("DescripcionArticulo");
                entity.Property(e => e.descripcion2Articulo).HasColumnName("Descripcion2Articulo");
                entity.Property(e => e.nivelCompuesto).HasColumnName("NivelCompuesto");
                entity.Property(e => e.UnidadesEscandallo).HasColumnName("UnidadesEscandallo");
                entity.Property(e => e.IdProcesoIME).HasColumnName("IdProcesoIME");
                //vincular con TMPIME materiales
                entity.HasMany(e => e.TmpIME_Materiales)
                 .WithOne()
                 .HasForeignKey(m => new { m.codigoEmpresa, m.formula, m.codigoArticulo });
                //vincular con TMPIME operaciones
                entity.HasMany(e => e.TmpIME_Operaciones)
                  .WithOne()
                  .HasForeignKey(m => new { m.codigoEmpresa, m.formula, m.codigoArticulo });

            });
            modelBuilder.Entity<Models.Formulas.Mat_FormulaResponse>(entity => 
            {
                entity.ToTable("VIS_CO_API_MatForm");
                entity.HasKey(e => new { e.formula, e.codigoArticulo, e.codigoEmpresa, e.orden });
                entity.Property(e => e.codigoEmpresa).HasColumnName("CodigoEmpresa");
                entity.Property(e => e.formula).HasColumnName("Formula");
                entity.Property(e => e.formulaComponente).HasColumnName("FormulaComponente");
                entity.Property(e => e.articuloComponente).HasColumnName("ArticuloComponente");
                entity.Property(e => e.descripcionArticulo).HasColumnName("DescripcionArticulo");
                entity.Property(e => e.descripcion2Articulo).HasColumnName("Descripcion2Articulo");
                entity.Property(e => e.orden).HasColumnName("Orden");
                entity.Property(e => e.operacion).HasColumnName("Operacion");
                entity.Property(e => e.nivelCompuesto).HasColumnName("NivelCompuesto");
                entity.Property(e => e.unidadesNecesarias).HasColumnName("unidadesNecesarias");
            });
            modelBuilder.Entity<Mat_FormulaRequest>(entity =>
            { 
                entity.ToTable("TmpIME_CO_MatForm");
                entity.HasKey(e => new { e.codigoEmpresa, e.formula, e.codigoArticulo, e.orden });
                entity.Property(e => e.codigoEmpresa).HasColumnName("CodigoEmpresa");
                entity.Property(e => e.formula).HasColumnName("Formula");
                entity.Property(e => e.formulaComponente).HasColumnName("FormulaComponente");
                entity.Property(e => e.articuloComponente).HasColumnName("ArticuloComponente");
                entity.Property(e => e.descripcionArticulo).HasColumnName("DescripcionArticulo");
                entity.Property(e => e.descripcion2Articulo).HasColumnName("Descripcion2Articulo");
                entity.Property(e => e.orden).HasColumnName("Orden");
                entity.Property(e => e.operacion).HasColumnName("Operacion");
                entity.Property(e => e.nivelCompuesto).HasColumnName("NivelCompuesto");
                entity.Property(e => e.unidadesNecesarias).HasColumnName("unidadesNecesarias");
            });
            modelBuilder.Entity<Oper_FormulaResponse>(entity =>
            {
                entity.ToTable("VIS_CO_API_OperForm");
                entity.HasKey(e => new { e.codigoEmpresa, e.formula, e.codigoArticulo, e.orden });
                entity.Property(e => e.codigoEmpresa).HasColumnName("CodigoEmpresa");
                entity.Property(e => e.formula).HasColumnName("Formula");
                entity.Property(e => e.codigoArticulo).HasColumnName("CodigoArticulo");
                entity.Property(e => e.orden).HasColumnName("Orden");
                entity.Property(e => e.operacion).HasColumnName("Operacion");
                entity.Property(e => e.descripcionOperacion).HasColumnName("DescripcionOperacion");
            });
            modelBuilder.Entity<Oper_FormulaRequest>(entity =>
            {
                entity.ToTable("TmpIME_CO_OperForm");
                entity.HasKey(e => new { e.codigoEmpresa, e.formula, e.codigoArticulo, e.orden });
                entity.Property(e => e.codigoEmpresa).HasColumnName("CodigoEmpresa");
                entity.Property(e => e.formula).HasColumnName("Formula");
                entity.Property(e => e.codigoArticulo).HasColumnName("CodigoArticulo");
                entity.Property(e => e.orden).HasColumnName("Orden");
                entity.Property(e => e.operacion).HasColumnName("Operacion");
                entity.Property(e => e.descripcionOperacion).HasColumnName("DescripcionOperacion");
            });
            modelBuilder.Entity<OperacionesResponse>(entity =>
            {
                entity.ToView("VIS_CO_ApiOperaciones");
                entity.HasKey(e => new { e.codigoEmpresa, e.operacion });
                entity.Property(e => e.operacion).HasColumnName("Operacion");
                entity.Property(e => e.descripcionOperacion).HasColumnName("DescripcionOperacion");
            });
            //--
            modelBuilder.Entity<Iniciador_ime>(entity =>
            {
                entity.ToTable("Iniciador_TmpIME");
                entity.HasKey(e => e.IdProcesoIME);
                entity.Property(e => e.IdProcesoIME).HasColumnName("IdProcesoIME");
                entity.Property(e => e.EstadoIME).HasColumnName("EstadoIME");
                entity.Property(e => e.sysUsuario).HasColumnName("sysUsuario");
                entity.Property(e => e.sysUserName).HasColumnName("sysUserName");
                entity.Property(e => e.Descripcion).HasColumnName("Descripcion");
            });
        }
    }
}
