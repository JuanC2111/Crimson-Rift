using UnityEngine;

// Máquina de estados genérica: guarda el estado actual y gestiona el cambio.
public class StateMachine
{
    public IState Current { get; private set; }

    public void Initialize(IState startState)
    {
        Current = startState;
        Current.Enter();
    }

    public void ChangeState(IState next)
    {
        if (next == Current) return;   // Evita reiniciar el mismo estado
        Current?.Exit();
        Current = next;
        Current.Enter();
    }

    public void Tick() => Current?.Tick();
    public void FixedTick() => Current?.FixedTick();
}
