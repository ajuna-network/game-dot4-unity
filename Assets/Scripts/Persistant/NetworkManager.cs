using Ajuna.NetWallet;
using System.Threading.Tasks;
using UnityEngine;
using Ajuna.NetApi.Model.Types;
using Ajuna.UnityInterface;
using System.Threading;
using System.Numerics;
using Ajuna.NetApi.Model.FrameSystem;

public class NetworkManager : MonoBehaviour
{
    public enum NodeState { None, Connect, Balance }

    public enum WorkerState { None, Connect, Shield, Balance }

    public static NetworkManager Instance;

    [SerializeField]
    public string NodeUrl = "ws://127.0.0.1:9944";

    [SerializeField]
    private string workerUrl = "ws://699f-84-75-48-249.ngrok.io";

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

        InvokeRepeating("PollNode", 0, 1f);
        InvokeRepeating("PollWorker", 0, 1f);
    }

    public void SetAccount(Account account)
    {
        Account = account;
        NodeClient.Account = account;
        WorkerClient.Account = account;
    }

    public NodeState GetNodeState()
    {
        if (NodeClient == null)
        {
            return NodeState.None;
        }

        if (!NodeClient.IsConnected)
        {
            return NodeState.Connect;
        }

        return NodeState.Balance;
    }

    public async Task PollNode()
    {
        AccountInfo accountInfo = null;
        switch (GetNodeState())
        {
            case NodeState.None:
                break;

            case NodeState.Connect:
                _ = await NodeClient.ConnectAsync(false, true, CancellationToken.None);
                break;

            case NodeState.Balance:
                accountInfo = await NodeClient.GetBalanceAsync(CancellationToken.None);
                break;
        }

        if (accountInfo == null || accountInfo.Data == null || accountInfo.Data.Free.Value == 0)
        {
            FreeBalance = new BigInteger(0);
            return;
        } 

        FreeBalance = accountInfo.Data.Free.Value;
    }

    public WorkerState GetWorkerState()
    {
        if (WorkerClient == null)
        {
            return WorkerState.None;
        }

        if (!WorkerClient.IsConnected)
        {
            return WorkerState.Connect;
        }

        if (!WorkerClient.HasShieldingKey)
        {
            return WorkerState.Shield;
        }

        return WorkerState.Balance;
    }

    public async Task PollWorker()
    {
        var balance = await WorkerClient.GetBalanceAsync();

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
