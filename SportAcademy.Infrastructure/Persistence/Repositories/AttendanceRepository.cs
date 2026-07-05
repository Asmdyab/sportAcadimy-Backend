using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SportAcademy.Application.Common.Pagination;
using SportAcademy.Application.DTOs.AttendanceDtos;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Entities;
using SportAcademy.Domain.Enums;
using SportAcademy.Infrastructure.Persistence.DBContext;
using SportAcademy.Infrastructure.Persistence.Extensions.QueryExtensions;

namespace SportAcademy.Infrastructure.Persistence.Repositories
{
    public class AttendanceRepository : BaseRepository<Attendance, int>, IAttendanceRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AttendanceRepository(ApplicationDbContext context, IMapper mapper)
            : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> GetGlobalAttendanceRate(CancellationToken ct = default)
        {
            var total = await _context.Attendances.CountAsync(ct);
            if (total == 0) return 0;
            var present = await _context.Attendances
                .CountAsync(a => a.AttendanceStatus == AttendanceStatus.Present, ct);
            return present * 100 / total;
        }

        public async Task<int> GetMonthlyAttendanceRate(Month month, CancellationToken ct = default)
        {
            var monthlyTotal = await _context.Attendances
                .CountAsync(a => a.AttendanceDate.Month == (int)month, ct);
            if (monthlyTotal == 0) return 0;
            var present = await _context.Attendances
                .Where(a => a.AttendanceDate.Month == (int)month)
                .CountAsync(a => a.AttendanceStatus == AttendanceStatus.Present, ct);
            return present * 100 / monthlyTotal;
        }

        public async Task<PagedData<AttendanceDto>> GetAllAsync(
            PageRequest page,
            CancellationToken cancellationToken = default)
            => await _context.Attendances
                .Include(a => a.Enrollment)
                .Include(a => a.SessionOccurrence)
                .ProjectTo<AttendanceDto>(_mapper.ConfigurationProvider)
                .ToPagedDataAsync(page, cancellationToken);

        public async Task<Attendance?> GetBySessionAndEnrollmentAsync(int sessionOccurrenceId, int enrollmentId, CancellationToken ct = default)
            => await _context.Attendances
                .FirstOrDefaultAsync(a => a.SessionOccurrenceId == sessionOccurrenceId && a.EnrollmentId == enrollmentId, ct);

        public async Task<List<AttendanceByDateRecordDto>> GetByDateAsync(DateTime date, CancellationToken ct = default)
        {
            var dateStart = date.Date;
            var dateEnd = dateStart.AddDays(1);

            return await _context.Attendances
                .Where(a => a.AttendanceDate >= dateStart && a.AttendanceDate < dateEnd)
                .Select(a => new AttendanceByDateRecordDto
                {
                    Id = a.Id,
                    SessionOccurrenceId = a.SessionOccurrenceId,
                    SportName = a.SessionOccurrence.GroupSchedule.TraineeGroup.Coach.Sport.Name,
                    CoachName = a.SessionOccurrence.GroupSchedule.TraineeGroup.Coach.Employee.FirstName
                                + " " + a.SessionOccurrence.GroupSchedule.TraineeGroup.Coach.Employee.LastName,
                    BranchName = a.SessionOccurrence.GroupSchedule.TraineeGroup.Branch.Name,
                    StartTime = a.SessionOccurrence.StartDateTime,
                    DurationInMinutes = a.SessionOccurrence.GroupSchedule.TraineeGroup.DurationInMinutes,
                    TraineeId = a.Enrollment.TraineeId,
                    TraineeName = a.Enrollment.Trainee.FirstName + " " + a.Enrollment.Trainee.LastName,
                    CheckInTime = a.CheckInTime.ToString(),
                    Status = a.AttendanceStatus.ToString(),
                    AttendanceDate = a.AttendanceDate
                })
                .ToListAsync(ct);
        }

        public async Task<List<AttendanceRecordDto>> GetBySessionAsync(int sessionOccurrenceId, CancellationToken ct = default)
            => await _context.Enrollments
                .Where(e => !e.IsDeleted && e.IsActive)
                .Where(e => e.TraineeGroup.GroupSchedules
                    .Any(gs => gs.SessionOccurrences
                        .Any(so => so.Id == sessionOccurrenceId)))
                .Select(e => new AttendanceRecordDto
                {
                    Id = e.Attendances
                        .Where(a => a.SessionOccurrenceId == sessionOccurrenceId)
                        .Select(a => a.Id)
                        .FirstOrDefault(),
                    TraineeId = e.TraineeId,
                    TraineeName = e.Trainee.FirstName + " " + e.Trainee.LastName,
                    CheckInTime = e.Attendances
                        .Where(a => a.SessionOccurrenceId == sessionOccurrenceId)
                        .Select(a => a.CheckInTime.ToString())
                        .FirstOrDefault(),
                    Status = e.Attendances
                        .Where(a => a.SessionOccurrenceId == sessionOccurrenceId)
                        .Select(a => a.AttendanceStatus.ToString())
                        .FirstOrDefault() ?? "Absent"
                })
                .ToListAsync(ct);

        public async Task<(int TotalSessions, int AttendedSessions)> GetAttendanceSummaryAsync(
            int traineeId,
            DateOnly? fromDate,
            DateOnly? toDate,
            CancellationToken cancellationToken)
        {
            var query = _context.Attendances
                .Include(a => a.Enrollment)
                .Include(a => a.SessionOccurrence)
                .Where(a => a.Enrollment.TraineeId == traineeId);

            if (fromDate.HasValue)
                query = query.Where(a =>
                    DateOnly.FromDateTime(a.SessionOccurrence.StartDateTime) >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(a =>
                    DateOnly.FromDateTime(a.SessionOccurrence.StartDateTime) <= toDate.Value);

            var total = await query.CountAsync(cancellationToken);
            var attended = await query
                .CountAsync(a => a.AttendanceStatus == AttendanceStatus.Present, cancellationToken);

            return (total, attended);
        }

        public async Task<PagedData<TraineeAttendanceReportDto>> GetAttendanceReportByTraineeAsync(int traineeId, PageRequest page, CancellationToken ct = default)
        {
            return await _context.TraineeAttendanceReports
                .Where(v => v.TraineeId == traineeId)
                .Select(v => new TraineeAttendanceReportDto(
                    v.TraineeId,
                    v.FirstName,
                    v.LastName,
                    v.GroupId,
                    v.GroupName,
                    v.SportName,
                    v.BranchName,
                    v.SubscriptionStartDate,
                    v.SubscriptionEndDate,
                    v.EnrollmentId,
                    v.IsActive,
                    v.TotalSessions,
                    v.AttendedSessions,
                    v.AbsentSessions,
                    v.AttendanceRate,
                    v.AbsenceRate,
                    0
                ))
                .ToPagedDataAsync(page, ct);
        }

        public async Task<PagedData<TraineeAttendanceReportDto>> GetAttendanceReportAsync(
            PageRequest page,
            CancellationToken cancellationToken = default)
        {
            return await _context.TraineeAttendanceReports
                .Select(v => new TraineeAttendanceReportDto(
                    v.TraineeId,
                    v.FirstName,
                    v.LastName,
                    v.GroupId,
                    v.GroupName,
                    v.SportName,
                    v.BranchName,
                    v.SubscriptionStartDate,
                    v.SubscriptionEndDate,
                    v.EnrollmentId,
                    v.IsActive,
                    v.TotalSessions,
                    v.AttendedSessions,
                    v.AbsentSessions,
                    v.AttendanceRate,
                    v.AbsenceRate,
                    0 // ConsecutiveAbsences — يتحسب في الـ Handler
                ))
                .ToPagedDataAsync(page, cancellationToken);
        }

        public async Task<Dictionary<int, List<AttendanceStatus>>> GetAttendanceStatusesByEnrollmentsAsync(
            IEnumerable<int> enrollmentIds,
            CancellationToken cancellationToken = default)
        {
            var records = await _context.Attendances
                .Where(a => enrollmentIds.Contains(a.EnrollmentId))
                .OrderByDescending(a => a.SessionOccurrence.StartDateTime)
                .Select(a => new { a.EnrollmentId, a.AttendanceStatus })
                .ToListAsync(cancellationToken);

            return records
                .GroupBy(a => a.EnrollmentId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(a => a.AttendanceStatus).ToList()
                );
        }
    }
}