using System;
using System.Net;
using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.IntegrationTests.Fixtures;
using AuctionService.IntegrationTests.Util;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests;

[Collection("Shared collection")]
public class AuctionControllerTests : IAsyncLifetime
{
    private readonly CustomWebAppFactory _factory;
    private readonly HttpClient _httpClient;
    private const string GT_ID = "afbee524-5972-4075-8800-7d1f9d7b0a0c";

    public AuctionControllerTests(CustomWebAppFactory factory)
    {
        _factory = factory;
        _httpClient = factory.CreateClient();
    }

    [Fact]
    public async Task GetAuctions_ShoudReturns3Auctions()
    {
        //arrange

        //act
        var response = await _httpClient.GetFromJsonAsync<List<AuctionDto>>("api/auctions");

        //assert
        Assert.Equal(3, response.Count);
    }

    [Fact]
    public async Task GetAuctionById_WithValidId_ShoudReturnsAuction()
    {
        //arrange?

        //act
        var response = await _httpClient.GetFromJsonAsync<AuctionDto>($"api/auctions/{GT_ID}");

        //assert
        Assert.Equal("GT", response.Model);
    }


    [Fact]
    public async Task GetAuctionById_WithInvalidId_ShoudReturns404NotFound()
    {
        //arrange?

        //act
        var response = await _httpClient.GetAsync($"api/auctions/{Guid.NewGuid()}");

        //assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetAuctionById_WithInvalidGuid_ShoudReturns400BadRequest()
    {
        //arrange?

        //act
        var response = await _httpClient.GetAsync($"api/auctions/notaguid");

        //assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }


    [Fact]
    public async Task CreateAuction_WithNoAuth_ShoudReturns401()
    {
        //arrange?
        var auction = new CreateAuctionDto { Make = "test" };

        //act
        var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);

        //assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }



    [Fact]
    public async Task CreateAuction_WithAuth_ShoudReturns201()
    {
        //arrange?
        var auction = GetAuctionForCreate();
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

        //act
        var response = await _httpClient.PostAsJsonAsync($"api/auctions", auction);

        //assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var createdAuction = await response.Content.ReadFromJsonAsync<AuctionDto>();
        Assert.Equal("bob", createdAuction.Seller);
    }

    [Fact]
    public async Task CreateAuction_WithInvalidCreateAuctionDto_ShouldReturn400()
    {
        // arrange
        var createAuctionDto = GetAuctionForCreate();
        createAuctionDto.Model = null;
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));

        // act
        var response = await _httpClient.PostAsJsonAsync($"api/auctions", createAuctionDto);

        // assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndUser_ShouldReturn200()
    {
        // arrange
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("bob"));
        var updateAuctionDto = new UpdateAuctionDto
        {
            Model = "test2"
        };

        // act
        var response = await _httpClient.PutAsJsonAsync($"api/auctions/{GT_ID}", updateAuctionDto);

        // assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAuction_WithValidUpdateDtoAndInvalidUser_ShouldReturn403()
    {
        // arrange
        _httpClient.SetFakeJwtBearerToken(AuthHelper.GetBearerForUser("alice"));
        var updateAuctionDto = new UpdateAuctionDto
        {
            Model = "test2"
        };

        // act
        var response = await _httpClient.PutAsJsonAsync($"api/auctions/{GT_ID}", updateAuctionDto);

        // assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }
    public Task DisposeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
        DbHelpers.ReinitDbForTest(db);
        return Task.CompletedTask;
    }

    private CreateAuctionDto GetAuctionForCreate()
    {
        return new CreateAuctionDto
        {
            Make = "test",
            Model = "test",
            ImageUrl = "test",
            Color = "test",
            Mileage = 10,
            Year = 10,
            ReservePrice = 10,
        };
    }

}
