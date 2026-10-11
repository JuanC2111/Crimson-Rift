using UnityEngine;

// Misma idea que los estados del jugador: cada estado decide qué hace y a cuál pasar.
public abstract class EnemyBaseState : IState
{
    protected readonly EnemyController enemy;
    protected readonly StateMachine machine;

    protected EnemyBaseState(EnemyController enemy, StateMachine machine)
    {
        this.enemy = enemy;
        this.machine = machine;
    }

    public virtual void Enter() { }
    public virtual void Tick() { }
    public virtual void FixedTick() { }
    public virtual void Exit() { }
}

// Patrulla: camina hacia adelante y se da la vuelta si hay pared o vacío.
public class EnemyPatrolState : EnemyBaseState
{
    public EnemyPatrolState(EnemyController e, StateMachine m) : base(e, m) { }

    public override void Enter() => enemy.PlayAnimation(enemy.RunAnimation);

    public override void FixedTick()
    {
        if (enemy.ShouldTurn()) enemy.Flip();
        enemy.Walk();
    }
}

// Golpe recibido: se detiene un instante, retrocede un poco y vuelve a patrullar.
public class EnemyHurtState : EnemyBaseState
{
    private float timer;

    public EnemyHurtState(EnemyController e, StateMachine m) : base(e, m) { }

    public override void Enter()
    {
        timer = enemy.HurtDuration;
        enemy.PlayAnimation(enemy.HurtAnimation);
        enemy.Knockback();
    }

    public override void Tick()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) machine.ChangeState(enemy.PatrolState);
    }
}

// Muerte: se queda quieto mientras suena la animación. HitFeedback lo elimina después.
public class EnemyDeathState : EnemyBaseState
{
    public EnemyDeathState(EnemyController e, StateMachine m) : base(e, m) { }

    public override void Enter() => enemy.PlayAnimation(enemy.DeathAnimation);

    public override void FixedTick() => enemy.Stop();
}