using System.ComponentModel.DataAnnotations;

namespace Stackr_Api.Models;

public class RankedItem
{
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [Range(1, int.MaxValue)]
    public int Rank { get; set; }
} 