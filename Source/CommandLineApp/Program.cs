namespace CommandLineApp;

internal class Program
{
    private static int Main(string[] args)
    {
        Console.WriteLine("MCP Package Manager (mcpm) - Foundation Setup");
        Console.WriteLine("Version: 1.0.0-foundation");
        Console.WriteLine("Status: Ready for Phase 1 implementation");
        
        if (args.Length > 0)
        {
            Console.WriteLine($"Arguments: {string.Join(" ", args)}");
        }
        
        return 0;
    }
}