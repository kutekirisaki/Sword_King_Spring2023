using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool canJump = true;
    private Animator controller;
    private int facingRight = -1;
    /* PREFABS */
    [SerializeField]
    private GameObject swordCollider;
    /* EDITABLE VARIABLES */
    [SerializeField]
    public float jumpVelocity = 5f;
    [SerializeField]
    public float movementSpeed = 5f;
    private float slashDuration = 0.3f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rb.velocity = Vector2.up * jumpVelocity;
            canJump = false;
        }
       
        if (Input.GetKey(KeyCode.Mouse0))
        {
            controller.Play("Slash");
            controller.SetBool("isHitting", true);
            StartCoroutine(Slash());
        }

        CalculateMovement();
    }
        
    IEnumerator Slash()
    {
        swordCollider.SetActive(true);
        float timeElapsed = 0;
        Quaternion startRotation = swordCollider.transform.rotation;
        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 0, facingRight * 120);
   
        while (timeElapsed < slashDuration)
        {
            swordCollider.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / slashDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        swordCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
        controller.SetBool("isHitting", false);
        swordCollider.SetActive(false);
    }


    private void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        transform.Translate(direction * movementSpeed * Time.deltaTime);

        if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            controller.SetBool("isMoving", true);
            facingRight = -1;
        } else if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            controller.SetBool("isMoving", true);
            facingRight = 1;
        } else
        {
            controller.SetBool("isMoving", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {   
        if (other.transform.tag == "Floor")
        {
            canJump = true;
        }
        
        if (other.transform.tag == "Spikes")
        {
            Debug.Log("killed");
            Destroy(this.gameObject);
        }
    }
}
