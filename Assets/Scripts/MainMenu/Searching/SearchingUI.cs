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
            Canceled,
            NeedToQueue,
            WaitForPlayers,
            CheckRunnerState,
            Extrinsics
        }

        IEnumerator Searching()
        {
            var searchState = SearchState.None;

            searchingText.text = "searching for a game";

            while(searchState != SearchState.MatchFound || searchState != SearchState.Canceled)
            {
                switch (searchState)
                {
                    case SearchState.NeedToQueue:
                        searchingText.text = "waiting for queue";
                        break;

                    case SearchState.WaitForPlayers:
                        searchingText.text = "waiting for players";
                        break;

                    case SearchState.CheckRunnerState:
                        searchingText.text = "waiting for runner";
                        break;

                    case SearchState.Extrinsics:
                        searchingText.text = "waiting for transaction";
                        break;

                    case SearchState.None:
                    case SearchState.MatchFound:
                    case SearchState.Canceled:
                    default:
                        break;
                }

                var searchStateTask = GetSearchState();
                while (!searchStateTask.IsCompleted)
                {
                    yield return new WaitForSeconds(1);
                }
                searchState = searchStateTask.Result;
            }

            StartCoroutine(nameof(MatchFound));
        }

        public async Task<SearchState> GetSearchState()
        {
            if (Network.Dot4GClient.HasExtrinsics > 0)
            {
                return SearchState.Extrinsics;
            }

            var playerQueued = await Network.Dot4GClient.GetPlayerQueueAsync();
            
            var runnerId = await Network.Dot4GClient.GetRunnerIdAsync();
 
            if (runnerId.Value == 1)
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