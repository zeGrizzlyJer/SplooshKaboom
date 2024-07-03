using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    PLAY,
    PAUSE,
    WIN,
    LOSE,
    MAINMENU,
}

public class GameManager : Singleton<GameManager>
{
    #region Properties
    private GameState gameState = GameState.MAINMENU;
    public GameState GameState
    {
        get
        {
            return gameState;
        }
        set
        {
            if (gameState == value) return;
            gameState = value;
            OnGameStateChanged?.Invoke();
        }
    }

    private int gameRound = 0;
    public int GameRound
    {
        get
        {
            return gameRound;
        }
        set
        {
            if (gameRound == value) return;
            gameRound = value;
            if (gameRound > roundsToWin)
            {
                GameState = GameState.WIN;
                SceneTransitionManager.Instance.LoadScene(2);
                return;
            }
            if (gameRound == 1)
            {
                barrelsRemaining = 0;
                maxBarrels = 3;
            }
            if (gameRound > 1 && gameRound % 2 == 1) maxBarrels++;
            barrelsRemaining = 0;
            OnRoundStart?.Invoke();
        }
    }

    private int shotsRemaining;
    public int ShotsRemaining
    {
        get
        {
            return shotsRemaining;
        }
        set
        {
            if (shotsRemaining == value) return;
            shotsRemaining = value;
            OnCannonShot?.Invoke(value);
        }
    }

    private int barrelsRemaining;
    public int BarrelsRemaining
    {
        get
        {
            return barrelsRemaining;
        }
        set
        {
            if (barrelsRemaining == value) return;
            barrelsRemaining = value;
            OnBarrelValueChange?.Invoke(value);
            if (barrelsRemaining == 0)
            {
                GameRound++;
            }
        }
    }
    #endregion

    #region Cleanup
    static public bool cleanedUp = false;
    public event Action OnApplicationCleanup;

    private void OnApplicationQuit()
    {
        OnApplicationCleanup?.Invoke();
        cleanedUp = true;
    }
    #endregion

    public event Action OnGameStateChanged;
    public event Action OnRoundStart;
    public event Action<int> OnCannonShot;
    public event Action<int> OnBarrelValueChange;

    [Header("Game Play Settings")]
    public int maxShots = 15;
    public bool canShoot = false;
    public int maxBarrels = 3;
    public int roundsToWin = 2;

    protected override void Awake()
    {
        base.Awake();
        OnGameStateChanged += DetermineCursorState;
        OnGameStateChanged += SetTimeScale;
    }

    #region GameStateChangeEffects
    private void SetTimeScale()
    {
        if (GameState != GameState.PAUSE) Time.timeScale = 1f;
    }

    private void DetermineCursorState()
    {
        if (gameState == GameState.PLAY)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    #endregion
}