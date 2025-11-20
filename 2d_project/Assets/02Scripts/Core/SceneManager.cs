using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 뱀파이어 서바이벌 스타일의 2D 무한 맵 시스템
/// 제한된 타일을 재사용하여 무한한 맵처럼 보이게 만듦
/// </summary>
public class SceneManager : MonoBehaviour
{
    [Header("타일 설정")]
    [SerializeField] private GameObject _tilePrefab; // 반복 사용할 타일 프리팹
    [SerializeField] private float _tileSize = 20f; // 각 타일의 크기 (20x20 유닛)
    [SerializeField] private int _gridSize = 3; // 타일 그리드 크기 (3 = 3x3 = 9개 타일)

    [Header("플레이어")]
    [SerializeField] private Transform _player; // 추적할 플레이어

    // 타일을 그리드 좌표로 관리하는 딕셔너리 (예: (0,0) -> 타일 오브젝트)
    private Dictionary<Vector2Int, GameObject> _tileGrid = new Dictionary<Vector2Int, GameObject>();

    // 플레이어가 현재 위치한 청크의 좌표
    private Vector2Int _currentPlayerChunk;

    void Start()
    {
        // 플레이어가 할당되지 않았으면 자동으로 찾기
        if (_player == null)
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                _player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player with tag 'Player' not found. Disabling SceneManager.", this);
                enabled = false;
                return;
            }
        }

        // 게임 시작 시 플레이어 주변에 타일 그리드 생성
        InitializeGrid();

        // 플레이어의 시작 청크 위치 저장
        _currentPlayerChunk = GetChunkPosition(_player.position);
    }

    void Update()
    {
        // 매 프레임마다 플레이어의 청크 위치 계산
        Vector2Int newPlayerChunk = GetChunkPosition(_player.position);

        // 플레이어가 다른 청크로 이동했는지 확인
        if (newPlayerChunk != _currentPlayerChunk)
        {
            // 청크가 바뀌면 타일들을 재배치
            UpdateTiles(newPlayerChunk);
            _currentPlayerChunk = newPlayerChunk;
        }
    }

    /// <summary>
    /// 게임 시작 시 플레이어 주변에 초기 타일 그리드 생성
    /// 예: _gridSize=3이면 -1~1 범위로 3x3 그리드 생성
    /// </summary>
    void InitializeGrid()
    {
        // _gridSize=3이면 halfSize=1, 즉 -1, 0, 1 범위로 생성
        int halfSize = _gridSize / 2;

        // X축 방향으로 반복
        for (int x = -halfSize; x <= halfSize; x++)
        {
            // Y축 방향으로 반복 (2D에서는 Y축이 세로)
            for (int y = -halfSize; y <= halfSize; y++)
            {
                // 그리드 좌표 (예: (-1, -1), (0, 0), (1, 1) 등)
                Vector2Int gridPos = new Vector2Int(x, y);

                // 그리드 좌표를 월드 좌표로 변환 (2D는 x, y 사용, z는 0)
                // 예: (0, 0) -> (0, 0, 0), (1, 0) -> (20, 0, 0)
                Vector3 worldPos = new Vector3(x * _tileSize, y * _tileSize, 0);

                // 타일 생성 및 배치
                GameObject tile = Instantiate(_tilePrefab, worldPos, Quaternion.identity, transform);
                tile.name = $"Tile_{x}_{y}";

                // 딕셔너리에 타일 저장 (나중에 재사용하기 위해)
                _tileGrid[gridPos] = tile;
            }
        }
    }

    /// <summary>
    /// 플레이어의 월드 좌표를 청크 좌표로 변환 (2D 버전)
    /// 예: 플레이어가 (25, 15)에 있고 _tileSize=20이면
    ///     청크 좌표는 (1, 1)이 됨
    /// </summary>
    Vector2Int GetChunkPosition(Vector3 worldPosition)
    {
        // 2D에서는 x, y 좌표 사용 (z는 무시)
        int x = Mathf.RoundToInt(worldPosition.x / _tileSize);
        int y = Mathf.RoundToInt(worldPosition.y / _tileSize);
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// 플레이어가 새로운 청크로 이동했을 때 타일들을 재배치
    /// 핵심: 기존 타일을 삭제하지 않고 위치만 이동시킴 (재사용)
    /// </summary>
    void UpdateTiles(Vector2Int newCenterChunk)
    {
        int halfSize = _gridSize / 2;
        var requiredPositions = new HashSet<Vector2Int>();

        // 새로운 중심 주변에 필요한 모든 타일 위치를 계산합니다.
        for (int x = -halfSize; x <= halfSize; x++)
        {
            for (int y = -halfSize; y <= halfSize; y++)
            {
                requiredPositions.Add(new Vector2Int(newCenterChunk.x + x, newCenterChunk.y + y));
            }
        }

        var currentPositions = new HashSet<Vector2Int>(_tileGrid.Keys);

        // 더 이상 필요 없는 타일(재활용 대상)과 새로 타일이 필요한 위치를 찾습니다.
        var positionsToRecycle = new Queue<Vector2Int>(currentPositions.Except(requiredPositions));
        var newPositions = new Queue<Vector2Int>(requiredPositions.Except(currentPositions));

        // 재활용 대상 타일을 새로운 위치로 이동시킵니다.
        while (newPositions.Count > 0)
        {
            if (positionsToRecycle.Count == 0)
            {
                Debug.LogError("재활용할 타일이 부족합니다. 그리드 크기가 일정하다면 발생해서는 안 되는 문제입니다.", this);
                break;
            }

            var oldPos = positionsToRecycle.Dequeue();
            var newPos = newPositions.Dequeue();

            // 타일 객체를 재사용합니다.
            GameObject tile = _tileGrid[oldPos];
            _tileGrid.Remove(oldPos);

            // 새로운 위치로 이동하고 정보를 업데이트합니다.
            tile.transform.position = new Vector3(newPos.x * _tileSize, newPos.y * _tileSize, 0);
            tile.name = $"Tile_{newPos.x}_{newPos.y}";

            // 그리드 딕셔너리를 업데이트합니다.
            _tileGrid[newPos] = tile;
        }
    }


    /// <summary>
    /// Scene 뷰에서 타일 배치를 시각적으로 확인하기 위한 기즈모 (2D)
    /// </summary>
    void OnDrawGizmos()
    {
        // 게임이 실행 중이고 플레이어가 있을 때만 그리기
        if (!Application.isPlaying || _player == null) return;

        // 플레이어가 현재 있는 청크를 노란색으로 표시
        Gizmos.color = Color.yellow;
        Vector3 chunkCenter = new Vector3(
            _currentPlayerChunk.x * _tileSize,
            _currentPlayerChunk.y * _tileSize,
            0
        );
        // 2D에서는 정사각형으로 표시
        Gizmos.DrawWireCube(chunkCenter, new Vector3(_tileSize, _tileSize, 0.1f));

        // 모든 활성화된 타일의 경계를 초록색으로 표시
        Gizmos.color = Color.green;
        foreach (var kvp in _tileGrid)
        {
            Vector3 pos = kvp.Value.transform.position;
            Gizmos.DrawWireCube(pos, new Vector3(_tileSize, _tileSize, 0.1f));
        }
    }
}