Car API Documentation

This API allows you to manage a collection of cars stored in Azure Table Storage. It provides endpoints to create, retrieve, list all, and seed car data.

Base URL: https://sd-cloudprogrammeringlabb3-fa.azurewebsites.net

Endpoints

Create Car

URL: /api/CreateCar?code=<your-function-key>
Method: POST
Description: Creates a new car entity in Azure Table Storage.
Request Body:
{
  "Make": "string",
  "Model": "string",
  "Year": int
}
Response:
201 Created: Returns the created car entity.
Example:
{
  "PartitionKey": "partition1",
  "RowKey": "some-guid",
  "Make": "Toyota",
  "Model": "Corolla",
  "Year": "2020"
}

Get Car

URL: /api/GetCar/{rowKey}?code=<your-function-key>
Method: GET
Description: Retrieves a car entity by its RowKey from Azure Table Storage.
Response:
200 OK: Returns the requested car entity.
404 Not Found: If the car entity is not found.
Example:
{
  "PartitionKey": "partition1",
  "RowKey": "some-guid",
  "Make": "Toyota",
  "Model": "Corolla",
  "Year": "2020"
}

Get All Cars

URL: /api/GetAllCars/api/GetCar/{rowKey}?code=<your-function-key>
Method: GET
Description: Retrieves all car entities from Azure Table Storage.
Response:
200 OK: Returns a list of all car entities.
Example:
[
  {
    "Make": "Toyota",
    "Model": "Corolla",
    "Year": 2020
  },
  {
    "Make": "Honda",
    "Model": "Civic",
    "Year": 2019
  }
]

Seed Data

URL: /api/SeedData/api/GetCar/{rowKey}?code=<your-function-key>
Method: POST
Description: Seeds initial car data into Azure Table Storage.
Response:
200 OK: Returns a success message.
Example:
"Seed data created successfully."

How to Use

Create a Car

Send a POST request to /api/CreateCar?code=<your-function-key> with the following JSON body:
{
  "Make": "Toyota",
  "Model": "Corolla",
  "Year": 2020
}

Get a Car

Send a GET request to /api/GetCar/{rowKey}?code=<your-function-key>.
Replace {rowKey} with the actual RowKey of the car you want to retrieve.
The response will contain the requested car entity if found.

Get All Cars

Send a GET request to /api/GetAllCars?code=<your-function-key>.
The response will contain a list of all car entities.

Seed Data

Send a POST request to /api/SeedData.
The response will confirm that the seed data was created successfully.

