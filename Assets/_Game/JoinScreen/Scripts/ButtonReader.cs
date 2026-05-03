using Rive;
using Rive.Components;
using UnityEngine;
using UnityEngine.Events;


public class ButtonReader : MonoBehaviour
{
    [Header("Rive")]
    [Tooltip("The Rive Widget that is displaying your health bar file.")]
    [SerializeField] private RiveWidget m_riveWidget;

    [Tooltip("ViewModel Trigger property name fired by the Rive file.")]
    [SerializeField] private string mOnGameStartPropName = "Trig";

    private ViewModelInstanceTriggerProperty mOnGameStartProp;

    [Tooltip("Invoked when the Rive file fires the gameOver trigger.")]
    public UnityEvent OnGameStart = new UnityEvent();

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

    private void HandleWidgetStatusChanged()
    {

        // Wait for the Rive Widget to load before accessing the state machine.
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

        // Get the gameOver property by name.
        mOnGameStartProp = viewModelInstance.GetTriggerProperty(mOnGameStartPropName);
        if (mOnGameStartProp == null)
        {
            Debug.LogError($"{nameof(ButtonReader)}: Trigger property '{mOnGameStartPropName}' not found.", this);
            return;
        }

        mOnGameStartProp.OnTriggered += HandleButtonTriggeredFromRive;

    }

    private void HandleButtonTriggeredFromRive()
    {
        OnGameStart.Invoke();
    }

}
