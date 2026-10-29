using Mandys.Application.DTOs;

namespace Mandys.Application.Services.Interfaces;

public interface IComboService
{
    Task<PagedCombosResponse> GetCombosAsync(int page, int pageSize, string? search, string? orderBy);

    Task<ComboResponse> GetComboByIdAsync(int id);

    Task<ComboResponse> CreateComboAsync(CreateComboRequest request);

    Task<ComboResponse> UpdateComboAsync(int id, UpdateComboRequest request);
    
    Task<ComboResponse> UpdateComboImageAsync(int id, Stream file, string contentType);

    Task DeleteComboAsync(int id);
}
