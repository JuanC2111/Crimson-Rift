using UnityEngine;

// Controlador genérico de enemigo caminante. Los nombres de animación son campos,
// así que el mismo script sirve para el hongo, el goblin o el esqueleto.
[RequireComponent(typeof(Rigidbody2D), typeof(Health), typeof(Collider2D))]
public class EnemyController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool spriteFacesRight = true; // Si camina "de espaldas", desmárcalo
    [SerializeField] private float patrolDistance = 4f;    // Cuánto se aleja a cada lado de donde lo pusiste

    [Header("Detección")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Daño")]
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private float hurtDuration = 0.35f;

    [Header("Animaciones")]
    [SerializeField] private string runAnimation = "Mushroom_Run";
    [SerializeField] private string hurtAnimation = "Mushroom_Hurt";
    [SerializeField] private string deathAnimation = "Mushroom_Death";

    [Header("Visual")]
    [SerializeField] private Transform visual;
    [SerializeField] private Animator animator;

    public string RunAnimation => runAnimation;
    public string HurtAnimation => hurtAnimation;
    public string DeathAnimation => deathAnimation;
    public float HurtDuration => hurtDuration;

    public StateMachine Machine { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    public EnemyHurtState HurtState { get; private set; }
    public EnemyDeathState DeathState { get; private set; }

    private Rigidbody2D rb;
    private Collider2D col;
    private Health health;
    private int facing = -1; // -1 izquierda, 1 derecha
    private float nextTurnTime;
    private float startX;    // Punto de origen de la patrulla
    private Vector2 lastHitSource;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        health = GetComponent<Health>();

        startX = transform.position.x;

        Machine = new StateMachine();
        PatrolState = new EnemyPatrolState(this, Machine);
        HurtState = new EnemyHurtState(this, Machine);
        DeathState = new EnemyDeathState(this, Machine);
    }

    private void OnEnable()
    {
        health.Hurt += OnHurt;
        health.Died += OnDied;
    }

    private void OnDisable()
    {
        health.Hurt -= OnHurt;
        health.Died -= OnDied;
    }

    private void Start()
    {
        ApplyFacing();
        Machine.Initialize(PatrolState);
    }

    private void Update() => Machine.Tick();

    private void FixedUpdate() => Machine.FixedTick();

    private void OnHurt(Vector2 source)
    {
        lastHitSource = source;
        Machine.ChangeState(HurtState);
    }

    private void OnDied() => Machine.ChangeState(DeathState);

    // ---------- Acciones que usan los estados ----------

    public void PlayAnimation(string clipName) => animator.Play(clipName);

    public void Walk() => rb.linearVelocity = new Vector2(facing * moveSpeed, rb.linearVelocity.y);

    public void Stop() => rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

    public void Knockback()
    {
        float dir = Mathf.Sign(transform.position.x - lastHitSource.x);
        if (dir == 0f) dir = 1f;
        rb.linearVelocity = new Vector2(dir * 3f, 3f);
    }

    public void Flip()
    {
        facing = -facing;
        nextTurnTime = Time.time + 0.3f; // Evita girar sin parar en un hueco angosto
        ApplyFacing();
    }

    // Decide si debe darse la vuelta: límite de patrulla, pared o borde.
    // Cada giro escribe en la Console el motivo, para poder diagnosticar.
    public bool ShouldTurn()
    {
        if (Time.time < nextTurnTime) return false;

        // 1) Límite de patrulla: no depende del suelo, así que siempre funciona.
        float offset = transform.position.x - startX;
        if (Mathf.Abs(offset) > patrolDistance && Mathf.Sign(offset) == facing)
        {
            Debug.Log(name + " gira por LIMITE en x=" + transform.position.x.ToString("F1"));
            return true;
        }

        // Si ya está cayendo, no decide con pared ni borde.
        if (rb.linearVelocity.y < -1f) return false;

        Bounds b = col.bounds;

        // 2) Pared delante.
        bool wall = Physics2D.Raycast(b.center, new Vector2(facing, 0f), b.extents.x + 0.1f, groundLayer);

        // 3) Borde: busca suelo un poco adelante de los pies.
        Vector2 footAhead = new Vector2(b.center.x + facing * (b.extents.x + 0.35f), b.min.y + 0.2f);
        bool groundAhead = Physics2D.Raycast(footAhead, Vector2.down, 0.8f, groundLayer);

        if (wall) Debug.Log(name + " gira por PARED en x=" + transform.position.x.ToString("F1"));
        if (!groundAhead) Debug.Log(name + " gira por BORDE en x=" + transform.position.x.ToString("F1"));

        return wall || !groundAhead;
    }

    private void ApplyFacing()
    {
        Vector3 s = visual.localScale;
        float sign = facing * (spriteFacesRight ? 1f : -1f);
        s.x = Mathf.Abs(s.x) * sign;
        visual.localScale = s;
    }

    // Daño por contacto: Health del jugador ya tiene su protección de 1 segundo.
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (health.IsDead) return;
        if (!collision.collider.CompareTag("Player")) return;

        if (collision.collider.TryGetComponent(out IDamageable target))
            target.TakeDamage(contactDamage, transform.position);
    }

    // Dibuja en la Scene la línea amarilla de patrulla cuando seleccionas el hongo.
    private void OnDrawGizmosSelected()
    {
        float centerX = Application.isPlaying ? startX : transform.position.x;
        float y = transform.position.y;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(centerX - patrolDistance, y, 0f), new Vector3(centerX + patrolDistance, y, 0f));
        Gizmos.DrawLine(new Vector3(centerX - patrolDistance, y - 0.5f, 0f), new Vector3(centerX - patrolDistance, y + 0.5f, 0f));
        Gizmos.DrawLine(new Vector3(centerX + patrolDistance, y - 0.5f, 0f), new Vector3(centerX + patrolDistance, y + 0.5f, 0f));
    }
}