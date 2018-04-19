using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour, IInput {

    // The input for horizontal movement
    public float Horizontal
    {
        get
        {
            return Input.GetAxis("Horizontal");
            return 0f;
        }
    }

    // The input for vertical movement
    public float Vertical
    {
        get
        {
            return Input.GetAxis("Vertical");
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

    // The input for horizontal movement
    public float RightHorizontal
    {
        get
        {
            return Input.GetAxis("RightHorizontal");
            return 0f;
        }
    }

    // The input for vertical movement
    public float RightVertical
    {
        get
        {
            return Input.GetAxis("RightVertical");
            return 0f;
        }
    }

}
