using Amazon.Lambda.Core;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.ECS;
using Amazon.ECS.Model;
using Threading=System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ECSServiceStopper;

/// <summary>
/// Holds function handlers for Lambda invocations 
/// </summary>
/// <param name="client">The ECS client</param>
public class Functions(IAmazonECS client)
{
    /// <summary>
    /// Stops an ECS service by setting its desired count to zero.
    /// </summary>
    /// <param name="input">(UNUSED) The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    [LambdaFunction(MemorySize = 128, Timeout = 10)]
    public async Threading.Task StopECSServiceAsync(string input, ILambdaContext context)
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