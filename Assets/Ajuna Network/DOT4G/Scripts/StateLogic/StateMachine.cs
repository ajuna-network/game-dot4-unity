using UnityEngine;

namespace StateLogic
{
    public abstract class StateMachine : MonoBehaviour
    {
        private IState currentState;
        public IState previousState;

        public IState CurrentState
        {
            get { return currentState; }
            set
            {
                if (currentState != null)
                {
                    //PrevState = State;
                    currentState.Exit();
                }

                previousState = currentState;
                currentState = value;
                print(currentState);
                currentState.Enter();
            }
        }

        private void Update()
        {
            currentState?.Action();
        }
    }
}