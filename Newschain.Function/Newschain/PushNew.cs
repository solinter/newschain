using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Nethereum.Web3;
using Newschain.Models;


namespace Newschain
{
    public static class PushNew
    {
        [FunctionName("PushNew")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] Models.NewData contract, TraceWriter log)
        {
            // You need to have the geth running while running this code. command: c:\geth\>geth --testnet
            var _getAddress = "./geth.ipc";
            var hashedContent = Hash(contract.Content);

            var ipcClient = new Nethereum.JsonRpc.IpcClient.IpcClient(_getAddress);
            var web3 = new Web3(ipcClient);

            // this will leave the account unlucked for 2 minutes
            Nethereum.Hex.HexTypes.HexBigInteger accountUnlockTime = new Nethereum.Hex.HexTypes.HexBigInteger(120);

            var accountPublicKey = "ACCOUNT_PUBLIC_KEY";
            var accountPassword = "ACCOUNT_PASSWORD";
            // Unlock the caller's account with the given password
            var unlockResult = await web3.Personal.UnlockAccount.SendRequestAsync(accountPublicKey, accountPassword, accountUnlockTime);

            var abi = @"[{""constant"":true,""inputs"":[],""name"":""Url"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""url"",""type"":""string""},{""name"":""hashContent"",""type"":""string""}],""name"":""AddPostContract"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""HashContent"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""}]";
            var byteCode = "6060604052341561000f57600080fd5b6104578061001e6000396000f300606060405260043610610057576000357c0100000000000000000000000000000000000000000000000000000000900463ffffffff1680638d21df8c1461005c578063a2da9d08146100ea578063b0e4a8601461018a575b600080fd5b341561006757600080fd5b61006f610218565b6040518080602001828103825283818151815260200191508051906020019080838360005b838110156100af578082015181840152602081019050610094565b50505050905090810190601f1680156100dc5780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b34156100f557600080fd5b610188600480803590602001908201803590602001908080601f0160208091040260200160405190810160405280939291908181526020018383808284378201915050505050509190803590602001908201803590602001908080601f016020809104026020016040519081016040528093929190818152602001838380828437820191505050505050919050506102b6565b005b341561019557600080fd5b61019d6102e8565b6040518080602001828103825283818151815260200191508051906020019080838360005b838110156101dd5780820151818401526020810190506101c2565b50505050905090810190601f16801561020a5780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b60008054600181600116156101000203166002900480601f0160208091040260200160405190810160405280929190818152602001828054600181600116156101000203166002900480156102ae5780601f10610283576101008083540402835291602001916102ae565b820191906000526020600020905b81548152906001019060200180831161029157829003601f168201915b505050505081565b81600090805190602001906102cc929190610386565b5080600190805190602001906102e3929190610386565b505050565b60018054600181600116156101000203166002900480601f01602080910402602001604051908101604052809291908181526020018280546001816001161561010002031660029004801561037e5780601f106103535761010080835404028352916020019161037e565b820191906000526020600020905b81548152906001019060200180831161036157829003601f168201915b505050505081565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f106103c757805160ff19168380011785556103f5565b828001600101855582156103f5579182015b828111156103f45782518255916020019190600101906103d9565b5b5090506104029190610406565b5090565b61042891905b8082111561042457600081600090555060010161040c565b5090565b905600a165627a7a72305820eb77a018bf2acdb7f9ea0a12c1ca4c23cfc215fa49cbdb1152c79814db0dbef60029";

            var compiledContract = web3.Eth.GetContract(abi, byteCode);

            var function = compiledContract.GetFunction("AddPostContract");

            var ret = function.CallAsync<>(contract.Url, hashedContent);
        }

        private static string Hash(string content)
        {
            var shaHash = SHA256.Create();
            byte[] data = shaHash.ComputeHash(Encoding.UTF8.GetBytes(content));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
    }
}
