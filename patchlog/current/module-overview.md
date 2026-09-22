# DesktopPet Overview

Last verified: 2026-09-22 (Stay/Focus tray menu wired, timer/color-update bugs fixed)

This document describes the current code state of `DesktopPet`.

Reference material used for background only:
- `DEV_GUIDELINES.md` (approval process, code conventions, CFG/performance rules)
- `DESKTOP_PET_BEHAVIOR_SPEC.md` (상태 머신, 모드, 좌표계, 구현 구조 — v1 상세 설계 기준 문서)
- `DESKTOP_PET_ASSET_GUIDE.md` (스프라이트 제작 규격, `character.json` 스키마)
- `DESKTOP_PET_IDEAS.md` (확정 명세 아님, 로드맵/아이디어 참고 자료)

The current code and this `patchlog/current/*` area take priority over older plans or decision logs.

## Responsibility

Windows 바탕화면 위에서 동작하는 데스크탑 펫(캐릭터) 애플리케이션. WPF(.NET 10, `net10.0-windows`) 기반 네이티브 앱.

## Main Layers

```text
DesktopPet/
  App.xaml / App.xaml.cs        앱 진입점, 전역 리소스
  MainWindow.xaml / .xaml.cs    메인 윈도우 (투명/항상 위 창, 드래그 이동, 자동 배회)
  PetWanderMovement.cs          좌우 이동의 다음 X좌표 계산 (방향은 인자로 받음, 경계 도달만 보고)
  TrayIconService.cs            시스템 트레이 아이콘 + 우클릭 종료 메뉴 (View와 분리, IDisposable)
  Behavior/
    PetState.cs                  PetStateId / PetFacing / PetMode enum
    PetBehaviorController.cs     상태 전환, 모드, 지속 시간, 확률 규칙 (순수 로직, View 비의존)
  AssemblyInfo.cs
  .editorconfig                 네이밍/nullable 컨벤션
  DEV_GUIDELINES.md             개발 협업/코드 규칙
  AGENTS.md                     patchlog 사용 규칙
  DESKTOP_PET_BEHAVIOR_SPEC.md  v1 상세 설계 (상태 머신, 모드, 좌표계, 구현 구조)
  DESKTOP_PET_ASSET_GUIDE.md    스프라이트 제작 규격, character.json 스키마
  DESKTOP_PET_IDEAS.md          로드맵/아이디어 참고 자료 (확정 명세 아님)
  Assets/Pets/default/          앱이 런타임에 읽는 최종 스킨 (exe 옆에 복사됨, 현재 README만 존재)
  ArtSource/default/            이미지 제작 작업용 원본 보관소 (배포 대상 아님)
  patchlog/                     현재 문서
```

아직 Views/Services/ViewModels 등 계층 분리는 없다. `DEV_GUIDELINES.md` 5번 규칙에 따라 실제 기능(펫 렌더링, 애니메이션, 설정 등)을 추가하는 시점에 도입 예정.

## Key Components

- `App` (`App.xaml.cs`): WPF 기본 애플리케이션 진입점. 커스텀 로직 없음.
- `MainWindow` (`MainWindow.xaml` / `.xaml.cs`): 200x200 크기, 테두리 없는(`WindowStyle=None`) 투명(`AllowsTransparency=True`, `Background=Transparent`) 항상 위(`Topmost=True`) 창. 작업표시줄에 표시되지 않음(`ShowInTaskbar=False`). 콘텐츠는 자리표시자 `Ellipse`(`x:Name="PetPlaceholder"`) 하나이며, 실제 스프라이트가 없는 동안 `PetBehaviorController.State`에 따라 Fill 색이 바뀐다 (Idle=CornflowerBlue, Walk=LimeGreen, Rest=Orange, Sleep=Gray, Drag=MediumPurple). `Window_MouseLeftButtonDown`에서 `_behavior.EnterDrag()` 후 `DragMove()`를 호출하고, 끝나면 `_behavior.ExitDrag()`로 모드 기본 상태로 복귀한다. `Loaded` 시점에 화면 작업 영역 하단에 배치하고 `DispatcherTimer`(16ms)를 시작한다.
- `PetWanderMovement` (`PetWanderMovement.cs`): 주어진 방향(`directionX`)으로 다음 X좌표를 계산하고 경계 도달 여부만 보고하는 순수 로직 클래스. 방향을 자체적으로 반전하지 않는다 — 방향 소유권은 `PetBehaviorController.Facing`에 있다. 초당 60 DIP 속도.
- `Behavior.PetState` (`Behavior/PetState.cs`): `PetStateId`(Idle/Walk/Rest/Sleep/React/Drag), `PetFacing`(Left/Right), `PetMode`(Normal/Stay/Focus) enum. React는 아직 미사용.
- `Behavior.PetBehaviorController` (`Behavior/PetBehaviorController.cs`): 현재 상태·방향·모드와 남은 지속 시간을 소유. `Tick(elapsedSeconds)`로 지속 시간을 소모하고 만료 시 `DESKTOP_PET_BEHAVIOR_SPEC.md`의 확률 규칙(Idle→Walk 35%/Rest 65%, Rest→Idle 75%/Sleep 25%)에 따라 다음 상태로 전환한다. `NotifyBoundaryHit()`로 Walk 중 경계 도달을 알리면 Idle로 전환. `EnterDrag()`/`ExitDrag()`로 드래그 상태 진입/종료(종료 시 현재 모드의 기본 상태로 복귀)를 처리. `SetMode()`로 Normal/Stay/Focus 전환(Stay→Rest 유지, Focus→Sleep 유지). View나 WPF 타입을 알지 못하는 순수 로직.
- `TrayIconService` (`TrayIconService.cs`): `System.Windows.Forms.NotifyIcon` 기반 시스템 트레이 아이콘. 생성자에서 초기 모드, 모드 선택 콜백(`Action<PetMode>`), 종료 콜백(`Action`)을 받는다. 우클릭 메뉴에 "일반 모드"/"여기서 쉬기"/"집중 모드"(체크 표시로 현재 모드 표시) + 구분선 + "종료"가 있다. 아이콘은 아직 전용 에셋이 없어 `SystemIcons.Application`(임시)을 사용한다. "설정" 메뉴 항목은 설정 화면이 아직 없어 포함하지 않음 — 설정 화면 구현 시 함께 추가 예정. `IDisposable`로 트레이 아이콘 해제를 명시적으로 처리하며, `MainWindow.Closed`에서 호출된다.

## Current Execution Flow

1. `App` 시작 → `MainWindow` 표시.
2. `MainWindow` 생성자에서 `TrayIconService`를 생성해 트레이 아이콘을 띄운다.
3. `MainWindow`는 투명/항상 위 200x200 창으로 뜨며, `Loaded` 시점에 작업 영역 하단에 배치되고 배회/행동 타이머가 시작된다.
4. `DispatcherTimer` Tick마다: 경과 시간을 최대 100ms로 clamp → `_behavior.Tick()`으로 상태 전환 처리 → 상태가 `Walk`이면 `PetWanderMovement.GetNextLeft()`로 위치 계산 후 `Left` 반영, 경계 도달 시 `_behavior.NotifyBoundaryHit()` 호출 → 상태가 바뀌었으면 자리표시자 색 갱신. 타이머 우선순위는 `DispatcherPriority.Normal`이다 (아래 "해결된 이슈" 참고).
5. 사용자가 원 영역에서 좌클릭하면 `_behavior.EnterDrag()`로 즉시 Drag 상태(보라색)가 되고 자동 배회/행동 틱이 멈춘 채 `DragMove()`로 창이 마우스를 따라 이동한다. 드래그가 끝나면 `_behavior.ExitDrag()`로 현재 모드의 기본 상태(Normal→Idle 등)로 복귀한다.
6. 트레이 아이콘 우클릭 → "일반 모드"/"여기서 쉬기"/"집중 모드"를 누르면 `_behavior.SetMode()` 호출 직후 `UpdatePlaceholderColor()`를 콜백 안에서 직접 호출해 클릭 즉시 색이 반영된다 (타이머 틱을 기다리지 않음). "종료"를 누르면 `System.Windows.Application.Current.Shutdown()`이 호출되어 앱이 종료된다. `MainWindow.Closed`에서 배회 타이머 정지 + 트레이 아이콘 해제.
7. React 상태, 걷기 스프라이트 애니메이션, 설정 메뉴/화면, 실제 펫 스프라이트는 아직 미구현 — 현재는 자리표시자 원의 색과 좌우 이동으로만 상태를 표시한다.

### 해결된 이슈

- **타이머가 자동으로 진행되지 않던 문제**: `DispatcherTimer`를 `DispatcherPriority.Render`로 생성했더니, 화면이 다시 그려질 이유가 없으면 틱이 꾸준히 실행되지 않아 자동 상태 전환(Idle→Walk/Rest 등)이 사실상 멈춰 있었다. `DispatcherPriority.Normal`로 바꿔 렌더 패스와 무관하게 꾸준히 틱이 돌도록 수정했다.
- **트레이 모드 메뉴 클릭 시 색이 안 바뀌던 문제**: 모드 선택 콜백이 `_behavior.SetMode()`만 호출하고 색 갱신은 다음 타이머 틱에 맡겼는데, 클릭 즉시 반응해야 하는 사용자 액션이라 콜백 안에서 `UpdatePlaceholderColor()`를 바로 호출하도록 수정했다.

## External Dependencies

- Target Framework: `net10.0-windows`
- `Nullable`, `ImplicitUsings` 활성화
- `UseWPF`, `UseWindowsForms` 모두 활성화 (트레이 아이콘용 `System.Windows.Forms.NotifyIcon` 참조 목적). 두 네임스페이스에 동명 타입(`Application` 등)이 있어 `App.xaml.cs`와 `MainWindow.xaml.cs`에서 `System.Windows.Application`으로 완전 한정해 참조한다.
- `Assets\**\*`는 `<None Update>` + `CopyToOutputDirectory=PreserveNewest`로 빌드 결과물(exe) 옆에 복사되는 느슨한 파일이다. 임베디드 리소스가 아니므로 사용자가 재빌드 없이 스킨을 교체할 수 있다. 자세한 이유는 `patchlog/decisions/2026-09-22-detailed-design-and-asset-root.md` 참고.
- 외부 NuGet 패키지 없음 (기본 WPF + WinForms SDK만 사용)
- Git remote: `origin` → `https://github.com/zoonuary/DesktopPet.git`

## Confirmed Constraints

- 투명/항상 위/드래그 이동/좌우 자동 배회/트레이 아이콘·종료/Idle·Walk·Rest·Sleep·Drag 상태 전환은 구현되어 사용자가 직접 실행해 확인함 (자리표시자 색 변화로 검증).
- Stay/Focus 모드 전환은 트레이 메뉴로 구현되어 사용자가 직접 클릭해 확인함.
- React 상태, 걷기 스프라이트 애니메이션, 설정 메뉴/화면, 설정 저장(CFG)은 아직 미구현.
- 좁은 작업 영역에서 Walk 대신 Rest를 선택하는 규칙(BEHAVIOR_SPEC 2장)은 아직 미구현 — 현재는 화면 폭과 무관하게 확률대로 Walk를 선택할 수 있다.
- CFG(설정 저장/복원) 시스템 없음 — 아직 저장할 설정값이 없음.

## v1 Scope (decided)

배경: `patchlog/decisions/2026-09-18-v1-scope.md` (방향), `patchlog/decisions/2026-09-22-detailed-design-and-asset-root.md` (상세 설계 채택 + 에셋 저장 방식).

- 비주얼/행동: `DESKTOP_PET_BEHAVIOR_SPEC.md` 기준 — Idle/Walk/Rest/Sleep/React/Drag 상태 머신, Normal/Stay/Focus 모드
- 에셋: `DESKTOP_PET_ASSET_GUIDE.md` 기준 — 256×256 프레임 스프라이트 스트립, `character.json` 스키마, 외부(비임베디드) 리소스 폴더
- 필수 기능: 투명/항상 위 창 + 드래그 이동(완료), 행동 상태 머신 Idle/Walk/Rest/Sleep/Drag(완료, 이미지 없이 색으로 검증), 자동 걷기 스프라이트 애니메이션(미구현), 트레이 아이콘 + 우클릭 메뉴(완료 — 모드 전환/종료, "설정" 항목만 보류)
- 배포: 설치 프로그램(MSI/Installer)

## Needs Confirmation

- 자동 업데이트 여부, 설치 프로그램 구체 도구(WiX/MSIX 등)는 실제 배포 단계에서 결정 예정.
- 다중 펫(여러 캐릭터 동시 실행) 지원 여부는 v1 이후로 보류.
- 트레이 우클릭 메뉴의 "설정" 항목과 실제 설정 화면은 아직 미구현 — 별도 기능으로 착수 예정.
- 트레이 아이콘은 임시 시스템 아이콘(`SystemIcons.Application`) 사용 중 — 실제 펫 브랜드 아이콘(.ico)으로 교체 필요.
- **블로커**: 원본 캐릭터 이미지가 아직 없음 (AI로 제작 예정, 진행 중). `ArtSource/default/reference.png`가 나와야 `DESKTOP_PET_ASSET_GUIDE.md`의 제작 순서(대표 자세 → 연속 프레임)를 시작할 수 있다.
