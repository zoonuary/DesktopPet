# AGENTS.md

## 1. 적용 범위

이 파일은 `DesktopPet` 저장소 전체 작업에 적용한다.

`DesktopPet`은 단일 모듈 독립 저장소이므로, 상위 프로젝트의 별도 `AGENTS.md`는 없다. `DEV_GUIDELINES.md`(승인 절차, 코드 컨벤션, 성능/CFG 규칙)를 함께 우선 적용한다.

## 2. 코드 수정 전 확인 문서

코드를 수정하기 전에는 `patchlog/README.md`를 먼저 읽어 patchlog 구조와 역할을 확인한 뒤, 관련 `patchlog/current/*`를 확인한다.

`patchlog/current/*`는 이 저장소의 현재 최종 동작과 구조를 설명하는 기준 문서다.

`patchlog/decisions/*`는 변경 결정, 이전 판단, 과거 맥락을 기록한 문서다. 이전 상태나 변경 전 코드 내용이 포함될 수 있으므로 현재 코드와 다를 수 있다.

현재 사실을 판단할 때는 다음 순서를 따른다:
- 현재 코드
- `patchlog/current/*`
- 관련 `patchlog/decisions/*`
- `patchlog/changelog.md`

## 3. 문서 업데이트 규칙

코드 수정으로 현재 동작, 구조, 책임 범위가 바뀌면 관련 `patchlog/current/*`를 업데이트한다.

변경 이력은 `patchlog/changelog.md`에 짧게 남긴다.

왜 그렇게 바꿨는지, 어떤 대안을 버렸는지는 필요할 때 `patchlog/decisions/*`에 별도 md 파일로 남긴다.

문서를 업데이트할 때는 과거 로그만 보고 쓰지 말고 반드시 현재 코드 상태를 먼저 확인한다.

## 4. Patchlog 반영 승인 절차

사용자가 수정된 내용에 대한 patchlog 기록을 요청했거나, 코드 수정 결과 기능 단위의 변화가 생겼다고 판단한 경우, 바로 patchlog를 수정하지 않는다.

먼저 사용자에게 `patch 내용을 log에 반영할까요?`라고 묻는다.

사용자가 `진행` 또는 명확한 긍정/승인 의사를 답하기 전까지는 `patchlog/current/*`, `patchlog/decisions/*`, `patchlog/changelog.md`를 수정하지 않는다.

승인 후에는 다음 순서로 진행한다:
- 메모리에 누적된 코드 내용에 의존하지 말고 현재 소스 코드를 다시 읽는다.
- `patchlog/README.md`를 확인한다.
- 관련 `patchlog/current/*`와 현재 소스 코드를 비교한다.
- 현재 동작이 달라졌으면 `patchlog/current/*`를 현재 코드 기준으로 동기화한다.
- 변경 이유, 설계 판단, 트레이드오프, 이전 방식과의 차이가 남길 가치가 있으면 `patchlog/decisions/*`에 기록한다.
- patchlog 반영 이력은 `patchlog/changelog.md`에 짧게 남긴다.
