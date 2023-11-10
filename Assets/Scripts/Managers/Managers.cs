using System.Collections;
using UnityEngine;

public class Managers : Singleton<Managers>
{
    public static InputManager Input => GetInstance._input;
    public static PoolManager Pool => GetInstance._pool;
    public static ResourceManager Resource => GetInstance._resource;
    public static SceneManagerEx Scene => GetInstance._scene;
    public static SoundManager Sound => GetInstance._sound;
    public static UIManager UI => GetInstance._ui;

    private static bool s_isInit = false;

    private readonly InputManager _input = new();
    private readonly PoolManager _pool = new();
    private readonly ResourceManager _resource = new();
    private readonly SceneManagerEx _scene = new();
    private readonly SoundManager _sound = new();
    private readonly UIManager _ui = new();

    private readonly WaitForEndOfFrame _waitForEndOfFrame = new();

    private void Start()
    {
        StartCoroutine(EndOfFrame());
    }

    public static void Init()
    {
        if (s_isInit)
        {
            Clear();
        }

        Input.Init();
        Pool.Init();
        Sound.Init();
        UI.Init();

        s_isInit = true;
    }

    public static void Clear()
    {
        if (!s_isInit)
        {
            return;
        }

        Input.Clear();
        Pool.Clear();
        Resource.Clear();
        Sound.Clear();
        UI.Clear();

        s_isInit = false;
    }

    private IEnumerator EndOfFrame()
    {
        while (true)
        {
            yield return _waitForEndOfFrame;

            if (s_isInit)
            {
                _input.ResetActions();
            }
        }
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
