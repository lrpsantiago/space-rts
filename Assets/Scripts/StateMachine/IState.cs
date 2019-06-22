
namespace SpaceRts
{
    public interface IState
    {
        void OnEnter();
        void Update();
        void OnLeave();
    }
}
