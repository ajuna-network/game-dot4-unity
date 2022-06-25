using _StateMachine;
using MainMenu.Faucet;
using MainMenu.Faucet.UI;
using UnityEngine;

namespace MainMenu.Faucet
{
    public class FaucetState : State<MainMenuManager, FaucetUI>
    {
        public NetworkManager Network => NetworkManager.Instance;

        public FaucetState(MainMenuManager stateMachine, FaucetUI ui) : base(stateMachine, ui)
        {
        }


        private string discordURL = "https://discord.gg/cE72GYcFgY";
        private string telegramURL = "https://t.me/ajunanetwork";
        private string twitterURL = "https://twitter.com/AjunaNetwork";
        private string ajunaURL = "https://ajuna.io/";

        public override void Enter()
        {
            StateUI.ShowUI();

            StateUI.discordBtn.onClick.AddListener(DiscordClicked);
            StateUI.telegramBtn.onClick.AddListener(TelegramClicked);
            StateUI.twitterBtn.onClick.AddListener(TwitterClicked);
            StateUI.ajunaBtn.onClick.AddListener(AjunaClicked);
            StateUI.faucetBtn.onClick.AddListener(FaucetClicked);

            StateUI.backBtn.onClick.AddListener(BackClicked);
        }

        public override void Exit()
        {
            StateUI.HideUI();

            StateUI.discordBtn.onClick.RemoveListener(DiscordClicked);
            StateUI.telegramBtn.onClick.RemoveListener(TelegramClicked);
            StateUI.twitterBtn.onClick.RemoveListener(TwitterClicked);
            StateUI.ajunaBtn.onClick.RemoveListener(AjunaClicked);
            StateUI.faucetBtn.onClick.RemoveListener(FaucetClicked);

            StateUI.backBtn.onClick.RemoveListener(BackClicked);
        }

        #region Conditions

        void BackClicked()
        {
            StateMachine.CurrentState = StateMachine.previousState;
        }

        void DiscordClicked()
        {
            Application.OpenURL(discordURL);
        }

        void TelegramClicked()
        {
            Application.OpenURL(telegramURL);
        }

        void TwitterClicked()
        {
            Application.OpenURL(twitterURL);
        }

        void AjunaClicked()
        {
            Application.OpenURL(ajunaURL);
        }

        void FaucetClicked()
        {
            // don't do as long as there is still an ongoing transaction
            if (Network.Dot4GClient.HasExtrinsics > 0)
            {
                return;
            }

            // let's leave alice alone
            if (Network.Wallet.AccountInfo != null && Network.Wallet.AccountInfo.Data.Free.Value > 10000000)
            {
                return;
            }

            _ = Network.Dot4GClient.FaucetAsync();
        }

        #endregion
    }
}