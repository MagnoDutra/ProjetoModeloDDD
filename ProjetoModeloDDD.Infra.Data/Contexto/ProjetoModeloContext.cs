using ProjetoModeloDDD.Domain.Entities;
using ProjetoModeloDDD.Infra.Data.EntityConfig;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

// Esse arquivo é a criacao do DBContext
namespace ProjetoModeloDDD.Infra.Data.Contexto
{
    public class ProjetoModeloContext : DbContext
    {
        // Aqui estou passando no construtor pai o nome da linha da conexao no appsettings.json
        public ProjetoModeloContext() : base("ProjetoModeloDDD")
        {
            
        }

        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>(); // Retira pluralizacao das tabelas, uma tabela que se chamaria produtos ele criaria como produtoes pois ele n sabe falar pt
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>(); // Nao precisa configurar no EF Core
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Properties().Where(p => p.Name == p.ReflectedType.Name + "Id").Configure(p => p.IsKey()); // No EF antigo é bom dizer para ele que a propriedade que termine com Id é uma primary key caso vc queira

            modelBuilder.Properties<string>().Configure(p => p.HasColumnType("varchar")); // Aqui quem for string vai ser o tipo varchar

            modelBuilder.Properties<string>().Configure(p => p.HasMaxLength(100)); // Nesse caso estamos dizendo que o maxlength de todos é 100.

            modelBuilder.Configurations.Add(new ClienteConfiguration());

        }

        // Aqui sobrescrevemos o commit
        public override int SaveChanges()
        {
            // Esse foreach esta vendo a propriedade DataCadastro e setando ela para que nao precisemos ficar fazendo isso em nosso controller.
            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("DataCadastro") != null))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("DataCadastro").CurrentValue = DateTime.Now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property("DataCadastro").IsModified = false;
                }
            }

            return base.SaveChanges();
        }
    }
}
