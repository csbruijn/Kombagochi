using Rive;
using Rive.Components;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PointerBehaviour : MonoBehaviour
{
    private GameManager gm;
    [SerializeField] private RiveWidget ripple;
    [SerializeField] private GameObject rippleObj;
    private ViewModelInstanceTriggerProperty RippleTrig;
    [SerializeField] private string trigName = "rippleTrig";
    [SerializeField] private float rippleRadius = 2f;
    [SerializeField] private float rippleForce = 10f;

    [Header("Slot Assignment")]
    [SerializeField] private MicrobeeSlot[] slots;   

    public static event Action<MicrobeeBehaviour> OnRemoval;

    void OnEnable() =>
        ripple.OnWidgetStatusChanged += OnWidgetStatusChanged;

    void OnDisable() =>
        ripple.OnWidgetStatusChanged -= OnWidgetStatusChanged;

    private void OnWidgetStatusChanged()
    {
        if (ripple.Status != WidgetStatus.Loaded)
            return;

        ViewModelInstance viewModelInstance = ripple.StateMachine?.ViewModelInstance;
        if (viewModelInstance == null)
        {
            Debug.LogError($"{nameof(PointerBehaviour)}: ViewModelInstance is null.", this);
            return;
        }

        RippleTrig = viewModelInstance.GetTriggerProperty(trigName);
        if (RippleTrig == null)
        {
            Debug.LogError($"{nameof(PointerBehaviour)}: Trigger property '{trigName}' not found.", this);
            return;
        }
    }

    public void OnClick(InputValue input)
    {
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Vector2 newPos = new Vector2(worldPos.x, worldPos.y);

        rippleObj.transform.position = newPos;
        RippleTrig.Trigger();
        HandleClick(newPos);
    }

   
    private void HandleClick(Vector2 origin)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, rippleRadius);

        foreach (Collider2D hit in hits)
        {
            hit.GetComponent<NutriBehaviour>()?.HandleRipplePush();

            if (hit.TryGetComponent<MicrobeeBehaviour>(out MicrobeeBehaviour mb))
            {
                if (mb.isSimulated)
                {
                    MicrobeeSlot freeSlot = FindNearestFreeSlot(origin);
                    if (freeSlot != null)
                    {
                        freeSlot.SetMicrobee(mb);
                        mb.SetSimulated(false);
                    }
                }
                else if (mb.health <= 0)
                {
                    Destroy(mb.gameObject);
                    OnRemoval.Invoke(mb);
                }
                else
                {
                    mb.OnClicked();
                }
            }
        }
    }


    /// <summary>
    /// assign the nearest spot 
    /// </summary>
    /// <param name="origin">coorditnates to look from </param>
    /// <returns></returns>
    private MicrobeeSlot FindNearestFreeSlot(Vector2 origin)
    {
        MicrobeeSlot nearest = null;
        float bestDist = float.MaxValue;

        foreach (MicrobeeSlot slot in slots)
        {
            if (slot.IsOccupied()) continue;

            float dist = Vector2.Distance(origin, slot.transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                nearest = slot;
            }
        }

        return nearest;
    }
}