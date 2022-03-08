namespace LteDev.RegexParsing
{
    public delegate bool TryFunc<TArg, TResult>(TArg arg, out TResult result);
    public delegate bool TryFunc<TArg1, TArg2, TResult>(TArg1 arg1, TArg2 arg2, out TResult result);
}
