using System;
using UnityEngine;

public class InputController : MonoBehaviour 
{

    public Action OnMouse0;
    public Action OnMouse1;
    public Action OnKeyEscape;
    public Action OnKeyT;
    public Action OnKeyR;
    public Action OnKeyV;

    public Action OnCancel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnMouse0?.Invoke();   
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            OnMouse1?.Invoke();
            OnCancel?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnKeyEscape?.Invoke();
            OnCancel?.Invoke();
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
