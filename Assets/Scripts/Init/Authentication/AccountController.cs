using System;
using System.Globalization;
using DOTMogCore;
using DOTMogCore.Manager;
using NLog;
using NLog.Config;
using SubstrateNetApi;
using SubstrateNetWallet;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class AccountController : MonoBehaviour
{
    public static AccountController Instance;
   

    [SerializeField] private Image connectionIndicator;

   
    
   


    // public TextMeshProUGUI NodeInfo;
    // public TextMeshProUGUI ChainName;
    // public TextMeshProUGUI ChainVersion;
    // public TextMeshProUGUI SubstrateVersion;
    // public TextMeshProUGUI BlockNumber;
    // public TextMeshProUGUI BlockTrackerBar;
    // public TextMeshProUGUI EpochValue;
    // public TextMeshProUGUI EraValue;


    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(this.gameObject);
        
       

      

      ////  Wallet.ChainInfoUpdated += OnChainInfoUpdated;

       
    }

    // void CheckConnection()
    // {
    //     Wallet.StartAsync(_websocketUrl);
    //     
    //     print("test");
    // }




    // private void OnChainInfoUpdated(object sender, ChainInfo chainInfo)
    // {
    //   
    //     
    //     if (chainInfo != null)
    //     {
    //         connectionIndicator.color = Color.green;
    //
    //         var blockNumber = chainInfo.BlockNumber;
    //
    //
    //         var barFill = (float) (blockNumber % 6) / 5;
    //         // TODO: this needs to be verified epoche 200 blocks 10 min, era 1200 blocks 60 min
    //         var blocknumberExt = blockNumber + 616;
    //         var blocksTillEpoch = blocknumberExt % 200;
    //         var epochPerc = (double) blocksTillEpoch / 200;
    //         var blocksTillEra = blocknumberExt % 1200;
    //         var eraPerc = (double) blocksTillEra / 1200;
    //
    //
    //         UnityMainThreadDispatcher.Instance().Enqueue(() =>
    //         {
    //             //  NodeInfo.text = WebSocketUrl;
    //             ChainName.text = chainInfo.Chain;
    //             ChainVersion.text = "SPEC." + chainInfo.RuntimeVersion.SpecVersion.ToString("000");
    //             SubstrateVersion.text = $"{chainInfo.Name} {chainInfo.Version}";
    //             BlockNumber.text = blockNumber.ToString("#,#", CultureInfo.InvariantCulture);
    //             //  BlockTrackerBar.Fill(barFill, lockState);
    //             EpochValue.text = epochPerc.ToString("P", CultureInfo.InvariantCulture);
    //             EraValue.text = eraPerc.ToString("P", CultureInfo.InvariantCulture);
    //         });
    //     }
    //     else
    //     {
    //         
    //         connectionIndicator.color = Color.red;
    //
    //         UnityMainThreadDispatcher.Instance().Enqueue(() =>
    //         {
    //             NodeInfo.text = "";
    //             ChainName.text = "";
    //             ChainVersion.text = "";
    //             SubstrateVersion.text = "";
    //             BlockNumber.text = "";
    //             EpochValue.text = "";
    //             EraValue.text = "";
    //         });
    //     }
    // }
}