using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCon : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public Camera playerCamera;
    public GameObject projectilePrefab;

    float gunheat;
    const float fireRate = 0.9f;

    public AudioSource footstepsSound;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();

        //Handle Drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        if (gunheat > 0) gunheat -= Time.deltaTime;

        if (Input.GetButtonDown("Fire2"))
        {
            if (gunheat <= 0)
            {
                GameObject projectileObject = Instantiate (projectilePrefab);
                projectileObject.transform.position = playerCamera.transform.position + playerCamera.transform.forward;
                projectileObject.transform.forward = playerCamera.transform.forward;
                gunheat = fireRate;
            }
        }

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
                footstepsSound.enabled = true;
        }
        else
        {
            footstepsSound.enabled = false;
        }

    /*if (Input.GetKeyDown(KeyCode.Escape))
    {
        Application.Quit();
    }*/
}

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        // limit Velocity
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Macguffin"))
        {
            SceneManager.LoadScene(3);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            SceneManager.LoadScene(2);
        }
    }
}