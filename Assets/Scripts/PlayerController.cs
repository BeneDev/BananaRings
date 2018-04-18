﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This script Controls the Player, reading the Input from a PlayerInput.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{

    #region Properties

    // The velocity of the Rigidbody
    public Vector3 Velocity
    {
        get
        {
            return rb.velocity;
        }
    }

    // The direction to shoot in. Uses the input from the right analog stick.
    public Vector3 ShootDirection
    {
        get
        {
            return shootDirection;
        }
    }

    #endregion

    #region Fields

    // Components attached to the player
    PlayerInput input;
    Rigidbody rb;

    // Boolean Fields
    bool bBoostMode = false;

    [Header("Movement"), SerializeField] float speed = 1f; // The speed, the player moves with
    [SerializeField] float boostAmount = 2.5f; // How much faster the player goes when boosting
    [SerializeField] float veloCap = 300f; // How fast the player can possibly go
    Vector3 direction; // Stores the direction, the player wants to move in
    [SerializeField] float rotationSpeed = 1f; // How fast the player can turn
    
    [Header("Shooting"), SerializeField] Transform[] guns; // The transforms of the different guns to shoot out of
    [SerializeField] GameObject gunObject; // The gun object which gets rotated depending on where the player aims
    [SerializeField] GameObject bolt; // The object which gets shot
    [SerializeField] float shotCooldown = 0.5f; // The cooldown between the different shots
    float shotCounter = 0f; // The counter to tick down between the shots. Only if this one is zero, the player can shoot
    Vector3 shootDirection; // The direction, the player aims at

    // Attributes of the particle effect
    // TODO store the different trailEffects in an array as well as the modules
    [Header("ParticleEffect"), SerializeField] ParticleSystem trailEffect;
    [SerializeField] ParticleSystem secondTrail;
    ParticleSystem.MainModule main;
    ParticleSystem.EmissionModule emission;
    ParticleSystem.MainModule secMain;
    ParticleSystem.EmissionModule secEmission;
    // TODO get the default size from the particle system itself
    [SerializeField] float defaultSize;
    [SerializeField] float defaultRate;

    #endregion

    #region UnityMessages

    private void Awake()
    {
        // Get the references of the Particle Systems for the thrust.
        emission = trailEffect.emission;
        main = trailEffect.main;
        secMain = secondTrail.main;
        secEmission = secondTrail.emission;

        // Get the player components needed.
        input = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        ManageBoostTrail();
    }

    private void FixedUpdate()
    {
        ReadInput();

        // Shoot if the cooldown is worn off
        if(shotCounter > 0)
        {
            shotCounter -= Time.fixedDeltaTime;
        }

        Thrust(bBoostMode);
    }

    #endregion

    #region Methods

    // Make the player move and change the thrust particle system depending on wether the player boosts or not
    void Thrust(bool mode)
    {
        // When in boost mode
        if (mode)
        {
            // Add velocity as long as the velocity Cap is not reached
            if (rb.velocity.magnitude < veloCap)
            {
                rb.velocity += direction * (speed + input.Boost * boostAmount) * Time.fixedDeltaTime;
            }
            // Set the trail particle effects to emit bigger particles faster
            main.startSizeMultiplier = 8f;
            emission.rateOverDistance = 3f;
            secMain.startSizeMultiplier = 8f;
            secEmission.rateOverDistance = 3f;
        }
        else
        {
            // Add velocity as long as the velocity Cap is not reached
            if (rb.velocity.magnitude < veloCap * 0.125f)
            {
                rb.velocity += direction * speed * Time.fixedDeltaTime;
            }
            // Set the trail particle effect values back to the default ones
            main.startSizeMultiplier = defaultSize;
            emission.rateOverDistance = defaultRate;
            secMain.startSizeMultiplier = defaultSize;
            secEmission.rateOverDistance = defaultRate;
        }
    }

    // Set the rotation of the trail particle effect
    private void ManageBoostTrail()
    {
        Quaternion trailRotation = Quaternion.LookRotation(-transform.forward);
        secondTrail.transform.rotation = trailRotation;
        trailEffect.transform.rotation = trailRotation;
    }

    // Handles the input to manipulate the player.
    void ReadInput()
    {
        // Get the direction of the left stick
        direction.x = input.Horizontal;
        direction.z = input.Vertical;

        // Get the direction of the right stick
        shootDirection.x = input.RightHorizontal;
        shootDirection.z = input.RightVertical;

        // Set boostMode field
        if (input.Boost > 0f)
        {
            bBoostMode = true;
        }
        else
        {
            bBoostMode = false;
        }

        // Rotate the player smoothly, depending on the velocity
        if (input.Horizontal != 0 || input.Vertical != 0)
        {
            Quaternion targetRotation = new Quaternion();
            targetRotation.SetLookRotation(direction); 

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // Rotate the guns on the ship depending on the input of the right stick
        if (input.RightHorizontal != 0 || input.RightVertical != 0 && gunObject)
        {
            gunObject.transform.forward = shootDirection;
        }

        // Instantiates a bullet when the player shoots
        if (input.Shoot)
        {
            if (guns.Length > 0 && bolt && shotCounter <= 0f)
            {
                foreach (Transform gun in guns)
                {
                    Instantiate(bolt, gun.position, gunObject.transform.rotation);
                }
                shotCounter = shotCooldown;
            }
        }
    }

    #endregion

}

