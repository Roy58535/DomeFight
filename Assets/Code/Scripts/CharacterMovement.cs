using UnityEngine;

public class CharacterMovement : ObjectOnSphere
{
    private float _hmove;
    private bool _jumpPending = false;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _jumpForce = 15f;

    protected override void Update()
    {
        base.Update();
        _hmove = Input.GetAxisRaw("Horizontal") * _speed;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpPending = true;
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
