using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace VRCourse.Interaction
{
    /// <summary>
    /// An interactable abstract class, inherit the XRBaseInteractable provided by XRI.
    ///
    /// Override the following event functions
    /// - OnCourseHoverEnter
    /// - OnCourseHoverExit
    /// - OnCourseSelectEnter
    /// - OnCourseSelectExit
    /// - OnCourseActivate
    /// - OnCourseDeactivate
    /// - Add more events if needed...
    /// 
    /// </summary>
    public abstract class SimpleVRInteractble : XRBaseInteractable
    {
        [Header("Course Debug")]
        [SerializeField] private bool logEvents = false; //Set to true to see console log

        #region overrides
        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);

            IsHovered = true;

            LastInteractor = new SimpleVRInteractorContext(args.interactorObject);

            if (logEvents)
                Debug.Log($"{name}: Hover Enter");

            OnVRHoverEnter(LastInteractor);
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);

            IsHovered = false;

            LastInteractor = new SimpleVRInteractorContext(args.interactorObject);

            if (logEvents)
                Debug.Log($"{name}: Hover Exit");

            // Avoid visually reset if still selected
            if (!IsSelected)
            {
                OnVRHoverExit(LastInteractor);
            }
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            IsSelected = true;

            LastInteractor = new SimpleVRInteractorContext(args.interactorObject);

            if (logEvents)
                Debug.Log($"{name}: Select Enter");

            OnVRSelectEnter(LastInteractor);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            IsSelected = false;

            LastInteractor = new SimpleVRInteractorContext(args.interactorObject);

            if (logEvents)
                Debug.Log($"{name}: Select Exit");

            OnVRSelectExit(LastInteractor);

            // If pointer is no longer hovering, now it is safe to remove highlight
            if (!IsHovered)
            {
                OnVRHoverExit(LastInteractor);
            }
        }

        protected override void OnActivated(ActivateEventArgs args)
        {
            base.OnActivated(args);

            IsActivated = true;

            LastInteractor = new SimpleVRInteractorContext(args.interactorObject);

            if (logEvents)
                Debug.Log($"{name}: Activate");

            OnVRActivate(LastInteractor);
        }

        protected override void OnDeactivated(DeactivateEventArgs args)
        {
            base.OnDeactivated(args);

            IsActivated = false;

            LastInteractor = new SimpleVRInteractorContext(args.interactorObject);

            if (logEvents)
                Debug.Log($"{name}: Deactivate");

            OnVRDeactivate(LastInteractor);
        }

        #endregion

        #region helper functions
        /// <summary>
        /// True while an interactor is hovering over this object.
        /// </summary>
        public new bool IsHovered { get; private set; }

        /// <summary>
        /// True while this object is selected/grabbed.
        /// </summary>
        public new bool IsSelected { get; private set; }

        /// <summary>
        /// True while this object is activated.
        /// Usually corresponds to trigger/button held.
        /// </summary>
        public bool IsActivated { get; private set; }

        /// <summary>
        /// Stores the most recent interactor context.
        /// Useful for accessing controller position/rotation later.
        /// </summary>
        protected SimpleVRInteractorContext LastInteractor { get; private set; }

        #endregion

        // =========================================================
        // Callbacks. the is what you need to implement for each objects
        // =========================================================

        protected virtual void OnVRHoverEnter(SimpleVRInteractorContext context) { }
        // Called when the pointer/controller first hovers over the object.

        protected virtual void OnVRHoverExit(SimpleVRInteractorContext context) { }
        // Called when the pointer/controller stops hovering over the object.

        protected virtual void OnVRSelectEnter(SimpleVRInteractorContext context) { }
        // Called when the object becomes selected/grabbed.

        protected virtual void OnVRSelectExit(SimpleVRInteractorContext context) { }
        // Called when the object is released/deselected.

        protected virtual void OnVRActivate(SimpleVRInteractorContext context) { }
        // Called when the activate/trigger action is performed on the selected object.

        protected virtual void OnVRDeactivate(SimpleVRInteractorContext context) { }
        // Called when the activate/trigger action is released/stopped.
    }
}