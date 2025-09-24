namespace BlockChain.Model;

/// <summary>
/// Represents a block in the blockchain
/// </summary>
public class Block
{
    public int Index { get; set; }
    public DateTime Timestamp { get; set; }
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    public int Proof { get; set; }
    public string PreviousHash { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{Index} [{Timestamp:yyyy-MM-dd HH:mm:ss}] Proof: {Proof} | PrevHash: {PreviousHash} | Trx: {Transactions.Count}";
    }
}
