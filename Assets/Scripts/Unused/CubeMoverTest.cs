using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInput))]
public class CubeMoverTest : MonoBehaviour {

    #region Properties



    #endregion

    #region Fields

    [Header("Physics"), SerializeField] float acceleration;
    [SerializeField] float veloCap = 1f;
    [SerializeField] float drag = 1f;
    Vector3 velocity;

    PlayerInput input;
    Rigidbody rb;

    #endregion

    #region Unity Messages

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
		
    }

    void Update()
    {
		
    }

    private void FixedUpdate()
    {
        ReadInput();
        //if (rb.velocity.magnitude < veloCap)
        //{
        //    rb.AddRelativeForce(velocity * acceleration * Time.fixedDeltaTime);
        //}
        //transform.Translate(velocity * acceleration * Time.fixedDeltaTime, Space.Self);
        transform.localPosition += velocity * acceleration * Time.fixedDeltaTime;
    }

    #endregion

    #region Private Methods

    void ReadInput()
    {
        if (velocity.magnitude < veloCap)
        {
            // Get the direction of the left stick
            velocity.x += input.Horizontal;
            velocity.z += input.Vertical;
        }
        //else
        //{
        //    velocity.x *= 0.7f;
        //    velocity.z *= 0.7f;
        //}
        velocity = velocity * (1 - Time.fixedDeltaTime * drag);

        // Rotate the player smoothly, depending on the velocity
        if (input.Horizontal != 0 || input.Vertical != 0)
        {
            Quaternion targetRotation = new Quaternion();
            targetRotation.SetLookRotation(velocity);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 1 * Time.fixedDeltaTime);
        }

    }

    #endregion
}
