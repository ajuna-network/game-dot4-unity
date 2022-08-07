using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ajuna.UnityInterface;
using MainMenu.Searching.UI;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class NetworkInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI walletTokens;
    [SerializeField] private TextMeshProUGUI workerTokens;
    [SerializeField] private Image walletConnectionState;
    [SerializeField] private Image teeConnectionState;

    private bool blinking = false;


    private void Start()
    {
        InvokeRepeating(nameof(CheckTeeConnection), 0, 1);
        InvokeRepeating(nameof(CheckWalletConnection), 0, 1);
        //InvokeRepeating(nameof(GetWalletTokens), 0, 1);
    }


    void CheckTeeConnection()
    {
        //if (NetworkManager.Instance.NodeClient.ExtrinsicManger.Running.Any())
        //{
        //    if (!blinking)
        //    {
        //        blinking = true;
        //        StartCoroutine(BlinkLight());
        //    }

        //    return;
        //}

        //blinking = false;
        teeConnectionState.color = NetworkManager.Instance.WorkerClient.IsConnected ? Color.green : Color.red;
    }


    IEnumerator BlinkLight()
    {
        while (blinking)
        {
            teeConnectionState.color = Color.red;

            yield return new WaitForSeconds(0.25f);

            teeConnectionState.color = Color.green;

            yield return new WaitForSeconds(0.25f);
            yield return null;
        }

        blinking = false;
    }


    void CheckWalletConnection()
    {
        if (NetworkManager.Instance.NodeClient != null && NetworkManager.Instance.NodeClient.IsConnected)
        {
            walletConnectionState.color = Color.yellow;

            if (NetworkManager.Instance.Wallet.IsUnlocked)
            {
                walletConnectionState.color = Color.green;
                GetWalletTokens();
            }
        }
        else
        {
            walletConnectionState.color = Color.red;
        }
    }


    void GetWalletTokens()
    {
        var tokens = NetworkManager.Instance.FreeBalance;
        var total = (double)tokens / Math.Pow(10, 12);
        walletTokens.text = total.ToString("0.000") + " AJUN";

        var workers = NetworkManager.Instance.WorkerBalance;
        workerTokens.text = workers.ToString("0") + " WURM";
    }
}