using System;
using UnityEngine;

public class InputController : MonoBehaviour 
{

    public Action OnMouse0;
    public Action OnKeyG;
    public Action OnKeyT;
    public Action OnKeyR;
    public Action OnKeyV;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnMouse0?.Invoke();   
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            OnKeyG?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            OnKeyT?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            OnKeyR?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            OnKeyV?.Invoke();
        }
    }

}
