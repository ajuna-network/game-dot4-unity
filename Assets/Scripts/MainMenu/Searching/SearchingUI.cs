using Ajuna.NetApi.Model.AjunaCommon;
using System.Collections;
using System.Linq;
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
            if (Network.NodeClient.ExtrinsicManger.Running.Any())
            {
                return false;
            }

            if (searchState == SearchState.NeedToQueue)
            {
                return await Network.NodeClient.QueueAsync(CancellationToken.None);
            }
        
            return false;
        }

        public async Task<SearchState> GetSearchState()
        {
            var playerQueued = await Network.NodeClient.GetPlayerQueueAsync(CancellationToken.None);
            
            var runnerId = await Network.NodeClient.GetRunnerIdAsync(CancellationToken.None);

            if (Network.FreeBalance == null || Network.FreeBalance < 1000000)
            {
                return SearchState.AskBigBag;
            }

            // having a runner ID means we have a game to join
            if (runnerId != null && runnerId.Value != 0)
            {
                var runnerState = await Network.NodeClient.GetRunnerStateAsync(runnerId, CancellationToken.None);
                if (runnerState != null && runnerState.Value == RunnerState.Accepted)
                {
                    return SearchState.MatchFound;
                } 

                return SearchState.CheckRunnerState;
            }
            else if (playerQueued != null && playerQueued.Value > 0)
            {
                return SearchState.WaitForPlayers; 
            }
            else if (runnerId == null || runnerId.Value == 0)
            {
                return SearchState.NeedToQueue;
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
            Shield,
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
                WorkerState.Connect => "boom me up, scotty",
                WorkerState.Shield => "bubble please",
                WorkerState.Faucet => "insert coin, sister",
                WorkerState.Game => "lfg for mc ",
                WorkerState.Join => "teleport please",
                _ => "searching stuff",
            };
        }

        public async Task<bool> ExecuteExtrinsic(WorkerState workerState)
        {
            return workerState switch
            {
                WorkerState.Connect => await Network.WorkerClient.ConnectAsync(false, false, CancellationToken.None),
                WorkerState.Shield => await Network.WorkerClient.GetShieldingKeyAsync(),
                WorkerState.Faucet => await Network.WorkerClient.FaucetAsync(),
                _ => false,
            };
        }

        public async Task<WorkerState> GetWorkerState()
        {
            if(!Network.WorkerClient.IsConnected)
            {
                return WorkerState.Connect;
            }

            if (!Network.WorkerClient.HasShieldingKey)
            {
                return WorkerState.Shield;
            }

            var balance = await Network.WorkerClient.GetBalanceAsync();
            if (balance == null)
            {
                return WorkerState.Faucet;
            }

            var gameBoard = await Network.WorkerClient.GetGameBoardAsync();
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