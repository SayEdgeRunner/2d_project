namespace Enemy
{
    /// <summary>
    /// 적의 생명 상태를 나타내는 열거형
    /// </summary>
    public enum EnemyLifeState
    {
        /// <summary>
        /// 살아있음 - 정상 동작, AI 활성화
        /// </summary>
        Alive,

        /// <summary>
        /// 사망 중 - 사망 연출 재생 중, AI 비활성화, 데미지 무시
        /// </summary>
        Dying,

        /// <summary>
        /// 사망 완료 - 풀 반환 준비
        /// </summary>
        Dead
    }
}
