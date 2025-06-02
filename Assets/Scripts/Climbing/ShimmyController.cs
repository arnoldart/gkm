using UnityEngine;

public class ShimmyController : MonoBehaviour
{
    private PlayerClimb playerClimb;

    public float sphereRadius;
    public float sphereGap;

    public float rayHeight = 1.6f;
    public float rayLength = 1.0f;

    public bool canMoveLeft;
    public bool canMoveRight;

    public bool leftBtn;
    public bool rightBtn;
    public float ledgeMoveSpeed = 0.5f;
    public float horizontalInput;
    public float horizontalValue;

    public RaycastHit ledgeHit;

    void Start()
    {
        playerClimb = GetComponent<PlayerClimb>();
    }

    void Update()
    {
        while (playerClimb.isClimbing)
        {
            Debug.DrawRay(transform.position + Vector3.up * rayHeight, transform.forward * rayLength, Color.magenta);

            Physics.Raycast(transform.position + Vector3.up * rayHeight, transform.forward, out ledgeHit, rayLength, playerClimb.ledgeLayer);

            CheckSphere();

            break;
        }
    }

    void CheckSphere()
    {
        if (ledgeHit.point != Vector3.zero)
        {
            // Right Hand Sphere check if it still ledge to move
            if (Physics.CheckSphere(ledgeHit.point + transform.right * sphereGap, sphereRadius, playerClimb.ledgeLayer))
            {
                canMoveRight = true;

                rightBtn = Input.GetKey(KeyCode.D);
            }
            else 
            {
                rightBtn = false;

                leftBtn = Input.GetKey(KeyCode.A);
                canMoveRight = false;
            }

            // Left Hand Sphere check if it still ledge to move
            if (Physics.CheckSphere(ledgeHit.point - transform.right * sphereGap, sphereRadius, playerClimb.ledgeLayer))
            {
                canMoveLeft = true;

                leftBtn = Input.GetKey(KeyCode.A);
            }
            else
            {
                leftBtn = false;
                canMoveLeft = false;
                rightBtn = Input.GetKey(KeyCode.D);
            }
        }

        // Horizontal Value
        if (leftBtn)
        {
            horizontalValue = -1;
        }
        else if (rightBtn)
        {
            horizontalValue = 1;
        }
        else
        {
            horizontalValue = 0;
        }

        playerClimb.animator.SetFloat("ledgeSpeed", horizontalValue, 0.05f, Time.deltaTime);
        transform.position += transform.right * horizontalValue * ledgeMoveSpeed * Time.deltaTime;
    }

    // private void CheckSphere()
    // {
    //     if (ledgeHit.point != Vector3.zero)
    //     {
    //         if (Physics.CheckSphere(ledgeHit.point + transform.right * sphereGap, sphereRadius, playerClimb.ledgeLayer))
    //         {
    //             canMoveRight = true;
    //             rightBtn = Input.GetKey(KeyCode.RightArrow);
    //         }
    //         else
    //         {
    //             rightBtn = false;
    //             canMoveRight = false;
    //             leftBtn = Input.GetKey(KeyCode.LeftArrow);
    //         }

    //         if (Physics.CheckSphere(ledgeHit.point - transform.right * sphereGap, sphereRadius, playerClimb.ledgeLayer))
    //         {
    //             canMoveLeft = true;
    //             leftBtn = Input.GetKey(KeyCode.LeftArrow);
    //         }
    //         else
    //         {
    //             canMoveLeft = false;
    //             rightBtn = Input.GetKey(KeyCode.RightArrow);
    //         }
    //     }

    //     if (leftBtn)
    //     {
    //         horizontalValue = -1f;
    //     }
    //     else if (rightBtn)
    //     {
    //         horizontalValue = 1f;
    //     }
    //     else
    //     {
    //         horizontalValue = 0f;
    //     }

    //     transform.position += transform.right * horizontalValue * ledgeMoveSpeed * Time.deltaTime;
    // }

    void OnDrawGizmos()
    {
        if(ledgeHit.point != Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(ledgeHit.point + transform.right * sphereGap, sphereRadius);
            Gizmos.DrawSphere(ledgeHit.point - transform.right * sphereGap, sphereRadius);
        }
    }
}
