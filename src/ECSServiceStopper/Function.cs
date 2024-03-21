using Amazon.ECS;
using Amazon.ECS.Model;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ECSServiceStopper;

public class Function(IAmazonECS client)
{
    /// <summary>
    /// Stops an ECS service
    /// </summary>
    /// <returns></returns>
    [LambdaFunction]
    public async System.Threading.Tasks.Task FunctionHandler()
    {
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
        await client.UpdateServiceAsync(request, token);
    }
}

[LambdaStartup]
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAWSService<IAmazonECS>();
    }
}
