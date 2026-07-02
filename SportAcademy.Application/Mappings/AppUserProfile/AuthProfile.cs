using AutoMapper;
using SportAcademy.Application.Commands.AuthCommands.Register;
using SportAcademy.Domain.Entities;
using SportAcademy.Domain.ValueObjects;

namespace SportAcademy.Application.Mappings.AppUserProfile
{
    public class AuthProfile : AutoMapper.Profile
    {
        public AuthProfile()
        {
            ShouldMapProperty = p => 
                p.Name != nameof(AppUser.PasswordHash)
                && p.Name != nameof(AppUser.SecurityStamp);

            CreateMap<RegisterCommand, AppUser>()
                .ForAllMembers(opt =>
                {
                    if (opt.DestinationMember.Name != nameof(AppUser.UserName) &&
                        opt.DestinationMember.Name != nameof(AppUser.Email) &&
                        opt.DestinationMember.Name != nameof(AppUser.PhoneNumber) &&
                        opt.DestinationMember.Name != nameof(AppUser.EmailConfirmed))
                    {
                        opt.Ignore();
                    }
                });

            CreateMap<RegisterCommand, Trainee>()
                .ForMember(d => d.FirstName, o => o.MapFrom(s => s.FirstName))
                .ForMember(d => d.LastName, o => o.MapFrom(s => s.LastName))
                .ForMember(d => d.SSN, o => o.MapFrom(s => s.SSN))
                .ForMember(d => d.BirthDate, o => o.MapFrom(s => s.BirthDate))
                .ForMember(d => d.Gender, o => o.MapFrom(s => s.Gender))
                .ForMember(d => d.Nationality, o => o.MapFrom(s => s.Nationality))
                .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PhoneNumber))
                .ForMember(d => d.Email, o => o.MapFrom(s => Email.Create(s.Email)))
                .ForMember(d => d.Address, o => o.MapFrom(s => Address.Create(s.Street ?? "", s.City ?? "")))
                .ForMember(d => d.BranchId, o => o.MapFrom(s => s.BranchId))
                .ForMember(d => d.NationalityCategoryId, o => o.MapFrom(s => s.NationalityCategoryId));
        }
    }
}
