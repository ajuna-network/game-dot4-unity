namespace StateLogic
{
    public interface IState
    {
        public void Enter();

        public void Action();

        public void Exit();
    }
}