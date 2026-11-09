using TMPro;
using UnityEngine;

public sealed class GateTextRenderQueue : MonoBehaviour
{
    [SerializeField] private TMP_Text[] _texts;
    [SerializeField] private int _renderQueue = 3100;

    private void Awake()
    {
        foreach (TMP_Text text in _texts)
        {
            text.fontSharedMaterial.renderQueue = _renderQueue;
        }
    }
}