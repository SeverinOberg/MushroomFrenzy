using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour 
{
    protected Unit unit;

    public bool isDead { get; private set; }

    // Backing fields
    private float health = 1;
    private string title = "Unit";
    private int faction = 0;
    private float movementSpeed = 2;
    // ---

    protected System.Action<Unit, float> OnMovementSpeedChanged;
    public float InitialMovementSpeed { get; private set; }
    
    private void Awake()
    {
        unit = this;
        InitialMovementSpeed = movementSpeed;
    }

    protected float Health { get => health; set => health = value; }

    protected string Title
    {
        get { return title; }
        set
        {
            if (value == "")
            {
                Debug.LogError("Title value can not be empty");
                return;
            }
            else
            {
                title = value;
            }
        }
    }
    
    public int Faction { 
        get { return faction; } 
        set
        {
            if (value < 0)
            {
                Debug.LogError("Faction can not be less than 0");
                return;
            }
            else
            {
                faction = value;
            }
        }
    }

    public float MovementSpeed
    {
        get { return movementSpeed; }
        set
        {
            if (value < 0)
            {
                movementSpeed = 0;
                return;
            }

            movementSpeed = value;
            OnMovementSpeedChanged?.Invoke(this, movementSpeed);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (!isDead)
        {
            health -= damageAmount;

            if (health < 1)
            {
                Die();
            }
            //Debug.Log(title + " took " + damageAmount + " damage. Has " + health + " left.");
        }
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

}
