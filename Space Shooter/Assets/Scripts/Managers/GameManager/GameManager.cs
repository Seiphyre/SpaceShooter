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

    private float _gameTime;

    private bool _shouldPlayIntro;
    private bool _shouldPlayOutro;

    // --v-- Game Management --v--

    //GameStatus _gameStatus = GameStatus.IS_NOT_RUNNING;
    //GameStatus _previousGameStatus = GameStatus.IS_NOT_RUNNING;

    GameState _currentState;

    GameState _previousGameState;

    Coroutine _gameOverCoroutine;

    // --v-- Events --v--

    public event Action OnGameInit;
    public event Action OnGameStart;
    public event Action OnGameStop;
    public event Action OnGameIsOver;
    public event Action OnGameTerminate;


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

        _gameTime = 0;
        _shouldPlayIntro = true;
        _shouldPlayOutro = true;

        _currentState = GameState.NOT_RUNNING;
        _previousGameState = GameState.NOT_RUNNING;
    }

    private void Start()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();

        LaunchGame(_shouldPlayIntro);
    }


    // --v-- Update --v--

    private void Update()
    {
        UpdateGameTime();
    }

    private void UpdateGameTime()
    {
        if (_currentState == GameState.RUNNING)
        {
            _gameTime += Time.deltaTime;
        }
    }


    // --v-- Event Handler --v--

    private void HandleOnPlayerOnDestruction(EntityDestructionContext context)
    {
        if (context == EntityDestructionContext.DESTROYED_BY_DAMAGE)
        {
            OnGameIsOver?.Invoke();
            StopGame(_shouldPlayOutro);
        }
    }


    // --v-- Game Management --v--

    // private void SetState(GameState state)
    // {
    //     if (state != null && _currentGameState != null && state.GetType().Equals(_currentGameState.GetType())) return;

    //     if (_currentGameState != null)
    //         _currentGameState.OnStateExit();

    //     _currentGameState = state;

    //     if (_currentGameState != null)
    //         _currentGameState.OnStateEnter();
    // }

    public void PauseGame()
    {
        if (_currentState == GameState.NOT_RUNNING) return;

        _spawnManager.PauseSpawning();

        Time.timeScale = 0;

        _previousGameState = _currentState;
        _currentState = GameState.IS_PAUSED;
    }

    public void ResumeGame()
    {
        if (_currentState != GameState.IS_PAUSED) return;

        _spawnManager.ResumeSpawning();

        Time.timeScale = 1;

        _currentState = GameState.RUNNING;
    }

    public void LaunchGame(bool enableIntro = true)
    {
        InitGame();

        if (enableIntro)
            PlayGameIntro(StartGame);
        else
            StartGame();
    }

    private void InitPlayer()
    {
        if (_player == null)
        {
            _player = Instantiate(_playerPrefab, _playerStartPos.position, _playerStartPos.rotation);
            _player.OnDestruction += HandleOnPlayerOnDestruction;
        }
        else
        {
            _player.transform.position = _playerStartPos.position;
            _player.transform.rotation = _playerStartPos.rotation;
        }
    }

    public void RestartGame(bool _shouldLaunchGameAgain = true)
    {
        // Resume the game
        if (_currentState == GameState.IS_PAUSED)
        {
            ResumeGame();
        }

        if (_currentState != GameState.NOT_RUNNING)
        {
            // End the game
            if (_currentState == GameState.INIT || _currentState == GameState.PLAY_INTRO)
            {
                CancelGameIntro();
                TerminateGame();
            }

            else if (_currentState == GameState.RUNNING)
            {
                StopGame(false);
            }

            else if (/*_currentState == GameState.STOP || */_currentState == GameState.PLAY_OUTRO/* || _currentState == GameState.TERMINATE*/)
            {
                CancelGameOutro();
                TerminateGame();
            }

            // Reset the game
            ResetGame();
        }

        // Start the game again
        if (_shouldLaunchGameAgain)
        {
            LaunchGame();
        }
    }

    public void QuitGame()
    {

        RestartGame(false);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }

    private void InitGame()
    {
        _currentState = GameState.INIT;

        InitPlayer();

        OnGameInit?.Invoke();
    }

    private void StartGame()
    {
        _spawnManager.StartSpawning(1f);

        OnGameStart?.Invoke();

        _currentState = GameState.RUNNING;
    }

    private void ResetGame()
    {
        _gameTime = 0;
        Time.timeScale = 1;

        AEntity[] entities = FindObjectsOfType<AEntity>();

        foreach (AEntity entity in entities)
            DestroyImmediate(entity.gameObject);
    }

    private void StopGame(bool enableOutro = true)
    {
        _currentState = GameState.STOP;

        // Stop spawning enemies
        _spawnManager.StopSpawning();

        // Invoke Event
        OnGameStop?.Invoke();

        if (enableOutro)
            PlayGameOutro(TerminateGame);
        else
            TerminateGame();
    }

    private void TerminateGame()
    {
        _currentState = GameState.TERMINATE;

        OnGameTerminate?.Invoke();
    }

    // --v-- Intro --v--

    private void PlayGameIntro(Action callback = null)
    {
        _currentState = GameState.PLAY_INTRO;

        _player.FollowPath(_playerStartAnim, _playerStartAnimDuration, callback);
    }

    private void CancelGameIntro()
    {
        _player.StopFollowingPath();
    }

    // --v-- Outro --v--

    private void PlayGameOutro(Action callback = null)
    {
        _gameOverCoroutine = StartCoroutine(PlayGameOutroRoutine(callback));
    }

    private IEnumerator PlayGameOutroRoutine(Action callback = null)
    {
        _currentState = GameState.PLAY_OUTRO;

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

        _gameOverCoroutine = null;
    }

    private void CancelGameOutro()
    {
        if (_gameOverCoroutine != null)
        {
            StopCoroutine(_gameOverCoroutine);
            _gameOverCoroutine = null;
        }
    }

    // --v-- Boundary / Map --v--

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


    // --v-- Draw Gizmos --v--

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



    // -- State Machine -----------------------------------------------------

    //     public abstract class GameState
    //     {
    //         protected GameManager _gm;

    //         public virtual void OnStateEnter() { }
    //         public virtual void OnStateExit() { }

    //         public GameState(GameManager gameManage)
    //         {
    //             _gm = gameManage;
    //         }
    //     }

    //     public class GameIsInit : GameState
    //     {
    //         public GameIsInit(GameManager gameManager) : base(gameManager) { }

    //         public override void OnStateEnter()
    //         {
    //             _gm.InitGame();
    //         }
    //     }

    //     public class GameIsPlayingIntro : GameState
    //     {
    //         public GameIsPlayingIntro(GameManager gameManager) : base(gameManager) { }

    //         public override void OnStateEnter()
    //         {
    //             _gm.PlayIntroAnimation(_gm.StartGame);
    //         }
    //     }

    //     public class GameIsStarting : GameState
    //     {
    //         public GameIsStarting(GameManager gameManager) : base(gameManager) { }

    //         public override void OnStateEnter()
    //         {
    //             _gm.StartGame();
    //         }
    //     }

    //     public class GameIsRunning : GameState
    //     {
    //         public GameIsRunning(GameManager gameManager) : base(gameManager) { }
    //     }

    //     public class GameIsStopping : GameState
    //     {
    //         public GameIsStopping(GameManager gameManager) : base(gameManager) { }

    //         public override void OnStateEnter()
    //         {
    //             _gm.StopGame();
    //         }
    //     }

    //     public class GameIsPlayingOutro : GameState
    //     {

    //         public GameIsPlayingOutro(GameManager gameManager) : base(gameManager) { }

    //         public override void OnStateEnter()
    //         {
    //             _gm._gameOverCoroutine = _gm.StartCoroutine(_gm.PlayOutroAnimationRoutine(_gm.StopGame));
    //         }
    //     }

    //     public class GameIsTerminate : GameState
    //     {
    //         public GameIsTerminate(GameManager gameManager) : base(gameManager) { }

    //         public override void OnStateEnter()
    //         {
    //             _gm.TerminateGame();
    //         }
    //     }

    //     public class GameIsNotRunning : GameState
    //     {
    //         public GameIsNotRunning(GameManager gameManager) : base(gameManager) { }
    //     }

    //     public class GameIsPaused : GameState
    //     {
    //         public GameIsPaused(GameManager gameManager) : base(gameManager) { }

    //         public override void OnStateEnter()
    //         {
    //             _gm._spawnManager.PauseSpawning();

    //             Time.timeScale = 0;
    //         }

    //         public override void OnStateExit()
    //         {
    //             _gm._spawnManager.ResumeSpawning();

    //             Time.timeScale = 1;
    //         }
    //     }
}

public enum GameState
{
    INIT,
    PLAY_INTRO,
    RUNNING,
    STOP,
    PLAY_OUTRO,
    TERMINATE,
    NOT_RUNNING,

    IS_PAUSED
}


/*public enum GameOverContext
{
    PLAYER_IS_DEAD,
    GAME_WILL_RESTART
}*/

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
