namespace SharedKernel.Domain.Abstractions;

public class Result<T>
{

    private Result(bool _isSucceded, T? value, Error _error)
    {
        if(_isSucceded == true && _error != Error.None ||
        _isSucceded == false && _error == Error.None)
        throw new ArgumentException("Wrong exception parameters", nameof(_error));

        IsSucceded = _isSucceded;
        Value = value;
        Error = _error;
    }
    public bool IsSucceded {get;}
    public bool IsFailed => !IsSucceded;
    public T? Value {get;}
    public Error Error {get;}

    public static implicit operator Result<T>(T Value)  => Success(Value);

    public static implicit operator Result<T>(Error Error) => Failure(Error);

    public static Result<T> Success(T value) => new(true, value, Error.None);
    public static Result<T> Failure(Error error) => new(false, default, error); 

    public TOut Match<TOut>(Func<T, TOut> onSuccess, Func<Error, TOut> onFailure) =>
    IsSucceded ? onSuccess(Value!) : onFailure(Error); 
}