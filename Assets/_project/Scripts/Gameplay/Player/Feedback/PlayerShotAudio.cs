using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public sealed class PlayerShotAudio : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BallLauncher _ballLauncher;
    [SerializeField] private AudioClip[] _shotClips;

    [Header("Sound")]
    [SerializeField, Range(0f, 1f)]
    private float _volume = 0.7f;

    [SerializeField]
    private Vector2 _pitchRange =
        new(0.95f, 1.05f);

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _audioSource.playOnAwake = false;
        _audioSource.loop = false;
        _audioSource.spatialBlend = 0f;
        _audioSource.dopplerLevel = 0f;
    }

    private void OnEnable()
    {
        if (_ballLauncher != null)
        {
            _ballLauncher.Fired += OnFired;
        }
    }

    private void OnDisable()
    {
        if (_ballLauncher != null)
        {
            _ballLauncher.Fired -= OnFired;
        }
    }

    private void OnFired(int revolverIndex)
    {
        if (_shotClips == null || _shotClips.Length == 0)
        {
            return;
        }

        AudioClip clip = _shotClips[
            Random.Range(0, _shotClips.Length)];

        if (clip == null)
        {
            return;
        }

        _audioSource.pitch = Random.Range(
            _pitchRange.x,
            _pitchRange.y);

        _audioSource.PlayOneShot(clip, _volume);
    }
}