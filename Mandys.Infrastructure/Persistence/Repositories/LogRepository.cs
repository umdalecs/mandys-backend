using Mandys.Application.Services.Interfaces;
using Mandys.Infrastructure.Persistence.Records;

namespace Mandys.Infrastructure.Persistence.Repositories;

public class LogRepository(ApplicationDbContext db) : ILogRepository
{
    public async Task CreateLog(string verb, string description)
    {
        await db.Logs.AddAsync(new LogRecord
        {
            Verb = verb,
            Description = description
        });

        await db.SaveChangesAsync();
    }
}
