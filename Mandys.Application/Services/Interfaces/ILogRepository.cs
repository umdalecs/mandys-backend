namespace Mandys.Application.Services.Interfaces;

public interface ILogRepository
{
    public Task CreateLog(string verb, string description);
}
