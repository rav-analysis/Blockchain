using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;


namespace Blockchain
{
    class Program
    {
        public class Block
        {
            public int Index { get; set; }
            public DateTime TimeStamp { get; set; }
            public string PreviousHash { get; set; }
            public string Hash { get; set; }
            public string Data { get; set; }

            public Block(DateTime timeStamp, string previousHash, string data)
            {
                Index = 0;
                TimeStamp = timeStamp;
                PreviousHash = previousHash;
                Data = data;
                Hash = CalculateHash();
            }

            public string CalculateHash()
            {
                SHA256 sha256 = SHA256.Create();

                byte[] inputBytes = Encoding.ASCII.GetBytes($"{TimeStamp}-{PreviousHash ?? ""}-{Data}");
                byte[] outputBytes = sha256.ComputeHash(inputBytes);

                return Convert.ToBase64String(outputBytes);
            }
        }

        public class Blockchain
        {
            public IList<Block> Chain { set; get; }

            public Blockchain()
            {
                InitializeChain();
                AddGenesisBlock();
            }


            public void InitializeChain()
            {
                Chain = new List<Block>();
            }

            public Block CreateGenesisBlock()
            {
                return new Block(DateTime.Now, null, "{}");
            }

            public void AddGenesisBlock()
            {
                Chain.Add(CreateGenesisBlock());
            }

            public Block GetLatestBlock()
            {
                return Chain[Chain.Count - 1];
            }

            public void AddBlock(Block block)
            {
                Block latestBlock = GetLatestBlock();
                block.Index = latestBlock.Index + 1;
                block.PreviousHash = latestBlock.Hash;
                block.Hash = block.CalculateHash();
                Chain.Add(block);
            }

            public bool IsValid()
            {
                for (int i = 1; i < Chain.Count; i++)
                {
                    Block currentBlock = Chain[i];
                    Block previousBlock = Chain[i - 1];

                    if (currentBlock.Hash != currentBlock.CalculateHash())
                    {
                        return false;
                    }

                    if (currentBlock.PreviousHash != previousBlock.Hash)
                    {
                        return false;
                    }
                }
                return true;
            }

        }


        static void Main(string[] args)
        {
            Blockchain phillyCoin = new Blockchain();
            phillyCoin.AddBlock(new Block(DateTime.Now, null, "{sender:Henry,receiver:MaHesh,amount:10}"));
            phillyCoin.AddBlock(new Block(DateTime.Now, null, "{sender:MaHesh,receiver:Henry,amount:5}"));
            phillyCoin.AddBlock(new Block(DateTime.Now, null, "{sender:Mahesh,receiver:Henry,amount:5}"));

            Console.WriteLine(JsonConvert.SerializeObject(phillyCoin, Formatting.Indented));

            phillyCoin.Chain[1].Data = "{sender:Henry,receiver:MaHesh,amount:1000}";


            Console.WriteLine($"Is Chain Valid: {phillyCoin.IsValid()}");

            Console.WriteLine($"Update amount to 1000");
            phillyCoin.Chain[1].Data = "{sender:Henry,receiver:MaHesh,amount:1000}";

            Console.WriteLine($"Is Chain Valid: {phillyCoin.IsValid()}");

            phillyCoin.Chain[1].Hash = phillyCoin.Chain[1].CalculateHash();
            Console.WriteLine($"Update the entire chain");
            phillyCoin.Chain[2].PreviousHash = phillyCoin.Chain[1].Hash;
            phillyCoin.Chain[2].Hash = phillyCoin.Chain[2].CalculateHash();
            phillyCoin.Chain[3].PreviousHash = phillyCoin.Chain[2].Hash;
            phillyCoin.Chain[3].Hash = phillyCoin.Chain[3].CalculateHash();
            Console.WriteLine($"Is Chain Valid: {phillyCoin.IsValid()}");
        }

    }
}
