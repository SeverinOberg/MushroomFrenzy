using System.Collections;
using UnityEngine;
using BehaviourTree;

public class Unit : BehaviourTree.Tree
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

    // Do not set any of the data in this Scriptable Object, only get.
    public UnitSO unitData;
    // ---

    public  bool  isDead        { get; private set; }
    public  float health        { get; private set; }
    public  float maxHealth     { get; set; }
    private float movementSpeed;

    public float MovementSpeed
    {
        get { return movementSpeed; }
        set
        {
            movementSpeed = value;
            OnMovementSpeedChanged?.Invoke(value);
        }
    }

    [SerializeField] protected SpriteRenderer spriteRenderer;
    private Color defaultColor;

    public    System.Action        OnHealthChanged;
    protected System.Action<float> OnMovementSpeedChanged;

    #endregion

    #region Unity

    protected virtual void Awake()
    {
        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();

        defaultColor = spriteRenderer.color;
    }

    protected override void Start()
    {
        base.Start();
        
        maxHealth = unitData.health;
        health = maxHealth;
        MovementSpeed = unitData.movementSpeed;
    }

    #endregion

    #region Methods

    public virtual bool TakeDamage(float value)
    {
        if (!isDead)
        {
            health -= value;
            OnHealthChanged?.Invoke();

            if (health < 1)
            {
                Die();
                OnHealthChanged?.Invoke();
                return true;
            }
        }
        return false;
    }

    public void Heal(float value)
    {
        if (!isDead)
        {
            health += value;

            if (health > maxHealth)
            {
                health = maxHealth;
            }

            OnHealthChanged?.Invoke();
        }
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

    Coroutine doClearSlow;

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

    private IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    public virtual void Die()
    {
        isDead = true;

        if (type != UnitTypes.Player && type != UnitTypes.Nest)
        {
            StartCoroutine(DeathDelay());
        }

        if (CompareTag("Player") || CompareTag("Farm"))
        {
            GameManager.Instance.LoseGame();
        }
    }

    protected override Node SetupTree()
    {
        return null;
    }

    #endregion

}
