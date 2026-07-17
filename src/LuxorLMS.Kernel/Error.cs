namespace LuxorLMS.Kernel;

public readonly record struct Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public bool IsError => Code != string.Empty;
}
