using MediatR;

namespace SharedKernel.Application.Abstractions.Messaging;
public interface ICommandBase;
public interface ICommand<out TResponse> : IRequest<TResponse>, ICommandBase;
public interface ICommand : IRequest, ICommandBase;