using Dapper;
using Microsoft.AspNetCore.Mvc;
using RaceFlowAPI.Database;
using RaceFlowAPI.Models;
using Serilog;

namespace RaceFlowAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class RacesController : ControllerBase
{
    // Create
    [HttpPost]
    public async Task<ActionResult<Race>> CreateRace(Race race)
    {
        var connection = await DatabaseClient.GetOpenConnectionAsync();
        
        try
        {
            const string sql = "INSERT INTO races (laps, start_time, end_time) VALUES(@Laps, @StartDate, @EndDate); SELECT LAST_INSERT_ID();";
            var result = await connection.QuerySingleAsync<int>(sql, race);
            if (result is 0)
                return BadRequest();

            Log.Information("Successful");
            race.Id = result;
            return Created("", race);
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
    public async Task<ActionResult<IEnumerable<Race>>> GetRaces()
    {
        var connection = await DatabaseClient.GetOpenConnectionAsync();
        
        try
        {
            const string sql = "SELECT * FROM races";
            var result = await connection.QueryAsync<Race>(sql);
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
    public async Task<ActionResult<Race>> GetRace(int id)
    {
        var connection = await DatabaseClient.GetOpenConnectionAsync();
        
        try
        {
            const string sql = "SELECT * FROM races WHERE id = @id";
            var parameters = new { id };
            var result = await connection.QuerySingleOrDefaultAsync<Race>(sql, parameters);
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
    public async Task<ActionResult<Race>> UpdateRace(int id, Race race)
    {
        var connection = await DatabaseClient.GetOpenConnectionAsync();
        
        try
        {
            race.Id = id;
            const string sql = "UPDATE races SET laps = @Laps, start_time = @StartTime, end_time = @EndTime WHERE id = @id";
            var result = await connection.ExecuteAsync(sql, race);
            if (result is 0)
                return NotFound($"Race with ID = {id} not found");
            
            Log.Information("Successful");
            return race;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "");
            return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data");
        }
    }
    
    // Delete
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Race>> DeleteRace(int id)
    {
        var connection = await DatabaseClient.GetOpenConnectionAsync();
        
        try
        {
            const string sql = "DELETE FROM races WHERE id = @id";
            var parameters = new { id };
            var result = await connection.ExecuteAsync(sql, parameters);
            if (result is 0)
                return NotFound($"Race with ID = {id} not found");

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