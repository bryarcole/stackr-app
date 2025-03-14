using System.ComponentModel.DataAnnotations;

namespace Stackr_Api.Models;

public class Stack
{
    public int Id { get; set; }
    
    [Required]
    public int ItemId { get; set; }
    
    [Required]
    public Item Item { get; set; } = null!;
    
    [Required]
    public int RankListId { get; set; }
    
    [Required]
    public RankList RankList { get; set; } = null!;
    
    [Required]
    public int Rank { get; set; }
    
    public DateTime CreatedAt { get; set; }
} 