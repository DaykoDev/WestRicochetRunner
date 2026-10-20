using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryRenderer : MonoBehaviour
{
    [SerializeField] private TrajectoryPredictor _trajectoryPredictor;

    [Header("Animation")]
    [SerializeField, Min(0f)] private float _scrollSpeed = 1.5f;

    private LineRenderer _lineRenderer;
    private Material _lineMaterial;

    private Vector3[] _cachedPoints;
    private float _textureOffset;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.textureMode = LineTextureMode.Tile;

        _cachedPoints = new Vector3[_trajectoryPredictor.MaxPointCount];

        _lineMaterial = _lineRenderer.material;

        SetVisible(false);
    }

    private void Update()
    {
        AnimateTexture();
    }

    private void OnDestroy()
    {
        if (_lineMaterial != null)
        {
            Destroy(_lineMaterial);
        }
    }

    public void UpdateTrajectory(Vector3 origin, Vector3 velocity, BallStats stats)
    {
        if (enabled == false)
        {
            return;
        }

        _trajectoryPredictor.CalculateTrajectory(origin, velocity, stats);

        UpdateLine();
    }

    public void SetVisible(bool isVisible)
    {
        _lineRenderer.enabled = isVisible;
        enabled = isVisible;
    }

    private void UpdateLine()
    {
        int pointCount = _trajectoryPredictor.PointCount;
        
        if (pointCount > _cachedPoints.Length)
        {
            return;
        }

        _lineRenderer.positionCount = pointCount;

        if (pointCount == 0)
        {
            return;
        }

        _trajectoryPredictor.GetTrajectoryPoints(_cachedPoints);
        _lineRenderer.SetPositions(_cachedPoints);
    }

    private void AnimateTexture()
    {
        if (_lineMaterial == null)
        {
            return;
        }

        _textureOffset -= _scrollSpeed * Time.deltaTime;

        _lineMaterial.mainTextureOffset = new Vector2(_textureOffset, 0f);
    }
}