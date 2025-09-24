# 🔗 Simple Blockchain Implementation in .NET 8

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen.svg)]()

A simple yet functional **blockchain implementation** built with **.NET 8** and **ASP.NET Core Web API**. This project demonstrates the fundamental concepts of blockchain technology including mining, transactions, proof-of-work consensus, and distributed network nodes.

## 🌟 Features

- **⛏️ Mining**: Proof-of-work algorithm with adjustable difficulty
- **💰 Transactions**: Create and manage transactions between parties
- **🔗 Blockchain**: Immutable chain of blocks with cryptographic hashing
- **🌐 Network Nodes**: Register and communicate with multiple nodes
- **🤝 Consensus Algorithm**: Resolve conflicts using the longest chain rule
- **📊 RESTful API**: Complete API for blockchain operations
- **📋 Swagger Documentation**: Interactive API documentation
- **✅ Input Validation**: Comprehensive validation for all endpoints
- **📝 Logging**: Structured logging throughout the application

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Running Locally

1. **Clone the repository**
   ```bash
   git clone https://github.com/soheilkavianimehr/blockchain.git
   cd blockchain
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Open your browser**
   - Navigate to `https://localhost:7000` for Swagger UI
   - Or use `http://localhost:5000` for HTTP

## 📖 API Documentation

### Blockchain Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/blockchain/mine` | Mine a new block |
| `GET` | `/api/blockchain/chain` | Get the full blockchain |

### Transaction Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/transactions/create` | Create a new transaction |

### Network Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/nodes/register` | Register new nodes |
| `GET` | `/api/nodes/consensus` | Run consensus algorithm |

### Health Check

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/health` | Check node health status |

## 💡 Usage Examples

### 1. Create a Transaction

```bash
curl -X POST "https://localhost:7000/api/transactions/create" \
  -H "Content-Type: application/json" \
  -d '{
    "sender": "Alice",
    "recipient": "Bob",
    "amount": 50
  }'
```

### 2. Mine a Block

```bash
curl -X POST "https://localhost:7000/api/blockchain/mine"
```

### 3. Get the Blockchain

```bash
curl -X GET "https://localhost:7000/api/blockchain/chain"
```

### 4. Register Network Nodes

```bash
curl -X POST "https://localhost:7000/api/nodes/register" \
  -H "Content-Type: application/json" \
  -d '{
    "urls": ["localhost:5001", "localhost:5002"]
  }'
```

### 5. Run Consensus

```bash
curl -X GET "https://localhost:7000/api/nodes/consensus"
```

## 🏗️ Architecture

### Core Components

- **Block**: Contains transactions, timestamp, proof, and previous block hash
- **Transaction**: Represents a transfer between sender and recipient
- **Node**: Represents a network participant in the blockchain
- **BlockChainService**: Core service implementing blockchain logic
- **Proof of Work**: SHA-256 based algorithm requiring 4 leading zeros

### Project Structure

```
BlockChain/
├── Model/
│   ├── Block.cs          # Block data structure
│   ├── Transaction.cs    # Transaction model
│   └── Node.cs          # Network node representation
├── Services/
│   ├── IBlockChainService.cs    # Service interface
│   └── BlockChainService.cs     # Core blockchain logic
├── Dto/
│   └── NodeRequest.cs    # Data transfer objects
├── Program.cs            # Application entry point
└── README.md           # This file
```

## 🔧 Configuration

### Application Settings

The application uses standard ASP.NET Core configuration. You can modify settings in:

- `appsettings.json` - General configuration
- `appsettings.Development.json` - Development-specific settings

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Environment (Development/Production) | Development |
| `ASPNETCORE_URLS` | Listening URLs | `https://localhost:7000;http://localhost:5000` |

## 🧪 Testing the Network

To test the distributed blockchain network:

1. **Start multiple instances** on different ports:
   ```bash
   dotnet run --urls="http://localhost:5000"
   dotnet run --urls="http://localhost:5001"
   dotnet run --urls="http://localhost:5002"
   ```

2. **Register nodes** with each other
3. **Create transactions** on different nodes
4. **Mine blocks** on different nodes
5. **Run consensus** to synchronize

## 🔐 Security Considerations

⚠️ **Important**: This is an educational implementation and should **NOT** be used in production without significant security enhancements:

- No authentication or authorization
- No transaction validation (beyond basic input validation)
- No network security measures
- Simplified proof-of-work (only 4 leading zeros)
- No wallet or key management
- No transaction fees or economic incentives

## 📚 Learning Resources

This implementation is based on the excellent tutorial from **HackerNoon**:

- 📖 **Original Tutorial**: [Learn Blockchains by Building One](https://hackernoon.com/learn-blockchains-by-building-one-117428612f46) by Daniel van Flymen
- 🌐 **HackerNoon**: [hackernoon.com](https://hackernoon.com) - Great resource for tech tutorials and blockchain education

### Additional Learning Materials

- [Bitcoin Whitepaper](https://bitcoin.org/bitcoin.pdf) - The original blockchain paper
- [Ethereum Whitepaper](https://ethereum.org/en/whitepaper/) - Smart contract blockchain
- [Blockchain Basics](https://www.coursera.org/learn/blockchain-basics) - Coursera course
- [Mastering Bitcoin](https://github.com/bitcoinbook/bitcoinbook) - Comprehensive Bitcoin book

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

### Development Setup

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Make your changes
4. Add tests if applicable
5. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
6. Push to the branch (`git push origin feature/AmazingFeature`)
7. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Daniel van Flymen** for the original [HackerNoon tutorial](https://hackernoon.com/learn-blockchains-by-building-one-117428612f46)
- **HackerNoon** community for providing excellent educational content
- **.NET Team** for the amazing framework and tools
- **Satoshi Nakamoto** for inventing blockchain technology

## 📞 Support

If you have any questions or need help getting started:

- 📧 Open an issue on GitHub
- 💬 Check the [Discussions](../../discussions) section
- 📖 Review the [Wiki](../../wiki) for additional documentation

---

**⭐ If you found this project helpful, please give it a star!**

---

*Built with ❤️ using .NET 8 and inspired by the HackerNoon blockchain tutorial.*
