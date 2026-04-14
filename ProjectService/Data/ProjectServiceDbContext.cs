using Microsoft.EntityFrameworkCore;
using ProjectService.Domain.Entities;

namespace ProjectService.Data;

public class ProjectServiceDbContext(DbContextOptions<ProjectServiceDbContext>  options) : DbContext(options)
{
    public DbSet<Project> Projects { get; set; }
}