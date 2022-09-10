using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    #region Variables/Properties
    // Do not set any of the data in this Scriptable Object, only get.
    public UnitSO unitData;

    public bool  isDead        { get; private set; }
    public float health        { get; private set; }
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

    private SpriteRenderer spriteRenderer;
    private Color          defaultColor;

    public System.Action OnHealthChanged;
    protected System.Action<float> OnMovementSpeedChanged;

    
    #endregion


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
    }

    protected virtual void Start()
    {
        health = unitData.health;
        movementSpeed = unitData.movementSpeed;
    }


    #region Health
    public void TakeDamage(float amount)
    {
        if (!isDead)
        {
            health -= amount;
            
            if (health < 1)
            {
                Die();
            }

            OnHealthChanged?.Invoke();
        }
    }

    public void Heal(float mount)
    {
        if (!isDead)
        {
            health += mount;

            if (health >= unitData.health)
            {
                health = unitData.health;
            }

            OnHealthChanged?.Invoke();
        }
    }

    public void BlinkRed()
    {
        spriteRenderer.color = Color.red;
        Invoke("SetDefaultColor", 0.2f);
    }

    private void SetDefaultColor()
    {
        spriteRenderer.color = defaultColor;
    }
    #endregion

    #region MovementSpeed

    public void SetMovementSpeedByPct(float percent)
    {
        var slowedMovementSpeed = (unitData.movementSpeed * percent) / 100.0f;
        movementSpeed = slowedMovementSpeed;
    }

    #endregion

    #region Die
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
    #endregion

}
