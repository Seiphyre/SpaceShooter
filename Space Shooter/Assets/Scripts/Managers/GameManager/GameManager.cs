using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{
    // --v-- Visible in Editor --v--

    [Header("Bounds")]

    [SerializeField]
    private float _boundsPadding = 0.1f;

    [SerializeField]
    [Range(0f, 100f)]
    private float _boundsExtraPaddingTop = 0.5f;

    [SerializeField]
    private float _boundsMargin = 1f;

    [Header("Player")]

    [SerializeField]
    private Player _playerPrefab;

    [SerializeField]
    private Transform _playerStartPos;

    [SerializeField]
    private CurvePath _playerStartAnim;

    [SerializeField]
    private float _playerStartAnimDuration = -1;

    [Header("Debug")]

    [SerializeField]
    private bool _shouldDisplayPadding = false;

    [SerializeField]
    private bool _shouldDisplayMargin = false;

    [SerializeField]
    private bool _shouldDisplayBounds = false;

    // --v-- Private Attributes --v--

    private MapInfo _mapInfo = new MapInfo();

    private Player _player;

    private SpawnManager _spawnManager;

    private float _gameTime = 0;

    private bool _gameIsPaused = true;

    private bool _gameIsRunning = false;



    public event Action OnGameOver;
    public event Action OnGameStartBegin;
    public event Action OnGameStartEnd;
    // public event Action OnGamePause;

    // ----------------------------------------------------


    public float GetBoundsPadding() { return _boundsPadding; }
    public float GetBoundsMargin() { return _boundsMargin; }
    public MapInfo GetMapInfo() { return _mapInfo; }
    public float GetGameTime() { return _gameTime; }

    public Player Player { get { return _player; } }


    // ----------------------------------------------------

    private void Awake()
    {
        UpdateMapBoundaryInformation();
    }

    private void Start()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();

        GameStart();
    }

    private void Update()
    {
        if (!_gameIsPaused && _gameIsRunning)
        {
            _gameTime += Time.deltaTime;
        }
    }

    public void PauseGame()
    {
        if (_gameIsPaused /*|| !_gameIsRunning*/) return;

        _spawnManager.PauseManager();
        _gameIsPaused = true;

        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        if (!_gameIsPaused /*|| !_gameIsRunning*/) return;

        _spawnManager.ResumeManager();
        _gameIsPaused = false;

        Time.timeScale = 1;
        // Prevoir un bullet time
    }

    private void InitPlayer()
    {
        _player = Instantiate(_playerPrefab, _playerStartPos.position, _playerStartPos.rotation);
        _player.OnSelfDestroy += GameOver;
    }

    private void GameOver()
    {
        _spawnManager.PauseManager();
        _gameIsRunning = false;
        _gameIsPaused = true;

        StartCoroutine(FinalBulletTime());

        if (OnGameOver != null)
            OnGameOver.Invoke();
    }

    private void GameStart()
    {
        // Set Game Manager
        _gameIsPaused = false;

        Action onPlayerStartAnimEnd = () =>
        {
            _spawnManager.StartManager(1f);
            _gameIsRunning = true;
            if (OnGameStartEnd != null)
                OnGameStartEnd.Invoke();
        };

        // Set Player
        InitPlayer();
        _player.FollowPath(_playerStartAnim, _playerStartAnimDuration, onPlayerStartAnimEnd);

        if (OnGameStartBegin != null)
            OnGameStartBegin.Invoke();
    }

    public void GameRestart()
    {
        _gameTime = 0;
        Time.timeScale = 1;
        _spawnManager.StopAndReset();

        AEntity[] entities = FindObjectsOfType<AEntity>();

        foreach (AEntity entity in entities)
            entity.Reset();

        GameStart();
    }

    private IEnumerator FinalBulletTime()
    {
        float duration = 1f;
        float t = 0;

        while (t <= 1)
        {
            Time.timeScale = Mathf.Lerp(1, 0, t);

            t += Time.unscaledDeltaTime / duration;

            yield return null;
        }

        Time.timeScale = 0f;
    }

    private void UpdateMapBoundaryInformation()
    {
        _mapInfo.Boundary = Camera.main.GetBoundary();

        Vector2 minBoundaryIn = new Vector2(_mapInfo.Boundary.Min.x + _boundsPadding, _mapInfo.Boundary.Min.y + _boundsPadding);
        Vector2 maxBoundaryIn = new Vector2(_mapInfo.Boundary.Max.x - _boundsPadding, _mapInfo.Boundary.Max.y - _boundsPadding - GetExtraPaddingTopValue());
        _mapInfo.BoundaryIn = new MinMaxBounds(minBoundaryIn, maxBoundaryIn);

        Vector2 minBoundaryOut = new Vector2(_mapInfo.Boundary.Min.x - _boundsMargin, _mapInfo.Boundary.Min.y - _boundsMargin);
        Vector2 maxBoundaryOut = new Vector2(_mapInfo.Boundary.Max.x + _boundsMargin, _mapInfo.Boundary.Max.y + _boundsMargin);
        _mapInfo.BoundaryOut = new MinMaxBounds(minBoundaryOut, maxBoundaryOut);
    }

    private float GetExtraPaddingTopValue()
    {
        float lenght = Camera.main.GetBoundary().Max.y - Camera.main.GetBoundary().Min.y - (_boundsPadding * 2);

        return Mathf.Lerp(0f, lenght, _boundsExtraPaddingTop / 100f);
    }

    public void OnDrawGizmos()
    {
        MinMaxBounds camBoundary = Camera.main.GetBoundary();

        if (_shouldDisplayPadding)
        {
            Vector3 a = new Vector3(camBoundary.Max.x - _boundsPadding, camBoundary.Min.y + _boundsPadding);
            Vector3 b = new Vector3(camBoundary.Max.x - _boundsPadding, camBoundary.Max.y - _boundsPadding - GetExtraPaddingTopValue());
            Vector3 c = new Vector3(camBoundary.Min.x + _boundsPadding, camBoundary.Max.y - _boundsPadding - GetExtraPaddingTopValue());
            Vector3 d = new Vector3(camBoundary.Min.x + _boundsPadding, camBoundary.Min.y + _boundsPadding);

            Gizmos.color = Color.red;

            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, d);
            Gizmos.DrawLine(d, a);
        }

        if (_shouldDisplayMargin)
        {
            Vector3 a = new Vector3(camBoundary.Max.x + _boundsMargin, camBoundary.Min.y - _boundsMargin);
            Vector3 b = new Vector3(camBoundary.Max.x + _boundsMargin, camBoundary.Max.y + _boundsMargin);
            Vector3 c = new Vector3(camBoundary.Min.x - _boundsMargin, camBoundary.Max.y + _boundsMargin);
            Vector3 d = new Vector3(camBoundary.Min.x - _boundsMargin, camBoundary.Min.y - _boundsMargin);

            Gizmos.color = Color.red;

            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, d);
            Gizmos.DrawLine(d, a);
        }

        if (_shouldDisplayBounds)
        {
            Vector3 a = new Vector3(camBoundary.Max.x, camBoundary.Min.y);
            Vector3 b = new Vector3(camBoundary.Max.x, camBoundary.Max.y);
            Vector3 c = new Vector3(camBoundary.Min.x, camBoundary.Max.y);
            Vector3 d = new Vector3(camBoundary.Min.x, camBoundary.Min.y);

            Gizmos.color = Color.white;

            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, d);
            Gizmos.DrawLine(d, a);
        }
    }
}

public class MapInfo
{
    public MinMaxBounds Boundary;
    public MinMaxBounds BoundaryIn;
    public MinMaxBounds BoundaryOut;

    public MapInfo()
    {
        Boundary = new MinMaxBounds();
        BoundaryIn = new MinMaxBounds();
        BoundaryOut = new MinMaxBounds();
    }
}
