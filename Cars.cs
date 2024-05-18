using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Data.Tables;
using System;
using System.Threading.Tasks;
using Azure;
using System.Collections.Generic;

/// <summary>
/// Represents a car with its make, model, and year.
/// </summary>
public class Car
{
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
}


/// <summary>
/// Function to create a new car entity in Azure Table Storage.
/// </summary>
public static class CreateCar
{
    [FunctionName("CreateCar")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<Car>(requestBody);

        var tableClient = new TableClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"), "Cars");

        await tableClient.CreateIfNotExistsAsync();

        var car = new TableEntity("partition1", Guid.NewGuid().ToString())
        {
            { "Make", data.Make },
            { "Model", data.Model },
            { "Year", data.Year.ToString() }
        };

        await tableClient.AddEntityAsync(car);

        return new OkObjectResult(car);
    }
}

/// <summary>
/// Function to retrieve a car entity by its RowKey from Azure Table Storage.
/// </summary>
public static class GetCar
{
    [FunctionName("GetCar")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetCar/{rowKey}")] HttpRequest req,
        ILogger log, string rowKey)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        var tableClient = new TableClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"), "Cars");

        try
        {
            var car = await tableClient.GetEntityAsync<TableEntity>("partition1", rowKey);
            return new OkObjectResult(car);
        }
        catch (RequestFailedException)
        {
            return new NotFoundResult();
        }
    }
}

/// <summary>
/// Function to retrieve all car entities from Azure Table Storage.
/// </summary>
public static class GetAllCars
{
    [FunctionName("GetAllCars")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetAllCars")] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request to get all cars.");

      
        var tableClient = new TableClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"), "Cars");

        List<Car> cars = new List<Car>();

        await foreach (var carEntity in tableClient.QueryAsync<TableEntity>())
        {
            cars.Add(new Car
            {
                Make = carEntity.GetString("Make"),
                Model = carEntity.GetString("Model"),
                Year = int.Parse(carEntity.GetString("Year"))
            });
        }

        return new OkObjectResult(cars);
    }
}

/// <summary>
/// Function to seed initial car data into Azure Table Storage.
/// </summary>

public static class SeedData
{
    [FunctionName("SeedData")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request to seed data.");

        // Initialize the table client
        var tableClient = new TableClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"), "Cars");

        // Create the table if it doesn't exist
        await tableClient.CreateIfNotExistsAsync();

        // Define a list of car entities to seed
        var cars = new List<TableEntity>
        {
            new TableEntity("partition1", Guid.NewGuid().ToString())
            {
                { "Make", "Toyota" },
                { "Model", "Corolla" },
                { "Year", "2020" }
            },
            new TableEntity("partition1", Guid.NewGuid().ToString())
            {
                { "Make", "Honda" },
                { "Model", "Civic" },
                { "Year", "2019" }
            },
            new TableEntity("partition1", Guid.NewGuid().ToString())
            {
                { "Make", "Ford" },
                { "Model", "Focus" },
                { "Year", "2018" }
            }
        };

        
        foreach (var car in cars)
        {
            await tableClient.AddEntityAsync(car);
        }

        return new OkObjectResult("Seed data created successfully.");
    }
}

