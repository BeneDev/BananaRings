﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement"), SerializeField] float speed = 1f;
    [SerializeField] float boostAmount = 2.5f;
    Vector3 velocity;

    PlayerInput input;

    [Header("Physics"), SerializeField] float lookDistance = 2f;
    LayerMask obstacles;
    [SerializeField] float colliderRadius = 1f;

    Rigidbody rb;

    // Attributes of the particle effect
    [Header("ParticleEffect"), SerializeField] ParticleSystem trailEffect;
    [SerializeField] ParticleSystem secondTrail;
    ParticleSystem.MainModule main;
    ParticleSystem.EmissionModule emission;
    ParticleSystem.MainModule secMain;
    ParticleSystem.EmissionModule secEmission;
    [SerializeField] float defaultSize;
    [SerializeField] float defaultRate;

    bool bBoostMode = false;

    private void Awake()
    {
        emission = trailEffect.emission;
        main = trailEffect.main;
        secMain = secondTrail.main;
        secEmission = secondTrail.emission;
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        int layer = LayerMask.NameToLayer("Obstacles");
        obstacles = 1 << layer;
    }

    // Update is called once per frame
    void Update()
    {
        ManageBoostTrail();
    }

    private void FixedUpdate()
    {
        ReadInput();

        Thrust(bBoostMode);
    }

    void Thrust(bool mode)
    {
        if (mode)
        {
            transform.position += velocity * (speed + input.Boost * boostAmount) * Time.fixedDeltaTime;
            //rb.velocity = velocity * (speed + input.Boost * boostAmount) * Time.fixedDeltaTime;
            main.startSizeMultiplier = 8f;
            emission.rateOverDistance = 3f;
            secMain.startSizeMultiplier = 8f;
            secEmission.rateOverDistance = 3f;
        }
        else
        {
            transform.position += velocity * speed * Time.fixedDeltaTime;
            //rb.velocity = velocity * speed * Time.fixedDeltaTime;
            main.startSizeMultiplier = defaultSize;
            emission.rateOverDistance = defaultRate;
            secMain.startSizeMultiplier = defaultSize;
            secEmission.rateOverDistance = defaultRate;
        }
    }

    // Sets the rotation of the trail particle effect
    private void ManageBoostTrail()
    {
        Quaternion trailRotation = Quaternion.LookRotation(-transform.forward);
        secondTrail.transform.rotation = trailRotation;
        trailEffect.transform.rotation = trailRotation;
    }

    void ReadInput()
    {
        velocity.x = input.Horizontal;
        velocity.z = input.Vertical;

        if (input.Boost > 0f)
        {
            bBoostMode = true;
        }
        else
        {
            bBoostMode = false;
        }

        if (input.Horizontal != 0 || input.Vertical != 0)
        {
            transform.forward = velocity;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, colliderRadius);
    }
}

