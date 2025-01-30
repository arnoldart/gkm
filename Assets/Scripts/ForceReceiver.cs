using System;
using UnityEngine;

public class ForceReceiver : MonoBehaviour
{
    private CharacterController Controller;

    private float verticalVelocity;

    public Vector3 Movement => Vector3.up * verticalVelocity;

    private void Start()
    {
        Controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (verticalVelocity <= 0f && Controller.isGrounded)
        {
            verticalVelocity = Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }
    }

    public void Jump(float jumpForce)
    {
        verticalVelocity += jumpForce;
    }
}
