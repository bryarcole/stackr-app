using System.ComponentModel.DataAnnotations;

namespace Stackr_Api.Models;

public class Item
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
} 