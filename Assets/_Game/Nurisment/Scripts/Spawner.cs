using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner instance;

    [Header("NutriSettings")]
    [SerializeField] private GameObject nutriPrefab;
    [SerializeField] private float timeInbetweenSpawns = .1f;
    [SerializeField] private int spawnCount = 10;
   
    [Header("Microbee")]
    [SerializeField] private GameObject microbeePrefab;
    [SerializeField, Range(0,1)] private float chanceToSpawn = 0.2f; 

    [Header("Spawnsettings")]
    [SerializeField] private Vector3 spawnPoint = Vector3.zero;
    [SerializeField] private float spawnOffsetY = 1;
    [SerializeField] private float speed = 1f;

    [SerializeField]

    private void Awake()
    {
        if (instance == null)
        instance = this; 
        else
            Destroy(this);
    }

    private void Start()
    {
        OnSpawnWaveGuaranteeMicrobee();
    }

    public void OnSpawnWave()
    {
        StartCoroutine(SpawnWave());
    }

    public void OnSpawnWaveGuaranteeMicrobee()
    {
        StartCoroutine(SpawnWave(true));
    }

    public void OnSpawnRandomMicrobee()
    {
        Vector2 calcSpawn = new Vector2(spawnPoint.x, spawnPoint.y + Random.RandomRange(-spawnOffsetY, spawnOffsetY));
        GameObject obj = SpawnMicrobee(calcSpawn); 
        MicrobeeBehaviour mb = obj.GetComponent<MicrobeeBehaviour>();
        mb.SetSimulated(true);
        mb.RandomType();
    }

    public void OnSpawnDeterminedMicrobee(NutriType compatibilityType)
    {
        Vector2 calcSpawn = new Vector2(spawnPoint.x, spawnPoint.y + Random.RandomRange(-spawnOffsetY, spawnOffsetY));
        GameObject obj =  SpawnMicrobee(calcSpawn);
        MicrobeeBehaviour mb = obj.GetComponent<MicrobeeBehaviour>();
        mb.SetSimulated(true);
        mb.nutriCompetibility = compatibilityType;
    }

    private IEnumerator SpawnWave(bool guaranteeMicrobee = default)
    {
        if (guaranteeMicrobee == default) guaranteeMicrobee = false;

        int i = 0;
        while (i < spawnCount)
        {
            Vector2 calcSpawn = new Vector2(spawnPoint.x, spawnPoint.y + Random.RandomRange(-spawnOffsetY, spawnOffsetY)); 
            GameObject obj  = SpawnNutri(calcSpawn);
            NutriBehaviour nb = obj.GetComponent<NutriBehaviour>();
            nb.RandomType();
            i++;
            yield return new WaitForSeconds(timeInbetweenSpawns);
        }

        if (guaranteeMicrobee || chanceToSpawn > Random.Range(0f, 1f))
        {
            OnSpawnRandomMicrobee();
        }
    }

    private GameObject SpawnNutri(Vector3 position)
    {
        GameObject obj = Instantiate(nutriPrefab, position, Quaternion.identity);

        Vector2 randomDir = Vector2.right;
        obj.GetComponent<Rigidbody2D>().AddForce(randomDir.normalized * speed, ForceMode2D.Impulse);
        return obj;
    }

    private GameObject SpawnMicrobee(Vector3 position)
    {
        GameObject obj = Instantiate(microbeePrefab, position, Quaternion.identity);
        Vector2 randomDir = Vector2.right;
        obj.GetComponent<Rigidbody2D>().AddForce(randomDir.normalized * speed, ForceMode2D.Impulse);
        return obj;

    }

}
