using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : ObjectOnSphere
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _jumpForce = 15f;
    [SerializeField] private float _groundFriction = 0.9f;
    [SerializeField] private float _airControl = 0.5f;
    [SerializeField] private float _airFriction = 0.5f;

    private CapsuleCollider _capsuleCollider;
    private float _hmove;
    private bool _jumpPending = false;
    private bool _isGrounded = false;
    private float _jumpTimer = 0;
    

    protected override void Awake()
    {
        base.Awake();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    protected override void Update()
    {
        base.Update();
        if (_jumpTimer > 0)
        {
            _jumpTimer -= Time.deltaTime;
        }

        //Input handling
        _hmove = Input.GetAxisRaw("Horizontal") * _speed;
        print(_hmove);

        //Ground check
        Vector3 checkingPosition = transform.position + SurfaceDownDir * _capsuleCollider.height * transform.localScale.y / 2;
        _isGrounded = Physics.Raycast(checkingPosition, SurfaceDownDir, 0.1f, LayerMask.GetMask("Ground"));
        Debug.DrawRay(checkingPosition, SurfaceDownDir * 0.1f, _isGrounded ? Color.green : Color.red);

        // Jump input
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpTimer = 0.15f;
        }
        if (_isGrounded && _jumpTimer > 0)
        {
            _jumpPending = true;
            _jumpTimer = 0;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        ApplyHorizontalMovement();

        //Apply jump force
        if (_jumpPending)
        {
            Rig.AddForce(-SurfaceDownDir * _jumpForce, ForceMode.VelocityChange);
            _jumpPending = false;
        }
    }

    private void ApplyHorizontalMovement()
    {
        //Apply horizontal movement
        Vector3 tangentialMovement = Vector3.Project(Rig.velocity, SurfaceDownDir);
        Vector3 currentAzimuthalVel = Vector3.Project(Rig.velocity, AzimuthDir);
        Vector3 azimuthalMovement = -AzimuthDir * _hmove;
        Vector3 velocityDiff = Vector3.zero;

        // Apply different friction based on whether the player is grounded or in the air
        if (_isGrounded)
        {
            if (Mathf.Abs(_hmove) > 0.01f)
            {
                velocityDiff = tangentialMovement + azimuthalMovement - Rig.velocity;
            }
            else
            {
                velocityDiff = -currentAzimuthalVel * _groundFriction;
            }
        }
        else
        {
            if (Mathf.Abs(_hmove) > 0.01f)
            {
                velocityDiff = (tangentialMovement + azimuthalMovement - Rig.velocity) * _airControl;
            }
            else
            {
                velocityDiff = -currentAzimuthalVel * _airFriction;
            }
        }
        Rig.AddForce(velocityDiff, ForceMode.VelocityChange);
    }
}
