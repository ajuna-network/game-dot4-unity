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

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;

    [SerializeField] 
    private string nodeUrl = "ws://127.0.0.1:9944";

    [SerializeField] 
    private string workerUrl = "ws://183c-84-75-48-249.ngrok.io";

    [SerializeField] 
    private string shardHex = "Fdb2TM3owt4unpvESoSMTpVWPvCiXMzYyb42LzSsmFLi";

    [SerializeField]
    private string mrenclaveHex = "Fdb2TM3owt4unpvESoSMTpVWPvCiXMzYyb42LzSsmFLi";

    public Wallet wallet;

    public Dot4GClient dot4gClient;

    void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        LoggingConfiguration config = new LoggingConfiguration();
        LogManager.Configuration = config;

        // initializing standard ajuna wallet
        wallet = new Wallet();

        // initializing unity interface for dot4g
        dot4gClient = new Dot4GClient(
            wallet,
            nodeUrl,
            shardHex,
            mrenclaveHex
        );
    }
}
