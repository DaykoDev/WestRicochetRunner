using TMPro;
using UnityEngine;

public class ObstacleHealthView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Obstacle _obstacle;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private Camera _camera;

    [Header("Display")]
    [SerializeField] private Vector3 _localOffset = new Vector3(0f, 1.5f, 0f);

    private void Awake()
    {
        if (_obstacle == null)
        {
            _obstacle = GetComponentInParent<Obstacle>();
        }

        if (_camera == null)
        {
            _camera = Camera.main;
        }

        if (_obstacle == null || _healthText == null)
        {
            Debug.LogError("ObstacleHealthView references are missing.", this);
            enabled = false;
            return;
        }

        _healthText.transform.localPosition = _localOffset;
    }

    private void OnEnable()
    {
        if (_obstacle == null || _healthText == null)
        {
            return;
        }

        _obstacle.HealthChanged += OnHealthChanged;
        Refresh();
    }

    private void OnDisable()
    {
        if (_obstacle != null)
        {
            _obstacle.HealthChanged -= OnHealthChanged;
        }
    }

    private void LateUpdate()
    {
        if (_camera == null)
        {
            return;
        }

        Vector3 direction = _camera.transform.position - _healthText.transform.position;
    }

    private void OnHealthChanged(float currentHealth, float maxHealth)
    {
        Refresh();
    }

    private void Refresh()
    {
        _healthText.text = Mathf.CeilToInt(_obstacle.CurrentHealth).ToString();
    }
}