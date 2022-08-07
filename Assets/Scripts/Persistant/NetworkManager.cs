using Ajuna.NetWallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Ajuna.NetApi.Model.Types;
using Ajuna.UnityInterface;
using Ajuna.NetApi.Model.FrameSystem;
using Ajuna.NetApi;
using System.Threading;
using System.Numerics;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;

    [SerializeField]
    public string NodeUrl = "ws://127.0.0.1:9944";

    [SerializeField]
    private string workerUrl = "ws://45cc-84-75-48-249.ngrok.io";

    [SerializeField]
    private string shardHex = "2WTKarArPH1jxUCCDMbLvmDKG9UiPZxfBrb2eQUWyU3K";

    [SerializeField]
    private string mrenclaveHex = "2WTKarArPH1jxUCCDMbLvmDKG9UiPZxfBrb2eQUWyU3K";

    [SerializeField]
    public string WalletName = "dev_wallet";

    private System.Random _random;

    public Wallet Wallet;

    public Account Account;

    public NodeClient NodeClient;

    public WorkerClient WorkerClient;

    //public AccountInfo AccountInfo;

    public BigInteger FreeBalance;

    public BigInteger WorkerBalance;

    void Awake()
    {
        _random = new System.Random();

        Instance = this;

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        //var randomBytes = new byte[16];
        //_random.NextBytes(randomBytes);

        //var mnemonic = string.Join(' ', Mnemonic.MnemonicFromEntropy(randomBytes, Mnemonic.BIP39Wordlist.English));
        //Account = Mnemonic.GetAccountFromMnemonic(mnemonic, "aA1234dd", Ajuna.NetApi.Model.Types.KeyType.Sr25519);

        NodeClient = new NodeClient(NodeUrl);
        WorkerClient = new WorkerClient(workerUrl, shardHex, mrenclaveHex);

        // initializing standard ajuna wallet
        Wallet = new Wallet();

        FreeBalance = new BigInteger(0);
        WorkerBalance = new BigInteger(0);

        InvokeRepeating("PollNode", 0, 2f);
        InvokeRepeating("PollWorker", 0, 2f);

    }

    public void SetAccount(Account account)
    {
        Account = account;
        NodeClient.Account = account;
        WorkerClient.Account = account;
    }

    public async Task PollNode()
    {
        var accountInfo = await NodeClient.GetBalanceAsync(CancellationToken.None);

        if (accountInfo == null || accountInfo.Data == null || accountInfo.Data.Free.Value == 0)
        {
            FreeBalance = new BigInteger(0);
            return;
        } 

        FreeBalance = accountInfo.Data.Free.Value;
    }

    public async Task PollWorker()
    {
        var balance = await WorkerClient.GetBalanceAsync();

        Debug.Log($"pooling worker here. balance is null? {balance == null}");

        if (balance == null || balance.Value == 0)
        {
            WorkerBalance = new BigInteger(0);
            return;
        }

        WorkerBalance = balance.Value;
    }

    internal bool IsMe(string address)
    {
        return Wallet.Account.Value == address;
    }
}
