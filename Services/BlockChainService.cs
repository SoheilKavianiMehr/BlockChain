using BlockChain.Model;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace BlockChain.Services;

/// <summary>
/// Service that implements a basic blockchain with proof of work consensus
/// Based on the tutorial from HackerNoon: https://hackernoon.com/learn-blockchains-by-building-one-117428612f46
/// </summary>
public class BlockChainService : IBlockChainService
{
    private readonly List<Transaction> _currentTransactions = new();
    private readonly List<Block> _chain = new();
    private readonly HashSet<Node> _nodes = new(); // Using HashSet to prevent duplicate nodes
    private readonly HttpClient _httpClient = new();

    private Block _lastBlock => _chain.Last();

    public string NodeId { get; private set; }

    /// <summary>
    /// Initialize the blockchain with a genesis block
    /// </summary>
    public BlockChainService()
    {
        NodeId = Guid.NewGuid().ToString().Replace("-", "");
        CreateNewBlock(proof: 100, previousHash: "1"); // Genesis block
    }

    /// <summary>
    /// Register a new node in the network
    /// </summary>
    private void RegisterNode(string address)
    {
        try
        {
            _nodes.Add(new Node { Address = new Uri(address) });
        }
        catch (UriFormatException ex)
        {
            throw new ArgumentException($"Invalid node address: {address}", nameof(address), ex);
        }
    }

    /// <summary>
    /// Validate if a chain is valid by checking hashes and proof of work
    /// </summary>
    private bool IsValidChain(List<Block> chain)
    {
        if (chain.Count == 0) return false;

        Block lastBlock = chain.First();
        int currentIndex = 1;

        while (currentIndex < chain.Count)
        {
            Block block = chain.ElementAt(currentIndex);
            Debug.WriteLine($"Last: {lastBlock}");
            Debug.WriteLine($"Current: {block}");
            Debug.WriteLine("----------------------------");

            // Check that the hash of the block is correct
            if (block.PreviousHash != GetHash(lastBlock))
                return false;

            // Check that the Proof of Work is correct
            if (!IsValidProof(lastBlock.Proof, block.Proof, lastBlock.PreviousHash))
                return false;

            lastBlock = block;
            currentIndex++;
        }

        return true;
    }

    /// <summary>
    /// Consensus algorithm - resolve conflicts by choosing the longest valid chain
    /// </summary>
    private async Task<bool> ResolveConflicts()
    {
        List<Block>? newChain = null;
        int maxLength = _chain.Count;

        foreach (Node node in _nodes)
        {
            try
            {
                var url = new Uri(node.Address, "/api/blockchain/chain");
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response from {node.Address}:");
                    Console.WriteLine(responseBody);

                    var model = new { chain = new List<Block>(), length = 0 };
                    var data = JsonConvert.DeserializeAnonymousType(responseBody, model);

                    if (data?.chain != null && data.chain.Count > maxLength && IsValidChain(data.chain))
                    {
                        maxLength = data.chain.Count;
                        newChain = data.chain;
                    }
                }
                else
                {
                    Console.WriteLine($"Request to {node.Address} failed. Status Code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request error for node {node.Address}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error for node {node.Address}: {ex.Message}");
            }
        }

        if (newChain != null)
        {
            _chain.Clear();
            _chain.AddRange(newChain);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Create a new block and add it to the chain
    /// </summary>
    private Block CreateNewBlock(int proof, string? previousHash = null)
    {
        var block = new Block
        {
            Index = _chain.Count,
            Timestamp = DateTime.UtcNow,
            Transactions = _currentTransactions.ToList(),
            Proof = proof,
            PreviousHash = previousHash ?? GetHash(_lastBlock)
        };

        _currentTransactions.Clear();
        _chain.Add(block);
        return block;
    }

    /// <summary>
    /// Simple proof of work algorithm - find a number that when hashed with the previous proof
    /// contains 4 leading zeros
    /// </summary>
    private int CreateProofOfWork(int lastProof, string previousHash)
    {
        int proof = 0;
        while (!IsValidProof(lastProof, proof, previousHash))
        {
            proof++;
        }

        return proof;
    }

    /// <summary>
    /// Check if the proof is valid by verifying it produces a hash with 4 leading zeros
    /// </summary>
    private bool IsValidProof(int lastProof, int proof, string previousHash)
    {
        string guess = $"{lastProof}{proof}{previousHash}";
        string result = GetSha256(guess);
        return result.StartsWith("0000"); // Difficulty level - 4 leading zeros
    }

    /// <summary>
    /// Calculate SHA256 hash of a block
    /// </summary>
    private string GetHash(Block block)
    {
        string blockText = JsonConvert.SerializeObject(block, Formatting.None);
        return GetSha256(blockText);
    }

    /// <summary>
    /// Calculate SHA256 hash of a string
    /// </summary>
    private static string GetSha256(string data)
    {
        using var sha = SHA256.Create();
        var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    // Public API methods

    /// <summary>
    /// Mine a new block with pending transactions
    /// </summary>
    public string Mine()
    {
        int proof = CreateProofOfWork(_lastBlock.Proof, _lastBlock.PreviousHash);

        // Reward the miner with 1 coin
        CreateTransaction(sender: "0", recipient: NodeId, amount: 1);
        Block block = CreateNewBlock(proof);

        var response = new
        {
            Message = "New Block Forged",
            Index = block.Index,
            Transactions = block.Transactions.ToArray(),
            Proof = block.Proof,
            PreviousHash = block.PreviousHash
        };

        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }

    /// <summary>
    /// Get the full blockchain
    /// </summary>
    public string GetFullChain()
    {
        var response = new
        {
            chain = _chain.ToArray(),
            length = _chain.Count
        };

        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }

    /// <summary>
    /// Register multiple nodes in the network
    /// </summary>
    public string RegisterNodes(List<string> nodes)
    {
        if (nodes == null || nodes.Count == 0)
        {
            throw new ArgumentException("Node list cannot be empty", nameof(nodes));
        }

        var addedNodes = new List<string>();
        foreach (string node in nodes)
        {
            try
            {
                string url = node.StartsWith("http") ? node : $"http://{node}";
                RegisterNode(url);
                addedNodes.Add(url);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Failed to register node {node}: {ex.Message}");
            }
        }

        var response = new
        {
            Message = $"{addedNodes.Count} new nodes have been added",
            TotalNodes = _nodes.Count,
            AddedNodes = addedNodes
        };

        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }

    /// <summary>
    /// Run the consensus algorithm to resolve conflicts
    /// </summary>
    public string Consensus()
    {
        bool replaced = ResolveConflicts().Result;
        string message = replaced ? "was replaced" : "is authoritative";

        var response = new
        {
            Message = $"Our chain {message}",
            Length = _chain.Count,
            Chain = _chain
        };

        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }

    /// <summary>
    /// Create a new transaction and add it to the pool
    /// </summary>
    public int CreateTransaction(string sender, string recipient, int amount)
    {
        if (string.IsNullOrWhiteSpace(sender))
            throw new ArgumentException("Sender cannot be empty", nameof(sender));
        if (string.IsNullOrWhiteSpace(recipient))
            throw new ArgumentException("Recipient cannot be empty", nameof(recipient));
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than 0", nameof(amount));

        var transaction = new Transaction
        {
            Sender = sender,
            Recipient = recipient,
            Amount = amount
        };

        _currentTransactions.Add(transaction);

        return _lastBlock.Index + 1;
    }

    /// <summary>
    /// Dispose of the HttpClient when the service is disposed
    /// </summary>
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
