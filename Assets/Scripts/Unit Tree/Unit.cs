using System.Collections;
using UnityEngine;
using BehaviourTree;

public class Unit : BehaviourTree.Tree
{
    #region Variables/Properties

    // Do not set any of the data in this Scriptable Object, only get.
    public UnitSO unitData;
    // ---

    public  bool  isDead        { get; private set; }
    public  float health        { get; private set; }
    private float movementSpeed;

    public float MovementSpeed
    {
        get { return movementSpeed; }
        set
        {
            movementSpeed = value;
            OnMovementSpeedChanged?.Invoke(movementSpeed);
        }
    }

    protected SpriteRenderer spriteRenderer;
    private Color defaultColor;

    public    System.Action        OnHealthChanged;
    protected System.Action<float> OnMovementSpeedChanged;

    #endregion

    #region Unity

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
    }

    protected override void Start()
    {
        base.Start();

        health = unitData.health;
        MovementSpeed = unitData.movementSpeed;
    }

    #endregion

    #region Methods

    public virtual bool TakeDamage(float value)
    {
        if (!isDead)
        {
            health -= value;
            
            if (health < 1)
            {
                Die();
                return true;
            }

            OnHealthChanged?.Invoke();
        }
        return false;
    }

    public void Heal(float value)
    {
        if (!isDead)
        {
            health += value;

            if (health >= unitData.health)
            {
                health = unitData.health;
            }

            OnHealthChanged?.Invoke();
        }
    }

    public void BlinkRed(bool returnToDefaultColor = true)
    {
        if (returnToDefaultColor)
        {
            spriteRenderer.color = Color.red;
            StartCoroutine(SetColorDelay(defaultColor, 0.2f));
        } 
        else
        {
            Color priorColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
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
        var slowedMovementSpeed = (unitData.movementSpeed * percent) / 100.0f;
        MovementSpeed = slowedMovementSpeed;
    }

    public virtual void Die()
    {
        isDead = true;

        if (gameObject.CompareTag("Player") == false)
        {
            StartCoroutine(DeathDelay());
        }

        if (gameObject.CompareTag("Player") || gameObject.CompareTag("Farm"))
        {
            GameManager.Instance.LoseGame();
        }
    }

    private IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    protected override Node SetupTree()
    {
        return null;
    }

    #endregion

}