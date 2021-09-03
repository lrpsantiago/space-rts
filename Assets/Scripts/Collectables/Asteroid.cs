using PushingBoxStudios;
using SpaceRts;
using UnityEngine;

namespace Assets.Scripts
{
    public class Asteroid : MonoBehaviour, ISelectableObject
    {
        [SerializeField]
        private Vector3 _rotationSpeed = new Vector3(15, 15, 15);

        private Gauge _oreGauge;

        public float OreAmount { get => _oreGauge.CurrentValue; }

        public string Name => "Asteroid";

        public string Description => $"Ore: {OreAmount}";

        public bool IsSelected { get; set; }

        public Transform Transform => transform;

        private void Start()
        {
            _oreGauge = new Gauge(100);

            var x = Random.Range(0, 360);
            var y = Random.Range(0, 360);
            var z = Random.Range(0, 360);

            transform.rotation = Quaternion.Euler(x, y, z);

            var offsetX = Random.Range(-0.25f, 0.25f);
            var offsetZ = Random.Range(-0.25f, 0.25f);

            transform.Translate(offsetX, 0, offsetZ);
        }

        private void Update()
        {
            var speed = _rotationSpeed * Time.deltaTime;
            transform.Rotate(speed);
        }

        public void ExtractOre(float amount)
        {
            _oreGauge.CurrentValue -= amount;
        }

        public void OnSelection()
        {
        }

        public void OnSelectionDismiss()
        {
        }

        public void OnPositionSelection(Vector3 position, Vector3? facingDirection)
        {
        }

        public void OnPointToAnotherObject(ISelectableObject anotherObject)
        {
        }
    }
}
