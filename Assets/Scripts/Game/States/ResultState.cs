using _StateMachine;
using Ajuna.NetApiExt.Model.AjunaWorker.Dot4G;
using Game.GameResults;
using UnityEngine.SceneManagement;

namespace Game.States
{
    public class ResultState : State<GameManager, ResultsUI>
    {
        public Dot4GObj Dot4GObj { get;}

        public ResultState(GameManager stateMachine, Dot4GObj dot4GObj, ResultsUI ui) : base(stateMachine, ui)
        {
            Dot4GObj = dot4GObj;
        }

        public override void Enter()
        {
            StateUI.ShowUI();
            StateUI.nextBtn.onClick.AddListener(NextClicked);

            Dot4GPlayer winner = null;
            if (Dot4GObj.Winner != null)
            {
                winner = Dot4GObj.Players[Dot4GObj.Winner];
            }

            StateUI.SetResultHeader(Dot4GObj.GamePhase, winner);
            //set ui text based on timeout or finished
            StateUI.GetScore();
        }

        void NextClicked()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
