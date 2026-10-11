using UnityEngine;

// Contrato que cumple cada estado del jugador (y luego de los enemigos).
public interface IState
{
    void Enter();      // Una vez, al entrar al estado
    void Tick();       // Cada frame: lógica y transiciones
    void FixedTick();  // Cada paso de física: mover el Rigidbody
    void Exit();       // Una vez, al salir
}
