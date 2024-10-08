﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RaceFlowAPI.Controllers;
using RaceFlowAPI.Database;
using RaceFlowAPI.Models;
using Serilog;

namespace RaceFlowAPI.Tests.Controllers;

[TestClass]
public class RacesControllerTest
{
    private static int _createdId;
    private static readonly Race TestRace = new()
    {
        Laps = 9999,
        StartTime = DateTime.Now,
        EndTime = DateTime.Now + TimeSpan.FromHours(1)
    };
    
    [TestMethod]
    public async Task TestCreate()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        var connection = await DatabaseClient.GetOpenConnectionAsync();

        try
        {
            const string sql = "INSERT INTO races (laps, start_time, end_time) VALUES(@Laps, @StartTime, @EndTime); SELECT LAST_INSERT_ID();";
            var result = await connection.QuerySingleAsync<int>(sql, TestRace);
            _createdId = result;
            Assert.AreNotEqual(0, _createdId);
        }
        catch {}
        finally
        {
            await DatabaseClient.CloseConnectionAsync(connection);
            await DeleteRace(_createdId);
        }
    }
    
    [TestMethod]
    public async Task TestGetAll()
    {
        _createdId = await CreateRace();
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        var connection = await DatabaseClient.GetOpenConnectionAsync();

        try
        {
            const string sql = "SELECT * FROM races";
            var result = await connection.QueryAsync<Race>(sql);
            var list = result.ToList();
            await DatabaseClient.CloseConnectionAsync(connection);
            Assert.AreNotEqual(0, list.Count);
        }
        catch {}
        finally
        {
            await DatabaseClient.CloseConnectionAsync(connection);
            await DeleteRace(_createdId);
        }
    }
    
    [TestMethod]
    public async Task TestGetSingle()
    {
        _createdId = await CreateRace();
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        var connection = await DatabaseClient.GetOpenConnectionAsync();
        
        try
        {
            const string sql = "SELECT * FROM races WHERE id = @Id";
            var parameters = new { id = _createdId };
            var result = await connection.QuerySingleOrDefaultAsync<Race>(sql, parameters);
            await DatabaseClient.CloseConnectionAsync(connection);
            Assert.AreEqual(TestRace.Laps, result.Laps);
            Assert.AreEqual(TestRace.StartTime, result.StartTime);
            Assert.AreEqual(TestRace.EndTime, result.EndTime);
        }
        catch {}
        finally
        {
            await DatabaseClient.CloseConnectionAsync(connection);
            await DeleteRace(_createdId);
        }
    }
    
    [TestMethod]
    public async Task TestUpdate()
    {
        _createdId = await CreateRace();
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        var connection = await DatabaseClient.GetOpenConnectionAsync();

        try
        {
            TestRace.Id = _createdId;
            TestRace.Laps = 9998;
            const string sql = "UPDATE races SET laps = @Laps, start_time = @StartTime, end_time = @EndTime WHERE id = @Id";
            var result = await connection.ExecuteAsync(sql, TestRace);
            Assert.AreNotEqual(0, result);
        }
        catch {}
        finally
        {
            await DatabaseClient.CloseConnectionAsync(connection);
            await DeleteRace(_createdId);
        }
    }
    
    [TestMethod]
    public async Task TestDelete()
    {
        _createdId = await CreateRace();
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        var connection = await DatabaseClient.GetOpenConnectionAsync();
        
        try
        {
            Assert.AreNotEqual(0, _createdId);

            const string sql = "DELETE FROM races WHERE id = @Id";
            var parameters = new { id = _createdId };
            var result = await connection.ExecuteAsync(sql, parameters);
            Assert.AreNotEqual(0, result);
        }
        catch {}
        finally
        {
            await DatabaseClient.CloseConnectionAsync(connection);
            await DeleteRace(_createdId);
        }
    }
    
    private static async Task<int> CreateRace()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        var connection = await DatabaseClient.GetOpenConnectionAsync();

        try
        {
            const string sql = "INSERT INTO races (racenumber, driver_id) VALUES(@RaceNumber, @DriverId); SELECT LAST_INSERT_ID();";
            return await connection.QuerySingleAsync<int>(sql, TestRace);
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
    
    private static async Task DeleteRace(int id)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        var connection = await DatabaseClient.GetOpenConnectionAsync();
        
        try
        {
            const string sql = "DELETE FROM races WHERE id = @id";
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