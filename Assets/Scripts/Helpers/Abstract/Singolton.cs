using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance;

    [SerializeField] protected bool _isNotDestroying = true;
    private static bool _isQuit = false;

    public static T Inst
    {
        get
        {
            if (_instance == null && !_isQuit)
            {
                T[] instances = FindObjectsOfType<T>();
                int instancesCount = instances.Length;

                if (instancesCount > 0)
                {
                    _instance = instances[0];
                    for (int i = 1; i < instancesCount; i++)
                        Destroy(instances[i]);
                }
                //else
                //{
                //    _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                //}
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
            _instance = this as T;
        else if (_instance != this)
            Destroy(gameObject);

        if (_isNotDestroying)
            DontDestroyOnLoad(gameObject);

    }

    protected virtual void OnApplicationQuit() => _isQuit = true;

    protected virtual void OnLoaded() { }
    protected virtual void OnUnloaded() { }

    protected virtual void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    protected virtual void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => OnLoaded();
    private void OnSceneUnloaded(Scene scene)=> OnUnloaded();

    

}
