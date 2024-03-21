# ECSServiceStopper
A simple .NET AWS Lambda that stops an ECS service

Requires the following environment variables to be set:  
  
`ECS_CLUSTER`  
The cluster name or ARN holding the service to stop
  
`ECS_SERVICE`  
The service name or ARN to stop



## Here are some steps to follow to get started from the command line:

Once you have edited your template and code you can deploy your application using the [Amazon.Lambda.Tools Global Tool](https://github.com/aws/aws-extensions-for-dotnet-cli#aws-lambda-amazonlambdatools) from the command line.

Install Amazon.Lambda.Tools Global Tools if not already installed.
```
    dotnet tool install -g Amazon.Lambda.Tools
```

If already installed check if new version is available.
```
    dotnet tool update -g Amazon.Lambda.Tools
```

Execute unit tests
```
    cd "ECSServiceStopper/test/ECSServiceStopper.Tests"
    dotnet test
```

Deploy function to AWS Lambda
```
    cd "ECSServiceStopper/src/ECSServiceStopper"
    dotnet lambda deploy-function
```
