using System.Runtime.CompilerServices;

namespace SharedKernel.Domain.Abstractions;

public class Result
{

    private Result(bool _isSucceded, Error _error)
    {
        if(_isSucceded == true && _error != Error.None ||
        _isSucceded == false && _error == Error.None)
        throw new ArgumentException("Wrong exception parameters", nameof(_error));

        isSucceded = _isSucceded;
        Error = _error;
    }
    public bool isSucceded {get;}
    public bool isFailed => !isSucceded;

    public Error Error {get;}

    public static Result Succes => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error); 
}