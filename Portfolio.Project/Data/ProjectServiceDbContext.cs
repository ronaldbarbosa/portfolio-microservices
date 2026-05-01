using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Portfolio.Project.Domain.Entities;

namespace Portfolio.Project.Data;

public class ProjectServiceDbContext(DbContextOptions<ProjectServiceDbContext>  options) : DbContext(options)
{
    public DbSet<Domain.Entities.Project> Projects { get; set; }
}