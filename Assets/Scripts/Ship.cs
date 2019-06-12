using UnityEngine;

public class Ship : MonoBehaviour
{
    [SerializeField]
    private float _acceleration = 0.2f;

    [SerializeField]
    private float _deceleration = 0.25f;

    [SerializeField]
    private float _movementSpeedMax = 2;

    private float _movementSpeed = 0;

    [SerializeField]
    private float _angularSpeed = 0;

    [SerializeField]
    private float _decelTime = 30;

    private void Start()
    {
        Invoke("Decel", _decelTime);
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, _angularSpeed * Time.deltaTime);

        _movementSpeed = Mathf.Clamp(_movementSpeed + _acceleration * Time.deltaTime, 0, _movementSpeedMax);

        transform.position += _movementSpeed * transform.forward * Time.deltaTime;
    }

    private void Decel()
    {
        _acceleration = -_deceleration;
    }
}
