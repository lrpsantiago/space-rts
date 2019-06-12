using UnityEngine;

public class Planet : MonoBehaviour
{
    private GameObject _atmosphere;

    [SerializeField]
    private float _rotationSpeed = 1;

    [SerializeField]
    private float _atmosphereSpeed = 1.2f;

    void Start()
    {

        _atmosphere = transform.Find("Atmosphere").gameObject;
    }

    void Update()
    {
        var velocity = _rotationSpeed * Time.deltaTime;
        transform.Rotate(transform.up, velocity);

        velocity = (_atmosphereSpeed - _rotationSpeed) * Time.deltaTime;
        _atmosphere.transform.Rotate(transform.up, velocity);
    }
}
