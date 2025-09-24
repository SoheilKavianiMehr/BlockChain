using BlockChain.Dto;
using BlockChain.Model;
using BlockChain.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddSingleton<IBlockChainService, BlockChainService>();

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add model validation
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .SelectMany(x => x.Value!.Errors)
            .Select(x => x.ErrorMessage);
        
        return new BadRequestObjectResult(new { Errors = errors });
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Blockchain API", 
        Version = "v1",
        Description = "A simple blockchain implementation built with .NET 8. Based on the HackerNoon tutorial by Daniel van Flymen.",
        Contact = new() { 
            Name = "Blockchain Project",
            Url = new Uri("https://hackernoon.com/learn-blockchains-by-building-one-117428612f46")
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blockchain API V1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();

// Blockchain endpoints
app.MapPost("/api/blockchain/mine", (IBlockChainService blockChain, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Mining new block...");
        var result = blockChain.Mine();
        logger.LogInformation("Block mined successfully");
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error occurred while mining block");
        return Results.Problem("An error occurred while mining the block");
    }
})
.WithName("MineBlock")
.WithTags("Blockchain")
.WithSummary("Mine a new block")
.WithDescription("Mines a new block with pending transactions and adds it to the blockchain");

app.MapGet("/api/blockchain/chain", (IBlockChainService blockChain, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Retrieving full blockchain...");
        var result = blockChain.GetFullChain();
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error occurred while retrieving blockchain");
        return Results.Problem("An error occurred while retrieving the blockchain");
    }
})
.WithName("GetFullChain")
.WithTags("Blockchain")
.WithSummary("Get the full blockchain")
.WithDescription("Returns the complete blockchain with all blocks and transactions");

// Transaction endpoints
app.MapPost("/api/transactions/create", ([FromBody] Transaction transaction, IBlockChainService blockChain, ILogger<Program> logger) =>
{
    try
    {
        // Validate the transaction
        var context = new ValidationContext(transaction);
        var results = new List<ValidationResult>();
        
        if (!Validator.TryValidateObject(transaction, context, results, true))
        {
            var errors = results.Select(r => r.ErrorMessage);
            return Results.BadRequest(new { Errors = errors });
        }

        logger.LogInformation("Creating new transaction from {Sender} to {Recipient} for amount {Amount}", 
            transaction.Sender, transaction.Recipient, transaction.Amount);
            
        var blockIndex = blockChain.CreateTransaction(transaction.Sender, transaction.Recipient, transaction.Amount);
        
        var response = new
        {
            Message = $"Transaction will be added to Block {blockIndex}",
            Transaction = transaction,
            BlockIndex = blockIndex
        };
        
        return Results.Created($"/api/blockchain/chain", response);
    }
    catch (ArgumentException ex)
    {
        logger.LogWarning("Invalid transaction data: {Message}", ex.Message);
        return Results.BadRequest(new { Error = ex.Message });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error occurred while creating transaction");
        return Results.Problem("An error occurred while creating the transaction");
    }
})
.WithName("CreateTransaction")
.WithTags("Transactions")
.WithSummary("Create a new transaction")
.WithDescription("Creates a new transaction between two parties");

// Node management endpoints
app.MapPost("/api/nodes/register", ([FromBody] NodeRequest nodeRequest, IBlockChainService blockChain, ILogger<Program> logger) =>
{
    try
    {
        // Validate the request
        var context = new ValidationContext(nodeRequest);
        var results = new List<ValidationResult>();
        
        if (!Validator.TryValidateObject(nodeRequest, context, results, true))
        {
            var errors = results.Select(r => r.ErrorMessage);
            return Results.BadRequest(new { Errors = errors });
        }

        logger.LogInformation("Registering {Count} new nodes", nodeRequest.Urls.Count);
        var result = blockChain.RegisterNodes(nodeRequest.Urls);
        return Results.Ok(result);
    }
    catch (ArgumentException ex)
    {
        logger.LogWarning("Invalid node registration data: {Message}", ex.Message);
        return Results.BadRequest(new { Error = ex.Message });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error occurred while registering nodes");
        return Results.Problem("An error occurred while registering nodes");
    }
})
.WithName("RegisterNodes")
.WithTags("Network")
.WithSummary("Register new nodes")
.WithDescription("Registers new nodes in the blockchain network");

app.MapGet("/api/nodes/consensus", (IBlockChainService blockChain, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Running consensus algorithm...");
        var result = blockChain.Consensus();
        logger.LogInformation("Consensus algorithm completed");
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error occurred during consensus");
        return Results.Problem("An error occurred during consensus");
    }
})
.WithName("RunConsensus")
.WithTags("Network")
.WithSummary("Run consensus algorithm")
.WithDescription("Runs the consensus algorithm to resolve conflicts between nodes");

// Health check endpoint
app.MapGet("/api/health", (IBlockChainService blockChain) =>
{
    return Results.Ok(new 
    { 
        Status = "Healthy", 
        NodeId = blockChain.NodeId,
        Timestamp = DateTime.UtcNow
    });
})
.WithName("HealthCheck")
.WithTags("Health")
.WithSummary("Health check")
.WithDescription("Returns the health status of the blockchain node");

app.Run();
