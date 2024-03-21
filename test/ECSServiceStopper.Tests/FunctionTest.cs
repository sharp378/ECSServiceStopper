using Amazon.ECS;
using Amazon.ECS.Model;
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

        // SUT & Assert
        await Assert.ThrowsAsync<NullReferenceException>(() =>
            _function.FunctionHandler());
    }

    [Fact]
    public async Threading.Task UpdateFailed_Exception()
    {
        // Setup
        _ecsClient
            .Setup(ecs => ecs.UpdateServiceAsync(
                It.IsAny<UpdateServiceRequest>(), 
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new AmazonECSException("ECS Error"));

        // SUT & Assert
        await Assert.ThrowsAsync<AmazonECSException>(() =>
            _function.FunctionHandler());
    }

    [Fact]
    public async Threading.Task Success()
    {
        // SUT
        await _function.FunctionHandler();
    }
}