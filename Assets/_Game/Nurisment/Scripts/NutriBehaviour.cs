using Rive;
using Rive.Components;
using UnityEngine;

public class NutriBehaviour : MonoBehaviour
{
    [SerializeField] private RiveWidget riveWidget; 
    public NutriType nutriType;
    public float nutriValue;
    [SerializeField] private float lifeSpawnTotal = 5f;
    private float lifeRemaining; 

    private ViewModelInstanceTriggerProperty fadeTriggerProperty;
    [SerializeField] private string fadeTrigName = "startFade";
    
    private ViewModelInstanceTriggerProperty deathTriggerProperty;
    [SerializeField] private string deathTrigName = "startFade";

    [SerializeField] private float ripplePushCD = .5f;
    private float timer = 0f;

    private bool touched = false;


    private void Start()
    {
        lifeRemaining = lifeSpawnTotal; 
    }

    private void OnEnable() =>
        riveWidget.OnWidgetStatusChanged += OnWidgetStatusChanged;

    private void OnDisable() =>
        riveWidget.OnWidgetStatusChanged -= OnWidgetStatusChanged;

    private void OnWidgetStatusChanged()
    {
        if (riveWidget.Status != WidgetStatus.Loaded) return;

        StateMachine stateMachine = riveWidget.StateMachine;
        if (stateMachine == null) return;

        ViewModel vm = riveWidget.Artboard.DefaultViewModel;
        if (vm == null) { Debug.LogError("No DefaultViewModel found."); return; }

        ViewModelInstance instance = vm.CreateInstanceByName(nutriType.ToString());

        stateMachine.BindViewModelInstance(instance);

        fadeTriggerProperty = instance.GetTriggerProperty(fadeTrigName);
        if(fadeTriggerProperty == null) Debug.LogError($"Trig property '{fadeTriggerProperty}' not found.", this);

        deathTriggerProperty = instance.GetTriggerProperty(deathTrigName);
        if (deathTriggerProperty == null) Debug.LogError($"Trig property '{deathTriggerProperty}' not found.", this);

    }


    private void FixedUpdate()
    {
        

        if (!touched) return; 
        if (lifeRemaining > 0f)
        {
            lifeRemaining -= Time.deltaTime;

            //float relativeTime = (lifeSpawnTotal - lifeRemaining) / lifeSpawnTotal;
            //Debug.Log(relativeTime);
            //transform.localScale = new Vector3(1- relativeTime, 1- relativeTime,1-  relativeTime); 

        }
        else Destroy(gameObject);

        if (timer < 0f) timer -= Time.deltaTime;
    }
    public void HandleRipplePush()
    {
        if (touched) return;
        if (timer > 0f) return;
        touched = true;
        timer = ripplePushCD;

        CircleCollider2D Col2D = GetComponent<CircleCollider2D>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        Col2D.isTrigger = false;
        rb.gravityScale = 1f;
        rb.drag = .5f;
    }

    public void RandomType()
    {
        NutriType randomState = (NutriType)System.Enum.GetValues(typeof(NutriType))
    .GetValue(Random.Range(0, System.Enum.GetValues(typeof(NutriType)).Length));
        nutriType = randomState;
    }
}

public enum NutriType
{
    Blue,
    Green, 
    Red,
    Yellow,
}