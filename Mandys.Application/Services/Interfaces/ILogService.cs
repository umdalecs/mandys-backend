namespace Mandys.Application.Services.Interfaces;

public interface ILogService
{
    public Task CreateLog(string verb, string description);
}
