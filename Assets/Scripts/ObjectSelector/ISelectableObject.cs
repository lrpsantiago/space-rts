﻿using UnityEngine;

namespace SpaceRts
{
    public interface ISelectableObject
    {
        string Name { get; }
        string Description { get; }

        bool IsSelected { get; set; }
        Transform Transform { get; }

        void OnSelection();
        void OnSelectionDismiss();
        void OnPositionSelection(Vector3 position, Vector3? facingDirection);
        void OnPointToAnotherObject(ISelectableObject anotherObject);
    }
}