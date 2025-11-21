using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 적 사망 연출을 담당하는 컴포넌트
    /// 애니메이션, 이펙트, 사운드만 재생 (생명주기 관리는 EnemyEntity가 담당)
    /// </summary>
    public class EnemyDeathPresenter : MonoBehaviour
    {
        [Header("애니메이션")]
        [SerializeField] private Animator _animator;
        [SerializeField] private string _deathAnimationTrigger = "Death";

        [Header("이펙트")]
        [SerializeField] private GameObject _deathEffectPrefab;
        [SerializeField] private Transform _effectSpawnPoint;

        [Header("사운드")]
        [SerializeField] private AudioClip _deathSound;

        /// <summary>
        /// 사망 연출 재생 (Fire and Forget)
        /// </summary>
        public void PlayDeathEffect()
        {
            PlayDeathAnimation();
            PlayDeathVFX();
            PlayDeathSound();
        }

        private void PlayDeathAnimation()
        {
            if (_animator && !string.IsNullOrEmpty(_deathAnimationTrigger))
            {
                _animator.SetTrigger(_deathAnimationTrigger);
            }
        }

        private void PlayDeathVFX()
        {
            if (!_deathEffectPrefab) return;

            Transform spawnPoint = _effectSpawnPoint ? _effectSpawnPoint : transform;
            Instantiate(_deathEffectPrefab, spawnPoint.position, spawnPoint.rotation);
        }

        private void PlayDeathSound()
        {
            if (_deathSound)
            {
                AudioSource.PlayClipAtPoint(_deathSound, transform.position);
            }
        }
    }
}
