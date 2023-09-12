
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace sample;

public abstract class Program
{
    private static string serviceName = "MyServiceName";
    private static string serviceVersion = "1.0.0";
    
    private static Tracer? _tracer;
    private static TracerProvider? _tracerProvider;

    public static void Main()
    {
        _tracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddSource(serviceName)
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
            .AddConsoleExporter()
            .Build();
        
        _tracer = _tracerProvider!.GetTracer(serviceName, serviceVersion);
        using var span = _tracer.StartActiveSpan("Outer scope");
        
        span.AddEvent("Starting");
        span.AddEvent("Important middle stuff");
        span.AddEvent("Finishing");
        try
        {
            using var innerSpan = _tracer.StartActiveSpan(name: "Inner scope");
            innerSpan.SetAttribute("Some data", 999);
            DoStuffOverThere();
        }
        catch (Exception ex)
        {
            span.SetAttribute("foo", 1);
            span.SetAttribute("bar", "Hello, World!");
            span.SetAttribute("baz", new int[] { 1, 2, 3 });
            span.SetStatus(Status.Ok);
            span.RecordException(ex);
        }
        
        DoSerilogging();
    }

    private static void DoSerilogging()
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.File("consoleapp.log")
            .CreateLogger();

        Log.Information("Hello, world!");

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var myClass = serviceProvider.GetService<MyClass>();
        myClass!.DoStuff();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(loggingBuilder =>
            loggingBuilder
                .AddSerilog(dispose: true)
                .AddConsole()
                .AddSimpleConsole(options =>
                    options.IncludeScopes = true)
                .Configure(options => 
                    options
                        .ActivityTrackingOptions = ActivityTrackingOptions.SpanId
                                                    | ActivityTrackingOptions.TraceId
                                                    | ActivityTrackingOptions.ParentId
                                                    | ActivityTrackingOptions.Baggage
                                                    | ActivityTrackingOptions.Tags)
        );

        services.AddTransient<MyClass>();
    }

    private static void DoStuffOverThere()
    {
        //Tracer.CurrentSpan.AddEvent("Doing some micro stuff over there");
        
        throw new NotImplementedException("OI!!!!");
    }
}
