using SpaceRts;
using UnityEngine;

namespace SpaceRts
{
    public class Planet : MonoBehaviour, ISelectableObject
    {
        private GameObject _atmosphere;

        [SerializeField]
        private float _rotationSpeed = 1;

        [SerializeField]
        private float _atmosphereSpeed = 1.2f;

        public bool IsSelected { get; set; }





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

        public void OnSelection()
        {
            IsSelected = true;

            var selection = transform.Find("Selection").gameObject;
            selection.SetActive(IsSelected);
        }

        public void OnDismiss()
        {
            IsSelected = false;

            var selection = transform.Find("Selection").gameObject;
            selection.SetActive(IsSelected);
        }

        public void OnPositionSelection(Vector3 position)
        {
            //set rally point
        }
    }
}