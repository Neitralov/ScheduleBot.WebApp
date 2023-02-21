namespace ScheduleBot.DataBase;

public sealed class DataBaseProvider : DbContext
{
    public DbSet<Subscriber> Subscribers => Set<Subscriber>();

    public DataBaseProvider()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Database/Database.db");
    }
}