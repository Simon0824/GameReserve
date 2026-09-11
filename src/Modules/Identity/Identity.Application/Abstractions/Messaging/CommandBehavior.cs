using MediatR;

namespace Identity.Application.Abstractioncs.Messaging;
public interface ICommand<out TResponse> : IRequest<TResponse>;
public interface ICommand : IRequest;