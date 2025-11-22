using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class EnemyTimeManager : Singleton<EnemyTimeManager>
    {
        private float _timeScale = 1f;
        private readonly HashSet<ITimeScalable> _scalableSet = new();  // O(1) 중복 체크용
        private readonly List<ITimeScalable> _scalableList = new();    // 할당 없는 순회용
        private readonly Dictionary<float, WaitForSeconds> _waitCache = new();
        private Coroutine _restoreCoroutine;
        
        public float TimeScale => _timeScale;
        public float DeltaTime => Time.deltaTime * _timeScale;
        
        public void Register(ITimeScalable scalable)
        {
            if (scalable != null && _scalableSet.Add(scalable))
            {
                _scalableList.Add(scalable);
                scalable.SetTimeScale(_timeScale);
            }
        }

        public void Unregister(ITimeScalable scalable)
        {
            if (scalable != null && _scalableSet.Remove(scalable))
            {
                _scalableList.Remove(scalable);
            }
        }

        public void FreezeAllEnemies(float duration)
        {
            SetTimeScale(0f);
            StartRestoreCoroutine(duration, 1f);
        }

        public void SlowAllEnemies(float scale, float duration)
        {
            SetTimeScale(Mathf.Clamp01(scale));
            StartRestoreCoroutine(duration, 1f);
        }

        public void SetTimeScale(float scale)
        {
            _timeScale = Mathf.Clamp(scale, 0f, 2f);

            for (int i = 0; i < _scalableList.Count; i++)
            {
                _scalableList[i].SetTimeScale(_timeScale);
            }
        }

        public void RestoreTimeScale()
        {
            StopRestoreCoroutine();
            SetTimeScale(1f);
        }

        private void StartRestoreCoroutine(float duration, float targetScale)
        {
            StopRestoreCoroutine();
            _restoreCoroutine = StartCoroutine(RestoreAfterDelay(duration, targetScale));
        }

        private void StopRestoreCoroutine()
        {
            if (_restoreCoroutine != null)
            {
                StopCoroutine(_restoreCoroutine);
                _restoreCoroutine = null;
            }
        }

        private IEnumerator RestoreAfterDelay(float duration, float targetScale)
        {
            yield return GetWaitForSeconds(duration);
            SetTimeScale(targetScale);
            _restoreCoroutine = null;
        }

        private WaitForSeconds GetWaitForSeconds(float duration)
        {
            if (!_waitCache.TryGetValue(duration, out var wait))
            {
                wait = new WaitForSeconds(duration);
                _waitCache[duration] = wait;
            }
            return wait;
        }

        protected override void OnDestroy()
        {
            _scalableSet.Clear();
            _scalableList.Clear();
            _waitCache.Clear();
            base.OnDestroy();
        }
    }
}
