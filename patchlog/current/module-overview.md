# DesktopPet Overview

Last verified: 2026-09-18

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
  MainWindow.xaml / .xaml.cs    메인 윈도우 (현재 기본 템플릿 상태, 콘텐츠 없음)
  AssemblyInfo.cs
  .editorconfig                 네이밍/nullable 컨벤션
  DEV_GUIDELINES.md             개발 협업/코드 규칙
  AGENTS.md                     patchlog 사용 규칙
  patchlog/                     현재 문서
```

아직 Views/Services/ViewModels 등 계층 분리는 없다. `DEV_GUIDELINES.md` 5번 규칙에 따라 실제 기능(펫 렌더링, 애니메이션, 설정 등)을 추가하는 시점에 도입 예정.

## Key Components

- `App` (`App.xaml.cs`): WPF 기본 애플리케이션 진입점. 커스텀 로직 없음.
- `MainWindow` (`MainWindow.xaml.cs`): `dotnet new wpf` 기본 템플릿 그대로. `Grid` 하나만 있고 콘텐츠 없음.

## Current Execution Flow

1. `App` 시작 → `MainWindow` 표시.
2. `MainWindow`는 기본 800x450 크기 창으로, 투명/항상 위/클릭스루 등 데스크탑 펫에 필요한 창 동작은 아직 구현되지 않음.

## External Dependencies

- Target Framework: `net10.0-windows`
- `Nullable`, `ImplicitUsings` 활성화
- 외부 NuGet 패키지 없음 (기본 WPF SDK만 사용)
- Git remote: `origin` → `https://github.com/zoonuary/DesktopPet.git`

## Confirmed Constraints

- 아직 실제 "펫" 기능(스프라이트, 애니메이션, 트레이 아이콘, 설정 저장 등)은 구현되지 않음. 프로젝트 스캐폴드 + 개발 규칙 문서 + patchlog 시스템만 갖춘 상태.
- CFG(설정 저장/복원) 시스템 없음 — 아직 저장할 설정값이 없음.

## v1 Scope (decided)

자세한 배경과 대안은 `patchlog/decisions/2026-09-18-v1-scope.md` 참고.

- 비주얼: 정지 이미지 + 상태별 스프라이트 시트
- 필수 기능: 투명/항상 위 창 + 드래그 이동, 자동 걷기 애니메이션, 트레이 아이콘 + 우클릭 메뉴
- 배포: 설치 프로그램(MSI/Installer)

## Needs Confirmation

- 자동 업데이트 여부, 설치 프로그램 구체 도구(WiX/MSIX 등)는 실제 배포 단계에서 결정 예정.
- 다중 펫(여러 캐릭터 동시 실행) 지원 여부는 v1 이후로 보류.
