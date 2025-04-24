using AuctionService.Data;
using AuctionService.Dtos;
using AuctionService.Entities;
using AuctionService.RequestHelpers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionController : ControllerBase
{
    private readonly ILogger<AuctionController> _logger;
    private readonly AuctionDbContext _auctionContext;
    private readonly IMapper _mapper;

    public AuctionController(
        ILogger<AuctionController> logger,
        AuctionDbContext auctionContext,
        IMapper mapper)
    {
        _mapper = mapper;
        _logger = logger;
        _auctionContext = auctionContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAuctions()
    {
        _logger.LogInformation("GetAuctions called");

        var auctions = await _auctionContext.Auctions
            .Include(x => x.Item)
            .OrderBy(x => x.Item.Make)
            .ToListAsync();

        return Ok(_mapper.Map<List<AuctionDto>>(auctions));
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> Create([FromBody] CreateAuctionDto auction)
    {
        var auctionToSave = _mapper.Map<Auction>(auction);
        auctionToSave.Seller = "Test";

        await _auctionContext.Auctions.AddAsync(auctionToSave);

        var isSaved = await _auctionContext.SaveChangesAsync() > 0;

        if (!isSaved) return BadRequest($"Couldn't save the auction");

        return CreatedAtAction(
            nameof(GetById),
            new { auctionToSave.Id },
            _mapper.Map<AuctionDto>(auctionToSave)
        );
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AuctionDto>> GetById(Guid id)
    {
        var auction = await _auctionContext.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        return auction is null ? NotFound() : Ok(_mapper.Map<AuctionDto>(auction));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateAuction([FromRoute] Guid id, [FromBody] UpdateAuctionDto updateAuction)
    {
        var foundAuction = await _auctionContext.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        _logger.LogInformation("The Color test is " + updateAuction.Color);

        if (foundAuction is null) return NotFound();

        foundAuction.Item.Make = updateAuction.Make ?? foundAuction.Item.Make;
        foundAuction.Item.Model = updateAuction.Model ?? foundAuction.Item.Model;
        foundAuction.Item.Color = updateAuction.Color ?? foundAuction.Item.Color;
        foundAuction.Item.Mileage = updateAuction.Mileage ?? foundAuction.Item.Mileage;
        foundAuction.Item.Year = updateAuction.Year ?? foundAuction.Item.Year;
        foundAuction.Item.ImageUrl = updateAuction.ImageUrl ?? foundAuction.Item.ImageUrl;

        var result = await _auctionContext.SaveChangesAsync() > 0;
        if (!result) return BadRequest();
        return Ok();
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteAuction([FromRoute] Guid id)
    {
        var foundAuction = await _auctionContext.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (foundAuction is null) return NotFound();
         _auctionContext.Auctions.Remove(foundAuction);

         var result = await _auctionContext.SaveChangesAsync() > 0;

         if (!result) return BadRequest();
         return Ok();
    }
}