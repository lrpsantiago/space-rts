using UnityEngine;

namespace PushingBoxStudios.SteampunkTd
{
    public class Billboard : MonoBehaviour
    {
        protected virtual void Update()
        {
            var cam = Camera.main;
            transform.LookAt(transform.position - cam.transform.rotation * Vector3.back, cam.transform.rotation * Vector3.up);
        }
    }
}