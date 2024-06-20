using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Camera cam;
    [SerializeField] private float speed;
    [SerializeField] private float sprintmultiplier;
    [SerializeField] private float gravity = 9.807f;
    [SerializeField] private float jumpStrenght;
    [SerializeField] private float acceleration;
    [SerializeField] private LayerMask player;
    [SerializeField] private float upwardsVelocity;
    private Vector3 rotation;
    private Vector3 velocity;
    private Vector2 targetMove;
    private Vector2 currentMove;
    private float sinTime;
    [SerializeField] private bool grounded;
    [SerializeField] private bool jumping;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //rotation

        //get mouse locations for rotation
        float mouseX = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        //set valeus and clamp
        rotation.x -= mouseX;
        rotation.x = Mathf.Clamp (rotation.x, -90, 90);
        rotation.y += mouseY;
        //apply valeus
        cam.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
        transform.rotation = Quaternion.Euler(0, rotation.y, 0);
        //rotation

        //movement
        if(Physics.CheckSphere(transform.position - transform.up * 0.65f, 0.45f,player) && !jumping)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

        if (!grounded)
        {
            upwardsVelocity -= gravity * Time.deltaTime;
        }
        else 
        {
            upwardsVelocity = 0;
        }



        //get input for movement
        targetMove = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetMove = targetMove.normalized;
        //set input to velocity

        if (Input.GetKey(KeyCode.LeftShift))
        {
            targetMove *= sprintmultiplier;
        }

        if(targetMove != currentMove)
        {
            sinTime += Time.deltaTime * acceleration;
            sinTime = Mathf.Clamp(sinTime, 0, Mathf.PI);
            float t = evaluate(sinTime);
            currentMove = Vector3.Lerp(currentMove, targetMove, t);
        }
        if (targetMove == currentMove)
        {
            sinTime = 0;
        }
        velocity = transform.up * (upwardsVelocity * Time.deltaTime) + transform.forward * (currentMove.y * speed * Time.deltaTime) +  transform.right * (currentMove.x * speed * Time.deltaTime);

                
        if(Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            upwardsVelocity = jumpStrenght;
            StartCoroutine(jump());
        }
        if (Input.GetKeyUp(KeyCode.Space) && upwardsVelocity > 0)
        {
            upwardsVelocity /= 2;
        }
        //set movement to velocity
        controller.Move(velocity);
    }
    
    public float evaluate(float x)
    {
        return 0.5f * Mathf.Sin(x - Mathf.PI / 2) + 0.5f;
    }
    IEnumerator jump()
    {
        jumping = true;
        yield return new WaitForSeconds(0.1f);
        jumping = false;
    }
}