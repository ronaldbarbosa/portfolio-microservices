using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ProjectService.Domain.Entities;

namespace ProjectService.Data;

public class ProjectServiceDbContext(DbContextOptions<ProjectServiceDbContext>  options) : DbContext(options)
{
    public DbSet<Project> Projects { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");
    }
}