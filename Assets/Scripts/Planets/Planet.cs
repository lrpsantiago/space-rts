using Assets.Scripts.FogOfWar;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SpaceRts.Planets
{
    public class Planet : MonoBehaviour, ISelectableObject
    {
        private static GameObject _shipPrefab;
        private static GameObject _rallyPointPrefab;

        [SerializeField]
        private float _rotationSpeed = 1;

        [SerializeField]
        private float _atmosphereSpeed = 1.2f;

        [SerializeField]
        private string _sizeDescription;

        [SerializeField]
        private string _typeDescription;

        private Transform _atmosphere;
        private GameObject _rallyPoint;
        private bool _isDiscovered;
        private bool _isHomeland;

        public string Name { get; private set; }

        public string Description => $"{_sizeDescription} {_typeDescription} Planet";

        public bool IsSelected { get; set; }

        public Transform Transform
        {
            get { return transform; }
        }

        private void Awake()
        {
            if (_shipPrefab == null)
            {
                _shipPrefab = Resources.Load<GameObject>("Prefabs/Units/Ship");
            }

            if (_rallyPointPrefab == null)
            {
                _rallyPointPrefab = Resources.Load<GameObject>("Prefabs/Planets/RallyPoint");
            }

            Name = GetName();
        }

        private void Start()
        {
            _atmosphere = transform.Find("Atmosphere");

            var rallyPoint = transform.position;

            rallyPoint.x += Mathf.Round(transform.lossyScale.z / 2 + 0.5f);
            rallyPoint.z -= Mathf.Round(transform.lossyScale.z / 2 + 0.5f);

            _rallyPoint = Instantiate(_rallyPointPrefab, rallyPoint, Quaternion.LookRotation(Vector3.back));
            _rallyPoint.SetActive(false);

            var angle = Random.Range(0, 360);
            transform.Rotate(transform.up, angle);
        }

        private void Update()
        {
            var velocity = _rotationSpeed * Time.deltaTime;
            transform.Rotate(transform.up, velocity);

            if (_atmosphere != null)
            {
                velocity = (_atmosphereSpeed - _rotationSpeed) * Time.deltaTime;
                _atmosphere.transform.Rotate(transform.up, velocity);
            }
        }

        public void CreateShip()
        {
            var rallyPos = _rallyPoint.transform.position;
            var forwardRally = rallyPos - transform.position;
            var shipRotation = Quaternion.LookRotation(forwardRally.normalized, Vector3.up);
            var surfacePos = transform.position + forwardRally.normalized * transform.lossyScale.x / 2;
            var shipObj = Instantiate(_shipPrefab, surfacePos, shipRotation);

            if (shipObj != null)
            {
                var ship = shipObj.GetComponent<Ship>();

                ship.DestinationQueue.Add(rallyPos);
                ship.DestinationFacingDirection = _rallyPoint.transform.forward;
            }
        }

        public void OnSelection()
        {
            IsSelected = true;

            var selection = transform.Find("Selection")
                .gameObject;

            selection.SetActive(IsSelected);
            _rallyPoint.SetActive(IsSelected);
        }

        public void OnSelectionDismiss()
        {
            IsSelected = false;

            var selection = transform.Find("Selection")
                .gameObject;

            selection.SetActive(IsSelected);
            _rallyPoint.SetActive(IsSelected);
        }

        public void OnPositionSelection(Vector3 position, Vector3? facingDirection)
        {
            _rallyPoint.transform.position = position;
            _rallyPoint.transform.rotation = Quaternion.LookRotation(facingDirection.Value, Vector3.up);
        }

        public void OnPointToAnotherObject(ISelectableObject anotherObject)
        {

        }

        public void Reveal()
        {
            var fog = GameObject.Find("FogOfWar")
                .GetComponent<FogOfWar>();

            var radius = 12f;

            if (_sizeDescription.Equals("Big"))
            {
                radius = 14f;
            }
            else if (_sizeDescription.Equals("Small"))
            {
                radius = 10f;
            }

            fog.RevealPosition(transform.position, radius);
        }

        private static string GetName()
        {
            var names = new string[]
            {
                "Adipose",
                "Aurora",
                "Belzagor",
                "Cairn",
                "Ceres",
                "Cygni",
                "Edore",
                "Erra",
                "Eternia",
                "Felucia",
                "Gliese",
                "Gor",
                "Haumea",
                "Ireta",
                "Karn",
                "Kharak",
                "Magrathea",
                "Medea",
                "Nacre",
                "Neopia",
                "Quasar",
                "Shora",
                "Tamaran",
                "Tenga",
                "Vekta",
                "Ventura",
                "Xanthu",
                "Yabusei",
                "Zahir",
                "Zyrgon"
            };

            var index = Random.Range(0, names.Length);

            return names[index];
        }
    }
}