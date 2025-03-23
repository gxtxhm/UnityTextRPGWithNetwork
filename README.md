# 🎮 Unity 기반 턴제 RPG 프로젝트

> **턴제 전투 시스템**, **멀티플레이**, **오브젝트 풀링**, **아이템 관리**, **세이브/로드 시스템** 등을 직접 설계 및 구현한 Unity 기반 RPG 게임입니다.

##  사용 기술
- **Engine:** Unity (C#)
- **Networking:** Photon PUN
- **Data Format:** JSON
- **UI/UX:** Coroutine, TMPro, Hover Tooltip
- **Design Pattern:** 싱글톤, 이벤트 기반(Action), 오브젝트 풀링, 인터페이스 활용

---

## 🔍 주요 기능 및 설계

###  오브젝트 풀링 시스템
- 인벤토리 버튼, 룸 리스트 버튼 등 **UI 재사용을 위한 풀링 시스템 직접 구현**
- `PoolingManager`에서 오브젝트를 미리 생성
- `ItemStruct` 클래스 내 `ReturnToPool()` 메서드를 통해 사용자 측에서 사용
- 다양한 타입을 관리하기 위해 `Dictionary<PoolingType, List<ItemStruct>>`로 구성

###  텍스트 출력 애니메이션
- `PrintStringByTick()` 함수로 **한 글자씩 출력되는 애니메이션 연출** 구현
- Coroutine 기반 + Action 델리게이트로 다음 동작을 자연스럽게 연결

###  아이템 시스템 및 JSON 기반 설정
- `items.json` 파일을 파싱하여 게임 시작 시 아이템 정보 불러오기
- `HpPotion`, `AttackPotion`, `ShieldPotion`, `RandomPotion` 등 상속 구조로 설계
- 지속 효과 아이템은 `DurationItem`을 상속받아 별도 관리 (`UpdateItemManager()`로 매 턴 체크)

###  이벤트 기반 전투 설계
- `Player`, `Monster`, `Boss` 등에 `OnDeadEvent`, `OnAttackEvent` 등록
- 사망 시 UI 업데이트, 로그 출력, 애니메이션 트리거 등 다양한 후처리 가능
- **낮은 결합도, 높은 확장성**을 고려해 설계

###  멀티플레이 기능 (Photon PUN)
- `NetworkManager`를 통해 관리함, `PhotonView`는 해당 매니저에만 부착
- RPC 기반으로 채팅, 방 입장/퇴장, 준비 상태 동기화
- 준비 완료 인원 수를 MasterClient에서만 카운트함

###  세이브/로드 기능
- 플레이어 상태, 인벤토리, 던전 진행 상황을 JSON으로 저장/불러오기
- 최근 3개의 세이브 파일을 자동으로 관리하며, 가장 오래된 파일은 자동 삭제

###  Hover 텍스트 UI
- `IInfoProvider` 인터페이스를 구현한 객체에 마우스 오버 시 정보 툴팁 제공
- `HoverText` 컴포넌트 자동 부착 및 텍스트 출력 → **정보 확인 편의성 향상**

---

##  주요 성과
- 다양한 시스템을 **직접 설계하고, 객체 지향적으로 구조화**
- Action 기반 이벤트 시스템, 데이터 외부화(JSON), 재사용 가능한 오브젝트 풀링 등으로 **유지보수성과 확장성** 확보
- 싱글/멀티 모드 전환이 가능한 구조 구현

---

