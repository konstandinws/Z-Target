using System;

// Énumération des différents états possibles du jeu
public enum GameStateType
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}

public class GameState
{
    // Instance singleton
    public static GameState Instance { get; private set; } = new GameState();
    
    // État actuel du jeu
    public GameStateType CurrentState { get; private set; } = GameStateType.Playing;
    
    // Événement déclenché quand l'état du jeu change
    public event Action<GameStateType> OnStateChanged;
    
    // Pour empêcher l'instanciation externe (pattern singleton)
    private GameState() { }
    
    // Changement d'état du jeu
    public void ChangeState(GameStateType newState)
    {
        // Si l'état est déjà le même, ne rien faire
        if (CurrentState == newState)
            return;
            
        CurrentState = newState;
        
        // Déclencher l'événement de changement d'état
        OnStateChanged?.Invoke(CurrentState);
    }
    
    // Méthodes d'aide pour changer d'état spécifique
    public void StartGame()
    {
        ChangeState(GameStateType.Playing);
    }
    
    public void PauseGame()
    {
        ChangeState(GameStateType.Paused);
    }
    
    public void ResumeGame()
    {
        ChangeState(GameStateType.Playing);
    }
    
    public void GameOver()
    {
        ChangeState(GameStateType.GameOver);
    }
    
    public void MainMenu()
    {
        ChangeState(GameStateType.MainMenu);
    }
    
    // Vérification de l'état actuel
    public bool IsPlaying()
    {
        return CurrentState == GameStateType.Playing;
    }
    
    public bool IsPaused()
    {
        return CurrentState == GameStateType.Paused;
    }
    
    public bool IsGameOver()
    {
        return CurrentState == GameStateType.GameOver;
    }
    
    public bool IsInMainMenu()
    {
        return CurrentState == GameStateType.MainMenu;
    }
}
