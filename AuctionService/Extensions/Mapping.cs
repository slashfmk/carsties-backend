using AuctionService.Dtos;
using AuctionService.Entities;

namespace AuctionService.Extensions;

public static class Mapping
{

    public static AuctionDto ToAuctionDto(this Auction auction)
    {
        var auctionDto = new AuctionDto
        {
            Id = auction.Id,
            AuctiondAt = auction.AuctiondAt,
            AuctionEnd = auction.AuctionEnd,
        };

        return auctionDto;
    }
    
}