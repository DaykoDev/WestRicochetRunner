using UnityEngine;

public class SceneReferences : MonoBehaviour
{
    [SerializeField] public MainMenuScreen MainMenuScreen { get; private set; }

    [SerializeField] public RunScreen RunScreen { get; private set; }

    [SerializeField] public VictoryScreen VictoryScreen { get; private set; }

    [SerializeField] public DefeatScreen DefeatScreen { get; private set; }
}