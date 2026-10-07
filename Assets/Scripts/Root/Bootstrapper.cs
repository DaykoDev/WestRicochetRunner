using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private SceneReferences _sceneReferences;

    private CompositionRoot _compositionRoot;

    private void Awake()
    {
        _compositionRoot = new CompositionRoot(_sceneReferences);
        _compositionRoot.Initialize();
    }

    private void OnDestroy()
    {
        _compositionRoot?.Dispose();
    }
}