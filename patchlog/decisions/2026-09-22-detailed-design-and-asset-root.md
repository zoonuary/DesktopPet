# v1 상세 설계 채택 및 외부 에셋 루트 결정

Date: 2026-09-22

## 배경

`patchlog/decisions/2026-09-18-v1-scope.md`에서 정한 v1 범위(스프라이트 비주얼, 투명/드래그/배회 창, 트레이 메뉴, MSI 배포)는 방향만 정한 수준이었다.

이후 사용자가 `DESKTOP_PET_BEHAVIOR_SPEC.md`, `DESKTOP_PET_ASSET_GUIDE.md`, `DESKTOP_PET_IDEAS.md`를 저장소에 추가했다. 세 문서는 이미 현재 코드(`PetWanderMovement.cs`, 트레이/드래그 동작)를 인지한 상태로 작성되어 있고, 서로 범위를 명시적으로 조율하고 있었다 (IDEAS.md는 스스로 "성장 시스템은 초기 범위 제외, 구체 구현은 BEHAVIOR_SPEC·ASSET_GUIDE 우선"이라고 명시).

## 결정

### 상세 설계 채택

`DESKTOP_PET_BEHAVIOR_SPEC.md`와 `DESKTOP_PET_ASSET_GUIDE.md`를 v1의 상세 설계 기준 문서로 채택한다. 내용을 트리밍하지 않고 그대로 커밋했다 — 두 문서가 이미 스스로 범위를 잘 정리해두어 제거할 불필요한 부분을 찾지 못했다.

`DESKTOP_PET_IDEAS.md`는 문서 자체가 명시한 대로 확정 명세가 아니라 로드맵/아이디어 참고 자료로 취급한다. 친밀도·성장·방석·공 등은 v1 이후 확장 후보로 보류한다.

`2026-09-18-v1-scope.md`의 대략적 범위는 이 두 문서로 구체화됐다. 상태 머신(Idle/Walk/Rest/Sleep/React/Drag), 운영 모드(Normal/Stay/Focus), DIP 좌표계, `Presentation/Behavior/Animation/Services` 구현 구조, CFG 저장 범위, 4단계 구현 순서가 새로 정의됨.

### 에셋 저장 방식: 외부 리소스 루트

펫 스킨 이미지(`Assets/Pets/<characterId>/`)를 WPF 임베디드 리소스로 컴파일해 넣지 않고, **빌드 결과물(exe) 옆에 복사되는 느슨한 파일**로 유지하기로 결정했다.

이유: 사용자가 재빌드 없이 exe 옆 `Assets/Pets/` 폴더를 열어 이미지나 `character.json`을 직접 교체해 커스텀 스킨을 쓸 수 있게 하기 위함.

구현: `DesktopPet.csproj`에 `<None Update="Assets\**\*"><CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory></None>` 추가. `Include`가 아니라 `Update`를 사용한 이유는 SDK 스타일 프로젝트의 암시적 글롭이 이미 해당 파일들을 `None`으로 포함하고 있어, `Include`로 다시 추가하면 중복 항목 오류(NETSDK1022)가 발생하기 때문이다.

`ArtSource/default/`(제작 원본 보관)는 반대로 배포 대상이 아니므로 `Assets/`와 분리된 위치에 두고 `CopyToOutputDirectory` 대상에서 제외했다.

## 영향

- `PetAssetLoader`(아직 미구현)는 임베디드 리소스 URI가 아니라 `AppContext.BaseDirectory` 기준 파일 경로로 스킨을 읽도록 설계해야 한다.
- 이미지 제작(AI 생성)은 사용자가 별도로 진행 중이며, 완성되면 `ArtSource/default/`에 먼저 넣고 검수·패키징 후 `Assets/Pets/default/`로 옮기는 흐름을 따른다.
- 지금 시점에는 실제 이미지 파일이 없어 `Assets/Pets/default/`, `ArtSource/default/`에는 안내용 `README.md`만 존재한다.
