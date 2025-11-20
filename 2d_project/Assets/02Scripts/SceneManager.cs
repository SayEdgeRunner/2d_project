using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 뱀파이어 서바이벌 스타일의 2D 무한 맵 시스템
/// 제한된 타일을 재사용하여 무한한 맵처럼 보이게 만듦
/// </summary>
public class InfiniteMapManager : MonoBehaviour
{
    [Header("타일 설정")]
    [SerializeField] private GameObject tilePrefab; // 반복 사용할 타일 프리팹
    [SerializeField] private float tileSize = 20f; // 각 타일의 크기 (20x20 유닛)
    [SerializeField] private int gridSize = 3; // 타일 그리드 크기 (3 = 3x3 = 9개 타일)

    [Header("플레이어")]
    [SerializeField] private Transform player; // 추적할 플레이어

    // 타일을 그리드 좌표로 관리하는 딕셔너리 (예: (0,0) -> 타일 오브젝트)
    private Dictionary<Vector2Int, GameObject> tileGrid = new Dictionary<Vector2Int, GameObject>();

    // 플레이어가 현재 위치한 청크의 좌표
    private Vector2Int currentPlayerChunk;

    void Start()
    {
        // 플레이어가 할당되지 않았으면 자동으로 찾기
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        // 게임 시작 시 플레이어 주변에 타일 그리드 생성
        InitializeGrid();

        // 플레이어의 시작 청크 위치 저장
        currentPlayerChunk = GetChunkPosition(player.position);
    }

    void Update()
    {
        // 매 프레임마다 플레이어의 청크 위치 계산
        Vector2Int newPlayerChunk = GetChunkPosition(player.position);

        // 플레이어가 다른 청크로 이동했는지 확인
        if (newPlayerChunk != currentPlayerChunk)
        {
            // 청크가 바뀌면 타일들을 재배치
            UpdateTiles(newPlayerChunk);
            currentPlayerChunk = newPlayerChunk;
        }
    }

    /// <summary>
    /// 게임 시작 시 플레이어 주변에 초기 타일 그리드 생성
    /// 예: gridSize=3이면 -1~1 범위로 3x3 그리드 생성
    /// </summary>
    void InitializeGrid()
    {
        // gridSize=3이면 halfSize=1, 즉 -1, 0, 1 범위로 생성
        int halfSize = gridSize / 2;

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
                Vector3 worldPos = new Vector3(x * tileSize, y * tileSize, 0);

                // 타일 생성 및 배치
                GameObject tile = Instantiate(tilePrefab, worldPos, Quaternion.identity, transform);
                tile.name = $"Tile_{x}_{y}";

                // 딕셔너리에 타일 저장 (나중에 재사용하기 위해)
                tileGrid[gridPos] = tile;
            }
        }
    }

    /// <summary>
    /// 플레이어의 월드 좌표를 청크 좌표로 변환 (2D 버전)
    /// 예: 플레이어가 (25, 15)에 있고 tileSize=20이면
    ///     청크 좌표는 (1, 1)이 됨
    /// </summary>
    Vector2Int GetChunkPosition(Vector3 worldPosition)
    {
        // 2D에서는 x, y 좌표 사용 (z는 무시)
        int x = Mathf.RoundToInt(worldPosition.x / tileSize);
        int y = Mathf.RoundToInt(worldPosition.y / tileSize);
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// 플레이어가 새로운 청크로 이동했을 때 타일들을 재배치
    /// 핵심: 기존 타일을 삭제하지 않고 위치만 이동시킴 (재사용)
    /// </summary>
    void UpdateTiles(Vector2Int newCenterChunk)
    {
        int halfSize = gridSize / 2;

        // 재배치된 타일들을 저장할 새 딕셔너리
        Dictionary<Vector2Int, GameObject> newGrid = new Dictionary<Vector2Int, GameObject>();

        // 새로운 플레이어 위치를 중심으로 필요한 타일 위치 계산
        for (int x = -halfSize; x <= halfSize; x++)
        {
            for (int y = -halfSize; y <= halfSize; y++)
            {
                // 새로운 그리드에서 이 타일이 있어야 할 절대 좌표
                // 예: 플레이어가 (2, 3) 청크에 있고, 상대위치가 (-1, 0)이면
                //     실제 타일은 (1, 3) 위치에 있어야 함
                Vector2Int targetGridPos = new Vector2Int(
                    newCenterChunk.x + x,
                    newCenterChunk.y + y
                );

                // 기존 타일 중 아직 사용하지 않은 타일 하나를 찾아서 재사용
                GameObject tile = FindClosestUnusedTile(targetGridPos, newGrid);

                if (tile != null)
                {
                    // 타일을 새로운 월드 위치로 이동 (2D는 x, y만 사용, z는 0)
                    Vector3 newWorldPos = new Vector3(
                        targetGridPos.x * tileSize,
                        targetGridPos.y * tileSize,
                        0
                    );
                    tile.transform.position = newWorldPos;

                    // 디버깅용 이름 변경
                    tile.name = $"Tile_{targetGridPos.x}_{targetGridPos.y}";

                    // 새 그리드에 타일 등록
                    newGrid[targetGridPos] = tile;
                }
            }
        }

        // 기존 그리드를 새 그리드로 교체
        // 이제 tileGrid는 플레이어 주변의 새로운 타일 배치를 가리킴
        tileGrid = newGrid;
    }

    /// <summary>
    /// 아직 재배치되지 않은 타일을 찾아서 반환
    /// 이미 newGrid에 등록된 타일은 건너뛰고, 사용 가능한 타일 반환
    /// </summary>
    GameObject FindClosestUnusedTile(Vector2Int targetPos, Dictionary<Vector2Int, GameObject> usedTiles)
    {
        // 기존 tileGrid의 모든 타일을 순회
        foreach (var kvp in tileGrid)
        {
            // 이 타일이 이미 새로운 위치에 배치되었으면 건너뛰기
            if (usedTiles.ContainsValue(kvp.Value))
                continue;

            // 아직 사용되지 않은 타일 발견! 이걸 재사용
            return kvp.Value;
        }

        // 모든 타일이 사용됨 (정상적으로는 발생하지 않음)
        return null;
    }

    /// <summary>
    /// Scene 뷰에서 타일 배치를 시각적으로 확인하기 위한 기즈모 (2D)
    /// </summary>
    void OnDrawGizmos()
    {
        // 게임이 실행 중이고 플레이어가 있을 때만 그리기
        if (!Application.isPlaying || player == null) return;

        // 플레이어가 현재 있는 청크를 노란색으로 표시
        Gizmos.color = Color.yellow;
        Vector3 chunkCenter = new Vector3(
            currentPlayerChunk.x * tileSize,
            currentPlayerChunk.y * tileSize,
            0
        );
        // 2D에서는 정사각형으로 표시
        Gizmos.DrawWireCube(chunkCenter, new Vector3(tileSize, tileSize, 0.1f));

        // 모든 활성화된 타일의 경계를 초록색으로 표시
        Gizmos.color = Color.green;
        foreach (var kvp in tileGrid)
        {
            Vector3 pos = kvp.Value.transform.position;
            Gizmos.DrawWireCube(pos, new Vector3(tileSize, tileSize, 0.1f));
        }
    }
}