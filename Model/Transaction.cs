using System.ComponentModel.DataAnnotations;

namespace BlockChain.Model;

/// <summary>
/// Represents a transaction between two parties in the blockchain
/// </summary>
public class Transaction
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public int Amount { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "Recipient is required")]
    public string Recipient { get; set; } = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "Sender is required")]
    public string Sender { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{Sender} -> {Recipient}: {Amount}";
    }
}
