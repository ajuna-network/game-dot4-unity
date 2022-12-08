using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Ajuna.UnityInterface;

public class NetworkInfo : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI walletTokens;

    [SerializeField] 
    private TextMeshProUGUI workerTokens;

    [SerializeField] 
    private Image walletConnectionState;

    [SerializeField] 
    private Image teeConnectionState;

    private bool blinking = false;


    private void Start()
    {
        InvokeRepeating(nameof(CheckTeeConnection), 0, 1);
        InvokeRepeating(nameof(CheckWalletConnection), 0, 1);
    }


    void CheckTeeConnection()
    {
        if (NetworkManager.Instance.WorkerClient.IsConnected)
        {
            teeConnectionState.color = Color.green;
            workerTokens.text = NetworkManager.Instance.WorkerBalance.ToString("0") + " WURM";
        } 
        else
        {
            teeConnectionState.color = Color.red;
        }       
    }

    IEnumerator BlinkLight()
    {
        while (blinking)
        {
            walletConnectionState.color = Color.magenta;

            yield return new WaitForSeconds(0.25f);

            walletConnectionState.color = Color.green;

            yield return new WaitForSeconds(0.25f);
            yield return null;
        }

        blinking = false;
    }

    void CheckWalletConnection()
    {
        if (NetworkManager.Instance.NodeClient != null && NetworkManager.Instance.NodeClient.IsConnected)
        {
            if (NetworkManager.Instance.NodeClient.ExtrinsicManger.Running.Any())
            {
                if (!blinking)
                {
                    blinking = true;
                    StartCoroutine(BlinkLight());
                }
                return;
            }

            blinking = false;

            walletConnectionState.color = Color.yellow;

            if (NetworkManager.Instance.Wallet.IsUnlocked)
            {
                walletConnectionState.color = Color.green;
                var total = (double)NetworkManager.Instance.FreeBalance / Math.Pow(10, 12);
                Debug.Log(total);
                walletTokens.text = total.ToString("0.000") + " AJUN";
            }
        }
        else
        {
            walletConnectionState.color = Color.red;
        }
    }
}