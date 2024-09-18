using System.Diagnostics.CodeAnalysis;

namespace LteDev.RegexParsing
{
    public delegate bool TryFunc<TArg, TResult>(TArg arg, [NotNullWhen(true)] out TResult? result);
    public delegate bool TryFunc<TArg1, TArg2, TResult>(TArg1 arg1, TArg2 arg2, [NotNullWhen(true)] out TResult? result);
    public delegate bool TryFunc<TArg1, TArg2, TArg3, TResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3, [NotNullWhen(true)] out TResult? result);
    public delegate bool TryFunc<TArg1, TArg2, TArg3, TArg4, TResult>(TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, [NotNullWhen(true)] out TResult? result);
}
