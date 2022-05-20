using _StateMachine;
using Game.Engine;
using Game.GameResults;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.States
{
    public class ResultState : State<GameManager, ResultsUI>
    {
        public ResultState(GameManager stateMachine, ResultsUI ui) : base(stateMachine, ui)
        {
        }

        public override void Enter()
        {
            StateUI.ShowUI();
            StateUI.nextBtn.onClick.AddListener(NextClicked);
            
            StateUI.SetResultText(EngineManager.Fullstate.GameState);
            //set ui text based on timeout or finished
        }

        void NextClicked()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
