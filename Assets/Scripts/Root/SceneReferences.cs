using UnityEngine;

public class SceneReferences : MonoBehaviour
{
    [SerializeField] private MainMenuScreen _mainMenuScreen;
    [SerializeField] private RunScreen _runScreen;
    [SerializeField] private VictoryScreen _victoryScreen;
    [SerializeField] private DefeatScreen _defeatScreen;
    [SerializeField] private Player _player;
    [SerializeField] private LevelGenerator _levelGenerator;

    public MainMenuScreen MainMenuScreen => _mainMenuScreen;
    public RunScreen RunScreen => _runScreen;
    public VictoryScreen VictoryScreen => _victoryScreen;
    public DefeatScreen DefeatScreen => _defeatScreen;
    public Player Player => _player;
    public LevelGenerator LevelGenerator => _levelGenerator;
}