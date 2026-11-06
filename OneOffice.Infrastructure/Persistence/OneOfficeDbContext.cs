using Microsoft.EntityFrameworkCore;

namespace OneOffice.Infrastructure.Persistence;

public sealed class OneOfficeDbContext(DbContextOptions<OneOfficeDbContext> options)
    : DbContext(options);
