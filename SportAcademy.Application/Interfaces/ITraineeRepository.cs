using SportAcademy.Application.Common.Pagination;
using SportAcademy.Application.DTOs.TraineeDtos;
using SportAcademy.Domain.Entities;
using SportAcademy.Domain.Enums;
using SportAcademy.Domain.ValueObjects;

namespace SportAcademy.Application.Interfaces
{
    public interface ITraineeRepository : IBaseRepository<Trainee, int>, IPersonRepository
    {
        Task UpdateSports(Trainee trainee, IEnumerable<int> sportIds);
        Task<List<int>> GetSportIdsByTraineeId(int id, CancellationToken cancellationToken = default);
        Task<bool> IsLinkedToSport(int sportId, CancellationToken cancellationToken = default);
        new Task<TraineeDetailsDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Trainee?> GetFullTrainee(int id, CancellationToken cancellationToken = default);
        Task<PagedData<TraineeOfSpecificDayDto>> GetAllTraineesOfSpecificDayAsync(DateTime date, PageRequest page, CancellationToken cancellationToken = default);
        Task<int> GetTraineesCountOfSpecificDayAsync(DateTime date, CancellationToken cancellationToken = default);
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> GetActiveTraineesCount(CancellationToken cancellationToken = default);
        Task<PagedData<TraineeCardDto>> GetFilteredPaginatedAsync(PageRequest page, bool? isSubscribed, string? sport, CancellationToken ct = default);
        Task<PagedData<TraineeCardDto>> SearchAsync(string term, PageRequest page, bool? isSubscribed, string? sport, CancellationToken ct = default);
        Task<PagedData<TraineeCardDto>> SearchByIdAsync(int id, PageRequest page, CancellationToken ct = default);
        Task<List<TraineeDropdownDto>> GetDropdownAsync(CancellationToken cancellationToken = default);
        Task<TraineeCode> GenerateTraineeCodeAsync(int familyId, int branchId, int nationalityCategoryId, AgeCategory ageCategory, CancellationToken cancellationToken = default);
        Task<TraineeDetailsDto?> GetByAppUserIdAsync(string appUserId, CancellationToken cancellationToken = default);
    }
}
