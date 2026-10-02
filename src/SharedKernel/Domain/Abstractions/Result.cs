namespace SharedKernel.Domain.Abstractions;

public class Result
{

    private Result(bool _isSucceded, Error _error)
    {
        if(_isSucceded == true && _error != Error.None ||
        _isSucceded == false && _error == Error.None)
        throw new ArgumentException("Wrong exception parameters", nameof(_error));

        IsSucceded = _isSucceded;
        Error = _error;
    }
    public bool IsSucceded {get;}
    public bool IsFailed => !IsSucceded;

    public Error Error {get;}

    public static implicit operator Result(Error error) => Failure(error);

    public static Result Success => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error); 
}