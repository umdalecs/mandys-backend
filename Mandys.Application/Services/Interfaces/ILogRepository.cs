using Mandys.DTOs;

namespace Mandys.Services.Interfaces;

public interface ILogRepository
{
    public Task CreateLog(string verb, string description);
}
