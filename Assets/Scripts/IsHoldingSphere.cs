using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(Collider))]
public class IsHoldingSphere : MonoBehaviour
{
    /// <summary>True while the sphere is grabbed by any interactor.</summary>
    public bool IsHolding { get; private set; } = false;

    /// <summary>1 if holding, 0 if not (used by your CSV logger).</summary>
    public int Hitt => IsHolding ? 1 : 0;

    /// <summary>Event fired whenever the holding state changes.</summary>
    public event Action<bool> HoldingChanged;

    // Base interactable covers XRGrabInteractable and Meta's GrabInteractable
    private XRBaseInteractable interactable;

    private void Awake()
    {
        // Try to find an interactable on this object (XRGrabInteractable or Meta’s)
        interactable = GetComponent<XRBaseInteractable>();
        if (interactable == null)
        {
            Debug.LogWarning("[IsHoldingSphere] No XRBaseInteractable found on this object. Add XRGrabInteractable or Meta’s GrabInteractable.");
        }
    }

    private void OnEnable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnSelectEntered);
            interactable.selectExited.AddListener(OnSelectExited);
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelectEntered);
            interactable.selectExited.RemoveListener(OnSelectExited);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        SetHolding(true);
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        SetHolding(false);
    }

    private void SetHolding(bool value)
    {
        if (IsHolding == value) return;
        IsHolding = value;
        HoldingChanged?.Invoke(IsHolding);
        // Debug.Log($"Sphere holding state: {IsHolding} (hitt={Hitt})");
    }
}
