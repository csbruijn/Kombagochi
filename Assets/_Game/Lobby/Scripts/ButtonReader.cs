using Rive;
using Rive.Components;
using UnityEngine;
using UnityEngine.Events;


public class ButtonReader : MonoBehaviour
{
    [Header("Rive")]
    [Tooltip("The Rive Widget that is displaying your file.")]
    [SerializeField] private RiveWidget m_riveWidget;

    [Tooltip("ViewModel Trigger property name fired by the Rive file.")]
    [SerializeField] private string mOnTriggerPropName = "Trig";

    private ViewModelInstanceTriggerProperty OnTriggerProp;

    [Tooltip("Invoked when the Rive file fires the gameOver trigger.")]
    public UnityEvent OnGameStart = new UnityEvent();

    [SerializeField] private string buttonTxPropNamet = "buttonText";
    [SerializeField] private string buttonTxt = "Redeem & Play"; 

    private bool isReady = false;

    public void CheckIfFilled(string val)
    {
        if (val != null || val != "")
        {
            isReady = true;
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
        OnTriggerProp = viewModelInstance.GetTriggerProperty(mOnTriggerPropName);
        if (OnTriggerProp == null)
        {
            Debug.LogError($"{nameof(ButtonReader)}: Trigger property '{mOnTriggerPropName}' not found.", this);
            return;
        }

        OnTriggerProp.OnTriggered += HandleButtonTriggeredFromRive;

        // Get the gameOver property by name.
        ViewModelInstanceStringProperty buttonTxtProp = viewModelInstance.GetStringProperty(buttonTxPropNamet);
        if (buttonTxtProp == null)
        {
            Debug.LogError($"{nameof(ButtonReader)}: Trigger property '{buttonTxPropNamet}' not found.", this);
            return;
        }

        buttonTxtProp.Value = buttonTxt; 
    }


    private int foolproof = 0; 
    private void HandleButtonTriggeredFromRive()
    {
        if (!isReady)
        {
            foolproof++;
            if (foolproof > 2) isReady = true; 
            return ;
        }
        OnGameStart.Invoke();
    }

}
