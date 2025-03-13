using System.ComponentModel.DataAnnotations;

namespace Stackr_Api.Models;

public class Ranking
{
    public int Id { get; set; }
    
    [Required]
    public int ItemId { get; set; }
    
    [Required]
    public Item Item { get; set; } = null!;
    
    [Required]
    public int RankingListId { get; set; }
    
    [Required]
    public RankingList RankingList { get; set; } = null!;
    
    [Required]
    public int Rank { get; set; }
    
    public DateTime CreatedAt { get; set; }
} 