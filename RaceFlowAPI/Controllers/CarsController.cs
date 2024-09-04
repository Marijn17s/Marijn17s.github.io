using Dapper;
using Microsoft.AspNetCore.Mvc;
using RaceFlowAPI.Database;
using RaceFlowAPI.Models;
using Serilog;

namespace RaceFlowAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CarsController : ControllerBase
{
    // Create
    [HttpPost]
    public async Task<ActionResult<Car>> CreateCar(Car car)
    {
        var connection = await DatabaseClient.GetOpenConnectionAsync();
        
        try
        {
            const string sql = "INSERT INTO cars (racenumber, driver_id) VALUES(@RaceNumber, @DriverId); SELECT LAST_INSERT_ID();";
            var result = await connection.QuerySingleAsync<int>(sql, car);
            if (result is 0)
                return BadRequest();

            Log.Information("Successful");
            car.Id = result;
            return Created("", car);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "");
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error inserting data");
        }
    }
    
    // Read
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Car>>> GetCars()
    {
        var connection = await DatabaseClient.GetOpenConnectionAsync();
        
        try
        {
            const string sql = "SELECT * FROM cars";
            var result = await connection.QueryAsync<Car>(sql);
            var list = result.ToList();
            if (list.Count is 0)
            {
                Log.Warning("No results");
                return NoContent();
            }
            
            Log.Information("Successful");
            return list;
        }
        catch(Exception ex)
        {
            Log.Error(ex, "");
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error retrieving data");
        }
        finally
        {
            await DatabaseClient.CloseConnectionAsync(connection);
        }
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Car>> GetCar(int id)
    {
        var connection = await DatabaseClient.GetOpenConnectionAsync();
        
        try
        {
            const string sql = "SELECT * FROM cars WHERE id = @id";
            var parameters = new { id };
            var result = await connection.QuerySingleOrDefaultAsync<Car>(sql, parameters);
            if (result is null)
                return NotFound();
            
            Log.Information("Successful");
            return result;
        }
        catch(Exception ex)
        {
            Log.Error(ex, "");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data");
        }
        finally
        {
            await DatabaseClient.CloseConnectionAsync(connection);
        }
    }
    
    // Update
    [HttpPut("{id:int}")]
    public async Task<ActionResult<Car>> UpdateCar(int id, Car car)
    {
        var connection = await DatabaseClient.GetOpenConnectionAsync();
        
        try
        {
            car.Id = id;
            const string sql = "UPDATE cars SET racenumber = @RaceNumber, driver_id = @DriverId WHERE id = @id";
            var result = await connection.ExecuteAsync(sql, car);
            if (result is 0)
                return NotFound($"Car with ID = {id} not found");
            
            Log.Information("Successful");
            return car;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data");
        }
    }
    
    // Delete
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Car>> DeleteCar(int id)
    {
        var connection = await DatabaseClient.GetOpenConnectionAsync();
        
        try
        {
            const string sql = "DELETE FROM cars WHERE id = @id";
            var parameters = new { id };
            var result = await connection.ExecuteAsync(sql, parameters);
            if (result is 0)
                return NotFound($"Car with ID = {id} not found");

            Log.Information("Successful");
            return Accepted("Successfully deleted data");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data");
        }
    }
}