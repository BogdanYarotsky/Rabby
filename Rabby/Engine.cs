namespace Rabby;

public static class Engine
{
    public static string Execute(string[] args) => args[0] switch
    {
        "declare" => Declare(args[1..]),
        "help" => GetHelp(args[1..]),
        "version" => GetVersion(args[1..]),
        _ => throw new ArgumentOutOfRangeException()
    };

    private static string GetVersion(string[] args) => args.Length switch
    {
        0 => typeof(Engine).Assembly.GetName().Version?.ToString() ?? "wtf",
        _ => "wtf"
    };

    private static string GetHelp(string[] args) => args.Length switch
    {
        0 => GetHelp(),
        _ => "wtf"
    };

    private static string GetHelp() => "Help text is WIP";

    private static string Declare(string[] args)
    {
        if (args == null) throw new ArgumentNullException(nameof(args));
        if (args.Length == 0) throw new ArgumentException();

        return args.First() switch
        {
            "exchange" => DeclareExchange(args[1..]),
            "queue" => DeclareQueue(args[1..]),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static string DeclareQueue(string[] args) => "todo";
    private static string DeclareExchange(string[] args) => "todo";
}
