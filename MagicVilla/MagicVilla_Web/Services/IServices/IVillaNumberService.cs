using MagicVilla_Web.Models.Dto;

namespace MagicVilla_Web.Services.IServices
{
    public interface IVillaNumberService
    {
        Task<T> GetAllVillaNumbersAsync<T>(string token);

        Task<T> GetAsync<T>(int id, string token);

        Task<T> CreateVillaNumberAsync<T>(VillaNumberCreateDto dto, string token);

        Task<T> UpdateAsync<T>(VillaNumberUpdateDto dto, string token);

        Task<T> DeleteAsync<T>(int id, string token);
    }
}
