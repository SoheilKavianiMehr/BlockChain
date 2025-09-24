namespace BlockChain.Services
{
    /// <summary>
    /// Interface for blockchain operations including mining, transactions, and consensus
    /// </summary>
    public interface IBlockChainService : IDisposable
    {
        /// <summary>
        /// Unique identifier for this blockchain node
        /// </summary>
        string NodeId { get; }

        /// <summary>
        /// Run the consensus algorithm to resolve conflicts with other nodes
        /// </summary>
        /// <returns>JSON string containing consensus result</returns>
        string Consensus();

        /// <summary>
        /// Create a new transaction in the transaction pool
        /// </summary>
        /// <param name="sender">Address of the sender</param>
        /// <param name="recipient">Address of the recipient</param>
        /// <param name="amount">Amount to transfer</param>
        /// <returns>Index of the block that will contain this transaction</returns>
        int CreateTransaction(string sender, string recipient, int amount);

        /// <summary>
        /// Get the full blockchain
        /// </summary>
        /// <returns>JSON string containing the complete blockchain</returns>
        string GetFullChain();

        /// <summary>
        /// Mine a new block with pending transactions
        /// </summary>
        /// <returns>JSON string containing the new block information</returns>
        string Mine();

        /// <summary>
        /// Register new nodes in the blockchain network
        /// </summary>
        /// <param name="nodes">List of node addresses to register</param>
        /// <returns>JSON string containing registration result</returns>
        string RegisterNodes(List<string> nodes);
    }
}