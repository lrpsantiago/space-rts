using UnityEngine;

namespace SpaceRts
{
    public interface ISelectableObject
    {
        bool IsSelected { get; set; }

        void OnSelection();
        void OnDismiss();
        void OnPositionSelection(Vector3 position);
    }
}
