using System;
using System.Numerics;
using UnityEngine;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Newtonsoft.Json;


public class SmartContractInteraction : MonoBehaviour
{
    private string url = "https://rpc.ankr.com/eth_goerli";
    private string contractAddress = "0x693984165233F42fC5CB2aE405ea4D465F8bCc36";
    private string contractABI = "[{\"inputs\":[{\"internalType\":\"uint8\",\"name\":\"_myArg\",\"type\":\"uint8\"}],\"name\":\"addTotal\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"myTotal\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";

    private string privateKey;
    private string userInfo;
    private Account account;
    
    private Web3 web3;
    private Contract contract;

    private Function addTotalFunction;
    private Function myTotalFunction;
    
    Web3Auth web3Auth;


    private void Start()
    {
        web3Auth = GetComponent<Web3Auth>();
        web3Auth.setOptions(new Web3AuthOptions()
        {
            clientId = "BAkN91HvqnqgG3TN44RgLIWWLNuxh8vQmuCzpnEzK7aOAobVu3RiRWwrE6rJkw8mkYmVJ4rWzbll91Sg-NSCHqU",
            redirectUrl = new System.Uri("torusapp://com.torus.Web3AuthUnity/auth"),
            network = Web3Auth.Network.TESTNET,
        });
        web3Auth.onLogin += onLogin;
    }
    
    public void login()
    {
        var selectedProvider = Provider.GOOGLE;

        var options = new LoginParams()
        {
            loginProvider = selectedProvider
        };

        web3Auth.login(options);
    }
    
    private void onLogin(Web3AuthResponse response)
    {
        userInfo = JsonConvert.SerializeObject(response.userInfo, Formatting.Indented);
        privateKey = response.privKey;
        var newAccount = new Account(privateKey);
        account = newAccount;

        Debug.Log(JsonConvert.SerializeObject(response, Formatting.Indented));
        //updateConsole(JsonConvert.SerializeObject(response, Formatting.Indented));
        account = new Account(privateKey);
        web3 = new Web3(account, url);
        contract = web3.Eth.GetContract(contractABI, contractAddress);

        addTotalFunction = contract.GetFunction("addTotal");
        myTotalFunction = contract.GetFunction("myTotal");
    }

    public async void GetTotal()
    {
        var total = await myTotalFunction.CallAsync<BigInteger>();
        Debug.Log("Total: " + total.ToString());
    }

    public async void AddToTotal()
    {
        BigInteger value = 5;

        try
        {
            var transactionReceipt = await addTotalFunction.SendTransactionAndWaitForReceiptAsync(account.Address, new HexBigInteger(300000), new HexBigInteger(0), null, value);
            string transactionHash = transactionReceipt.TransactionHash;
            Debug.Log("Transaction Hash: " + transactionHash);
            Debug.Log("Transaction complete!");
        }
        catch (Exception ex)
        {
            Debug.Log("Transaction failed: " + ex.Message);
        }
    }

}