using System;
using System.Collections;
using UnityEngine;
using BehaviorDesigner.Runtime;

[Serializable]
public class Unit : MonoBehaviour, IAttack
{
    public enum UnitTypes
    {
        Default,
        Base,
        Player,
        Merchant,
        Turret,
        Defense,
        Generator,
        Enemy,
    }

    [Header("Unit")]
    [SerializeField] private UnitSO         unitSO;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D    _rigidbody;
    
    private bool      isDead;
    private float     health;
    private float     maxHealth;
    private float     movementSpeed;
   
    public SpriteRenderer SpriteRenderer       { get { return spriteRenderer; } } 
    public Rigidbody2D    Rigidbody            { get { return _rigidbody;     } } 

    public GameObject     Prefab               { get { return unitSO.prefab;        } } 
    public Factions       Faction              { get { return unitSO.faction;       } }
    public UnitTypes      UnitType             { get { return unitSO.type;          } } 
    public string         Title                { get { return unitSO.title;         } }
    public string         Description          { get { return unitSO.description;   } }
    public float          DefaultMovementSpeed { get { return unitSO.movementSpeed; } }
    
    public bool  IsDead         { get { return isDead;         } private set { isDead        = value; OnSetIsDead?.Invoke();             } }
    public float Health         { get { return health;         } private set { health        = value; OnSetHealth?.Invoke();             } }
    public float MaxHealth      { get { return maxHealth;      } private set { maxHealth     = value; OnSetMaxHealth?.Invoke();          } }
    public float MovementSpeed  { get { return movementSpeed;  } private set { movementSpeed = value; OnSetMovementSpeed?.Invoke(value); } }
    
    public Action        OnSetIsDead;
    public Action        OnSetHealth;
    public Action        OnSetMaxHealth;
    public Action<float> OnSetMovementSpeed;

    private Coroutine doClearSlow;
    
    protected virtual void Start()
    {
        InjectSOIntoBackingFields();
    }

    private void InjectSOIntoBackingFields()
    {
        MaxHealth     = unitSO.maxHealth;
        Health        = unitSO.maxHealth;
        MovementSpeed = unitSO.movementSpeed;
    }

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
        if (Rigidbody.velocity.normalized != Vector2.zero)
        {
            Rigidbody.velocity = Rigidbody.velocity * 0.95f;
        }
    }

    public void AddForce(Vector2 direction, float forceMultiplier)
    {
        if (!Rigidbody) return;
        Rigidbody.AddForce(direction * forceMultiplier, ForceMode2D.Impulse);
    }

    public void Blink(Color color, float seconds = 0.2f, bool returnToPriorColor = false)
    {
        if (returnToPriorColor)
        {
            Color priorColor = SpriteRenderer.color;
            SpriteRenderer.color = color;
            StartCoroutine(DoSetColorDelay(priorColor, seconds));
        } 
        else
        {
            SpriteRenderer.color = color;
            StartCoroutine(DoSetColorDelay(Color.white, seconds));
        }
    }

    public IEnumerator DoSetColorDelay(Color color, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SpriteRenderer.color = color;
    }

    public void SetMovementSpeedByPct(float percent)
    {
        if (MovementSpeed < DefaultMovementSpeed)
            return;

        if (percent == 100 || percent == 0)
        {
            MovementSpeed = 0;
        }
        else
        {
            MovementSpeed = (DefaultMovementSpeed * percent) / 100.0f;
        }
    }

    public void SetMovementSpeedByPct(float percent, float slowDuration)
    {
        if (MovementSpeed < DefaultMovementSpeed) 
            return;

        if (percent == 100 || percent == 0)
        {
            MovementSpeed = 0;
        }
        else
        {
            MovementSpeed = (MovementSpeed * percent) / 100.0f;
        }

        if (doClearSlow != null)
        {
            StopCoroutine(doClearSlow);
        }
        doClearSlow = StartCoroutine(DoClearSlow(slowDuration));
    }

    public void AddMaxHealth(float value, bool healNewMaxHealth = true)
    {
        MaxHealth += value;
        if (healNewMaxHealth)
        {
            Heal(value);
        }
    }

    // Subtract max health from value. If the value is <= 0, MaxHealth and Health will be set to 1.
    public void SubtractMaxHealth(float value)
    {
        if (MaxHealth + value <= 0)
        {
            MaxHealth = 1;
            Health    = 1;
            return;
        }

        MaxHealth -= value;
        Health    -= value;
    }

    public void SetMovementSpeed(float value)
    {
        MovementSpeed = value;
    }

    public void AddMovementSpeed(float value)
    {
        MovementSpeed += value;
    }

    private IEnumerator DoClearSlow(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        MovementSpeed = DefaultMovementSpeed;
    }

    public virtual void Die(float destroyDelaySeconds = 3)
    {
        IsDead = true;

        if (UnitType != UnitTypes.Player && UnitType != UnitTypes.Base)
        {
            StartCoroutine(DoDestroyDelay(destroyDelaySeconds));
        }
    }

    private IEnumerator DoDestroyDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

    public virtual void Attack() { }
    public virtual bool Attack(Unit target) { return false; }

}

[Serializable]
public class SharedUnit : SharedVariable<Unit>
{
    public static implicit operator SharedUnit(Unit value) { return new SharedUnit { Value = value }; }
}
