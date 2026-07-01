using MediatR;
using SportAcademy.Application.Common.Result;

namespace SportAcademy.Application.Commands.BranchCommands.ActivateBranch;

public record ActivateBranchCommand(int Id) : IRequest<Result<bool>>;
