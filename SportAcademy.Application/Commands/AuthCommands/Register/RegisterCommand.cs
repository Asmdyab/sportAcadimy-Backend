using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Domain.Enums;

namespace SportAcademy.Application.Commands.AuthCommands.Register
{
    public record RegisterCommand : IRequest<Result<string>>
    {
        public required string UserName { get; init; }
        public required string Email { get; init; }
        public required string Password { get; init; }
        public required string PhoneNumber { get; init; }
        public bool EmailConfirmed { get; init; } = true;

        // Trainee-specific fields
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string SSN { get; init; }
        public DateOnly BirthDate { get; init; }
        public Gender Gender { get; init; }
        public Nationality Nationality { get; init; }
        public string? City { get; init; }
        public string? Street { get; init; }
        public int BranchId { get; init; }
        public int NationalityCategoryId { get; init; }
    }
}
