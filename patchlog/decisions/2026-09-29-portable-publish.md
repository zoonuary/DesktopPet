# 테스트용 배포 방식: 포터블 self-contained 단일 실행 파일

Date: 2026-09-29

## 배경

다른 PC에서 설치 과정 없이 바로 실행해 테스트할 수 있는 빌드 결과물이 필요했다.

## 결정

정식 설치 프로그램(MSI/Installer, WiX/Inno Setup 등 도구 선택 포함)은 이번 단계에서 만들지 않는다. `patchlog/decisions/2026-09-18-v1-scope.md`에서 이미 "설치 프로그램 구체 도구는 실제 배포 단계에서 결정"이라고 못박아뒀고, 지금은 테스트가 목적이라 그 단계가 아니다.

대신 `dotnet publish`로 .NET 런타임까지 포함한 **win-x64 self-contained 단일 실행 파일**을 만든다. 대상 PC에 .NET이 설치되어 있지 않아도 실행 파일만 복사하면 바로 실행된다.

## 절차

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
```

출력 위치: `bin/Release/net10.0-windows/win-x64/publish/`

- `DesktopPet.exe` — 단일 실행 파일. .NET 런타임 + WPF/WinForms 포함이라 약 170MB.
- `DesktopPet.pdb` — 디버그 심볼. 배포에 필수는 아니지만 남겨둠.
- `Assets/Pets/default/` — `DesktopPet.csproj`의 `CopyToOutputDirectory` 설정 덕분에 `dotnet publish`에서도 자동으로 함께 복사된다. 별도 처리 불필요.

테스트 배포 시에는 `publish/` 폴더 전체를 zip으로 묶어 전달한다 (exe만 옮기면 `Assets`가 빠져 스킨 로드가 실패하고 자리표시자로 폴백한다).

## 영향

- `bin/`은 `.gitignore`에 이미 포함되어 있어 `publish/` 산출물이 저장소에 커밋되지 않는다.
- 정식 설치 프로그램(시작 메뉴 등록, 제거 항목, 설치 경로 관리 등)은 여전히 미정 — 실제 배포 단계에서 별도로 결정한다.
