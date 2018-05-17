using System.Collections;
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
    Camera cam;

    // Boolean Fields
    bool bBoostMode = false;

    [Header("Movement"), SerializeField] float speed = 1f; // The speed, the player moves with
    [SerializeField] float boostAmount = 2.5f; // How much faster the player goes when boosting
    [SerializeField] float veloCap = 300f; // How fast the player can possibly go
    Vector3 direction; // Stores the direction, the player wants to move in
    [SerializeField] float rotationSpeed = 1f; // How fast the player can turn
    [SerializeField] float turretRotationSpeed = 1f;

    [Header("Physics"), SerializeField] float gravity = 1f;
    [SerializeField] float gravityCap = 10f;
    [SerializeField] float hoverDistance = 1f;
    [SerializeField] float upwardsVeloCap = 0.002f;

    Vector3 velocity;

    RaycastHit groundRay;
    Vector3 toPlanet;

    LayerMask groundLayer;

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

    // Attributes for Sound
    [Header("Sound"), SerializeField] AudioSource thrustSound;
    [Range(0, 1), SerializeField] float thrustVolumeBoost = 1f;
    [Range(0, 1), SerializeField] float thrustVolumeNoBoost = 0.3f;

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
        cam = Camera.main;

        // Create the ground layer mask
        int layer = LayerMask.NameToLayer("Ground");
        groundLayer = 1 << layer;
    }
    
    void Update()
    {
        ManageBoostTrail();
    }

    private void FixedUpdate()
    {
        // Shoot if the cooldown is worn off
        if (shotCounter > 0)
        {
            shotCounter -= Time.fixedDeltaTime;
        }
        ManageGravity();
        ReadInput();
        Thrust(bBoostMode);
        transform.position += velocity;
    }

    #endregion

    #region Methods

    // Make the player move and change the thrust particle system depending on wether the player boosts or not
    void Thrust(bool mode)
    {
        // When in boost mode
        if (mode)
        {
            // Play the thrust sound with a high volume
            StartCoroutine(FadeTo(thrustSound, thrustVolumeBoost, 0.3f));
            //TODO make the boostAmount control the volume of the thrust sound
            print(boostAmount);
            thrustSound.volume = boostAmount / 2;
            cam.GetComponent<CameraShake>().shakeDuration = 0.5f;
            //TODO make the force apply only forwards, to prevent exiting the orbit (control direction relative to the planet
            // Add velocity as long as the velocity Cap is not reached
            if (rb.velocity.magnitude < veloCap)
            {
                rb.velocity += transform.forward * (speed + input.Boost * boostAmount) * Time.fixedDeltaTime;
            }
            // Set the trail particle effects to emit bigger particles faster
            main.startSizeMultiplier = 8f;
            emission.rateOverDistance = 3f;
            secMain.startSizeMultiplier = 8f;
            secEmission.rateOverDistance = 3f;
        }
        else
        {
            // Fade out
            if (thrustSound.volume > 0)
            {
                StartCoroutine(FadeTo(thrustSound, 0f, 1f));
            }
        }
    }

    // Set the rotation of the trail particle effect
    private void ManageBoostTrail()
    {
        Quaternion trailRotation = Quaternion.LookRotation(-transform.forward);
        secondTrail.transform.rotation = trailRotation;
        trailEffect.transform.rotation = trailRotation;
    }

    // Play the given sound at the given volume
    private void PlaySoundAtVolume(AudioSource source, float volume)
    {
        if (source)
        {
            source.volume = volume;
            if (!source.isPlaying)
            {
                // Fade In
                source.Play();
            }
        }
    }

    // Fades the sound to the given target volume in the given duration
    private IEnumerator FadeTo(AudioSource source, float targetVolume, float duration)
    {
        float currentVolume = source.volume;

        for(float t = 0; t < duration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(currentVolume, targetVolume, t / duration);
            yield return new WaitForEndOfFrame();
        }

        source.volume = targetVolume;
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

            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }

        // Rotate the guns on the ship depending on the input of the right stick
        if (input.RightHorizontal != 0 || input.RightVertical != 0 && gunObject)
        {
            //gunObject.transform.forward = shootDirection;
            Quaternion targetRotation = new Quaternion();
            targetRotation.SetLookRotation(shootDirection);

            gunObject.transform.rotation = Quaternion.Lerp(gunObject.transform.rotation, targetRotation, turretRotationSpeed * Time.fixedDeltaTime);
        }

        // Instantiates a bullet when the player shoots
        if (input.Shoot)
        {
            if (guns.Length > 0 && bolt && shotCounter <= 0f)
            {
                foreach (Transform gun in guns)
                {
                    Instantiate(bolt, gun.position, gunObject.transform.rotation);
                    cam.GetComponent<CameraShake>().shakeDuration = 0.2f;
                }
                shotCounter = shotCooldown;
            }
        }
    }

    private void ManageGravity()
    {
        Physics.Raycast(transform.position + new Vector3(0f, -0.5f, 2.5f), -transform.up, out groundRay, 1000f, groundLayer);
        toPlanet = groundRay.collider.gameObject.transform.position - transform.position;
        Quaternion newRot = Quaternion.FromToRotation(Vector3.up, groundRay.normal);
        newRot.y = transform.rotation.y;
        transform.rotation = newRot;
        if(groundRay.collider != null)
        {
            if(groundRay.distance > hoverDistance + 1)
            {
                if (velocity.y < gravityCap)
                {
                    velocity += toPlanet.normalized * gravity * Time.fixedDeltaTime;
                }
            }
            else if(groundRay.distance < hoverDistance -1)
            {
                if (velocity.y < upwardsVeloCap)
                {
                    velocity += -toPlanet.normalized * Time.fixedDeltaTime;
                }
            }
        }
        //if (groundRay.collider != null)
        //{
        //    toPlanet = groundRay.collider.gameObject.transform.position - transform.position;
        //    transform.forward = toPlanet;
        //    print(groundRay.distance);
        //    if (groundRay.distance > hoverDistance + 1) // + groundRay.collider.gameObject.GetComponent<SphereCollider>().radius)
        //    {
        //        if (velocity.y < gravityCap)
        //        {
        //            velocity += toPlanet.normalized * gravity * Time.fixedDeltaTime;
        //        }
        //    }
        //    else if (groundRay.distance < hoverDistance - 1) // + groundRay.collider.gameObject.GetComponent<SphereCollider>().radius)
        //    {
        //        if (velocity.y < upwardsVeloCap)
        //        {
        //            velocity += -toPlanet.normalized * Time.fixedDeltaTime;
        //        }
        //    }
        //    else
        //    {
        //        velocity = new Vector3(0f, 0f, 0f);
        //    }
        //}
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position + new Vector3(0f, -0.5f, 2.5f), Vector3.down * 10f);
    }

    #endregion

}

