

namespace UserSignUpAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        //
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=Localhost;Database=verifyemaildb;user id=sa;password=xansiety;Integrated Security=true; Encrypt=false;MultipleActiveResultSets=true;");
        } 
        public DbSet<User> Users => Set<User>();


    }
}
