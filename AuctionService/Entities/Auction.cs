namespace AuctionService.Entities;

public class Auction
{
    public Guid Id { get; set; }
    public int ReservePrice { get; set; } = 0;
    public string Seller { get; set; } = string.Empty;
    public string Winner { get; set; } = string.Empty;
    public int? SoldAmount { get; set; }
    public int? CurrentPrice { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdateddAt { get; set; } = DateTime.UtcNow;
    public DateTime AuctiondAt { get; set; }
    public DateTime AuctionEnd { get; set; }
    public Status Status { get; set; }
    
    public Item? Item { get; set; }
}