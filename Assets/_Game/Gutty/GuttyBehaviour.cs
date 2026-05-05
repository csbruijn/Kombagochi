using Rive;
using Rive.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuttyBehaviour : MonoBehaviour
{
    private NutriDetect NutriDetect;
    [SerializeField] private float nutriValueTest = 50;
    [SerializeField] private float decaySpeed = 1f;

    [SerializeField] private RiveWidget statusbar;
    private ViewModelInstanceNumberProperty HealthMount;
    [SerializeField] private string HealthMountName;

    [SerializeField] private RiveWidget Gutty;
    private ViewModelInstanceTriggerProperty EatTrig;
    [SerializeField] private string EatTrigName;

    public float health
    {
        get;
        private set;
    } = 100; 

    // Start is called before the first frame update
    void Awake()
    {
        NutriDetect = GetComponentInChildren<NutriDetect>();
        if (NutriDetect == null)
        {
            Debug.LogError("No nutridetect capability attached.");
        }
        else NutriDetect.GB = this;
    }

    public void HandleNutriMatch(NutriBehaviour nb)
    {
        Debug.Log("Eat nutri");
        Destroy(nb.gameObject);
        SetHealth(health + nutriValueTest);
    }

    public void SetHealth(float val)
    {
        if (!IsReady())
        {
            Debug.LogError($"Tried setting Health of {this.gameObject} while not initialized!");
            return;
        }

        float newVal;
        if (val > 0 && val < 100)
            newVal = val;
        else if (val < 0)
            newVal = 0;
        else
            newVal = 100;
        health = newVal;

        HealthMount.Value = newVal;   
    }

    private void Update()
    {
        if (!IsReady()) return;
 
        if (GameManager.instance.GameStarted)
        SetHealth(health - Time.deltaTime*decaySpeed);
    }

    bool statusBarLoaded = false;
    bool guttyVisLoaded = false;

    public bool IsReady()
    {
        if (!statusBarLoaded) return false;
        if (!guttyVisLoaded) return false;  
            
        return true;
    }

    void OnEnable()
    {
        statusbar.OnWidgetStatusChanged += OnStatusBarStatusChanged;
        Gutty.OnWidgetStatusChanged += OnGuttyStatusChanged;
    }

    void OnDisable() 
    {
        statusbar.OnWidgetStatusChanged -= OnStatusBarStatusChanged;
        Gutty.OnWidgetStatusChanged -= OnGuttyStatusChanged;
    }
    
    private void OnStatusBarStatusChanged()
    {
        if (statusbar.Status != WidgetStatus.Loaded)
            return;

        ViewModelInstance viewModelInstance = statusbar.StateMachine?.ViewModelInstance;
        if (viewModelInstance == null)
        {
            Debug.LogError($"{nameof(gameObject.name)}: ViewModelInstance is null. " +
                           "Make sure Data Binding Mode is set to Auto Bind Default / Selected.", this);
            return;
        }

        HealthMount = viewModelInstance.GetNumberProperty(HealthMountName);
        if (HealthMount == null)
        {
            Debug.LogError($"{nameof(gameObject.name)}: Trigger property '{HealthMount}' not found.", this);
            return;
        }
        statusBarLoaded = true;
    }

    private void OnGuttyStatusChanged()
    {

        if (Gutty.Status != WidgetStatus.Loaded)
            return;

        //ViewModelInstance viewModelInstance = Gutty.StateMachine?.ViewModelInstance;
        //if (viewModelInstance == null)
        //{
        //    Debug.LogError($"{nameof(this.name)}: ViewModelInstance is null. " +
        //                   "Make sure Data Binding Mode is set to Auto Bind Default / Selected.", this);
        //    return;
        //}

        //EatTrig = viewModelInstance.GetTriggerProperty(EatTrigName);
        //if (EatTrig == null)
        //{
        //    Debug.LogError($"{nameof(this.name)}: Trigger property '{EatTrig}' not found.", this);
        //    return;
        //}
        guttyVisLoaded = true;

    }
}
