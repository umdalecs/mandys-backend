using Mandys.DTOs;

namespace Mandys.Services.Interfaces;

public interface ILogService
{
    public Task CreateLog(string verb, string description);
}
