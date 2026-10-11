using UnityEngine;

public abstract class PlayerBaseState : IState
{
    protected readonly PlayerController player;
    protected readonly StateMachine machine;

    protected PlayerBaseState(PlayerController player, StateMachine machine)
    {
        this.player = player;
        this.machine = machine;
    }

    public virtual void Enter() { }
    public virtual void Tick() { }
    public virtual void FixedTick() { }
    public virtual void Exit() { }
}

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerController p, StateMachine m) : base(p, m) { }

    public override void Enter() => player.PlayAnimation("Player_Idle");

    public override void Tick()
    {
        if (player.CanJump) machine.ChangeState(player.JumpState);
        else if (!player.IsGrounded) machine.ChangeState(player.FallState);
        else if (player.AttackPressed) machine.ChangeState(player.AttackState);
        else if (Mathf.Abs(player.MoveInput) > 0.01f) machine.ChangeState(player.RunState);
    }

    public override void FixedTick() => player.Move(0f);
}

public class PlayerRunState : PlayerBaseState
{
    public PlayerRunState(PlayerController p, StateMachine m) : base(p, m) { }

    public override void Enter() => player.PlayAnimation("Player_Run");

    public override void Tick()
    {
        if (player.CanJump) machine.ChangeState(player.JumpState);
        else if (!player.IsGrounded) machine.ChangeState(player.FallState);
        else if (player.AttackPressed) machine.ChangeState(player.AttackState);
        else if (Mathf.Abs(player.MoveInput) < 0.01f) machine.ChangeState(player.IdleState);
    }

    public override void FixedTick() => player.Move(player.MoveInput);
}

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerController p, StateMachine m) : base(p, m) { }

    public override void Enter()
    {
        player.PerformJump();
        player.PlayAnimation("Player_Jump");
    }

    public override void Tick()
    {
        if (player.JumpReleased && player.Rb.linearVelocity.y > 0f) player.CutJump();

        if (player.Rb.linearVelocity.y <= 0f) machine.ChangeState(player.FallState);
    }

    public override void FixedTick() => player.Move(player.MoveInput);
}

public class PlayerFallState : PlayerBaseState
{
    public PlayerFallState(PlayerController p, StateMachine m) : base(p, m) { }

    public override void Enter()
    {
        player.SetGravity(player.FallGravityScale);
        player.PlayAnimation("Player_Fall");
    }

    public override void Tick()
    {
        if (player.CanJump) machine.ChangeState(player.JumpState);
        else if (player.IsGrounded)
        {
            bool moving = Mathf.Abs(player.MoveInput) > 0.01f;
            machine.ChangeState(moving ? player.RunState : player.IdleState);
        }
    }

    public override void FixedTick() => player.Move(player.MoveInput);

    public override void Exit() => player.SetGravity(player.GravityScale);
}

// Ataque: el héroe se queda quieto, y a mitad de la animación cae el golpe una sola vez.
public class PlayerAttackState : PlayerBaseState
{
    private float timer;
    private bool hitDone;
    private PlayerCombat combat;

    public PlayerAttackState(PlayerController p, StateMachine m) : base(p, m) { }

    public override void Enter()
    {
        if (combat == null) combat = player.GetComponent<PlayerCombat>();
        timer = 0f;
        hitDone = false;
        player.PlayAnimation("Player_Attack");
    }

    public override void Tick()
    {
        timer += Time.deltaTime;

        if (!hitDone && timer >= combat.HitDelay)
        {
            combat.DoHit();
            hitDone = true;
        }

        if (timer >= player.AttackDuration)
            machine.ChangeState(player.IsGrounded ? player.IdleState : player.FallState);
    }

    public override void FixedTick() => player.Move(0f);
}