using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // 싱글톤 인스턴스를 저장하는 static 변수
    private static T _instance;

    // 멀티스레드 환경에서 동시 접근을 방지하기 위한 lock 객체
    private static object _lockObj = new object();

    // 애플리케이션 종료 시 인스턴스 접근을 방지하는 플래그
    private static bool _applicationIsQuitting = false;


    public static T Instance
    {
        get
        {
            // 애플리케이션 종료 중이면 null 반환 (Unity 에러 방지)
            if (_applicationIsQuitting)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' 인스턴스가 이미 삭제되었습니다.");
                return null;
            }

            // 멀티스레드 환경에서 안전하게 인스턴스 생성
            lock (_lockObj)
            {
                if (_instance == null)
                {
                    // 씬에서 기존 인스턴스 찾기
                    _instance = FindObjectOfType<T>();

                    // 씬에 인스턴스가 없으면 새로 생성
                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = $"[Singleton] {typeof(T)}";

                        // 씬 전환 시에도 파괴되지 않도록 설정
                        DontDestroyOnLoad(singleton);
                        Debug.Log($"[Singleton] '{typeof(T)}' 인스턴스가 생성되었습니다.");
                    }
                }

                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        lock (_lockObj)
        {
            // 첫 번째 인스턴스인 경우
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject); // 씬 전환 시 유지
            }
            // 이미 인스턴스가 존재하는 경우 (중복 방지)
            else if (_instance != this)
            {
                Debug.LogWarning($"[Singleton] '{typeof(T)}'의 중복 인스턴스가 발견되었습니다. 파괴합니다.");
                Destroy(gameObject); // 중복 인스턴스 제거
            }
        }
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    private void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }
}