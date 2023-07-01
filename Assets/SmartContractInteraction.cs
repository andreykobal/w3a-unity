using System;
using System.Numerics;
using UnityEngine;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;

public class SmartContractInteraction : MonoBehaviour
{
    private string url = "https://rpc.ankr.com/eth_goerli";
    private string privateKey = "193913ddf890c9c955faef076c316da4c216a6b923b1c61bdae26e8252828938";
    private string contractAddress = "0x693984165233F42fC5CB2aE405ea4D465F8bCc36";
    private string contractABI = "[{\"inputs\":[{\"internalType\":\"uint8\",\"name\":\"_myArg\",\"type\":\"uint8\"}],\"name\":\"addTotal\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"myTotal\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";

    private Web3 web3;
    private Contract contract;
    private Account account;

    private Function addTotalFunction;
    private Function myTotalFunction;

    private void Start()
    {
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
            Debug.Log("Transaction complete!");
        }
        catch (Exception ex)
        {
            Debug.Log("Transaction failed: " + ex.Message);
        }
    }

}