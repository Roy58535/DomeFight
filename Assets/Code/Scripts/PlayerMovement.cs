using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class PlayerMovement : ObjectOnSphere
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _jumpForce = 15f;

    private CapsuleCollider _capsuleCollider;
    private float _hmove;
    private bool _jumpPending = false;
    private bool _isGrounded = false;
    private float _jumpTimer = 0;

    protected override void Start()
    {
        base.Start();
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

        //Ground check
        Vector3 checkingPosition = transform.position + SurfaceDownDir * _capsuleCollider.height / 2;
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
        //Apply jump force
        if (_jumpPending)
        {
            Rig.AddForce(-SurfaceDownDir * _jumpForce, ForceMode.VelocityChange);
            _jumpPending = false;
        }

        //Apply horizontal movement
        Vector3 tangentialMovement = Vector3.Project(Rig.velocity, SurfaceDownDir);
        Vector3 azimuthalMovement = -AzimuthDir * _hmove;
        Rig.velocity = tangentialMovement + azimuthalMovement;
    }
}
