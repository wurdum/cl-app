using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ClApp;

public static class Program
{
    public static Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Root command description.");
        rootCommand.AddCommand(new Subcommand1());

        var commandLineBuilder = new CommandLineBuilder(rootCommand);
        commandLineBuilder.UseHost(_ => Host.CreateDefaultBuilder(args), builder =>
        {
            builder
                .ConfigureServices(services =>
                {
                    services.AddLogging(lb => lb
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddSimpleConsole(scfo => scfo.SingleLine = true));
                })
                .UseCommandHandler<Subcommand1, Subcommand1Handler>();
        });

        return commandLineBuilder
            .UseDefaults()
            .Build()
            .InvokeAsync(args);
    }
}

public class Subcommand1 : Command
{
    public Subcommand1() : base("sub1", "Subcommand1 description.")
    {
        AddOption(new Option<int>("--opt1", () => 42, "Option1 description."));
        AddOption(new Option<string>("--opt2", "Option2 description."));
    }
}

public class Subcommand1Handler : ICommandHandler
{
    private readonly ILogger<Subcommand1Handler> _logger;

    public Subcommand1Handler(ILogger<Subcommand1Handler> logger)
    {
        _logger = logger;
    }

    public int? Opt1 { get; set; }

    public string? Opt2 { get; set; }

    public Task<int> InvokeAsync(InvocationContext context)
    {
        _logger.LogInformation("Subcommand1 is executed with the options: Opt1 '{Opt1}', Opt2 '{Opt2}'.", Opt1, Opt2);
        return Task.FromResult(0);
    }
}
