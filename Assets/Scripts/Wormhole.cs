using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SpaceRts
{
    public class Wormhole : MonoBehaviour, ISelectableObject
    {
        public bool IsSelected { get; set; }

        public Transform Transform
        {
            get { return transform; }
        }

        public void OnPointToAnotherObject(ISelectableObject anotherObject)
        {
        }

        public void OnPositionSelection(Vector3 position, Vector3? facingDirection)
        {
        }

        public void OnSelection()
        {
        }

        public void OnSelectionDismiss()
        {
        }
    }
}
