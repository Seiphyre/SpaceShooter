using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : Singleton<GameManager>
{
    // ----- [ Attributes ] --------------------------------------------

    // --v-- Game Bounds --v--

    [Header("Game Bounds")]

    [SerializeField]
    private float _boundsPadding = 0.1f;

    [SerializeField]
    [Range(0f, 100f)]
    private float _boundsExtraPaddingTop = 0.5f;

    [SerializeField]
    private float _boundsMargin = 1f;

    private MapInfo _mapInfo = new MapInfo();

    // --v-- Player --v--

    [Header("Player")]

    [SerializeField]
    private Player _playerPrefab;

    [SerializeField]
    private Transform _playerStartPos;

    [SerializeField]
    private CurvePath _playerStartAnim;

    [SerializeField]
    private float _playerStartAnimDuration = -1;

    private Player _player;

    // --v-- Debug --v--

    [Header("Debug")]

    [SerializeField]
    private bool _shouldDisplayPadding = false;

    [SerializeField]
    private bool _shouldDisplayMargin = false;

    [SerializeField]
    private bool _shouldDisplayBounds = false;

    // --v-- Spawn Manager --v--

    private SpawnManager _spawnManager;

    // --v-- Game variables --v--

    private float _gameTime = 0;

    // --v-- Game Management --v--

    private bool _gameIsPaused = false;

    private bool _gameIsRunning = false;

    Coroutine _gameOverCoroutine;

    // --v-- Events --v--

    public event Action OnGameOverBegin;
    public event Action OnGameOverEnd;
    public event Action OnGameStartBegin;
    public event Action OnGameStartEnd;
    public event Action OnGameRestart;


    // ----- [ Getters / Setters ] --------------------------------------------

    public float BoundsPadding { get { return _boundsPadding; } }

    public float BoundsMargin { get { return _boundsMargin; } }

    public MapInfo MapInfo { get { return _mapInfo; } }

    public float GameTime { get { return _gameTime; } }

    public Player Player { get { return _player; } }



    // ----- [ Functions ] --------------------------------------------

    // --v-- Start/Awake --v--

    private void Awake()
    {
        UpdateMapBoundaryInformation();
    }

    private void Start()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();

        GameStart();
    }

    // --v-- Update --v--

    private void Update()
    {
        UpdateGameTime();
    }

    private void UpdateGameTime()
    {
        if (!_gameIsPaused && _gameIsRunning)
        {
            _gameTime += Time.deltaTime;
        }
    }

    // --v-- Game Management --v--

    public void PauseGame()
    {
        if (_gameIsPaused || !_gameIsRunning) return;

        _spawnManager.PauseSpawning();
        _gameIsPaused = true;

        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        if (!_gameIsPaused || !_gameIsRunning) return;

        _spawnManager.ResumeSpawning();
        _gameIsPaused = false;

        Time.timeScale = 1;
        // Prevoir un bullet time
    }

    private void GameOver()
    {
        _gameIsPaused = true;

        _spawnManager.StopSpawning();

        if (OnGameOverBegin != null)
            OnGameOverBegin.Invoke();

        Action callback = () =>
        {
            _gameIsRunning = false;

            if (OnGameOverEnd != null)
                OnGameOverEnd.Invoke();
        };

        _gameOverCoroutine = StartCoroutine(GameOverAnimation(callback));
    }

    private void GameStop()
    {
        _gameIsPaused = true;

        _spawnManager.StopSpawning();

        if (OnGameOverBegin != null)
            OnGameOverBegin.Invoke();

        Action callback = () =>
        {
            _gameIsRunning = false;

            if (OnGameOverEnd != null)
                OnGameOverEnd.Invoke();
        };

    }

    private void GameStart()
    {
        // Set Game Manager
        _gameIsRunning = true;

        Action onPlayerStartAnimEnd = () =>
        {
            _spawnManager.StartSpawning(1f);

            if (OnGameStartEnd != null)
                OnGameStartEnd.Invoke();
        };

        // Set Player
        InitPlayer();
        _player.FollowPath(_playerStartAnim, _playerStartAnimDuration, onPlayerStartAnimEnd);

        if (OnGameStartBegin != null)
            OnGameStartBegin.Invoke();
    }

    private void InitPlayer()
    {
        _player = Instantiate(_playerPrefab, _playerStartPos.position, _playerStartPos.rotation);
        _player.OnSelfDestroy += GameOver;
    }

    public void GameRestart()
    {
        if (_gameIsRunning)
        {
            _spawnManager.StopSpawning();

            if (_player != null)
                Destroy(_player.gameObject);
        }

        _gameTime = 0;
        Time.timeScale = 1;

        _gameIsPaused = false;

        AEntity[] entities = FindObjectsOfType<AEntity>();

        foreach (AEntity entity in entities)
            entity.Reset();

        OnGameRestart?.Invoke();

        GameStart();
    }

    // --v-- Game over animation --v--

    private IEnumerator GameOverAnimation(Action callback = null)
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

        if (callback != null)
            callback();
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
