using Mandys.Application.Services.Interfaces;

namespace Mandys.Application.Services.Implementations;

public class LogService(ILogRepository logRepository) : ILogService
{
    public async Task CreateLog(string verb, string description)
    {
        await logRepository.CreateLog(verb, description);
    }
}