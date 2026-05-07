using Rive;
using Rive.Components;
using System.Collections.Generic;
using UnityEngine;
using static GuttyBehaviour;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class NutriBehaviour : MonoBehaviour
{
    [SerializeField] private RiveWidget riveWidget; 
    
    [SerializeField] private float ripplePushCD = .5f;
    private float timer = 0f;

    public NutriType nutriType;

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
    }

    private void FixedUpdate()
    {
        if (timer < 0f) timer -= Time.deltaTime;
    }
    public void HandleRipplePush()
    {
        if (timer > 0f) return;

        timer = ripplePushCD; 
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