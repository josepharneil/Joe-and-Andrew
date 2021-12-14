// A simple interface defining what a State is for the StateMchine.
public interface IState
{
    void OnEnter(); // Upon entering this state, what do we do?
    void Tick(); // What to do on a tick update. (A tick is one call of the Unity function Update)
    void FixedTick(); // What to do on a fixedtick update. (A tick is one call of the Unity function FixedUpdate)
    
    void OnExit(); // Upon exiting this state, what do we do?
}