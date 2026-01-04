using Aya.Contract.Services;
using Gaia.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Nestor.Db.Services;

namespace Sprava.Services;

public sealed class SpravaDbContext
    : NestorDbContext,
        IStaticFactory<DbContextOptions, NestorDbContext>
{
    public SpravaDbContext() { }

    public SpravaDbContext(DbContextOptions options)
        : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseModel(SpravaDbContextModel.Instance);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new FileEntityTypeConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    public static NestorDbContext Create(DbContextOptions input)
    {
        return new SpravaDbContext(input);
    }
}

public class SpravaDbContextFactory : IDesignTimeDbContextFactory<SpravaDbContext>
{
    public SpravaDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SpravaDbContext>();
        optionsBuilder.UseSqlite("");

        return new(optionsBuilder.Options);
    }
}
