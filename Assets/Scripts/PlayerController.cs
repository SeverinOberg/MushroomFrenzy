using UnityEngine;

public class PlayerController : MonoBehaviour 
{

    private float horizontalInput;
    private float verticalInput;

    private Vector2 movement;

    private float speed = 7;

    private void Update()
    {

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        movement = new Vector2(horizontalInput, verticalInput).normalized;

        transform.Translate(movement * speed * Time.deltaTime);
    }

}
