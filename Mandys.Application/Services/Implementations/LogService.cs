
using Mandys.Services.Interfaces;

namespace Mandys.Services.Implementations;

public class LogService(ILogRepository logRepository) : ILogService
{
    public async Task CreateLog(string verb, string description)
    {
        await logRepository.CreateLog(verb, description);
    }
}