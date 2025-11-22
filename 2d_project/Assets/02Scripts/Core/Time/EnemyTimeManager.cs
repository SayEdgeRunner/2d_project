using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class EnemyTimeManager : Singleton<EnemyTimeManager>
    {
        private float _timeScale = 1f;
        private readonly HashSet<ITimeScalable> _scalables = new();
        private Coroutine _restoreCoroutine;
        
        public float TimeScale => _timeScale;
        public float DeltaTime => Time.deltaTime * _timeScale;
        
        public void Register(ITimeScalable scalable)
        {
            if (scalable != null)
            {
                _scalables.Add(scalable);
                scalable.SetTimeScale(_timeScale);
            }
        }

        public void Unregister(ITimeScalable scalable)
        {
            if (scalable != null)
            {
                _scalables.Remove(scalable);
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

            foreach (var scalable in _scalables)
            {
                scalable.SetTimeScale(_timeScale);
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
            yield return new WaitForSeconds(duration);
            SetTimeScale(targetScale);
            _restoreCoroutine = null;
        }

        protected override void OnDestroy()
        {
            _scalables.Clear();
            base.OnDestroy();
        }
    }
}
