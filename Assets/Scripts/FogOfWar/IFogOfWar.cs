using UnityEngine;

namespace Assets.Scripts.FogOfWar
{
    public interface IFogOfWar
    {
        void AddViewer(Transform transform, float viewRadius);
        void UpdateViewerRadius(Transform transform, float newRadius);
        void RemoveViewer(Transform transform);
        void RevealPosition(Vector3 position, float radius);
        void RevealAll();
        void HideAll();
    }
}

