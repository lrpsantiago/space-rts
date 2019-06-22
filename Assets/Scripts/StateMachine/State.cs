
namespace SpaceRts
{
    public abstract class State<T> : IState where T : StateMachine
    {
        internal T Owner { get; private set; }

        public State(T owner)
        {
            Owner = owner;
        }

        public abstract void OnEnter();

        public abstract void Update();

        public abstract void OnLeave();
    }
}
