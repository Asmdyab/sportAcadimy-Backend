using SportAcademy.Application.Commands.Trainees.CreateTrainee;
using SportAcademy.Application.Commands.Trainees.UpdateTrainee;
using SportAcademy.Application.DTOs.SportDtos;
using SportAcademy.Application.DTOs.TraineeDtos;
using SportAcademy.Domain.Entities;
using SportAcademy.Domain.ValueObjects;

namespace SportAcademy.Application.Mappings.TraineeProfile
{
    public class TraineeProfile : AutoMapper.Profile
    {
        public TraineeProfile()
        {
            CreateMap<DateOnly, DateTime>()
                .ConvertUsing(d => d.ToDateTime(TimeOnly.MinValue));

            CreateMap<SportTrainee, string>()
                .ConvertUsing(st => st.Sport.Name);

            CreateMap<Trainee, TraineeCardDto>()
                .ConstructUsing(src => new TraineeCardDto(
                    src.Id,
                    src.FirstName,
                    src.LastName,
                    GetAge(src),
                    src.Email != null ? src.Email.ToString() : string.Empty,
                    src.PhoneNumber,
                    src.JoinDate.ToDateTime(TimeOnly.MinValue),
                    src.IsSubscribed,
                    src.Sports != null
                        ? src.Sports.Select(st => new TraineeSportSkillDto
                        {
                            SkillLevel = st.SkillLevel,
                            SportName = st.Sport != null ? st.Sport.Name : string.Empty
                        }).ToList()
                        : new List<TraineeSportSkillDto>(),
                    src.Enrollments != null && src.Enrollments.Any()
                        ? GetCoachName(src.Enrollments.First().TraineeGroup)
                        : null,
                    src.Branch != null ? src.Branch.Name : string.Empty
                ))
                .ReverseMap();

            CreateMap<Trainee, TraineeDetailsDto>()
                .ConstructUsing(src => new TraineeDetailsDto(
                    src.Id,
                    src.FirstName,
                    src.LastName,
                    src.Email != null ? src.Email.ToString() : string.Empty,
                    src.PhoneNumber,
                    src.ParentNumber,
                    src.GuardianName,
                    src.Branch != null ? src.Branch.Name : string.Empty,
                    src.BirthDate,
                    src.Gender.ToString(),
                    src.Sports != null
                        ? src.Sports.Select(s => s.Sport != null ? s.Sport.Name : string.Empty)
                            .Where(n => !string.IsNullOrEmpty(n)).ToList()
                        : new List<string>(),
                    src.IsSubscribed,
                    src.Enrollments != null ? src.Enrollments.Count : 0,
                    src.JoinDate.ToDateTime(TimeOnly.MinValue)
                ));

            CreateMap<Trainee, CreateTraineeCommand>()
                .ForMember(dest => dest.Sports, 
                    opt => opt.MapFrom(src => src.Sports.Select(st => new SportIdNameDto(st.SportId,
                        st.Sport.Name
                )).ToHashSet()))
                .ReverseMap()
                .ForMember(dest => dest.Address,
                    opt => opt.MapFrom(src => Address.Create(src.Street, src.City)))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => Email.Create(src.Email)))
                .ForMember(dest => dest.Sports, 
                    opt => opt.MapFrom(src => src.Sports.Select(s => new SportTrainee
                        {
                            SportId = s.Id
                        }).ToList()
                ))
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Trainee, UpdateTraineePersonalCommand>();

            CreateMap<UpdateTraineePersonalCommand, Trainee>()
                .ForMember(src => src.Sports, opt => opt.Ignore())
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Trainee, TraineeDto>()
                .ForMember(dest => dest.Sports, opt => opt.MapFrom(src => src.Sports.Select(st => new SportIdNameDto(st.Sport.Id,
                    st.Sport.Name
                )).ToHashSet()))
                .ReverseMap()
                .ForMember(dest => dest.Sports, opt => opt.MapFrom(src => src.Sports.Select(s => new SportTrainee
                {
                    SportId = s.Id
                }).ToList()));
        }

        private static string? GetCoachName(TraineeGroup? traineeGroup)
        {
            if (traineeGroup?.Coach?.Employee == null)
                return null;
            return $"{traineeGroup.Coach.Employee.FirstName} {traineeGroup.Coach.Employee.LastName}";
        }

        private static int GetAge(Trainee trainee)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var birthDate = (DateOnly)trainee.BirthDate;
            var age = today.Year - birthDate.Year;

            if (birthDate > today.AddYears(-age))
                age--;

            return age;
        }
    }
}
