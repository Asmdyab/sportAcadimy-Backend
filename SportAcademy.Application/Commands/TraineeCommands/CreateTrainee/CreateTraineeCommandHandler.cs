using AutoMapper;
using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Contract;
using SportAcademy.Domain.Entities;
using SportAcademy.Domain.Enums;
using SportAcademy.Domain.Exceptions.SharedExceptions;
using SportAcademy.Domain.Exceptions.TraineeExceptions;
using SportAcademy.Domain.ValueObjects;

namespace SportAcademy.Application.Commands.Trainees.CreateTrainee
{
    public class CreateTraineeCommandHandler : IRequestHandler<CreateTraineeCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly ITraineeService _traineeService;
        private readonly ITraineeRepository _traineeRepository;
        private readonly IFamilyRepository _familyRepository;
        private readonly string _operationType = OperationType.Add.ToString();

        public CreateTraineeCommandHandler(
            ITraineeService traineeService,
            IMapper mapper,
            ITraineeRepository traineeRepository,
            IFamilyRepository familyRepository)
        {
            _mapper = mapper;
            _traineeService = traineeService;
            _traineeRepository = traineeRepository;
            _familyRepository = familyRepository;
        }

        public async Task<Result<int>> Handle(CreateTraineeCommand request, CancellationToken cancellationToken)
        {
            var trainee = _mapper.Map<Trainee>(request)
                ?? throw new AutoMapperMappingException("Error occurred while mapping.");

            if (trainee.FamilyId <= 0)
            {
                var families = await _familyRepository.GetAllAsync(cancellationToken);
                var firstFamily = families.FirstOrDefault();
                if (firstFamily is not null)
                    trainee.FamilyId = firstFamily.Id;
            }

            if (!_traineeService.IsSSNValid(trainee.SSN, trainee.BirthDate))
                throw new SSNSyntaxErrorException();

            var isSSNExist = await _traineeRepository
                .IsSSNExistAsync(trainee.SSN, cancellationToken);
            if (isSSNExist)
                throw new SSNNotUniqueException();

            var isPhoneNumberExist = await _traineeRepository
                .IsPhoneNumberExistAsync(trainee.PhoneNumber, cancellationToken: cancellationToken);
            if (isPhoneNumberExist)
                throw new PhoneNumberNotUniqueException();

            var ageCategory = trainee.AgeCategory;

            bool isAdult = ageCategory == AgeCategory.Adult;
            bool isGuardianInfoMissing = (string.IsNullOrWhiteSpace(trainee.ParentNumber)
                || string.IsNullOrWhiteSpace(trainee.GuardianName));
            if (!isAdult && isGuardianInfoMissing)
                throw new GuardianInfoMissingException();

            cancellationToken.ThrowIfCancellationRequested();

            trainee.TraineeCode = await _traineeRepository.GenerateTraineeCodeAsync(
                trainee.FamilyId,
                trainee.BranchId,
                trainee.NationalityCategoryId,
                ageCategory,
                cancellationToken);

            trainee.IsSubscribed = false;

            cancellationToken.ThrowIfCancellationRequested();

            await _traineeRepository.AddAsync(trainee, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            return Result<int>.Success(trainee.Id, _operationType);
        }
    }
}