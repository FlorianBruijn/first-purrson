using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement2 : MonoBehaviour
{
    [SerializeField]private Transform orientation;
    [SerializeField] private Camera cam;
    [SerializeField] float mouseSensitivity;
    [SerializeField] private float Gravity = 9.807f;
    [SerializeField] private float acceleration;
    [SerializeField] float sprintMultiplier;
    [SerializeField] float airMultiplier;
    [SerializeField] float jumpForce;
    [SerializeField] float dashForce;
    [Range(0f, 1f)]
    [SerializeField] float groundDrag;
    [Range(0f, 1f)]
    [SerializeField] float airDrag;
    [SerializeField] float speed;
    [SerializeField] LayerMask ground;
    private Vector3 rotation;
    private bool sprinting;
    Vector2 currentMove;
    private float sinTime;
    [SerializeField] private bool grounded;
    [SerializeField]private Vector3 Velocity;
    private CharacterController controller;
    private void Start() 
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        checkGround();
        moveMouse();
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            switchSprint();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {

        }
        if(grounded)
        {
            if(Velocity.y < 0f)
            {
                dontFall();
            }
            if (sprinting)
            {
                walk(speed * sprintMultiplier, false);
            }
            else
            {
                walk(speed, false);
            }
            if(Input.GetKeyDown(KeyCode.Space))
            {
                jump();
            }
            drag(groundDrag);
        }
        else
        {
            fall();
            drag(airDrag);
            walk(speed * airMultiplier, true);
        }
        if(Inputs().magnitude == 0)
        {
            checkStop();
        }
        move();
    }

    private void dash()
    {
        AddForce(dashForce, orientation.forward);
    }

    private void jump()
    {
        Velocity.x *= 0.5f;
        Velocity.z *= 0.5f;
        AddForce(jumpForce, transform.up);
    }
    public void AddForce(float force, Vector3 Direction)
    {
        Velocity += Direction * force;
    }

    void dontFall()
    {
        Velocity.y = 0;
    }

    void fall()
    {
        Velocity.y -= Gravity * Time.deltaTime;
    }

    private Vector2 Inputs()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
    }

    private void walk(float speed, bool keepOther)
    {
        if(keepOther)
        {
            Velocity += (transform.forward * Inputs().y + transform.right * Inputs().x) * speed * Time.deltaTime;
        }
        else
        {
            if(Inputs() != currentMove)
            {
                sinTime += Time.deltaTime * acceleration;
                sinTime = Mathf.Clamp(sinTime, 0, Mathf.PI);
                float t = evaluate(sinTime);
                currentMove = Vector3.Lerp(currentMove, Inputs(), t);
            }
            if (Inputs() == currentMove)
            {
                sinTime = 0;
            }
            Velocity = (transform.forward * currentMove.y + transform.right * currentMove.x) * speed + transform.up * Velocity.y;
        }
    }

    private void drag(float dragForce)
    {
        Velocity *= Mathf.Pow(dragForce, Time.deltaTime);
    }

    private void checkGround()
    {
        grounded = Physics.CheckSphere(transform.position - transform.up * 0.65f, 0.45f, ground) && Velocity.y <= 0f;
    }

    void move()
    {
        controller.Move(Velocity * Time.deltaTime);
    }

    void switchSprint()
    {
        sprinting = !sprinting;
    }

    void checkStop()
    {
        if(new Vector2(Velocity.x,Velocity.z).magnitude < 1f && new Vector2(Velocity.x,Velocity.z).magnitude > -1f)
        {
            Velocity.x = 0f;
            Velocity.z = 0f;
        }
    }

    void moveMouse()
    {
        float mouseX = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        rotation.x -= mouseX;
        rotation.x = Mathf.Clamp (rotation.x, -90, 90);
        rotation.y += mouseY;
        orientation.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
        cam.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
        transform.rotation = Quaternion.Euler(0, rotation.y, 0);
    }
    public float evaluate(float x)
    {
        return 0.5f * Mathf.Sin(x - Mathf.PI / 2) + 0.5f;
    }

}
