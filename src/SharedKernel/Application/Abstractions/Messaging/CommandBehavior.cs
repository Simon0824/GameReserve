using MediatR;

namespace SharedKernel.Application.Abstractions.Messaging;
public interface ICommand<out TResponse> : IRequest<TResponse>;
public interface ICommand : IRequest;