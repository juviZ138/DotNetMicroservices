using System;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController(IAuctionRepository auctionRepository, IMapper mapper
    , IPublishEndpoint publishEndpoint) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string date)
    {
        return await auctionRepository.GetAuctionsAsync(date);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
    {
        var auction = await auctionRepository.GetAuctionByIdAsync(id);

        if (auction == null) return NotFound();

        return auction;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
    {
        var auction = mapper.Map<Auction>(createAuctionDto);

        auction.Seller = User.Identity.Name;

        auctionRepository.AddAuction(auction);

        var newAuction = mapper.Map<AuctionDto>(auction);

        await publishEndpoint.Publish(mapper.Map<AuctionCreated>(newAuction));

        var result = await auctionRepository.SaveChangesAsync();

        if (!result) return BadRequest("Could not save changes to db");

        return CreatedAtAction(nameof(GetAuctionById), new { auction.Id },
            newAuction);
    }


    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await auctionRepository.GetAuctionEntityById(id);

        if (auction == null) return NotFound();

        if (auction.Seller != User.Identity.Name) return Forbid();

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;

        await publishEndpoint.Publish(mapper.Map<AuctionUpdated>(auction));

        var result = await auctionRepository.SaveChangesAsync();

        if (result) return Ok();

        return BadRequest("Problem saving changes");
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
        var auction = await auctionRepository.GetAuctionEntityById(id);
        if (auction == null) return NotFound();

        if (auction.Seller != User.Identity.Name) return Forbid();

        auctionRepository.RemoveAuction(auction);

        await publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

        var result = await auctionRepository.SaveChangesAsync();

        if (!result)
            return BadRequest("Problem saving changes");

        return Ok();
    }
}
