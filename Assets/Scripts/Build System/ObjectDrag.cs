using System;
using System.Collections;
using UnityEngine;

public class ObjectDrag : MonoBehaviour 
{
    // Gets set when Initialized in BuildingSystem.
    public Turret turret;
    // ---

    private SpriteRenderer _highlightSprite;
    private BoxCollider2D _collider;

    private bool _buildMode;

    private Vector2 _mousePosition;

    private void Awake()
    {
        _highlightSprite = GetComponentsInChildren<SpriteRenderer>()[0];

       _collider = GetComponent<BoxCollider2D>();

        
    }

    private void Start()
    {
        turret.isSleeping = true;
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && _buildMode)
        {
            Destroy(gameObject);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && _buildMode)
        {
            transform.position = BuildingSystem.current.SnapCoordinateToGrid(_mousePosition);

            Collider2D hit = Physics2D.OverlapBox(transform.position, transform.localScale, 0.0f);
            if (hit)
            {
                StartCoroutine(Highlight());
                return;
            }

            turret.isSleeping = false;
            SetBuildMode(false);
            ResourceManager.Instance.Wood -= turret.turretSO.woodCost;
            ResourceManager.Instance.Stone -= turret.turretSO.stoneCost;
        }

        if (_buildMode)
        {
            _mousePosition = Utilities.GetMouseWorldPosition();
            transform.position = Vector3.Lerp(transform.position, BuildingSystem.current.SnapCoordinateToGrid(_mousePosition), Time.deltaTime * 15);
        } 
    }

    private IEnumerator Highlight()
    {
        _highlightSprite.enabled = true;
        yield return new WaitForSeconds(0.2f);
        _highlightSprite.enabled = false;
    }

    public void SetBuildMode(bool boolean)
    {
        _buildMode = boolean;
        if (_buildMode)
        {
            _collider.enabled = false;
        }
        else
        {
            _collider.enabled = true;
        }
    }

}
