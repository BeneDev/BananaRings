using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public Vector3 Velocity
    {
        get
        {
            return rb.velocity;
        }
    }

    public Vector3 ShootDirection
    {
        get
        {
            return shootDirection;
        }
    }

    [Header("Movement"), SerializeField] float speed = 1f;
    [SerializeField] float boostAmount = 2.5f;
    [SerializeField] float veloCap = 300f;
    Vector3 velocity;

    PlayerInput input;

    [Header("Physics"), SerializeField] float lookDistance = 2f;
    LayerMask obstacles;
    [SerializeField] float colliderRadius = 1f;

    [Header("Shooting"), SerializeField] Transform[] guns;
    [SerializeField] GameObject gunObject;
    [SerializeField] GameObject bolt;
    [SerializeField] float shotDelay = 0.5f;
    float shotCounter = 0f;
    Vector3 shootDirection;

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
        if(shotCounter > 0)
        {
            shotCounter -= Time.fixedDeltaTime;
        }

        Thrust(bBoostMode);
    }

    void Thrust(bool mode)
    {
        if (mode)
        {
            //transform.position += velocity * (speed + input.Boost * boostAmount) * Time.fixedDeltaTime;
            if (rb.velocity.magnitude < veloCap)
            {
                rb.velocity += velocity * (speed + input.Boost * boostAmount) * Time.fixedDeltaTime;
            }
            //rb.AddForce(velocity * (speed + input.Boost * boostAmount) * Time.fixedDeltaTime);
            main.startSizeMultiplier = 8f;
            emission.rateOverDistance = 3f;
            secMain.startSizeMultiplier = 8f;
            secEmission.rateOverDistance = 3f;
        }
        else
        {
            //transform.position += velocity * speed * Time.fixedDeltaTime;
            if (rb.velocity.magnitude < veloCap * 0.125f)
            {
                rb.velocity += velocity * speed * Time.fixedDeltaTime;
            }
            //rb.AddForce(velocity * speed * Time.fixedDeltaTime);
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

        shootDirection.x = input.RightHorizontal;
        shootDirection.z = input.RightVertical;

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

        if (input.RightHorizontal != 0 || input.RightVertical != 0 && gunObject)
        {
            gunObject.transform.forward = shootDirection;
        }

        if (input.Shoot)
        {
            if (guns.Length > 0 && bolt && shotCounter <= 0f)
            {
                foreach (Transform gun in guns)
                {
                    Instantiate(bolt, gun.position, gunObject.transform.rotation);
                }
                shotCounter = shotDelay;
            }
        }
    }
}

