using System.ComponentModel.DataAnnotations;

namespace BlockChain.Model;

/// <summary>
/// Represents a node in the blockchain network
/// </summary>
public class Node
{
    [Required]
    public Uri Address { get; set; } = null!;

    public override string ToString()
    {
        return Address.ToString();
    }

    public override bool Equals(object? obj)
    {
        return obj is Node node && Address.Equals(node.Address);
    }

    public override int GetHashCode()
    {
        return Address.GetHashCode();
    }
}
