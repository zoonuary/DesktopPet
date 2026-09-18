# DesktopPet Overview

Last verified: 2026-09-18 (auto-wander movement added)

This document describes the current code state of `DesktopPet`.

Reference material used for background only:
- `DEV_GUIDELINES.md` (approval process, code conventions, CFG/performance rules)

The current code and this `patchlog/current/*` area take priority over older plans or decision logs.

## Responsibility

Windows 바탕화면 위에서 동작하는 데스크탑 펫(캐릭터) 애플리케이션. WPF(.NET 10, `net10.0-windows`) 기반 네이티브 앱.

## Main Layers

```text
DesktopPet/
  App.xaml / App.xaml.cs        앱 진입점, 전역 리소스
  MainWindow.xaml / .xaml.cs    메인 윈도우 (투명/항상 위 창, 드래그 이동, 자동 배회)
  PetWanderMovement.cs          좌우 자동 배회 위치 계산 로직 (View와 분리)
  AssemblyInfo.cs
  .editorconfig                 네이밍/nullable 컨벤션
  DEV_GUIDELINES.md             개발 협업/코드 규칙
  AGENTS.md                     patchlog 사용 규칙
  patchlog/                     현재 문서
```

아직 Views/Services/ViewModels 등 계층 분리는 없다. `DEV_GUIDELINES.md` 5번 규칙에 따라 실제 기능(펫 렌더링, 애니메이션, 설정 등)을 추가하는 시점에 도입 예정.

## Key Components

- `App` (`App.xaml.cs`): WPF 기본 애플리케이션 진입점. 커스텀 로직 없음.
- `MainWindow` (`MainWindow.xaml` / `.xaml.cs`): 200x200 크기, 테두리 없는(`WindowStyle=None`) 투명(`AllowsTransparency=True`, `Background=Transparent`) 항상 위(`Topmost=True`) 창. 작업표시줄에 표시되지 않음(`ShowInTaskbar=False`). 현재 콘텐츠는 자리표시자 `Ellipse`(CornflowerBlue) 하나뿐. `Window_MouseLeftButtonDown` 핸들러에서 `DragMove()`를 호출해 마우스 드래그로 창을 이동하며, 드래그 중에는 `_isDragging` 플래그로 자동 배회를 멈춘다. `Loaded` 시점에 화면 작업 영역 하단에 배치하고 `DispatcherTimer`(16ms)를 시작한다.
- `PetWanderMovement` (`PetWanderMovement.cs`): 좌우 자동 배회의 다음 X좌표를 계산하는 순수 로직 클래스. 초당 60px 속도로 이동하며 작업 영역 좌우 경계에 닿으면 방향을 반전한다. `MainWindow`와 분리되어 있어 UI 없이도 동작 검증 가능.

## Current Execution Flow

1. `App` 시작 → `MainWindow` 표시.
2. `MainWindow`는 투명/항상 위 200x200 창으로 뜨며, `Loaded` 시점에 작업 영역 하단에 배치되고 자동 배회 타이머가 시작된다.
3. `DispatcherTimer` Tick마다 `PetWanderMovement.GetNextLeft()`로 다음 위치를 계산해 `Left`에 반영, 화면 좌우 경계에서 방향 반전.
4. 사용자가 원 영역에서 좌클릭 드래그하면 자동 배회가 일시정지되고 `DragMove()`로 창이 마우스를 따라 이동, 드래그가 끝나면 배회 재개.
5. 걷기 애니메이션(스프라이트 전환), 트레이 아이콘/우클릭 메뉴, 실제 펫 스프라이트는 아직 미구현 — 현재는 정지된 원이 좌우로만 이동.

## External Dependencies

- Target Framework: `net10.0-windows`
- `Nullable`, `ImplicitUsings` 활성화
- 외부 NuGet 패키지 없음 (기본 WPF SDK만 사용)
- Git remote: `origin` → `https://github.com/zoonuary/DesktopPet.git`

## Confirmed Constraints

- 투명/항상 위/드래그 이동/좌우 자동 배회 창 동작은 구현되어 사용자가 직접 실행해 확인함.
- 실제 펫 스프라이트(이미지), 걷기 스프라이트 애니메이션, 트레이 아이콘/메뉴, 설정 저장(CFG)은 아직 미구현.
- CFG(설정 저장/복원) 시스템 없음 — 아직 저장할 설정값이 없음.

## v1 Scope (decided)

자세한 배경과 대안은 `patchlog/decisions/2026-09-18-v1-scope.md` 참고.

- 비주얼: 정지 이미지 + 상태별 스프라이트 시트
- 필수 기능: 투명/항상 위 창 + 드래그 이동, 자동 걷기 애니메이션, 트레이 아이콘 + 우클릭 메뉴
- 배포: 설치 프로그램(MSI/Installer)

## Needs Confirmation

- 자동 업데이트 여부, 설치 프로그램 구체 도구(WiX/MSIX 등)는 실제 배포 단계에서 결정 예정.
- 다중 펫(여러 캐릭터 동시 실행) 지원 여부는 v1 이후로 보류.
