using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace VRCourse.Interaction
{
    /// <summary>
    /// A simplified wrapper around XRI's interactor information.
    /// Use this instead of touching XRI event args directly.
    /// </summary>

    public class SimpleVRInteractorContext
    {
        public IXRInteractor Interactor { get; }

        public Transform Transform { get; }

        public GameObject GameObject { get; }

        public Vector3 Position => Transform != null
            ? Transform.position
            : Vector3.zero;

        public Vector3 Forward => Transform != null
            ? Transform.forward
            : Vector3.forward;

        public Quaternion Rotation => Transform != null
            ? Transform.rotation
            : Quaternion.identity;

        public SimpleVRInteractorContext(IXRInteractor interactor)
        {
            Interactor = interactor;

            if (interactor is Component component)
            {
                Transform = component.transform;
                GameObject = component.gameObject;
            }
        }
    }
}


