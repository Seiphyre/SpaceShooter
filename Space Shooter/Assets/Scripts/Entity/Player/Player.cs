using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : ADamagableEntity// AEntity//, IDamagable
{
    // ----- [ Attributes ] --------------------------------------------

    // --v-- Life --v--

    // [Header("Life")]

    // [SerializeField]
    // private int _maxLife = 3;

    // [SerializeField]
    // private Color _highLife = Color.green;
    // [SerializeField]
    // private Color _midLife = new Color(1, 0.65f, 0); // Orange
    // [SerializeField]
    // private Color _lowLife = Color.red;

    // [SerializeField]
    // private Transform _explosionPrefab;

    // private int _currentLife;

    // --v-- Movement --v--

    [Header("Player/Movement")]

    [SerializeField]
    private float _speed = 3.5f;

    private bool _allowManualMovement = true;

    // --v-- Weapon --v--

    [Space]
    [Header("Player/Weapon")]

    [SerializeField]
    private Transform _bulletSpawn = null;

    [SerializeField]
    private Transform _bulletPrefab = null;

    [SerializeField]
    private float _fireRate = 0.5f;

    private float _fireRateCountdown = 0;

    // --v-- Other --v-- 

    // private MeshRenderer _mr;

    private Animator _animator;

    private int _crystals;


    // --v-- Follow Path Coroutine --v--

    private Coroutine _followPathCoroutineInstance;

    private CurvePath _followingPath = null;

    private Action _followPathCallback = null;

    // // --v-- Events --v--

    // public event Action OnSelfDestroy;
    public event Action<int> OnGatherCrystals;


    // ----- [ Getters / Setters ] -----------------------------------------------------

    public int Crystals { get { return _crystals; } }



    // ----- [ Functions ] -----------------------------------------------------

    // --v-- Unity Messages --v-- 

    protected override void Awake()
    {
        base.Awake();

        // _mr = GetComponent<MeshRenderer>();
        _animator = GetComponent<Animator>();

        if (!_bulletPrefab)
        {
            Debug.LogWarning("[Player] Missing reference on _bulletPrefab.");
        }

        if (!_bulletSpawn)
        {
            Debug.LogWarning("[Player] Missing reference on _bulletSpawn.");
        }

        // These values need to be reset
        _fireRateCountdown = 0;
        _crystals = 0;
    }

    // protected override void Start()
    // {
    //     base.Start();
    // }

    private void Update()
    {
        // Movement 
        if (_allowManualMovement)
            CalculateMovement();

        // Weapon
        UpdateFireCooldown();
        if (Input.GetKey(KeyCode.Space) && _fireRateCountdown == 0f)
        {
            FireBullet();
        }
    }

    // --v-- Weapon --v--

    private void UpdateFireCooldown()
    {
        if (_fireRateCountdown > 0f)
        {
            _fireRateCountdown -= Time.deltaTime;
            _fireRateCountdown = Mathf.Max(0f, _fireRateCountdown);
        }
    }

    private void FireBullet()
    {
        Instantiate(_bulletPrefab, _bulletSpawn.transform.position, _bulletSpawn.transform.rotation);
        _fireRateCountdown = _fireRate;
    }


    // --v-- Movement --v--

    private void CalculateMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        // Apply movement
        transform.Translate(direction * _speed * Time.deltaTime, Space.World);

        // Animation
        ResetMovementAnimation();

        if (verticalInput == 1)
            _animator.SetBool("IsMovingLeft", true);
        else if (verticalInput == -1)
            _animator.SetBool("IsMovingRight", true);

        if (horizontalInput == 1)
            _animator.SetBool("IsMovingForward", true);
        else if (horizontalInput == -1)
            _animator.SetBool("IsMovingBackward", true);

        // Replacing the player if out of boundary
        CalculateMovementRestriction();
    }

    private void CalculateMovementRestriction()
    {
        // 
        // for the following, we are assuming the main camera is orthonorme
        // Boundaries will be calculated based on the main camera
        //

        MinMaxBounds positionBoundary = GetBoundaryPositionIn();

        if (positionBoundary.Min.x > positionBoundary.Max.x || positionBoundary.Min.y > positionBoundary.Max.y)
        {
            Debug.LogWarning("The player is too big for the boundary. Boundaries are disabled");
            return;
        }

        float x = Mathf.Clamp(transform.position.x, positionBoundary.Min.x, positionBoundary.Max.x);
        float y = Mathf.Clamp(transform.position.y, positionBoundary.Min.y, positionBoundary.Max.y);

        transform.position = new Vector3(x, y, 0);
    }

    public void FollowPath(CurvePath path, float duration = -1, Action callback = null)
    {
        // Use real speed
        if (duration == -1)
            duration = path.Length / _speed;

        // Disable manual movement 
        _allowManualMovement = false;

        callback += () =>
        {
            _allowManualMovement = true;
        };

        // Reset animation
        ResetMovementAnimation();

        // Start following the path
        _followingPath = path;
        _followPathCallback = callback;
        _followPathCoroutineInstance = StartCoroutine(FollowPathRoutine(path, duration, callback));
    }

    public void StopFollowingPath()
    {
        if (_followPathCoroutineInstance == null) return;

        StopCoroutine(_followPathCoroutineInstance);
        _followPathCoroutineInstance = null;

        // Reset position & rotation
        transform.position = _followingPath.EvaluatePosition(1);
        transform.rotation = _followingPath.EvaluateRotation(1);

        // Reset Animator
        ResetMovementAnimation();

        if (_followPathCallback != null)
            _followPathCallback();
    }

    private IEnumerator FollowPathRoutine(CurvePath path, float duration, Action callback)
    {
        _animator.applyRootMotion = true;

        float t = 0;
        while (t <= 1)
        {
            Vector3 prevPos = transform.localPosition;
            transform.position = path.EvaluatePosition(t);
            transform.rotation = path.EvaluateRotation(t);

            Vector3 moveDirection = transform.localPosition - prevPos;
            Vector3 lookDirection = transform.forward;

            // Animation
            ResetMovementAnimation();

            if (Vector3.Angle(lookDirection, moveDirection) < 90)
                _animator.SetBool("IsMovingForward", true);
            else
                _animator.SetBool("IsMovingBackward", true);

            t += Time.deltaTime / duration;

            yield return null;
        }

        // Reset animation
        _animator.SetBool("IsMovingForward", false);
        _animator.SetBool("IsMovingBackward", false);

        _animator.applyRootMotion = false;

        callback?.Invoke();

        _followPathCoroutineInstance = null;
        _followingPath = null;
        _followPathCallback = null;
    }

    private void ResetMovementAnimation()
    {
        _animator.SetBool("IsMovingForward", false);
        _animator.SetBool("IsMovingBackward", false);
        _animator.SetBool("IsMovingLeft", false);
        _animator.SetBool("IsMovingRight", false);
    }


    // --v-- Crystal gathering --v--

    void OnParticleCollision(GameObject other)
    {
        ParticleSystem ps;
        List<ParticleCollisionEvent> collisionEvents;
        int nbrOfCollision = 1;

        // Get Particle system
        ps = other.GetComponent<ParticleSystem>();
        if (ps == null)
        {
            Debug.Log("OnParticleCollision : ParticleSystem not found");
            return;
        }

        // Init ParticleCollisionEvent List
        collisionEvents = new List<ParticleCollisionEvent>();

        // Get nbr of collision 
        nbrOfCollision = ps.GetCollisionEvents(transform.gameObject, collisionEvents);

        // Update crystal value
        _crystals += nbrOfCollision * Crystal.Value;

        OnGatherCrystals?.Invoke(nbrOfCollision * Crystal.Value);
    }

}
