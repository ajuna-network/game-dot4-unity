using Ajuna.NetWallet;
using Ajuna.UnityInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using UnityEngine;
using System.Security.Cryptography;
using System.Collections;
using Ajuna.NetApi;
using Ajuna.NetApi.Model.AjunaWorker;

public class Test : MonoBehaviour
{
    public static Test Instance;

    [SerializeField]
    public string NodeUrl = "ws://127.0.0.1:9944";

    [SerializeField]
    private string workerUrl = "ws://183c-84-75-48-249.ngrok.io";

    [SerializeField]
    private string shardHex = "Fdb2TM3owt4unpvESoSMTpVWPvCiXMzYyb42LzSsmFLi";

    [SerializeField]
    private string mrenclaveHex = "Fdb2TM3owt4unpvESoSMTpVWPvCiXMzYyb42LzSsmFLi";

    private Dot4GClient dot4GClient;

    private Task<RSAParameters> shieldingTask;

    private Task<bool> connectTask, faucetTask;

    private Task<Balance> balanceTask;

    public Wallet Wallet;

    void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        LoggingConfiguration config = new LoggingConfiguration();
        LogManager.Configuration = config;

        // initializing standard ajuna wallet
        Wallet = new Wallet();

        dot4GClient = new Dot4GClient(Wallet,
            "ws://5099-84-75-48-249.ngrok.io",
            "Fdb2TM3owt4unpvESoSMTpVWPvCiXMzYyb42LzSsmFLi",
            "Fdb2TM3owt4unpvESoSMTpVWPvCiXMzYyb42LzSsmFLi");

        connectTask = dot4GClient.ConnectTeeAsync();

        yield return new WaitUntil(() =>
            dot4GClient.IsTeeConnected && dot4GClient.WorkerClient.GetJsonRpc() != null
        );

        shieldingTask = dot4GClient.GetShieldingKeyAsync();

        yield return new WaitUntil(() =>
            shieldingTask.IsCompleted
        );

        Debug.Log("Completed");
    }

    // Update is called once per frame
    void Update()
    {
        //if (connectTask != null && connectTask.IsCompleted)
        //{
        //    Debug.Log($"IsConnected[{connectTask.Result}] = {dot4GClient.IsTeeConnected}");
        //    connectTask = null;
        //}

        //if (shieldingTask != null && shieldingTask.IsCompleted)
        //{
        //    Debug.Log($"shieldingTask = {shieldingTask.Status} {shieldingTask.Exception}");
        //    Debug.Log($"Result = {shieldingTask.Result}");
        //    Debug.Log($"Result = {Utils.Bytes2HexString(shieldingTask.Result.Modulus)}");
        //    shieldingTask = null;
        //}

        if (faucetTask != null && faucetTask.IsCompleted)
        {
            Debug.Log($"get faucetTask[{faucetTask.Status}] = {faucetTask.Exception}");
            faucetTask = null;
        }

        if (balanceTask != null && balanceTask.IsCompleted)
        {
            Debug.Log($"get balanceTask[{balanceTask.Status}] = {balanceTask.Exception} {balanceTask.Result}");
            balanceTask = null;
        }

    }

    public void OnClickWorkerFaucet()
    {
        if (!dot4GClient.IsTeeConnected || !dot4GClient.HasShieldingKey)
        {
            Debug.Log($"IsTeeConnected[{dot4GClient.IsTeeConnected}] || HasShieldingKey[{dot4GClient.HasShieldingKey}]");
        }

        if (faucetTask != null)
        {
            Debug.Log("Faucet Task still on going!");
            return;
        }

        faucetTask = dot4GClient.FaucetWorkerAsync();
    }

    public void OnClickGetBalance()
    {
        if (!dot4GClient.IsTeeConnected || !dot4GClient.HasShieldingKey)
        {
            Debug.Log($"IsTeeConnected[{dot4GClient.IsTeeConnected}] || HasShieldingKey[{dot4GClient.HasShieldingKey}]");
        }

        if(balanceTask != null)
        {
            Debug.Log("Balance Task still on going!");
            return;
        }

        balanceTask = dot4GClient.GetBalanceWorkerAsync();
    }
}
