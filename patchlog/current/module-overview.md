# DesktopPet Overview

Last verified: 2026-09-22 (detailed design docs adopted, asset folders scaffolded)

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
  PetWanderMovement.cs          좌우 자동 배회 위치 계산 로직 (View와 분리)
  TrayIconService.cs            시스템 트레이 아이콘 + 우클릭 종료 메뉴 (View와 분리, IDisposable)
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
- `MainWindow` (`MainWindow.xaml` / `.xaml.cs`): 200x200 크기, 테두리 없는(`WindowStyle=None`) 투명(`AllowsTransparency=True`, `Background=Transparent`) 항상 위(`Topmost=True`) 창. 작업표시줄에 표시되지 않음(`ShowInTaskbar=False`). 현재 콘텐츠는 자리표시자 `Ellipse`(CornflowerBlue) 하나뿐. `Window_MouseLeftButtonDown` 핸들러에서 `DragMove()`를 호출해 마우스 드래그로 창을 이동하며, 드래그 중에는 `_isDragging` 플래그로 자동 배회를 멈춘다. `Loaded` 시점에 화면 작업 영역 하단에 배치하고 `DispatcherTimer`(16ms)를 시작한다.
- `PetWanderMovement` (`PetWanderMovement.cs`): 좌우 자동 배회의 다음 X좌표를 계산하는 순수 로직 클래스. 초당 60px 속도로 이동하며 작업 영역 좌우 경계에 닿으면 방향을 반전한다. `MainWindow`와 분리되어 있어 UI 없이도 동작 검증 가능.
- `TrayIconService` (`TrayIconService.cs`): `System.Windows.Forms.NotifyIcon` 기반 시스템 트레이 아이콘. 생성자에서 종료 콜백(`Action`)을 받아 우클릭 메뉴 "종료" 클릭 시 호출한다. 아이콘은 아직 전용 에셋이 없어 `SystemIcons.Application`(임시)을 사용한다. "설정" 메뉴 항목은 설정 화면이 아직 없어 포함하지 않음 — 설정 화면 구현 시 함께 추가 예정. `IDisposable`로 트레이 아이콘 해제를 명시적으로 처리하며, `MainWindow.Closed`에서 호출된다.

## Current Execution Flow

1. `App` 시작 → `MainWindow` 표시.
2. `MainWindow` 생성자에서 `TrayIconService`를 생성해 트레이 아이콘을 띄운다.
3. `MainWindow`는 투명/항상 위 200x200 창으로 뜨며, `Loaded` 시점에 작업 영역 하단에 배치되고 자동 배회 타이머가 시작된다.
4. `DispatcherTimer` Tick마다 `PetWanderMovement.GetNextLeft()`로 다음 위치를 계산해 `Left`에 반영, 화면 좌우 경계에서 방향 반전.
5. 사용자가 원 영역에서 좌클릭 드래그하면 자동 배회가 일시정지되고 `DragMove()`로 창이 마우스를 따라 이동, 드래그가 끝나면 배회 재개.
6. 트레이 아이콘 우클릭 → "종료"를 누르면 `System.Windows.Application.Current.Shutdown()`이 호출되어 앱이 종료된다. `MainWindow.Closed`에서 배회 타이머 정지 + 트레이 아이콘 해제.
7. 걷기 애니메이션(스프라이트 전환), 설정 메뉴/화면, 실제 펫 스프라이트는 아직 미구현 — 현재는 정지된 원이 좌우로만 이동.

## External Dependencies

- Target Framework: `net10.0-windows`
- `Nullable`, `ImplicitUsings` 활성화
- `UseWPF`, `UseWindowsForms` 모두 활성화 (트레이 아이콘용 `System.Windows.Forms.NotifyIcon` 참조 목적). 두 네임스페이스에 동명 타입(`Application` 등)이 있어 `App.xaml.cs`와 `MainWindow.xaml.cs`에서 `System.Windows.Application`으로 완전 한정해 참조한다.
- `Assets\**\*`는 `<None Update>` + `CopyToOutputDirectory=PreserveNewest`로 빌드 결과물(exe) 옆에 복사되는 느슨한 파일이다. 임베디드 리소스가 아니므로 사용자가 재빌드 없이 스킨을 교체할 수 있다. 자세한 이유는 `patchlog/decisions/2026-09-22-detailed-design-and-asset-root.md` 참고.
- 외부 NuGet 패키지 없음 (기본 WPF + WinForms SDK만 사용)
- Git remote: `origin` → `https://github.com/zoonuary/DesktopPet.git`

## Confirmed Constraints

- 투명/항상 위/드래그 이동/좌우 자동 배회/트레이 아이콘·종료 동작은 구현되어 사용자가 직접 실행해 확인함.
- 실제 펫 스프라이트(이미지), 걷기 스프라이트 애니메이션, 설정 메뉴/화면, 설정 저장(CFG)은 아직 미구현.
- CFG(설정 저장/복원) 시스템 없음 — 아직 저장할 설정값이 없음.

## v1 Scope (decided)

배경: `patchlog/decisions/2026-09-18-v1-scope.md` (방향), `patchlog/decisions/2026-09-22-detailed-design-and-asset-root.md` (상세 설계 채택 + 에셋 저장 방식).

- 비주얼/행동: `DESKTOP_PET_BEHAVIOR_SPEC.md` 기준 — Idle/Walk/Rest/Sleep/React/Drag 상태 머신, Normal/Stay/Focus 모드
- 에셋: `DESKTOP_PET_ASSET_GUIDE.md` 기준 — 256×256 프레임 스프라이트 스트립, `character.json` 스키마, 외부(비임베디드) 리소스 폴더
- 필수 기능: 투명/항상 위 창 + 드래그 이동(완료), 자동 걷기 애니메이션, 트레이 아이콘(완료) + 우클릭 메뉴
- 배포: 설치 프로그램(MSI/Installer)

## Needs Confirmation

- 자동 업데이트 여부, 설치 프로그램 구체 도구(WiX/MSIX 등)는 실제 배포 단계에서 결정 예정.
- 다중 펫(여러 캐릭터 동시 실행) 지원 여부는 v1 이후로 보류.
- 트레이 우클릭 메뉴의 "설정" 항목과 실제 설정 화면은 아직 미구현 — 별도 기능으로 착수 예정.
- 트레이 아이콘은 임시 시스템 아이콘(`SystemIcons.Application`) 사용 중 — 실제 펫 브랜드 아이콘(.ico)으로 교체 필요.
- **블로커**: 원본 캐릭터 이미지가 아직 없음 (AI로 제작 예정, 진행 중). `ArtSource/default/reference.png`가 나와야 `DESKTOP_PET_ASSET_GUIDE.md`의 제작 순서(대표 자세 → 연속 프레임)를 시작할 수 있다.
