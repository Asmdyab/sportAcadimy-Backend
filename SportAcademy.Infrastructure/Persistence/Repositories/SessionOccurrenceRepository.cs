using Microsoft.EntityFrameworkCore;
using SportAcademy.Application.Common.Pagination;
using SportAcademy.Application.DTOs.SessionOccurrenceDtos;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Entities;
using SportAcademy.Domain.Enums;
using SportAcademy.Infrastructure.Persistence.DBContext;
using SportAcademy.Infrastructure.Persistence.Extensions.QueryExtensions;

namespace SportAcademy.Infrastructure.Persistence.Repositories
{
    public class SessionOccurrenceRepository : BaseRepository<SessionOccurrence, int>, ISessionOccurrenceRepository
    {
        private readonly ApplicationDbContext _context;

        public SessionOccurrenceRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PagedData<SessionOccurrenceCardDto>> SearchAsync(string term, PageRequest page, CancellationToken ct = default)
        {
            var query = _context.SessionOccurrences
                .Select(s => new SessionOccurrenceCardDto
                {
                    Id = s.Id,
                    SportName = s.GroupSchedule.TraineeGroup.Coach.Sport.Name,
                    CoachName = s.GroupSchedule.TraineeGroup.Coach.Employee.FirstName + " " + s.GroupSchedule.TraineeGroup.Coach.Employee.LastName,
                    BranchName = s.GroupSchedule.TraineeGroup.Branch.Name,
                    StartTime = s.StartDateTime,
                    DurationInMinutes = s.GroupSchedule.TraineeGroup.DurationInMinutes,
                    TraineeGroupId = s.GroupSchedule.TraineeGroup.Id,
                    TraineeGroupName = s.GroupSchedule.TraineeGroup.Name,
                    TraineesCount = s.GroupSchedule.TraineeGroup.Enrollments.Count(e => e.IsActive && !e.IsDeleted),
                    TotalEnrolled = s.GroupSchedule.TraineeGroup.Enrollments.Count(e => e.IsActive && !e.IsDeleted),
                    TotalPresent = s.Attendances.Count(a => a.AttendanceStatus == AttendanceStatus.Present),
                    TotalAbsent = s.Attendances.Count(a => a.AttendanceStatus == AttendanceStatus.Absent),
                    TotalLate = 0,
                    Date = s.StartDateTime.Date
                });

            if (!string.IsNullOrWhiteSpace(term))
            {
                var t = term.ToLower();
                query = query.Where(s =>
                    s.SportName!.ToLower().Contains(t) ||
                    s.CoachName!.ToLower().Contains(t) ||
                    s.BranchName!.ToLower().Contains(t));
            }

            return await query.ToPagedDataAsync(page, ct);
        }

        public async Task<PagedData<SessionOccurrenceCardDto>> GetByDateAsync(DateTime date, PageRequest page, int? traineeGroupId = null, CancellationToken ct = default)
        {
            var dateStart = date.Date;
            var dateEnd = dateStart.AddDays(1);

            var query = _context.SessionOccurrences
                .Where(s => s.StartDateTime >= dateStart && s.StartDateTime < dateEnd);

            if (traineeGroupId.HasValue)
                query = query.Where(s => s.GroupSchedule.TraineeGroup.Id == traineeGroupId.Value);

            var projected = query.Select(s => new SessionOccurrenceCardDto
                {
                    Id = s.Id,
                    SportName = s.GroupSchedule.TraineeGroup.Coach.Sport.Name,
                    CoachName = s.GroupSchedule.TraineeGroup.Coach.Employee.FirstName + " " + s.GroupSchedule.TraineeGroup.Coach.Employee.LastName,
                    BranchName = s.GroupSchedule.TraineeGroup.Branch.Name,
                    StartTime = s.StartDateTime,
                    DurationInMinutes = s.GroupSchedule.TraineeGroup.DurationInMinutes,
                    TraineeGroupId = s.GroupSchedule.TraineeGroup.Id,
                    TraineeGroupName = s.GroupSchedule.TraineeGroup.Name,
                    TraineesCount = s.GroupSchedule.TraineeGroup.Enrollments.Count(e => e.IsActive && !e.IsDeleted),
                    TotalEnrolled = s.GroupSchedule.TraineeGroup.Enrollments.Count(e => e.IsActive && !e.IsDeleted),
                    TotalPresent = s.Attendances.Count(a => a.AttendanceStatus == AttendanceStatus.Present),
                    TotalAbsent = s.Attendances.Count(a => a.AttendanceStatus == AttendanceStatus.Absent),
                    TotalLate = 0,
                    Date = s.StartDateTime.Date
                });

            return await projected.ToPagedDataAsync(page, ct);
        }

        public async Task<int> GetCountAsync(CancellationToken ct = default)
            => await _context.SessionOccurrences.CountAsync(ct);

        public async Task<PagedData<SessionGroupCardDto>> GetGroupsByDateAsync(DateTime date, PageRequest page, int? traineeGroupId = null, CancellationToken ct = default)
        {
            var dateStart = date.Date;
            var dateEnd = dateStart.AddDays(1);

            var groupsQuery = _context.TraineeGroups
                .AsNoTracking()
                .Where(tg => tg.GroupSchedules
                    .Any(gs => gs.SessionOccurrences
                        .Any(so => so.StartDateTime >= dateStart && so.StartDateTime < dateEnd)));

            if (traineeGroupId.HasValue)
                groupsQuery = groupsQuery.Where(tg => tg.Id == traineeGroupId.Value);

            var totalCount = await groupsQuery.CountAsync(ct);

            var groups = await groupsQuery
                .OrderBy(tg => tg.Name)
                .Skip(page.Skip)
                .Take(page.PageSize)
                .Select(tg => new SessionGroupCardDto
                {
                    TraineeGroupId = tg.Id,
                    TraineeGroupName = tg.Name,
                    SportName = tg.Coach.Sport.Name,
                    CoachName = tg.Coach.Employee.FirstName + " " + tg.Coach.Employee.LastName,
                    BranchName = tg.Branch.Name,
                    DurationInMinutes = tg.DurationInMinutes,
                    Occurrences = tg.GroupSchedules
                        .SelectMany(gs => gs.SessionOccurrences)
                        .Where(so => so.StartDateTime >= dateStart && so.StartDateTime < dateEnd)
                        .Select(so => new SessionOccurrenceBriefDto
                        {
                            Id = so.Id,
                            StartDateTime = so.StartDateTime,
                            TraineesCount = tg.Enrollments.Count(e => e.IsActive && !e.IsDeleted),
                            TotalEnrolled = tg.Enrollments.Count(e => e.IsActive && !e.IsDeleted),
                            TotalPresent = so.Attendances.Count(a => a.AttendanceStatus == AttendanceStatus.Present),
                            TotalAbsent = so.Attendances.Count(a => a.AttendanceStatus == AttendanceStatus.Absent),
                            TotalLate = 0
                        })
                        .ToList()
                })
                .ToListAsync(ct);

            return new PagedData<SessionGroupCardDto>
            {
                Items = groups,
                TotalCount = totalCount,
                Page = page.Page,
                PageSize = page.PageSize
            };
        }

        public async Task<bool> ExistsByScheduleAndDateTimeAsync(int groupScheduleId, DateTime startDateTime, CancellationToken ct = default)
            => await _context.SessionOccurrences
                .AnyAsync(s => s.GroupScheduleId == groupScheduleId && s.StartDateTime == startDateTime, ct);
    }
}
