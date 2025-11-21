using UnityEngine;

namespace Enemy
{
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
