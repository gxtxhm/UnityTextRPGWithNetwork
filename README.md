
# 🎮 Unity 기반 턴제 RPG

> **턴제 전투 시스템**, **멀티플레이**, **오브젝트 풀링**, **아이템 관리**, **세이브/로드 시스템** 등을 직접 설계 및 구현한 Unity 기반 RPG 게임입니다.

## 📽️ 데모 영상
[👉 시연 영상 보러가기](https://youtu.be/your_video_link_here)

---

## 🧱 시스템 아키텍처

![System Diagram](./images/Unity_RPG_System_Diagram_Spaced_EN.png)

---

## ⚔️ 전투 시스템
- `Player`, `Monster`, `Boss` 객체들이 `OnAttackEvent`, `OnDeadEvent` 이벤트를 통해 상호작용합니다.
- 이벤트 기반 설계로 UI, 로그, 애니메이션 등을 느슨하게 연결할 수 있어 확장에 유리합니다.

📷 예시 코드:
![AttackEvent Code](./images/code_attack_event.png)

📝 특징
- 데미지 처리 후 체력이 0 이하일 경우 `OnDeadEvent` 발생
- 후처리 로직(UI 업데이트, 효과 등)은 이벤트에 등록만 하면 동작

---

## 🧪 오브젝트 풀링
- 인벤토리 및 멀티플레이 룸 버튼 생성 시 **동적 생성**은 비효율적
- `PoolingManager` + `ItemStruct` 조합으로 미리 생성, 재사용 구조 설계

📷 예시 코드:
![Pooling Code](./images/code_pooling.png)

📝 장점
- 메모리 할당/해제 비용 절감
- `ReturnToPool()`을 통해 객체 생명 주기 명확화

---

## 🧩 아이템 시스템
- 아이템은 `items.json` 파일로 외부 설정
- `HpPotion`, `AttackPotion`, `ShieldPotion`, `RandomPotion` 등 상속 구조로 설계
- `DurationItem`을 통해 지속 효과를 턴 기반으로 관리

📷 예시 코드:
![Item Code](./images/code_item_use.png)

📝 특징
- `ItemManager`에서 아이템 등록/사용/지속시간 관리 통합
- `OnUsedItem`, `OnEndItem` 등의 델리게이트 이벤트도 활용

---

## 🌐 멀티플레이 기능
- Photon PUN을 활용한 멀티플레이 방 생성/입장/채팅 기능
- `NetworkManager` 단일 진입점 설계, 모든 RPC 관리 일원화

📷 예시 화면:
![Lobby UI](./images/ui_lobby.png)

📝 동작
- 플레이어 준비 상태를 MasterClient가 감지하여 시작 버튼 활성화
- 대화, 준비, 퇴장 등 RPC로 모두 실시간 동기화

---

## 💾 세이브 / 로드 시스템
- `SaveLoadManager`를 통해 Player 상태, Inventory, 던전 진행 상황을 JSON으로 저장
- 최근 3개 저장 파일만 유지, 자동 삭제 포함

📷 예시 코드:
![SaveLoad Code](./images/code_saveload.png)

📝 구조
- `PlayerData`, `ProgressData`, `ItemInfo` 클래스로 직렬화
- UI에서 슬롯 버튼 클릭 시 바로 불러오기

---

## 📝 기타 기능
- `HoverText` + `IInfoProvider`를 통해 마우스 오버 시 툴팁 출력
- `PrintStringByTick()`으로 대사 텍스트 애니메이션 효과 구성

---

## 📁 실행 방법
1. Unity 2021.x 이상에서 프로젝트 열기
2. `Scenes/StartScene` 실행
3. Resources/Json/items.json 필수
4. 멀티플레이는 Photon App ID 필요

---

## ✨ 향후 개선 계획
- [ ] 캐릭터 애니메이션 및 전투 이펙트
- [ ] AI 몬스터 공격 로직
- [ ] 멀티 전투 턴 기반 동기화 개선
