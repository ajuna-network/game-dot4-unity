using Ajuna.NetApi.Model.AjunaCommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MainMenu.Searching.UI
{
    public class SearchingUI : UICanvas
    {
        public Button cancelBtn;
        public TextMeshProUGUI searchingText;

        public NetworkManager Network => NetworkManager.Instance;

        public enum SearchState { 
            None,
            MatchFound,
            NeedToQueue,
            WaitForPlayers,
            CheckRunnerState,
            Extrinsics,
            AskBigBag
        }

        IEnumerator Searching()
        {
            var searchState = SearchState.None;
            while (searchState != SearchState.MatchFound)
            {
                searchingText.text = GetSeachingText(searchState);
                
                var excuteExtrinsicTask = ExecuteExtrinsic(searchState);

                var searchStateTask = GetSearchState();

                while (!searchStateTask.IsCompleted || !excuteExtrinsicTask.IsCompleted)
                {
                    yield return new WaitForSeconds(1);

                }
                searchState = searchStateTask.Result;
            }

            StartCoroutine(nameof(MatchFound));
        }

        private string GetSeachingText(SearchState searchState)
        {
            return searchState switch
            {
                SearchState.AskBigBag => "find sister bajun big bag first",
                SearchState.NeedToQueue => "waiting for queue",
                SearchState.WaitForPlayers => "waiting for players",
                SearchState.CheckRunnerState => "waiting for runner",
                SearchState.Extrinsics => "waiting for transaction",
                SearchState.MatchFound => "match found",
                _ => "searching for a game",
            };
        }

        public async Task<bool> ExecuteExtrinsic(SearchState searchState)
        {
            if (Network.Dot4GClient.HasExtrinsics > 0)
            {
                return false;
            }
            if (searchState == SearchState.NeedToQueue)
            {
                return await Network.Dot4GClient.QueueAsync();
            }
        
            return false;
        }

        public async Task<SearchState> GetSearchState()
        {
            var playerQueued = await Network.Dot4GClient.GetPlayerQueueAsync();
            
            var runnerId = await Network.Dot4GClient.GetRunnerIdAsync();


            if (Network.Wallet.AccountInfo == null)
            {
                return SearchState.AskBigBag;
            }

            if (runnerId.Value != 0)
            {
                var runnerState = await Network.Dot4GClient.GetRunnerStateAsync(runnerId);
                if (runnerState != null && runnerState.Value == RunnerState.Accepted)
                {
                    return SearchState.MatchFound;
                } 

                return SearchState.CheckRunnerState;
            }
            else if (playerQueued.Value == 0)
            {
                return SearchState.NeedToQueue;
            }
            else if (playerQueued.Value == 1)
            {
                return SearchState.WaitForPlayers;
            }

            return SearchState.None;
        }

        IEnumerator MatchFound()
        {
            searchingText.text = "match found";
            yield return new WaitForSeconds(1);
            // await Task.Delay(TimeSpan.FromSeconds(1));


            StartCoroutine(nameof(Joining));
        }

        IEnumerator Joining()
        {
            searchingText.text = "joining";
            yield return new WaitForSeconds(1);
            // await Task.Delay(TimeSpan.FromSeconds(1));
            JoinMatch();
        }

        void JoinMatch()
        {
            SceneManager.LoadScene("Game");
        }
    }
}