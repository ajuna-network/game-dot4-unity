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

                yield return new WaitForSeconds(1);

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

            // having a runner ID means we have a game to join
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

            StartCoroutine(nameof(Joining));
        }
        public enum WorkerState
        {
            None,
            Connect,
            Faucet,
            Game,
            Join,
        }

        IEnumerator Joining()
        {
            var workerState = WorkerState.None;

            while (workerState != WorkerState.Join)
            {
                searchingText.text = GetSeachingText(workerState);

                var excuteExtrinsicTask = ExecuteExtrinsic(workerState);

                var workerStateTask = GetWorkerState();

                yield return new WaitForSeconds(1);

                while (!workerStateTask.IsCompleted || !excuteExtrinsicTask.IsCompleted)
                {
                    yield return new WaitForSeconds(1);

                }
                workerState = workerStateTask.Result;
            }

            JoinMatch();
        }

        private string GetSeachingText(WorkerState workerState)
        {
            return workerState switch
            {
                WorkerState.Connect => "waiting for tea",
                WorkerState.Faucet => "waiting for sister",
                WorkerState.Game => "waiting for game",
                WorkerState.Join => "joining",
                _ => "searching stuff",
            };
        }

        public async Task<bool> ExecuteExtrinsic(WorkerState workerState)
        {
            switch (workerState)
            {
                case WorkerState.Connect:
                    return await Network.Dot4GClient.ConnectTeeAsync();

                case WorkerState.Faucet:
                    return await Network.Dot4GClient.FaucetWorkerAsync();

                default:
                    return false;
            }
        }

        public async Task<WorkerState> GetWorkerState()
        {
            if(!Network.Dot4GClient.IsTeeConnected)
            {
                return WorkerState.Connect;
            }

            var balance = await Network.Dot4GClient.GetBalanceWorkerAsync();
            if (balance == null)
            {
                return WorkerState.Faucet;
            }

            var gameBoard = await Network.Dot4GClient.GetGameBoardAsync();
            if (gameBoard == null)
            {
                return WorkerState.Game;
            }

            return WorkerState.Join;
        }

        void JoinMatch()
        {
            SceneManager.LoadScene("Game");
        }
    }
}