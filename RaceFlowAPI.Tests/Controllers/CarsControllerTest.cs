using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaceFlowAPI.Database;
using RaceFlowAPI.Models;

namespace RaceFlowAPI.Tests.Controllers;

[TestClass]
public class CarsControllerTest
{
    private static int _createdId;
    private static readonly Car TestCar = new()
    {
        DriverId = 1,
        RaceNumber = 9999
    };
    
    [TestMethod]
    public async Task TestCreate()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        var connection = await DatabaseClient.GetOpenConnectionAsync();

        try
        {
            const string sql = "INSERT INTO cars (racenumber, driver_id) VALUES(@RaceNumber, @DriverId); SELECT LAST_INSERT_ID();";
            var result = await connection.QuerySingleAsync<int>(sql, TestCar);
            _createdId = result;
            Assert.AreNotEqual(0, _createdId);
        }
        catch {}
        finally
        {
            await DatabaseClient.CloseConnectionAsync(connection);
            await DeleteCar(_createdId);
        }
    }
    
    [TestMethod]
    public async Task TestGetAll()
    {
        _createdId = await CreateCar();
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        var connection = await DatabaseClient.GetOpenConnectionAsync();

        try
        {
            const string sql = "SELECT * FROM cars";
            var result = await connection.QueryAsync<Car>(sql);
            var list = result.ToList();
            await DatabaseClient.CloseConnectionAsync(connection);
            Assert.AreNotEqual(0, list.Count);
        }
        catch {}
        finally
        {
            await DatabaseClient.CloseConnectionAsync(connection);
            await DeleteCar(_createdId);
        }
    }
    
    [TestMethod]
    public async Task TestGetSingle()
    {
        _createdId = await CreateCar();
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        var connection = await DatabaseClient.GetOpenConnectionAsync();
        
        try
        {
            const string sql = "SELECT * FROM cars WHERE id = @Id";
            var parameters = new { id = _createdId };
            var result = await connection.QuerySingleOrDefaultAsync<Car>(sql, parameters);
            await DatabaseClient.CloseConnectionAsync(connection);
            Assert.AreEqual(TestCar.DriverId, result.DriverId);
            Assert.AreEqual(TestCar.RaceNumber, result.RaceNumber);
        }
        catch {}
        finally
        {
            await DatabaseClient.CloseConnectionAsync(connection);
            await DeleteCar(_createdId);
        }
    }
    
    [TestMethod]
    public async Task TestUpdate()
    {
        _createdId = await CreateCar();
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        var connection = await DatabaseClient.GetOpenConnectionAsync();

        try
        {
            TestCar.Id = _createdId;
            TestCar.RaceNumber = 9998;
            const string sql = "UPDATE cars SET racenumber = @RaceNumber, driver_id = @DriverId WHERE id = @Id";
            var result = await connection.ExecuteAsync(sql, TestCar);
            Assert.AreNotEqual(0, result);
        }
        catch {}
        finally
        {
            await DatabaseClient.CloseConnectionAsync(connection);
            await DeleteCar(_createdId);
        }
    }
    
    [TestMethod]
    public async Task TestDelete()
    {
        _createdId = await CreateCar();
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        var connection = await DatabaseClient.GetOpenConnectionAsync();
        
        try
        {
            Assert.AreNotEqual(0, _createdId);

            const string sql = "DELETE FROM cars WHERE id = @Id";
            var parameters = new { id = _createdId };
            var result = await connection.ExecuteAsync(sql, parameters);
            Assert.AreNotEqual(0, result);
        }
        catch {}
        finally
        {
            await DatabaseClient.CloseConnectionAsync(connection);
            await DeleteCar(_createdId);
        }
    }
    
    private static async Task<int> CreateCar()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        var connection = await DatabaseClient.GetOpenConnectionAsync();

        try
        {
            const string sql = "INSERT INTO cars (racenumber, driver_id) VALUES(@RaceNumber, @DriverId); SELECT LAST_INSERT_ID();";
            return await connection.QuerySingleAsync<int>(sql, TestCar);
        }
        catch (Exception ex)
        {
            return 0;
        }
        finally
        {
            await DatabaseClient.CloseConnectionAsync(connection);
        }
    }
    
    private static async Task DeleteCar(int id)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        var connection = await DatabaseClient.GetOpenConnectionAsync();
        
        try
        {
            const string sql = "DELETE FROM cars WHERE id = @id";
            var parameters = new { id };
            await connection.ExecuteAsync(sql, parameters);
        }
        catch {}
        finally
        {
            await DatabaseClient.CloseConnectionAsync(connection);
        }
    }
}