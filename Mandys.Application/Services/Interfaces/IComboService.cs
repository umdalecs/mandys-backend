using Mandys.DTOs;

namespace Mandys.Services.Interfaces;

public interface IComboService
{
    Task<PagedCombosResponse> GetCombosAsync(int page, int pageSize, string? search);

    Task<ComboResponse> GetComboByIdAsync(int id);

    Task<ComboResponse> CreateComboAsync(CreateComboRequest request);

    Task<ComboResponse> UpdateComboAsync(int id, UpdateComboRequest request);

    Task DeleteComboAsync(int id);
}
