using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInput))]
public class CubeMoverTest : MonoBehaviour {

    #region Properties

    public Vector3 Direction
    {
        get
        {
            return direction;
        }
    }

    #endregion

    #region Fields

    [Header("Physics"), SerializeField] float acceleration;
    [SerializeField] float veloCap = 1f;
    [SerializeField] float drag = 1f;
    float velocity;

    Vector3 direction;

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
        //ReadInput();
        //if (rb.velocity.magnitude < veloCap)
        //{
        //    rb.AddRelativeForce(velocity * acceleration * Time.fixedDeltaTime);
        //}
        //transform.Translate(velocity * acceleration * Time.fixedDeltaTime, Space.Self);
        //velocity = transform.InverseTransformDirection(velocity);
        //velocity = Camera.main.transform.TransformVector(velocity);
        // Get the direction of the left stick




        direction.x = input.Horizontal;
        direction.z = input.Vertical;
        if (velocity < veloCap)
        {
            velocity += acceleration * direction.magnitude;
            print(direction.magnitude);
        }
        velocity = velocity * (1 - Time.fixedDeltaTime * drag);
        transform.position += transform.up * velocity * Time.fixedDeltaTime;
        //transform.position += transform.up * acceleration * Time.fixedDeltaTime;

    }

    #endregion

    #region Private Methods

    void ReadInput()
    {
        //if (velocity.magnitude < veloCap)
        //{
        //    // Get the direction of the left stick
        //    direction.x += input.Horizontal;
        //    direction.z += input.Vertical;
        //    velocity = direction * acceleration;
        //}
        // Subtract the drag from the velocity
        //velocity = velocity * (1 - Time.fixedDeltaTime * drag);

        // Rotate the player smoothly, depending on the velocity
        //if (input.Horizontal != 0 || input.Vertical != 0)
        //{
        //    Quaternion targetRotation = new Quaternion();
        //    targetRotation.SetLookRotation(velocity);

        //    print("Rotate");
        //    transform.rotation.SetFromToRotation(transform.forward, velocity.normalized);

        //    //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 1 * Time.fixedDeltaTime);
        //}

    }

    #endregion
}
