using AutoMapper;
using MediatR;
using SportAcademy.Application.Common.Result;
using SportAcademy.Application.DTOs.EmployeeDtos;
using SportAcademy.Application.Interfaces;
using SportAcademy.Domain.Entities;
using SportAcademy.Domain.Enums;
using SportAcademy.Domain.Exceptions.EmployeeExceptions;
using SportAcademy.Domain.Exceptions.SharedExceptions;
using SportAcademy.Domain.ValueObjects;

namespace SportAcademy.Application.Commands.EmployeeCommands.UpdateEmployee
{
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Result<EmployeeDto>>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly string _operationType = OperationType.Update.ToString();

        public UpdateEmployeeCommandHandler(
            IMapper mapper,
            IEmployeeRepository employeeRepository)
        {
            _mapper = mapper;
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<EmployeeDto>> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new EmployeeNotFoundException($"{request.Id}");

            if (request.PhoneNumber != null && request.PhoneNumber != employee.PhoneNumber)
            {
                var isPhoneNumberExist = await _employeeRepository
                    .IsPhoneNumberExistAsync(request.PhoneNumber, employee.Id, cancellationToken);
                if (isPhoneNumberExist)
                    throw new PhoneNumberNotUniqueException();
            }

            ApplyUpdates(employee, request);

            cancellationToken.ThrowIfCancellationRequested();

            await _employeeRepository.UpdateAsync(employee, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            var employeeDto = _mapper.Map<EmployeeDto>(employee)
                ?? throw new AutoMapperMappingException("Error occurred while mapping.");

            return Result<EmployeeDto>.Success(employeeDto, _operationType);
        }

        private static void ApplyUpdates(Employee employee, UpdateEmployeeCommand request)
        {
            if (request.PhoneNumber != null)
                employee.PhoneNumber = request.PhoneNumber;

            if (request.SecondPhoneNumber != null)
                employee.SecondPhoneNumber = request.SecondPhoneNumber;

            if (request.Position != null)
                employee.Position = Enum.Parse<Position>(request.Position, ignoreCase: true);

            if (request.Salary.HasValue)
                employee.Salary = request.Salary.Value;

            if (request.BranchId.HasValue)
                employee.BranchId = request.BranchId.Value;

            if (request.Nationality != null)
                employee.Nationality = Enum.Parse<Nationality>(request.Nationality, ignoreCase: true);

            UpdateAddress(employee, request);
        }

        private static void UpdateAddress(Employee employee, UpdateEmployeeCommand request)
        {
            var newStreet = request.Street ?? employee.Address?.Street;
            var newCity = request.City ?? employee.Address?.City;

            if (request.Street != null || request.City != null)
                employee.Address = Address.Create(newStreet!, newCity!);
        }
    }
}
