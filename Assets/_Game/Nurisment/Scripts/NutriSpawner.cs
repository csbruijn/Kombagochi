using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NutriSpawner : MonoBehaviour
{
    [SerializeField] private GameObject nutriPrefab;
    [SerializeField] private List<GameObject> activeNutris = new List<GameObject>();

    [SerializeField] private int maxPassiveNutri;
    [SerializeField] private float timeInbetweenSpawns = 5f;

    [SerializeField] private Vector3 spawnPoint = Vector3.zero;

    private void OnEnable() =>
        NutriDetect.OnEaten += HandleNutriEaten;
    
    private void OnDisable() =>
        NutriDetect.OnEaten -= HandleNutriEaten;
    
    private void Start()
    {
        StartCoroutine(PeriodicSpawnNutri());
    }

    private IEnumerator PeriodicSpawnNutri()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeInbetweenSpawns);
            //Debug.Log($"Acitve nutri: {activeNutris.Count}");

            if (activeNutris.Count < maxPassiveNutri) SpawnNutri(spawnPoint);
        }
    }

    public void SpawnNutri(Vector3 position)
    {
        GameObject obj = Instantiate(nutriPrefab, position, Quaternion.identity);
        activeNutris.Add(obj);
        NutriBehaviour nb = obj.GetComponent<NutriBehaviour>();
        nb.spawnedFrom = this;
        nb.RandomType();
        Vector2 randomDir = new Vector2(Random.RandomRange(-1,1) ,Random.RandomRange(-1,1) );  
        obj.GetComponent<Rigidbody2D>().AddForce(randomDir.normalized * 0.5f, ForceMode2D.Impulse);

    }

    private void HandleNutriEaten(NutriBehaviour nutri, GuttyBehaviour gb)
    {
        activeNutris.Remove(nutri.gameObject);
        Destroy(nutri.gameObject);
    }

    public void HandleDespawn(NutriBehaviour nb)
    {
        activeNutris.Remove(nb.gameObject);
        Destroy(nb.gameObject);
    }
}