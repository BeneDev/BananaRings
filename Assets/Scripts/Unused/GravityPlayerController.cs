﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPlayerController : MonoBehaviour {

    #region Properties

    public Vector3 ToPlanet
    {
        get
        {
            return toPlanet;
        }
    }

    #endregion

    #region Fields

    [Header("Physics"), SerializeField] float gravity = 1f;
    [SerializeField] float gravityCap = 10f;
    [SerializeField] float hoverDistance = 1f;
    [SerializeField] float upwardsVeloCap = 0.002f;

    Vector3 velocity;

    RaycastHit groundRay;
    Vector3 toPlanet;

    CubeMoverTest playerController;

    LayerMask groundLayer;

    #endregion

    #region Unity Messages

    private void Awake()
    {
        // Create the ground layer mask
        int layer = LayerMask.NameToLayer("Ground");
        groundLayer = 1 << layer;

        playerController = GetComponent<CubeMoverTest>();
    }

    void Start()
    {
		
    }

    void Update()
    {
		
    }

    private void FixedUpdate()
    {
        Physics.Raycast(transform.position + new Vector3(0f, -0.5f, 2.5f), transform.forward, out groundRay, 100f, groundLayer);
        ManageGravity();
        transform.position += velocity;
    }

    #endregion

    #region Private Methods

    private void ManageGravity()
    {
        if (groundRay.collider != null)
        {
            toPlanet = groundRay.collider.gameObject.transform.position - transform.position;
            //transform.forward = toPlanet;
            Quaternion lookRotation = new Quaternion();
            lookRotation.SetLookRotation(toPlanet, playerController.Direction);
            transform.rotation = lookRotation;
            if (groundRay.distance > hoverDistance +1)
            {
                if (velocity.y < gravityCap)
                {
                    velocity += toPlanet.normalized * gravity * Time.fixedDeltaTime;
                }
            }
            else if (groundRay.distance < hoverDistance -1)
            {
                if (velocity.y < upwardsVeloCap)
                {
                    velocity += -toPlanet.normalized * Time.fixedDeltaTime;
                }
            }
            else
            {
                velocity = Vector3.zero;
            }
        }
    }

    #endregion
}
