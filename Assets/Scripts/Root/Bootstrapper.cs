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

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            _compositionRoot?.FlushSave();
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus == false)
        {
            _compositionRoot?.FlushSave();
        }
    }

    private void OnApplicationQuit()
    {
        _compositionRoot?.FlushSave();
    }

    private void OnDestroy()
    {
        _compositionRoot?.Dispose();
    }
}