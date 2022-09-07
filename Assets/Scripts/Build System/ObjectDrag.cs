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

    private bool _dragMode;

    private Vector2 _mousePosition;

    private void Awake()
    {

        _highlightSprite = GetComponentsInChildren<SpriteRenderer>()[0];

       _collider = GetComponent<BoxCollider2D>();

    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && _dragMode)
        {
            ResourceManager.Instance.Wood += turret.turretSO.woodCost;
            ResourceManager.Instance.Stone += turret.turretSO.stoneCost;
            Destroy(gameObject);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && _dragMode)
        {
            transform.position = BuildingSystem.current.SnapCoordinateToGrid(_mousePosition);

            Collider2D hit = Physics2D.OverlapBox(transform.position, transform.localScale, 0.0f);
            if (hit)
            {
                StartCoroutine(Highlight());
                return;
            }

            turret.isSleeping = false;
            SetDragMode(false);
            Destroy(this);
        }

        if (_dragMode)
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

    public void SetDragMode(bool boolean)
    {
        _dragMode = boolean;
        if (_dragMode)
        {
            _collider.enabled = false;
        }
        else
        {
            _collider.enabled = true;
        }
    }

}
