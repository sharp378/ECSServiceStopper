using Amazon.ECS;
using Amazon.ECS.Model;
using Amazon.Lambda.TestUtilities;
using Moq;
using Xunit;
using Threading = System.Threading.Tasks;

namespace ECSServiceStopper.Tests;

public class FunctionTest
{
    private readonly Function _function;
    private Mock<IAmazonECS> _ecsClient;

    public FunctionTest()
    {
        Environment.SetEnvironmentVariable("ECS_CLUSTER", "cluster");
        Environment.SetEnvironmentVariable("ECS_SERVICE", "service");

        _ecsClient = new Mock<IAmazonECS>();

        _function = new Function(_ecsClient.Object);
    }

    [Fact]
    public async Threading.Task EnvNotSet_Exception()
    {
        // Setup
        Environment.SetEnvironmentVariable("ECS_CLUSTER", null);
        Environment.SetEnvironmentVariable("ECS_SERVICE", null);

        var input = "";
        var context = new TestLambdaContext();

        // SUT & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            _function.FunctionHandler(input, context));
    }

    [Fact]
    public async Threading.Task UpdateFailed_Exception()
    {
        // Setup
        var input = "";
        var context = new TestLambdaContext();

        _ecsClient
            .Setup(ecs => ecs.UpdateServiceAsync(
                It.IsAny<UpdateServiceRequest>(), 
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonECSException("ECS Error"));

        // SUT & Assert
        await Assert.ThrowsAsync<AmazonECSException>(() =>
            _function.FunctionHandler(input, context));
    }

    [Fact]
    public async Threading.Task Success()
    {
        // Setup
        var input = "";
        var context = new TestLambdaContext();

        // SUT
        await _function.FunctionHandler(input, context);
    }
}
