# Project_Up

![2](https://github.com/user-attachments/assets/3d99a146-3410-4abd-ba03-0728574b6868)
> 3D 환경에서 플레이어 이동, 상호작용, 아이템 수집/사용이 가능한 Unity 프로젝트.  
> 최근 업데이트로 장비 시스템을 제거하고 **소비형 아이템 중심의 인벤토리**로 단순화하였습니다.

---

## 📌 프로젝트 개요
`Project_Up`은 Unity를 기반으로 제작된 서바이벌 스타일 시스템 예제입니다.  
플레이어는 맵을 탐험하며 자원을 채집하고, 소비형 아이템을 사용해 체력·스태미나·속도 등을 관리할 수 있습니다.

---

## 실행 방법  
Unity 2021.3 이상 버전에서 프로젝트 열기  
Scenes 폴더의 메인 씬 실행  
### 조작키  
- WASD : 이동  
- Shift : 달리기  
- Space : 점프  
- Tab : 인벤토리 열기/닫기  
- E : 상호작용  

## 🛠 주요 기능

### 1. 플레이어 시스템
- **이동/점프/달리기** : 3인칭 시점, 카메라 충돌 보정.
- **스태미나 소비** : 달리기 시 실시간 소모.
- **플레이어 상태** : 체력·스태미나·속도 증가 버프 관리.
- **애니메이션** : 이동 속도·점프·낙하 상태 반영.

### 2. 아이템 & 인벤토리
- **ItemData** : 소비형(`Consumable`), 환경 오브젝트(`Environment`) 타입 지원.
- **UIInventory** : 슬롯 기반 인벤토리, 스택 가능 아이템, 아이템 사용/드롭.
- **ItemObject** : 상호작용 시 인벤토리에 추가.

### 3. 상호작용 시스템
- 카메라 중앙에서 Raycast로 오브젝트 탐지.
- UI 프롬프트 표시 및 인터페이스 호출.

### 4. 환경 오브젝트
- **JumpPad** : 플레이어 점프력 부여 + 애니메이션 트리거.
- **Move_SportCar** : 왕복/원형 궤도 이동 + 휠 회전.
- **Resource** : 채집 시 아이템 드롭, 수량 감소.

### 5. UI & 시각 효과
- **UICondition** : 체력·스태미나·버프 상태 표시.
- **DamageIndicator** : 피해 시 화면 플래시.
- **FootSteps** : 이동 속도에 따른 발자국 소리.

---

## 🆕 최근 변경 사항
- 장비 시스템(`Equip`, `EquipTool`) **삭제**.
- `ItemType`에서 `Equipable` 제거 → **Consumable**, **Environment**만 유지.
- `UIInventory` 장착/해제 버튼, 관련 코드 **전부 제거**.
- `Player` 클래스에서 `equip` 필드 제거.
- 인벤토리 → **소비형 아이템 사용 + 드롭 중심** 구조로 단순화.

---

## 📂 폴더 구조

```plaintext
Assets/
├── 02.Scripts/
│   ├── Character/
│   │   ├── CharacterManager.cs
│   │   ├── Player.cs
│   │   ├── PlayerController.cs
│   │   ├── PlayerCondition.cs
│   │   ├── PlayerAnimation.cs
│   │
│   ├── Interaction/
│   │   ├── Interaction.cs
│   │   ├── ItemObject.cs
│   │   ├── Resource.cs
│   │
│   ├── Inventory/
│   │   ├── ItemData.cs
│   │   ├── ItemSlot.cs
│   │   ├── UIInventory.cs
│   │
│   ├── UI/
│   │   ├── Condition.cs
│   │   ├── UICondition.cs
│   │   ├── DamageIndicator.cs
│   │
│   ├── Environment/
│   │   ├── JumpPad.cs
│   │   ├── Move_SportCar.cs
│   │
│   └── Sound/
│       └── FootSteps.cs


