using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CoinSFX : MonoBehaviour
{
    [Header("Эффекты")]
    [SerializeField] private ParticleSystem _particleCatch;
    [SerializeField] private ParticleSystem _particleBreaking;
    private Transform _particleCatchTransform;

    [Header("Звуки")]
    [SerializeField] private AudioClip _audioSpawn;
    [SerializeField] private AudioClip _audioCatch;
    [SerializeField] private AudioClip _audioBreaking;

    [Header("Монета")]
    [SerializeField] private GameObject _coin;
    private Transform _coinTransform;


    [Header("Точки")]
    [SerializeField] private Transform _nearPoint;
    [SerializeField] private Transform _farPoint;

    private Vector3 NearPoint => _nearPoint.localPosition;
    private Vector3 FarPoint => _farPoint.localPosition;
   

    [Space]
    [SerializeField] private float _speedSpawn = 10f;
    [SerializeField] private float _speedCatch = 3f;

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _coinTransform = _coin.transform;
        _particleCatchTransform = _particleCatch.transform;
    }

    private void OnEnable() => _coin.SetActive(false);

    private void OnTriggerExit(Collider other) => StartCoroutine(SpawnExecute());

    public void Catch() => StartCoroutine(CatchExecute()); 

    public void Breaking()
    {
        _coin.SetActive(false);
        _particleBreaking.Play();
        _audioSource.PlayOneShot(_audioBreaking, 1.2f);
    }

    private IEnumerator SpawnExecute()
    {
        _coin.SetActive(true);
        yield return StartCoroutine(Move(_coinTransform, FarPoint, NearPoint, _speedSpawn));
        _audioSource.PlayOneShot(_audioSpawn);
    }

    private IEnumerator CatchExecute()
    {
        _particleCatch.Play();
        _audioSource.PlayOneShot(_audioCatch, 1.1f);
        
        StartCoroutine(Move(_particleCatchTransform, NearPoint, FarPoint, _speedCatch));
        yield return StartCoroutine(Move(_coinTransform, NearPoint, FarPoint, _speedCatch));
        
        _particleCatch.Stop();
        _coin.SetActive(false);
    }

    private IEnumerator Move(Transform obj, Vector3 start, Vector3 end, float speed)
    {
        while (start != end)
        {
            obj.localPosition = start.MoveTowards(end, speed * Time.deltaTime);
            yield return null;
        }
    }
}
