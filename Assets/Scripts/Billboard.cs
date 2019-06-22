using UnityEngine;

namespace PushingBoxStudios.SteampunkTd
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField]
        private bool _yAxisOnly = false;

        protected virtual void Start()
        {
            Update();
        }

        protected virtual void Update()
        {
            var camRotation = Camera.main.transform.rotation;

            if (!_yAxisOnly)
            {
                transform.LookAt(transform.position - camRotation * Vector3.back, camRotation * Vector3.up);
                return;
            }

            camRotation.x = 0;
            camRotation.z = 0;

            transform.LookAt(transform.position - camRotation * Vector3.back, camRotation * Vector3.up);
        }
    }
}