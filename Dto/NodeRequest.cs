using System.ComponentModel.DataAnnotations;

namespace BlockChain.Dto;

/// <summary>
/// Request object for registering new nodes
/// </summary>
public class NodeRequest
{
    [Required]
    [MinLength(1, ErrorMessage = "At least one URL is required")]
    public List<string> Urls { get; set; } = new List<string>();
}
