using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    // 어떤 스킬을 가지고 있는지 관리하고
    // 레벨업 때 선택지를 만들고
    // 선택된 스킬을 적용하고

    // 어떤 스킬이 선택되었는지 알아야 함
    // 예를 들어서
    // 산데비스탄에서 점멸이 선택되었구나,
    // 해커에서는 디버프가 선택되었구나,
    // 버서커에서는 아무것도 선택되지 않았구나 이런 것
    private Dictionary<ESkillCategory, SkillBase> _ownedSkills = new Dictionary<ESkillCategory, SkillBase>();
}
