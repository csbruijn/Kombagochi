using Rive;
using Rive.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem; 

public class GuttyBehaviour : MonoBehaviour
{
    private NutriDetect NutriDetect;
    public NutriType nutriCompetibility;
    
    [Header("Health")]
    [SerializeField] private float nutriValueTest = 50;
    [SerializeField] private float decaySpeed = 1f;
    public float health { get; private set; } = 100;


   // [SerializeField] private RiveWidget statusbar;
    

    [Header("Rive widget")]
    [SerializeField] private RiveWidget Gutty;

    private ViewModelInstanceTriggerProperty EatTrig;
    [SerializeField] private string EatTrigName;

    private ViewModelInstanceNumberProperty HealthMount;
    [SerializeField] private string HealthMountName;

    private ViewModelInstanceNumberProperty EyeX;
    private ViewModelInstanceNumberProperty EyeY;
    [SerializeField] private string EyeXName = "eyeX";
    [SerializeField] private string EyeYName = "eyeY";

    private ViewModelInstanceColorProperty primaryColour;
    [SerializeField] private string colourName = "primaryColour";

    [Header("Dumb way to do colours to check if it works")]

    [SerializeField] private UnityEngine.Color red;
    [SerializeField] private UnityEngine.Color green;
    [SerializeField] private UnityEngine.Color blue;
    [SerializeField] private UnityEngine.Color yellow;


    // Start is called before the first frame update
    void Awake()
    {
        NutriDetect = GetComponentInChildren<NutriDetect>();
        if (NutriDetect == null)
        {
            Debug.LogError("No nutridetect capability attached.");
        }
        //else NutriDetect.GB = this;
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

        UpdateEyes();
    }

    private void UpdateEyes()
    {
        if (EyeX == null || EyeY == null) return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 dir = (Vector2)worldPos - (Vector2)transform.position;

        EyeX.Value = dir.x -10;
        EyeY.Value = -dir.y +10;
    }


    bool guttyVisLoaded = false;

    public bool IsReady()
    {
        if (!guttyVisLoaded) return false;  
        return true;
    }

    void OnEnable()
    {
        NutriDetect.OnEaten += HandleNutriMatch;
        Gutty.OnWidgetStatusChanged += OnWidgetStatusChanged;
    }

    void OnDisable() 
    {
        NutriDetect.OnEaten -= HandleNutriMatch;
        Gutty.OnWidgetStatusChanged -= OnWidgetStatusChanged;
    }
    
    private void OnWidgetStatusChanged()
    {

        if (Gutty.Status != WidgetStatus.Loaded)
            return;

        ViewModelInstance viewModelInstance = Gutty.StateMachine?.ViewModelInstance;
        if (viewModelInstance == null)
        {
            Debug.LogError($"{nameof(gameObject.name)}: ViewModelInstance is null. " +
                           "Make sure Data Binding Mode is set to Auto Bind Default / Selected.", this);
            return;
        }

        HealthMount = viewModelInstance.GetNumberProperty(HealthMountName);
        if (HealthMount == null)
            Debug.LogError($"Health property '{HealthMount}' not found.", this);
        
        EyeX = viewModelInstance.GetNumberProperty(EyeXName);
        if (EyeX == null)
            Debug.LogError($"Eye X property '{EyeXName}' not found.", this);

        EyeY = viewModelInstance.GetNumberProperty(EyeYName);
        if (EyeY == null)
            Debug.LogError($"Eye Y property '{EyeYName}' not found.", this);

        primaryColour = viewModelInstance.GetColorProperty(colourName);
        if (primaryColour == null)
            Debug.LogError($"colour property {colourName} not found.", this);

        // PLEASE FUTURE ME DO THIS MUCH BETTER
        switch (nutriCompetibility)
        {
            case NutriType.Blue: primaryColour.Value = blue;
                break;
            case NutriType.Green:
                primaryColour.Value = green;
                break;
            case NutriType.Red:
                primaryColour.Value = red;
                break;
            case NutriType.Yellow:
                primaryColour.Value = yellow;
                break;

        }

        guttyVisLoaded = true;

    }

    public enum GuttyStates
    {
        happy,
        satisfied,
        concerned,
        dead
    }
}
