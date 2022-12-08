using StateLogic;
using MainMenu.Searching;
using MainMenu.Setup.UI;

namespace MainMenu.Setup
{
    public class SetupState : State<MainMenuManager, SetupUI>
    {
        public SetupState(MainMenuManager stateMachine, SetupUI ui) : base(stateMachine, ui)
        {
        }

        public override void Enter()
        {
            StateUI.ShowUI();

            StateUI.backBtn.onClick.AddListener(BackClicked);
            StateUI.startBtn.onClick.AddListener(StartClicked);
            
            StateUI.pvpTog.onValueChanged.AddListener(PvpClicked);
            StateUI.pvaiTog.onValueChanged.AddListener(PvAIClicked);


            StateMachine.gameSettings.pvP = StateUI.pvpTog.isOn;
            StateMachine.gameSettings.pvAI = StateUI.pvaiTog.isOn;
        }

        public override void Exit()
        {
            StateUI.HideUI();

            StateUI.backBtn.onClick.RemoveListener(BackClicked);
            StateUI.startBtn.onClick.AddListener(StartClicked);
            
            StateUI.pvpTog.onValueChanged.RemoveListener(PvpClicked);
            StateUI.pvaiTog.onValueChanged.RemoveListener(PvAIClicked);
        }

        void PvpClicked(bool value)
        {
            StateMachine.gameSettings.pvP = value;
        }

        void PvAIClicked(bool value)
        {
            StateMachine.gameSettings.pvAI = value;
        }

        //Later conditions can be components, like buttonclick, animation played ect

        #region Conditions

        void StartClicked()
        {
            StateMachine.CurrentState = new SearchingState(StateMachine, StateMachine.searchingUI);
        }
        
        void BackClicked()
        {
            StateMachine.CurrentState = StateMachine.previousState;
        }

        #endregion
    }
}