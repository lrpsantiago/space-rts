using UnityEngine;

namespace Assets.Scripts
{
    public class LocationSelector : MonoBehaviour
    {
        private PathfinderRobot _robot;
        private Transform _tileSelector;

        public void Start()
        {
            _robot = GameObject.Find("Robot").GetComponent<PathfinderRobot>();
            _tileSelector = GameObject.Find("TileSelector").transform;
        }

        public void Update()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var layers = new string[] { "Land" };
            var mask = LayerMask.GetMask(layers);
            var hits = Physics.RaycastAll(ray, Mathf.Infinity, mask);

            if (hits.Length > 0)
            {
                var hit = hits[0];
                Debug.DrawLine(ray.origin, hit.point);

                if (hit.collider.tag == "Land")
                {
                    var pos = hit.point;

                    pos.x = Mathf.Round(pos.x);
                    pos.y = Mathf.Round(pos.y);
                    pos.z = Mathf.Round(pos.z);

                    _tileSelector.position = pos;

                    if(Input.GetMouseButtonDown(1))
                    {
                        _robot.FindPath(pos);
                    }
                }
            }
        }
    }
}
