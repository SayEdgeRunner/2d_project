using System;
using System.Collections;
using System.Collections.Generic;
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
        
        private static readonly int PropColor = Shader.PropertyToID("_Color");
        private static readonly int PropOutlineColor = Shader.PropertyToID("_OutlineColor");
        private static readonly int PropOutlineWidth = Shader.PropertyToID("_OutlineWidth");
        private static readonly int PropFillAmount = Shader.PropertyToID("_FillAmount");
        private static readonly int PropRadiusX = Shader.PropertyToID("_RadiusX");
        private static readonly int PropRadiusY = Shader.PropertyToID("_RadiusY");
        private static readonly int PropAngle = Shader.PropertyToID("_Angle");
        private static readonly int PropWidth = Shader.PropertyToID("_Width");
        private static readonly int PropHeight = Shader.PropertyToID("_Height");
        private static readonly int PropLength = Shader.PropertyToID("_Length");
        private static readonly int PropRadius = Shader.PropertyToID("_Radius");

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private MaterialPropertyBlock _propBlock;
        private Coroutine _fillCoroutine;
        
        private static Mesh _sharedQuadMesh;
        private static int _sharedMeshRefCount;
        
        private Dictionary<Shader, Material> _materialCache;

        private float _timeScale = 1f;
        private bool _isPaused = false;

        private AttackShape _currentShape;
        private float _currentDuration;
        private float _elapsed;
        private Action _onComplete;

        public bool IsActive => _meshRenderer != null && _meshRenderer.enabled;

        public float FillAmount
        {
            get
            {
                if (_propBlock == null || _meshRenderer == null) return 0f;
                _meshRenderer.GetPropertyBlock(_propBlock);
                return _propBlock.GetFloat(PropFillAmount);
            }
        }

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.enabled = false;

            _propBlock = new MaterialPropertyBlock();
            _materialCache = new Dictionary<Shader, Material>();
            
            InitializeSharedMesh();
            _meshFilter.sharedMesh = _sharedQuadMesh;
            
            PrewarmMaterials();
        }

        private static void InitializeSharedMesh()
        {
            if (_sharedQuadMesh == null)
            {
                _sharedQuadMesh = TelegraphMeshGenerator.CreateQuad(1f, 1f);
                _sharedQuadMesh.name = "Telegraph_SharedQuad";
            }
            _sharedMeshRefCount++;
        }

        private void PrewarmMaterials()
        {
            Shader[] shaders = { _circleShader, _sectorShader, _boxShader, _capsuleShader };

            foreach (var shader in shaders)
            {
                if (shader != null)
                {
                    _materialCache[shader] = new Material(shader);
                }
            }
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

        #region ITimeScalable

        public void SetTimeScale(float scale)
        {
            _timeScale = scale;
            _isPaused = scale == 0f;
        }

        public void Pause() => SetTimeScale(0f);
        public void Resume() => SetTimeScale(1f);

        #endregion
        
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

            _currentShape = null;
            _onComplete = null;
        }

        public void SetFillAmount(float amount)
        {
            _propBlock.SetFloat(PropFillAmount, Mathf.Clamp01(amount));
            _meshRenderer.SetPropertyBlock(_propBlock);
        }

        private void SetupMeshAndMaterial(AttackShape shape, Vector2 facingDirection, Color fillColor)
        {
            Shader shader = GetShaderForShape(shape);
            if (shader == null)
            {
                Debug.LogError($"[AttackTelegraph] No shader found for shape type: {shape.GetType().Name}");
                return;
            }
            
            Material material = GetOrCreateMaterial(shader);
            _meshRenderer.sharedMaterial = material;
            
            _propBlock.Clear();
            _propBlock.SetColor(PropColor, fillColor);
            _propBlock.SetColor(PropOutlineColor, _defaultOutlineColor);
            _propBlock.SetFloat(PropOutlineWidth, _outlineWidth);
            _propBlock.SetFloat(PropFillAmount, 0f);

            ConfigureShaderParameters(shape);
            _meshRenderer.SetPropertyBlock(_propBlock);
            
            Vector2 meshSize = GetMeshSizeForShape(shape);
            transform.localScale = new Vector3(meshSize.x, meshSize.y, 1f);
        }

        private Material GetOrCreateMaterial(Shader shader)
        {
            if (!_materialCache.TryGetValue(shader, out var material))
            {
                material = new Material(shader);
                _materialCache[shader] = material;
            }
            return material;
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
                    _propBlock.SetFloat(PropRadiusX, circle.RadiusX);
                    _propBlock.SetFloat(PropRadiusY, circle.RadiusY);
                    break;

                case SectorAttackShape sector:
                    _propBlock.SetFloat(PropRadiusX, sector.RadiusX);
                    _propBlock.SetFloat(PropRadiusY, sector.RadiusY);
                    _propBlock.SetFloat(PropAngle, sector.Angle);
                    break;

                case BoxAttackShape box:
                    _propBlock.SetFloat(PropWidth, box.Length);
                    _propBlock.SetFloat(PropHeight, box.Width);
                    break;

                case CapsuleAttackShape capsule:
                    _propBlock.SetFloat(PropLength, capsule.Length);
                    _propBlock.SetFloat(PropRadius, capsule.Radius);
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
            
            foreach (var material in _materialCache.Values)
            {
                if (material != null)
                {
                    Destroy(material);
                }
            }
            _materialCache.Clear();
            
            _sharedMeshRefCount--;
            if (_sharedMeshRefCount <= 0 && _sharedQuadMesh != null)
            {
                Destroy(_sharedQuadMesh);
                _sharedQuadMesh = null;
            }
        }
    }
}
