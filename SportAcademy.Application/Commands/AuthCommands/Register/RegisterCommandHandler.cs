using AutoMapper;
using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Contract;
using SportAcademy.Domain.Entities;
using SportAcademy.Domain.Enums;
using SportAcademy.Domain.Exceptions.SharedExceptions;
using SportAcademy.Domain.Exceptions.UserExceptions;

namespace SportAcademy.Application.Commands.AuthCommands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
    {
        private readonly string _operation = OperationType.Signup.ToString();
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ITraineeRepository _traineeRepository;
        private readonly ITraineeService _traineeService;
        private readonly IFamilyRepository _familyRepository;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IMapper mapper,
            IJwtTokenService jwtTokenService,
            IProfileRepository profileRepository,
            ITraineeRepository traineeRepository,
            ITraineeService traineeService,
            IFamilyRepository familyRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _jwtTokenService = jwtTokenService;
            _profileRepository = profileRepository;
            _traineeRepository = traineeRepository;
            _traineeService = traineeService;
            _familyRepository = familyRepository;
        }

        public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken ct)
        {
            var isUserNameExist = await _userRepository.IsUsernameExistAsync(request.UserName, ct);
            var isEmailExist = await _userRepository.IsEmailExistAsync(request.Email, ct);

            if (isUserNameExist)
                throw new UserNameExistException();

            if (isEmailExist)
                throw new EmailExistException();

            var user = _mapper.Map<AppUser>(request)
                ?? throw new AutoMapperMappingException("Error occurred while mapping.");

            ct.ThrowIfCancellationRequested();

            var identityResult = await _userRepository.Register(user, request.Password, "Trainee");
            if (!identityResult.Succeeded)
                throw new UserRegistrationException(identityResult.Errors.Select(e => e.Description).ToList());

            ct.ThrowIfCancellationRequested();

            var trainee = _mapper.Map<Trainee>(request)
                ?? throw new AutoMapperMappingException("Error occurred while mapping trainee.");

            if (trainee.FamilyId <= 0)
            {
                var families = await _familyRepository.GetAllAsync(ct);
                var firstFamily = families.FirstOrDefault();
                if (firstFamily is not null)
                    trainee.FamilyId = firstFamily.Id;
            }

            if (!_traineeService.IsSSNValid(trainee.SSN, trainee.BirthDate))
                throw new SSNSyntaxErrorException();

            var isSSNExist = await _traineeRepository.IsSSNExistAsync(trainee.SSN, ct);
            if (isSSNExist)
                throw new SSNNotUniqueException();

            var isPhoneNumberExist = await _traineeRepository.IsPhoneNumberExistAsync(trainee.PhoneNumber, excludedId: 0, ct);
            if (isPhoneNumberExist)
                throw new PhoneNumberNotUniqueException();

            ct.ThrowIfCancellationRequested();

            trainee.AppUserId = user.Id;
            trainee.JoinDate = DateOnly.FromDateTime(DateTime.UtcNow);
            trainee.IsSubscribed = false;
            trainee.TraineeCode = await _traineeRepository.GenerateTraineeCodeAsync(
                trainee.FamilyId,
                trainee.BranchId,
                trainee.NationalityCategoryId,
                trainee.AgeCategory,
                ct);

            await _traineeRepository.AddAsync(trainee, ct);

            ct.ThrowIfCancellationRequested();

            var token = _jwtTokenService.GenerateJwtToken(user, "Trainee");

            ct.ThrowIfCancellationRequested();

            var profile = new Domain.Entities.Profile
            {
                AppUserId = user.Id,
            };

            await _profileRepository.AddAsync(profile, ct);

            return Result<string>.Success(token, _operation);
        }
    }
}
