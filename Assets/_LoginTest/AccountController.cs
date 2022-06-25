using NLog;
using NLog.Config;
using SubstrateNetApi;
using SubstrateNetWallet;
using System.Threading.Tasks;
using UnityEngine;

public class AccountController : MonoBehaviour
{
    [SerializeField]
    private string _websocketUrl = "wss://mogiway-01.dotmog.com";

    public string WebSocketUrl => _websocketUrl;

    public Wallet Wallet { get; private set; }

    private bool ChainInfoUpdateEventReceived = false;

    private void Awake()
    {
        LoggingConfiguration config = new LoggingConfiguration();
        LogManager.Configuration = config;
    }

    // Start is called before the first frame update
    void Start()
    {
        Wallet = new Wallet();
        //Wallet.ChainInfoUpdated += Wallet_ChainInfoUpdated;
    }

    private void Wallet_ChainInfoUpdated(object sender, ChainInfo e)
    {
        ChainInfoUpdateEventReceived = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (ChainInfoUpdateEventReceived)
        {
            Debug.Log($"ChainInfoUpdateEvent Received! {Wallet.ChainInfo.BlockNumber}");
            ChainInfoUpdateEventReceived = false;
        }
    }

    public async Task ConnectAsync()
    {
        await Wallet.StartAsync(_websocketUrl);

    }

    public async Task DisconnectAsync()
    {
        await Wallet.StopAsync();
    }

    void OnApplicationQuit()
    {
        _ = DisconnectAsync();
    }

}
