using Rive;
using Rive.Components;
using UnityEngine;
using UnityEngine.InputSystem;

public class pointerBehaviour : MonoBehaviour
{
    private GameManager gm;

    [SerializeField] private RiveWidget ripple;
    [SerializeField] private GameObject rippleObj; 

    private ViewModelInstanceTriggerProperty RippleTrig;
    [SerializeField] private string trigName = "rippleTrig";

    [SerializeField] private float rippleRadius = 3f;
    [SerializeField] private float rippleForce = 10f;


    private void Awake()
    {
        //gm = GameManager.instance;
        //ripple = gm.Ripple;
    }

    void OnEnable()=>
        ripple.OnWidgetStatusChanged += OnWidgetStatusChanged;
    
    void OnDisable()=>
        ripple.OnWidgetStatusChanged -= OnWidgetStatusChanged;
    
    private void OnWidgetStatusChanged()
    {
        if (ripple.Status != WidgetStatus.Loaded)
            return;

        ViewModelInstance viewModelInstance = ripple.StateMachine?.ViewModelInstance;
        if (viewModelInstance == null)
        {
            Debug.LogError($"{nameof(pointerBehaviour)}: ViewModelInstance is null. " +
                           "Make sure Data Binding Mode is set to Auto Bind Default / Selected.", this);
            return;
        }

        RippleTrig = viewModelInstance.GetTriggerProperty(trigName);
        if (RippleTrig == null)
        {
            Debug.LogError($"{nameof(pointerBehaviour)}: Trigger property '{RippleTrig}' not found.", this);
            return;
        }
    }
    
    public void OnClick(InputValue input)
    {
        Debug.Log("Click");

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Vector2 newPos = new Vector2(worldPos.x, worldPos.y);

        rippleObj.transform.position = newPos;
        RippleTrig.Trigger();

        PushNearbyNutri(newPos);
    }


    private void PushNearbyNutri(Vector2 origin)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, rippleRadius);

        Debug.Log(hits.Length);
        foreach (Collider2D hit in hits)
        {
            if (!hit.TryGetComponent<NutriBehaviour>(out NutriBehaviour nb)) continue;

            if (!hit.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb)) continue;

            Vector2 direction = (rb.position - origin).normalized;

            float distance = Vector2.Distance(origin, rb.position);
            float falloff = 1f - Mathf.Clamp01(distance / rippleRadius);

            rb.AddForce(direction * rippleForce * falloff, ForceMode2D.Impulse);
        }
    }
}
