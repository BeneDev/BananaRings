using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour, IInput {

    // Prevents the controller from reading tiny input, caused by old sticks
    [Range(0, 1)] [SerializeField] float controllerThreshhold;

    // The input for horizontal movement
    public float Horizontal
    {
        get
        {
            if (Input.GetAxis("Horizontal") >= controllerThreshhold || Input.GetAxis("Horizontal") <= -controllerThreshhold)
            {
                return Input.GetAxis("Horizontal");
            }
            return 0f;
        }
    }

    // The input for vertical movement
    public float Vertical
    {
        get
        {
            if (Input.GetAxis("Vertical") >= controllerThreshhold || Input.GetAxis("Vertical") <= -controllerThreshhold)
            {
                return Input.GetAxis("Vertical");
            }
            return 0f;
        }
    }

    public float Boost
    {
        get
        {
            return Input.GetAxis("Boost");
        }
    }

    public bool Shoot
    {
        get
        {
            if (Input.GetAxisRaw("Shoot") == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
