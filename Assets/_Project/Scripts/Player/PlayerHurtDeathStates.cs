using UnityEngine;

// Golpe recibido: empuja al héroe y lo deja sin control un instante.
public class PlayerHurtState : PlayerBaseState
{
    private float timer;

    public PlayerHurtState(PlayerController p, StateMachine m) : base(p, m) { }

    public override void Enter()
    {
        timer = player.HurtDuration;
        player.PlayAnimation("Player_Hurt");
        player.Knockback();
    }

    public override void Tick()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            machine.ChangeState(player.IsGrounded ? player.IdleState : player.FallState);
    }
}

// Muerte: no tiene transiciones de salida. Reiniciar el nivel lo decide otro sistema.
public class PlayerDeathState : PlayerBaseState
{
    public PlayerDeathState(PlayerController p, StateMachine m) : base(p, m) { }

    public override void Enter() => player.PlayAnimation("Player_Death");

    public override void FixedTick() => player.Move(0f);
}