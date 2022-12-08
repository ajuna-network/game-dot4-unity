using UnityEngine;

public enum State
{
    InitState,
    SplashState,
    LandingState,
    AuthenticationState,
    LeaderboardState,
    AchievementState,
    FaucetState,
    
    GameSetupState,
    GamePlayingState,
    GameResultsState
}

public class EnumStateMachine : MonoBehaviour
{
    protected State currentState;
    private State previousState;



    // public StateBase CurrentState
    // {
    //     get
    //     {
    //         return currentState;
    //     }
    //
    //     set
    //     {
    //         if (currentState != null)
    //         {
    //             currentState.ExitState(this);
    //         }
    //
    //         currentState = value;
    //
    //         currentState.EnterState(this);
    //     }
    // }
    //
    // public void UpdateS(StateBase state, EnumStateMachine stateMachine)
    // {
    //     if (currentState != null)
    //     {
    //         currentState.ExitState(stateMachine);
    //     }
    //
    //     currentState = state;
    //
    //     currentState.EnterState(stateMachine);
    // }



    protected void UpdateState(State nextState)
    {
        // if (currentState != null)
        // {
        //     StopCoroutine();
        // }
        previousState = currentState;
        currentState = nextState;
        string methodName = nextState.ToString();
        StartCoroutine(methodName);
    }

    public void BackClicked()
    {
        UpdateState(previousState);
    }
}