using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpawnManager : MonoBehaviour
{
    // ------ [ Attributes ] ---------------------------------------------------


    // --v-- Spawn Information --v--

    [Header("Spawn Settings")]

    [SerializeField]
    private Transform _enemyContainer = null;

    [SerializeField]
    private int _spawnQuantity = 10;

    [SerializeField]
    private List<AEnemy> _enemyPrefabs = null;

    private Dictionary<int, Spawn> _spawns = new Dictionary<int, Spawn>();

    private float _nextSpawnTime = 0f;

    private float _time = 0f;

    [SerializeField]
    private bool _isPaused = true;

    // --v-- Spawn Difficulty --v--

    [Header("Difficulty")]

    [SerializeField]
    private AnimationCurve _spawnFrequencyCurve;

    [SerializeField]
    private AnimationCurve _spawnQuantityCurve;

    // --v-- Debug --v--

    [Header("Debug")]

    [SerializeField]
    private bool _shouldDisplaySpawns = false;

    private bool _spawnsHaveBeenCreated = false;



    // ------ [ Functions ] -----------------------------------------------------



    // --v-- Unity Messages --v--

    private void Start()
    {
        CreateSpawns();
        _spawnsHaveBeenCreated = true;

        if (_spawnFrequencyCurve.length == 0)
        {
            _spawnFrequencyCurve = AnimationCurve.Constant(0, 1, 5);
            Debug.LogWarning(" [SpawnManager] _spawnFrequencyCurve is empty. Default value has been set.");
        }

        if (_spawnQuantityCurve.length == 0)
        {
            _spawnQuantityCurve = AnimationCurve.Constant(0, 1, 1);
            Debug.LogWarning(" [SpawnManager] _spawnQuantityCurve is empty. Default value has been set.");
        }
    }

    private void Update()
    {
        if (!_isPaused)
        {
            _time += Time.deltaTime;

            CalculateSpawn();
        }
    }

    // --v-- Spawn status --v--

    public void PauseManager()
    {
        _isPaused = true;
    }

    public void StartManager(float delay = 0)
    {
        _isPaused = false;

        _nextSpawnTime = _time + delay;
    }

    public void ResumeManager()
    {
        _isPaused = false;
    }

    public void StopAndReset()
    {
        PauseManager();
        _time = 0f;

        // Reset Enemies
    }

    // --v-- Spawn Creation --v--

    private void CreateSpawns()
    {
        MapInfo map = GameManager.Instance.GetMapInfo();

        float availableHeight = map.BoundaryIn.Max.y - map.BoundaryIn.Min.y;

        float spawnHeight = availableHeight / _spawnQuantity;
        float spawnX = map.Boundary.Max.x;
        float spawnY = -1f;

        int i = 0;
        while (i < _spawnQuantity)
        {
            spawnY = map.BoundaryIn.Max.y - (i * spawnHeight) - (spawnHeight / 2);
            //Debug.Log("" + map.BoundaryIn.Max.y + " - (" + i + " * " + spawnHeight + ") - (" + spawnHeight + " / 2)");
            _spawns.Add(i + 1, new Spawn(new Vector2(spawnX, spawnY), new Vector2(-1, spawnHeight), i + 1));
            i++;
        }


    }

    // --v-- Calculate Spawn --v--

    private void CalculateSpawn()
    {
        // Time to spawn
        if (_time >= _nextSpawnTime)
        {
            UpdateNextSpawnTime();

            int enemyQuantity = Mathf.FloorToInt(_spawnQuantityCurve.Evaluate(_time));

            for (int i = 1; i <= enemyQuantity; i++)
            {
                Spawn spawn = null;
                spawn = FindAvailableSpawn();

                if (spawn != null)
                {
                    SpawnEnemy(spawn);
                }
                else
                {
                    Debug.Log("It's time to create a Queue system ...");
                    // Add to queue
                }
            }
        }
    }

    private void UpdateNextSpawnTime()
    {
        _nextSpawnTime += _spawnFrequencyCurve.Evaluate(_time);
    }

    private void SpawnEnemy(Spawn spawn)
    {
        AEnemy instanciatedEnemy;

        int randomId = Random.Range(0, _enemyPrefabs.Count);
        instanciatedEnemy = spawn.Instanciate(_enemyPrefabs[randomId]);

        if (instanciatedEnemy != null)
            instanciatedEnemy.transform.parent = _enemyContainer;
    }

    // --v-- Spawn List Sorting --v--

    private Spawn FindAvailableSpawn()
    {
        KeyValuePair<int, Spawn> result;
        System.Random rand = new System.Random();

        result = _spawns.OrderBy(x => rand.Next()).FirstOrDefault(keyPairValue => keyPairValue.Value.IsAvailable() == true);

        if (result.Equals(default(KeyValuePair<int, Spawn>)))
            return null;

        return result.Value;
    }

    // --v-- Gizmo --v--

    public void OnDrawGizmos()
    {
        if (_shouldDisplaySpawns)
        {
            float availableHeight = Camera.main.GetBoundary().Max.y - Camera.main.GetBoundary().Min.y;

            float spawnHeight = availableHeight / _spawnQuantity;

            int i = 0;
            while (i < _spawnQuantity)
            {
                Gizmos.color = Color.white;
                if (Application.isPlaying && _spawnsHaveBeenCreated)
                {
                    if (_spawns[i + 1].IsAvailable())
                        Gizmos.color = Color.green;
                    else
                        Gizmos.color = Color.red;
                }

                Gizmos.DrawSphere(new Vector3(Camera.main.GetBoundary().Max.x, Camera.main.GetBoundary().Max.y - (i * spawnHeight) - (spawnHeight / 2), 0f), 0.3f);

                i++;
            }
        }
    }

}
