namespace LuxorLMS.Kernel;

public readonly struct Result<T>
{
    public T? Value { get; }
    public Error Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    private Result(T value)
    {
        Value = value;
        IsSuccess = true;
        Error = Error.None;
    }

    private Result(Error error)
    {
        Value = default;
        IsSuccess = false;
        Error = error;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);

    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(Error);
    }
}

public readonly struct Result
{
    public Error Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    private Result(Error error)
    {
        Error = error;
        IsSuccess = error == Error.None;
    }

    private Result(bool isSuccess, Error error)
    {
        Error = error;
        IsSuccess = isSuccess;
    }

    public static Result Success() => new(true, Error.None);
    public static Result Success(Error error) => new(true, error);
    public static Result Failure(Error error) => new(error);
}
