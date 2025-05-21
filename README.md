# 🎮 Unity 기반 턴제 텍스트 RPG 프로젝트

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
## 설계 과정에서의 주요 고민

### 1. 버튼 생성 방식에 대한 고민 (Room List / 인벤토리)

처음에는 룸 리스트나 인벤토리 UI를 열 때마다 **프리팹을 동적으로 Instantiate** 하여 버튼을 생성하는 방식으로 구현하였습니다.  
하지만 이 방식은 반복적인 GameObject 생성으로 인해 **성능 저하**가 발생할 수 있다는 문제가 있었습니다.

#### 해결 방법: 오브젝트 풀링 매니저 도입

- 버튼 프리팹을 미리 일정 개수만큼 생성하여 **PoolingManager**에서 관리
- 오브젝트 종류가 다양해질 것을 고려해, `PoolingType` Enum을 정의
- 다음과 같은 구조로 버튼을 재사용:
  ```csharp
  Dictionary<PoolingType, List<ItemStruct>> poolingMap;

### 2. 인벤토리 자료구조에 대한 고민

처음에는 `Dictionary<이름, 개수>` 방식이 가장 효율적이라 판단했습니다.
하지만 같은 이름이라도 옵션이 다른 아이템(장비아이템 등)은 구분이 불가능할 것이라고 판단했습니다.

#### 해결 방법: 자료구조 변경

`Dictionary<이름, List<GameObject>>` 형태로 실제 오브젝트를 직접 관리하는 방식으로 변경

### 시연 영상
https://www.youtube.com/watch?v=HJFJ_HV83JI

