namespace SharedKernel.Domain.Abstractions;

public class ResultGeneric<T>
{

    private ResultGeneric(bool _isSucceded, T? value, Error _error)
    {
        if(_isSucceded == true && _error != Error.None ||
        _isSucceded == false && _error == Error.None)
        throw new ArgumentException("Wrong exception parameters", nameof(_error));

        isSucceded = _isSucceded;
        Value = value;
        Error = _error;
    }
    public bool isSucceded {get;}
    public bool isFailed => !isSucceded;
    public T? Value {get;}
    public Error Error {get;}

    public static ResultGeneric<T> Succes(T value) => new(true, value, Error.None);
    public static ResultGeneric<T> Failure(Error error) => new(false, default, error); 
}