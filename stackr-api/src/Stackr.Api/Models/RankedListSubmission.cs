using System.ComponentModel.DataAnnotations;

namespace Stackr_Api.Models;

public class RankedListSubmission
{
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [MinLength(1)]
    public List<RankedItem> Items { get; set; } = new();
} 