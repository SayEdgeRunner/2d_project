using System;
using System.Collections;
using Core;
using UnityEngine;

namespace Enemy
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class AttackTelegraph : MonoBehaviour, ITimeScalable
    {
        [Header("Shader References")]
        [SerializeField] private Shader _circleShader;
        [SerializeField] private Shader _sectorShader;
        [SerializeField] private Shader _boxShader;
        [SerializeField] private Shader _capsuleShader;

        [Header("Default Settings")]
        [SerializeField] private Color _defaultFillColor = new Color(1f, 0f, 0f, 0.3f);
        [SerializeField] private Color _defaultOutlineColor = new Color(1f, 0f, 0f, 1f);
        [SerializeField] private float _outlineWidth = 0.02f;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private Material _material;
        private Coroutine _fillCoroutine;
        private Mesh _currentMesh;
        
        private float _timeScale = 1f;
        private bool _isPaused = false;
        
        private AttackShape _currentShape;
        private float _currentDuration;
        private float _elapsed;
        private Action _onComplete;

        public bool IsActive => _meshRenderer != null && _meshRenderer.enabled;
        public float FillAmount => _material != null ? _material.GetFloat("_FillAmount") : 0f;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.enabled = false;
        }

        private void OnEnable()
        {
            EnemyTimeManager.Instance?.Register(this);
        }

        private void OnDisable()
        {
            EnemyTimeManager.Instance?.Unregister(this);
            Hide();
        }

        public void SetTimeScale(float scale)
        {
            _timeScale = scale;
            _isPaused = scale == 0f;
        }

        public void Pause() => SetTimeScale(0f);
        public void Resume() => SetTimeScale(1f);
        
        public void Show(AttackShape shape, float duration, Color? fillColor,
                         Vector2 facingDirection, Action onComplete = null)
        {
            if (shape == null)
            {
                Debug.LogWarning("[AttackTelegraph] Shape is null!");
                return;
            }
            
            Hide();

            _currentShape = shape;
            _currentDuration = duration;
            _elapsed = 0f;
            _onComplete = onComplete;

            SetupMeshAndMaterial(shape, facingDirection, fillColor ?? _defaultFillColor);

            UpdateTransform(shape, facingDirection);

            _meshRenderer.enabled = true;

            if (duration > 0)
            {
                _fillCoroutine = StartCoroutine(FillCoroutine(duration));
            }
            else
            {
                SetFillAmount(1f);
                _onComplete?.Invoke();
            }
        }

        public void Hide()
        {
            if (_fillCoroutine != null)
            {
                StopCoroutine(_fillCoroutine);
                _fillCoroutine = null;
            }

            if (_meshRenderer != null)
            {
                _meshRenderer.enabled = false;
            }

            if (_material != null)
            {
                Destroy(_material);
                _material = null;
            }

            if (_currentMesh != null)
            {
                Destroy(_currentMesh);
                _currentMesh = null;
            }

            _currentShape = null;
            _onComplete = null;
        }

        public void SetFillAmount(float amount)
        {
            if (_material != null)
            {
                _material.SetFloat("_FillAmount", Mathf.Clamp01(amount));
            }
        }

        private void SetupMeshAndMaterial(AttackShape shape, Vector2 facingDirection, Color fillColor)
        {
            Shader shader = GetShaderForShape(shape);
            if (shader == null)
            {
                Debug.LogError($"[AttackTelegraph] No shader found for shape type: {shape.GetType().Name}");
                return;
            }

            _material = new Material(shader);
            _material.SetColor("_Color", fillColor);
            _material.SetColor("_OutlineColor", _defaultOutlineColor);
            _material.SetFloat("_OutlineWidth", _outlineWidth);
            _material.SetFloat("_FillAmount", 0f);
            
            ConfigureShaderParameters(shape);
            
            Vector2 meshSize = GetMeshSizeForShape(shape);
            _currentMesh = TelegraphMeshGenerator.CreateQuad(meshSize.x, meshSize.y);
            _meshFilter.mesh = _currentMesh;
            _meshRenderer.material = _material;
        }

        private Shader GetShaderForShape(AttackShape shape)
        {
            return shape switch
            {
                CircleAttackShape => _circleShader,
                SectorAttackShape => _sectorShader,
                BoxAttackShape => _boxShader,
                CapsuleAttackShape => _capsuleShader,
                _ => _circleShader
            };
        }

        private Vector2 GetMeshSizeForShape(AttackShape shape)
        {
            return shape switch
            {
                CircleAttackShape circle => TelegraphMeshGenerator.GetCircleSize(circle),
                SectorAttackShape sector => TelegraphMeshGenerator.GetSectorSize(sector),
                BoxAttackShape box => TelegraphMeshGenerator.GetBoxSize(box),
                CapsuleAttackShape capsule => TelegraphMeshGenerator.GetCapsuleSize(capsule),
                _ => new Vector2(shape.GetApproximateRadius() * 2f, shape.GetApproximateRadius() * 2f)
            };
        }

        private void ConfigureShaderParameters(AttackShape shape)
        {
            switch (shape)
            {
                case CircleAttackShape circle:
                    _material.SetFloat("_RadiusX", circle.RadiusX);
                    _material.SetFloat("_RadiusY", circle.RadiusY);
                    break;

                case SectorAttackShape sector:
                    _material.SetFloat("_RadiusX", sector.RadiusX);
                    _material.SetFloat("_RadiusY", sector.RadiusY);
                    _material.SetFloat("_Angle", sector.Angle);
                    break;

                case BoxAttackShape box:
                    _material.SetFloat("_Width", box.Length);
                    _material.SetFloat("_Height", box.Width);
                    break;

                case CapsuleAttackShape capsule:
                    _material.SetFloat("_Length", capsule.Length);
                    _material.SetFloat("_Radius", capsule.Radius);
                    break;
            }
        }

        private void UpdateTransform(AttackShape shape, Vector2 facingDirection)
        {
            Vector2 offset = GetShapeOffset(shape, facingDirection);
            transform.localPosition = new Vector3(offset.x, offset.y, 0);
            
            float rotation = GetShapeRotation(shape, facingDirection);
            transform.localRotation = Quaternion.Euler(0, 0, rotation);
        }

        private Vector2 GetShapeOffset(AttackShape shape, Vector2 facingDirection)
        {
            if (!shape.FollowFacingDirection)
            {
                return new Vector2(shape.ForwardOffset, shape.VerticalOffset);
            }

            Vector2 forward = facingDirection.normalized * shape.ForwardOffset;
            Vector2 perpendicular = new Vector2(-facingDirection.y, facingDirection.x).normalized * shape.VerticalOffset;

            return forward + perpendicular;
        }

        private float GetShapeRotation(AttackShape shape, Vector2 facingDirection)
        {
            float baseAngle = Mathf.Atan2(facingDirection.y, facingDirection.x) * Mathf.Rad2Deg;
            return baseAngle + shape.RotationOffset;
        }

        private IEnumerator FillCoroutine(float duration)
        {
            SetFillAmount(0f);
            _elapsed = 0f;
            
            yield return null;

            while (_elapsed < duration)
            {
                yield return null;

                if (!_isPaused)
                {
                    _elapsed += Time.deltaTime * _timeScale;
                    float t = Mathf.Clamp01(_elapsed / duration);
                    SetFillAmount(t);
                }
            }

            SetFillAmount(1f);
            _fillCoroutine = null;
            _onComplete?.Invoke();
        }

        private void OnDestroy()
        {
            Hide();
        }
    }
}
