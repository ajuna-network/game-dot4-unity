namespace _StateMachine
{
    public interface IState
    {
        public void Enter();

        public void Action();

        public void Exit();
    }


    public abstract class State<T,U> : IState where T : StateMachine where U : UICanvas
    {
        protected T StateMachine;
        protected U StateUI;

        protected State(T stateMachine, U ui)
        {
            StateMachine = stateMachine;
            StateUI = ui;
        }


        public virtual void Enter()
        {
        }


        public virtual void Action()
        {
      
        }

        public virtual void Exit()
        {
        }
    }
}