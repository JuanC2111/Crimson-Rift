using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(Health))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 7f;

    [Header("Salto")]
    [SerializeField] private float jumpVelocity = 15f;
    [SerializeField] private float gravityScale = 3f;
    [SerializeField] private float fallGravityScale = 4.5f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField, Range(0f, 1f)] private float jumpCutFactor = 0.5f;

    [Header("Ataque")]
    [SerializeField] private float attackDuration = 0.5f;

    [Header("Daño")]
    [SerializeField] private float hurtDuration = 0.35f;  // 4 cuadros a 12 por segundo
    [SerializeField] private float knockbackForce = 6f;

    [Header("Detección de suelo")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.4f, 0.1f);
    [SerializeField] private LayerMask groundLayer;

    [Header("Visual")]
    [SerializeField] private Transform visual;
    [SerializeField] private Animator animator;

    public Rigidbody2D Rb { get; private set; }
    public float MoveInput { get; private set; }
    public bool IsGrounded { get; private set; }
    public bool JumpReleased { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool CanJump => jumpBufferCounter > 0f && coyoteCounter > 0f;
    public float GravityScale => gravityScale;
    public float FallGravityScale => fallGravityScale;
    public float AttackDuration => attackDuration;
    public float HurtDuration => hurtDuration;

    public StateMachine Machine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerRunState RunState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerFallState FallState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    public PlayerHurtState HurtState { get; private set; }
    public PlayerDeathState DeathState { get; private set; }

    private Health health;
    private Vector2 lastHitSource;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction attackAction;
    private float coyoteCounter;
    private float jumpBufferCounter;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Rb.gravityScale = gravityScale;
        health = GetComponent<Health>();

        Machine = new StateMachine();
        IdleState = new PlayerIdleState(this, Machine);
        RunState = new PlayerRunState(this, Machine);
        JumpState = new PlayerJumpState(this, Machine);
        FallState = new PlayerFallState(this, Machine);
        AttackState = new PlayerAttackState(this, Machine);
        HurtState = new PlayerHurtState(this, Machine);
        DeathState = new PlayerDeathState(this, Machine);

        moveAction = new InputAction("Move", InputActionType.Value);
        moveAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/a")
            .With("Positive", "<Keyboard>/d");
        moveAction.AddCompositeBinding("1DAxis")
            .With("Negative", "<Keyboard>/leftArrow")
            .With("Positive", "<Keyboard>/rightArrow");
        moveAction.AddBinding("<Gamepad>/leftStick/x");

        jumpAction = new InputAction("Jump", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.AddBinding("<Keyboard>/w");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");

        attackAction = new InputAction("Attack", InputActionType.Button);
        attackAction.AddBinding("<Keyboard>/j");
        attackAction.AddBinding("<Mouse>/leftButton");
        attackAction.AddBinding("<Gamepad>/buttonWest");
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        attackAction.Enable();
        health.Hurt += OnHurt;
        health.Died += OnDied;
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        attackAction.Disable();
        health.Hurt -= OnHurt;
        health.Died -= OnDied;
    }

    private void OnDestroy()
    {
        moveAction.Dispose();
        jumpAction.Dispose();
        attackAction.Dispose();
    }

    private void Start() => Machine.Initialize(IdleState);

    private void Update()
    {
        CheckGround();
        UpdateTimers();
        ReadInput();
        FlipVisual();
        Machine.Tick();
        TestKeys();
    }

    private void FixedUpdate() => Machine.FixedTick();

    // Los eventos de Health cambian el estado: el controller solo traduce "me golpearon" a un estado.
    private void OnHurt(Vector2 source)
    {
        lastHitSource = source;
        Machine.ChangeState(HurtState);
    }

    private void OnDied() => Machine.ChangeState(DeathState);

    public void Move(float input) =>
        Rb.linearVelocity = new Vector2(input * moveSpeed, Rb.linearVelocity.y);

    public void PerformJump()
    {
        Rb.gravityScale = gravityScale;
        Rb.linearVelocity = new Vector2(Rb.linearVelocity.x, jumpVelocity);
        coyoteCounter = 0f;
        jumpBufferCounter = 0f;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayJump();
    }

    public void CutJump() =>
        Rb.linearVelocity = new Vector2(Rb.linearVelocity.x, Rb.linearVelocity.y * jumpCutFactor);

    public void SetGravity(float scale) => Rb.gravityScale = scale;

    public void PlayAnimation(string clipName) => animator.Play(clipName);

    // Empuja al héroe en sentido contrario al golpe y un poco hacia arriba.
    public void Knockback()
    {
        float dir = Mathf.Sign(transform.position.x - lastHitSource.x);
        if (dir == 0f) dir = 1f;
        Rb.linearVelocity = new Vector2(dir * knockbackForce, knockbackForce);
    }

    private void CheckGround()
    {
        bool touching = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer) != null;
        IsGrounded = touching && Rb.linearVelocity.y <= 0.01f;
    }

    private void UpdateTimers()
    {
        coyoteCounter = IsGrounded ? coyoteTime : coyoteCounter - Time.deltaTime;
        jumpBufferCounter -= Time.deltaTime;
    }

    private void ReadInput()
    {
        MoveInput = moveAction.ReadValue<float>();
        if (jumpAction.WasPressedThisFrame()) jumpBufferCounter = jumpBufferTime;
        JumpReleased = jumpAction.WasReleasedThisFrame();
        AttackPressed = attackAction.WasPressedThisFrame();
    }

    private void FlipVisual()
    {
        if (Mathf.Abs(MoveInput) < 0.01f) return;
        Vector3 s = visual.localScale;
        s.x = Mathf.Abs(s.x) * Mathf.Sign(MoveInput);
        visual.localScale = s;
    }

    // TEMPORAL para probar: K te hace daño, R reinicia el nivel si moriste. Se borra al final.
    private void TestKeys()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current.kKey.wasPressedThisFrame)
            health.TakeDamage(1, (Vector2)transform.position + Vector2.left);
        if (health.IsDead && Keyboard.current.rKey.wasPressedThisFrame)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }
}