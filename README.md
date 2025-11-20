# Commit & Branch Convention
## 1. Commit Convention
커밋 메시지는 다음 규칙을 따릅니다.

### 규칙
- type 목록  
  - `feat` : 기능 추가  
  - `fix` : 버그 수정  
  - `refactor` : 리팩토링  
  - `style` : 코드 스타일 변경 (포맷팅 등)  
  - `docs` : 문서 작업  
  - `test` : 테스트 코드  
  - `chore` : 빌드/환경 설정 등

### 예시
- feat: 플레이어 점프 기능 추가
- fix: 적 피격 판정 오류 해결
- chore: 빌드 설정 업데이트

  <br>
  
## 2. Branch Convention
브랜치 이름은 다음 규칙을 따릅니다.

### 규칙
- 모두 **소문자** 사용
- initial : 본인의 이니셜 (예: `jh`, `hy`, `ks`)
- type : 커밋 규칙의 type과 동일
- description : 여러 단어일 경우 하이픈(`-`) 사용

### 예시
- jh/feat/player-move
- jh/fix/score-save-bug
- jh/refactor/bullet-factory
