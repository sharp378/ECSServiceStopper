using Amazon;
using Amazon.ECS;
using Amazon.ECS.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using Threading=System.Threading.Tasks;

namespace ECSServiceStopper;

public class Function
{
    private static IAmazonECS? _client;

    public Function(IAmazonECS client)
    {
        _client = client;
    }

    /// <summary>
    /// The main entry point for the Lambda function. The main function is called once during the Lambda init phase. It
    /// initializes the .NET Lambda runtime client passing in the function handler to invoke for each Lambda event and
    /// the JSON serializer to use for converting Lambda JSON format to the .NET types. 
    /// </summary>
    private static async Threading.Task Main()
    {
        var handler = FunctionHandler;
        await LambdaBootstrapBuilder.Create(handler, new SourceGeneratorLambdaJsonSerializer<LambdaFunctionJsonSerializerContext>())
            .Build()
            .RunAsync();
    }

    /// <summary>
    /// Stops an ECS service by setting its desired count to zero.
    /// </summary>
    /// <param name="input">(UNUSED) The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public static  async Threading.Task FunctionHandler(string input, ILambdaContext context)
    {
        if (_client is null)
        {
            throw new NullReferenceException("ECS Client not initialized");
        }

        var cluster = Environment.GetEnvironmentVariable("ECS_CLUSTER")
            ?? throw new NullReferenceException("ECS_CLUSTER environment variable is required");
        var service = Environment.GetEnvironmentVariable("ECS_SERVICE")
            ?? throw new NullReferenceException("ECS_SERVICE environment variable is required");

        var request = new UpdateServiceRequest
        {
            Cluster = cluster,
            Service = service,
            DesiredCount = 0
        };

        var token = new CancellationToken();
        await _client.UpdateServiceAsync(request, token);
    }
}

/// <summary>
/// This class is used to initialize dependency injection
/// </summary>
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // the following line produces an AOT warning... 
        // doing a manual setup for now 
        // services.AddAWSService<IAmazonECS>();
        
        services.AddSingleton<IAmazonECS, AmazonECSClient>();
    }
}

/// <summary>
/// This class is used to register the input event and return type for the FunctionHandler method with the System.Text.Json source generator.
/// There must be a JsonSerializable attribute for each type used as the input and return type or a runtime error will occur 
/// from the JSON serializer unable to find the serialization information for unknown types.
/// </summary>
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(Threading.Task))]
public partial class LambdaFunctionJsonSerializerContext : JsonSerializerContext
{
    // By using this partial class derived from JsonSerializerContext, we can generate reflection free JSON Serializer code at compile time
    // which can deserialize our class and properties. However, we must attribute this class to tell it what types to generate serialization code for.
    // See https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-source-generation
}