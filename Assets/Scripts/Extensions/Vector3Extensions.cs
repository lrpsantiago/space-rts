using UnityEngine;

namespace SpaceRts
{
    public static class Vector3Extensions
    {
        public static Vector3 Round(this Vector3 vector)
        {
            vector.x = Mathf.Round(vector.x);
            vector.y = Mathf.Round(vector.y);
            vector.z = Mathf.Round(vector.z);

            return vector;
        }
    }
}
