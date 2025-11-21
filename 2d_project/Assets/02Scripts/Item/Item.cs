using UnityEngine;

public enum EItemtype
    {
    ExperienceUp,
    HealthUp
    }
public class Item : MonoBehaviour
{
    [Header("아이템 타입")]
    public EItemtype Type;
    [SerializeField] private Transform _player; // 추적할 플레이어

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") == false) return;

        Destroy(gameObject);
    }
}
