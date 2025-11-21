using UnityEngine;
using System;

/// <summary>
/// 게임 시간을 중앙에서 관리하는 매니저 클래스
/// 싱글톤 패턴으로 구현되어 어디서든 접근 가능
/// </summary>
public class TimeManager : MonoBehaviour
{
    // === 싱글톤 인스턴스 ===
    private static TimeManager _instance;
    
    public static TimeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // TimeManager가 없으면 새로 생성
                GameObject go = new GameObject("TimeManager");
                _instance = go.AddComponent<TimeManager>();
                // 씬 전환 시에도 파괴되지 않도록 설정
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    [Header("시간")]
    private const float MINUTE   = 60.0f; 
    private const float RESET    = 0f;

    [Header("게임 진행사항")]
    private bool _isGameRunning = false;  // 게임 진행 중인지 체크

    // 현재 게임의 플레이 시간 (초 단위)
    // 게임을 실행할 때마다 0부터 시작
    private float _currentGameTime;
    
    // 게임 일시정지 상태
    private bool _isPaused;
    
    // 시간 흐름 속도 배율
    private float _timeScale = 1f;

    // 게임 클리어 제한 시간 (초 단위)
    // 기본값: 600초 = 10분
    private float _clearTime = 600f;

    // 1분(60초)마다 발생하는 이벤트
    public event Action<float> OnMinutePassed;

    // 제한 시간(10분)이 다 되었을 때 발생하는 이벤트
    public event Action OnTimeUp;


    // 1분 경과를 체크하기 위한 타이머
    private float _minuteTimer;  // 1분 체크용

    // 제한 시간 도달 이벤트가 이미 발생했는지 체크하는 플래그
    private bool _timeUpTriggered;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (_isPaused || !_isGameRunning) return;  // 게임 시작 전엔 시간 안 흐름

        float deltaTime = Time.deltaTime * _timeScale;
        _currentGameTime += deltaTime;



        // 1분마다 이벤트 발생
        _minuteTimer += deltaTime;
        if (_minuteTimer >= MINUTE)
        {
            _minuteTimer = RESET;
            OnMinutePassed?.Invoke(_currentGameTime);
        }

        // 제한 시간에 도달했고, 아직 이벤트를 발생시키지 않았다면
        if (_currentGameTime >= _clearTime && !_timeUpTriggered)
        {
            // 이벤트 중복 발생 방지 플래그 설정
            _timeUpTriggered = true;
            // 시간 종료 이벤트 발생
            OnTimeUp?.Invoke();
        }
    }


    // 현재 게임 시간 반환 (초)
    public float GetCurrentTime()
    {
        return _currentGameTime;
    }


    // 게임 시간을 시간:분:초 형식으로 반환
    public string GetCurrentTimeFormatted()
    {
        return FormatTime(_currentGameTime);
    }


    /// 남은 시간을 초 단위로 반환
    public float GetRemainingTime()
    {
        // 제한 시간에서 현재 시간을 뺌
        // Mathf.Max로 음수가 되지 않도록 보장 (최소값 0)
        return Mathf.Max(0f, _clearTime - _currentGameTime);
    }

    /// 남은 시간을 "분:초" 형식의 문자열로 반환
    public string GetRemainingTimeFormatted()
    {
        // 남은 시간을 포맷팅해서 반환
        return FormatTime(GetRemainingTime());
    }

    // 시간 포맷 헬퍼 함수
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / MINUTE);

        int seconds = Mathf.FloorToInt(timeInSeconds % MINUTE);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    /// 게임 시간이 제한 시간에 도달했는지 확인
    public bool IsTimeUp()
    {
        // 현재 시간이 제한 시간 이상인지 확인
        return _currentGameTime >= _clearTime;
    }

    // 게임 일시정지/재개
    public void SetPaused(bool paused)
    {
        _isPaused = paused;
    }

    public bool IsPaused()
    {
        return _isPaused;
    }

    // 타임 스케일 설정 (슬로우 모션 등)
    public void SetTimeScale(float scale)
    {
        _timeScale = Mathf.Max(0f, scale);
    }

    public float GetTimeScale()
    {
        return _timeScale;
    }

    // 게임 시간을 0으로 초기화하고 새 게임 시작
    public void ResetTime()
    {
        // 현재 게임 시간을 0으로 초기화
        _currentGameTime = RESET;
        // 1분 타이머 초기화
        _minuteTimer = RESET;
        // 시간 종료 플래그 초기화
        _timeUpTriggered = false;
        // 일시정지 해제
        _isPaused = false;
    }

    // 게임 시작 시 호출
    public void StartGame()
    {
        _isGameRunning = true;
        ResetTime();
    }

    // 게임 종료 시 호출
    public void StopGame()
    {
        _isGameRunning = false;
    }

}