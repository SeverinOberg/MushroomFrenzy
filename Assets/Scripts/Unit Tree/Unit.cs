using System;
using System.Collections;
using UnityEngine;
using BehaviorDesigner.Runtime;

[Serializable]
public class Unit : MonoBehaviour
{

    #region Variables/Properties
    public enum UnitTypes
    {
        Default,
        Turret,
        Player,
        Enemy,
        Nest,
        Mushroom,
        Base,
        Obstacle,
    }

    public UnitTypes type;

    // DO NOT set any of the data in this Scriptable Object, only get.
    public UnitSO unitData;
    // ---

    [SerializeField] protected SpriteRenderer spriteRenderer;
    protected Rigidbody2D rb;

    private bool  isDead;
    private float health;
    private float maxHealth;
    private float movementSpeed;

    public bool  IsDead        { get { return isDead; }        set { if (value == false) { Debug.LogWarning("IsDead must only be true"); return; } isDead = value; OnSetIsDead?.Invoke(); } }
    public float Health        { get { return health; }        set { health        = value; OnSetHealth?.Invoke(); } }
    public float MaxHealth     { get { return maxHealth; }     set { maxHealth     = value; OnSetMaxHealth?.Invoke(); } }
    public float MovementSpeed { get { return movementSpeed; } set { movementSpeed = value; OnSetMovementSpeed?.Invoke(value); } }

    public    Action        OnSetIsDead;
    public    Action        OnSetHealth;
    public    Action        OnSetMaxHealth;
    protected Action<float> OnSetMovementSpeed;

    private Color         defaultColor;
    private Coroutine     doClearSlow;
    #endregion

    #region Unity
    protected virtual void Awake()
    {
        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();

        defaultColor = spriteRenderer.color;
    }

    protected virtual void Start()
    {
        MaxHealth     = unitData.health;
        Health        = MaxHealth;
        MovementSpeed = unitData.movementSpeed;
    }
    #endregion

    #region Methods
    public virtual bool TakeDamage(Unit instigator, float value)
    {
        if (!IsDead)
        {
            if (Health - value <= 0)
            {
                Health = 0;
                Die();
                return true;
            }

            Health -= value;
        }
        return false;
    }

    public virtual bool TakeDamage(float value)
    {
        if (!IsDead)
        {
            if (Health - value <= 0)
            {
                Health = 0;
                Die();
                return true;
            }

            Health -= value;
        }
        return false;
    }

    public void Heal(float value)
    {
        if (!IsDead)
        {
            Health += value;

            if (Health + value > MaxHealth)
            {
                Health = MaxHealth;
            }
        }
    }

    // Force reduce velocity to keep Unit from gliding
    protected void ForceReduceVelocity()
    {
        if (rb.velocity.normalized != Vector2.zero)
        {
            rb.velocity = rb.velocity * 0.95f;
        }
    }

    public void AddForce(Vector2 direction, float forceMultiplier)
    {
        if (!rb) return;
        rb.AddForce(direction * forceMultiplier, ForceMode2D.Impulse);
    }

    public void Blink(Color color, bool returnToDefaultColor = true)
    {
        if (returnToDefaultColor)
        {
            spriteRenderer.color = color;
            StartCoroutine(SetColorDelay(defaultColor, 0.2f));
        } 
        else
        {
            Color priorColor = spriteRenderer.color;
            spriteRenderer.color = color;
            StartCoroutine(SetColorDelay(priorColor, 0.2f));
        }
    }

    public IEnumerator SetColorDelay(Color color, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        spriteRenderer.color = color;
    }

    public void SetMovementSpeedByPct(float percent)
    {
        if (MovementSpeed < unitData.movementSpeed)
            return;

        if (percent == 100 || percent == 0)
        {
            MovementSpeed = 0;
        }
        else
        {
            MovementSpeed = (unitData.movementSpeed * percent) / 100.0f;
        }
    }

    public void SetMovementSpeedByPct(float percent, float slowDuration)
    {
        if (MovementSpeed < unitData.movementSpeed) 
            return;

        if (percent == 100 || percent == 0)
        {
            MovementSpeed = 0;
        }
        else
        {
            MovementSpeed = (unitData.movementSpeed * percent) / 100.0f;
        }

        if (doClearSlow != null)
        {
            StopCoroutine(doClearSlow);
        }
        doClearSlow = StartCoroutine(DoClearSlow(slowDuration));
    }

    private IEnumerator DoClearSlow(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        MovementSpeed = unitData.movementSpeed;
    }

    public virtual void Die(float deathDelaySeconds = 3)
    {
        IsDead = true;

        if (type != UnitTypes.Player)
        {
            StartCoroutine(DeathDelay(deathDelaySeconds));
        }

        if (CompareTag("Player") || CompareTag("Farm"))
        {
            GameManager.Instance.LoseGame();
        }
    }

    private IEnumerator DeathDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
    #endregion

}

[Serializable]
public class SharedUnit : SharedVariable<Unit>
{
    public static implicit operator SharedUnit(Unit value) { return new SharedUnit { Value = value }; }
}
