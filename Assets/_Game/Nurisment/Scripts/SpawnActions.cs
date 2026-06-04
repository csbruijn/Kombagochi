using Rive;
using Rive.Components;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class SpawnActions: MonoBehaviour
{
    [Header("Rive")]
    [Tooltip("The Rive Widget that is displaying your file.")]
    [SerializeField] private RiveWidget m_riveWidget;

    [Tooltip("ViewModel Trigger property name fired by the Rive file.")]
    [SerializeField] private string mOnTriggerPropName = "click";
    private ViewModelInstanceTriggerProperty OnTriggerProp;

    [Tooltip("Invoked when the Rive file fires the gameOver trigger.")]
    public UnityEvent OnButtonTrigger = new UnityEvent();

    private ViewModelInstanceNumberProperty timerProp;
    [SerializeField] private string timerPropNamet = "timer";
    private float timer = 0;
    [SerializeField] private float resetTime = 180;

    public bool speedUp = true;
    [SerializeField] float speedUpFactor = 10; 



     private bool overlayActive = false;

    [SerializeField]  private GameObject overlay;

    bool firstTime = true; 
    private void HandleButtonTriggeredFromRive()
    {
        if (timer > 0)
        {
            //toggle overlay 

            ToggleOverlay();
            
            return;
        }
        if (firstTime)
        { timer = resetTime / 2; firstTime = false; }
        else timer = resetTime; 

        OnButtonTrigger.Invoke();
    }

    public void RedeemCode()
    {
        timer = 1;
        ToggleOverlay();
    }

    public void ToggleOverlay()
    {
        bool active = !overlay.activeSelf;

        overlay.SetActive(active);

        Time.timeScale = active ? 0f : 1f;
    }

    private void Update()
    {

        if (!widgetIsLoaded) return;

        if (timer > 0)
        {
            float multiplier = 1;
            if (speedUp) multiplier *= speedUpFactor;
            timer -= Time.deltaTime * multiplier;

            timerProp.Value = timer; 
        }
    }



    void OnEnable()
    {
        if (m_riveWidget == null)
        {
            Debug.LogError($"{nameof(ButtonReader)}: No RiveWidget assigned.", this);
            return;
        }

        m_riveWidget.OnWidgetStatusChanged += HandleWidgetStatusChanged;
        HandleWidgetStatusChanged();
    }


    void OnDisable()
    {
        if (m_riveWidget != null)
        {
            m_riveWidget.OnWidgetStatusChanged -= HandleWidgetStatusChanged;
        }
    }

    bool widgetIsLoaded = false;
    private void HandleWidgetStatusChanged()
    {

        if (m_riveWidget.Status != WidgetStatus.Loaded)
        {
            return;
        }

        StateMachine m_stateMachine = m_riveWidget.StateMachine;

        ViewModelInstance viewModelInstance = m_riveWidget.StateMachine?.ViewModelInstance;

        if (viewModelInstance == null)
        {
            Debug.LogError($"{nameof(ButtonReader)}: ViewModelInstance is null. " +
                           "Make sure Data Binding Mode is set to Auto Bind Default / Selected.", this);
            return;
        }

        OnTriggerProp = viewModelInstance.GetTriggerProperty(mOnTriggerPropName);
        if (OnTriggerProp == null)
        {
            Debug.LogError($"{nameof(ButtonReader)}: Trigger property '{mOnTriggerPropName}' not found.", this);
            return;
        }

        OnTriggerProp.OnTriggered += HandleButtonTriggeredFromRive;

        timerProp = viewModelInstance.GetNumberProperty(timerPropNamet);
        if (timerProp == null)
        {
            Debug.LogError($"{nameof(ButtonReader)}: Trigger property '{timerPropNamet}' not found.", this);
            return;
        }
        timerProp.Value = timer;

        widgetIsLoaded = true;
    }



}
