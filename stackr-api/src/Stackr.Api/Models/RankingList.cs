using System.ComponentModel.DataAnnotations;

namespace Stackr_Api.Models;

public class RankList
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public int UserId { get; set; }
    
    [Required]
    public User User { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; }
} 