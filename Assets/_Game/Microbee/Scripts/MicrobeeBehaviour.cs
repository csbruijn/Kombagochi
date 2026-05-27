using Rive;
using Rive.Components;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem; 

public class MicrobeeBehaviour : MonoBehaviour
{
    public bool isSimulated { get; private set; }  

    private NutriDetect NutriDetect;
    public NutriType nutriCompetibility;
    
    [Header("Health")]
    [SerializeField] private float decaySpeed = 1f;
    public float health { get; private set; } = 50;
    public float size { get; private set; } = 0;
    bool markedToSplit = false;

    [Header("fattyAcid")]
    [SerializeField] private bool spawnNutri;
    [SerializeField] private GameObject NutriFattyAcid;

    [Header("Rive widget")]
    bool guttyVisLoaded = false;
    [SerializeField] private RiveWidget Gutty;

    private ViewModelInstanceTriggerProperty EatTrig;
    [SerializeField] private string EatTrigName;

    private ViewModelInstanceNumberProperty HealthMount;
    [SerializeField] private string HealthMountName = "health";

    private ViewModelInstanceNumberProperty EyeX;
    private ViewModelInstanceNumberProperty EyeY;
    [SerializeField] private string EyeXName = "followX";
    [SerializeField] private string EyeYName = "followY";

    private ViewModelInstanceColorProperty primaryColour;
    [SerializeField] private string colourName = "colourPrimary";

    [Header("Dumb way to do colours to check if it works")]

    [SerializeField] private UnityEngine.Color red;
    [SerializeField] private UnityEngine.Color green;
    [SerializeField] private UnityEngine.Color blue;
    [SerializeField] private UnityEngine.Color yellow;



    void Awake()
    {
        if (isSimulated) return;
        NutriDetect = GetComponentInChildren<NutriDetect>();
        if (NutriDetect == null) 
            Debug.LogError("No nutridetect capability attached.");
    }

    private void Update()
    {
        if (!IsReady()) return;

        if (GameManager.instance.GameStarted && !isSimulated)
            SetHealth(health - Time.deltaTime * decaySpeed);

        UpdateEyes();
    }

    public void HandleNutriMatch(NutriBehaviour nb, MicrobeeBehaviour gb)
    {
        if (gb != this) return;

        if (health <= 0) return; 
        
        Debug.Log("Eat nutri");
        Destroy(nb.gameObject);
        SetHealth(health + nb.nutriValue);
        SetSize(size + 1);


        if (!spawnNutri) return;
        SpawnFattyAcid();
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

    public void SetSize(float val)
    {
        if (!IsReady())
        {
            Debug.LogError($"Tried setting Health of {this.gameObject} while not initialized!");
            return;
        }
        if (markedToSplit) return; 

        float newVal;

        if (val >= 10)
        {
            newVal = 10; 
            markedToSplit = true;
            StartCoroutine(SplitMicrobee(0)); //PERHAPS HERE I TIME IT WITH THE ANIMATOR
        }
        else if (val > 0 && val < 10)
        {
            newVal = val;
        }
        else
        {
            newVal = 0;
        }

        size = newVal;

        float scale = (20 + val) / 20;  
        transform.localScale = new Vector3(scale, scale, scale);
    }

    private void UpdateEyes()
    {
        if (EyeX == null || EyeY == null) return;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 dir = (Vector2)worldPos - (Vector2)transform.position;

        EyeX.Value = dir.x -10;
        EyeY.Value = -dir.y +10;
    }

    private void SpawnFattyAcid()
    {
        NutriType fattyType;

        switch (nutriCompetibility)
        {
            case NutriType.Blue:
                fattyType = NutriType.Green;
                break;
            case NutriType.Green:
                fattyType = NutriType.Red;
                break;
            case NutriType.Red:
                fattyType = NutriType.Yellow;
                break;
            case NutriType.Yellow:
                fattyType = NutriType.Blue;
                break;
            default:
                fattyType = NutriType.Yellow;
                Debug.LogError($"Unrecognised nutriType {nutriCompetibility}", this);
                break;
        }

        Vector2 pos = new Vector2(transform.position.x, transform.position.y + 2);
        GameObject obj = Instantiate(NutriFattyAcid, pos, Quaternion.identity);
        obj.GetComponent<NutriBehaviour>().nutriType = fattyType;

        obj.GetComponent<Rigidbody2D>().AddForce(Vector2.up, ForceMode2D.Impulse);
    }

    private IEnumerator SplitMicrobee(float delay)
    {
        yield return new WaitForSeconds(delay);
        Spawner.instance.OnSpawnDeterminedMicrobee(nutriCompetibility);
        markedToSplit = false;
        SetSize(0);
    }

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

        // PLEASE FUTURE ME DO THIS BETTER
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

    public void RandomType()
    {
        nutriCompetibility = (NutriType)Random.Range(0, System.Enum.GetValues(typeof(NutriType)).Length);
    }

    public void SetSimulated (bool state)
    {
        isSimulated = state;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = !state;  
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        col.isTrigger = state;
    }

    public enum GuttyStates
    {
        happy,
        satisfied,
        concerned,
        dead
    }
}
