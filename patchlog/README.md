# Patchlog Map

이 디렉터리는 `DesktopPet` 저장소의 현재 동작, 변경 결정, 변경 이력을 patchlog로 관리한다.

## Patchlog란?

patchlog는 코드 변경의 현재 상태와 변경 이유를 함께 남기는 기록 체계다.

일반 README처럼 사용법 전체를 설명하는 문서가 아니라, 코드가 바뀌었을 때 다음 작업(혹은 나중의 나 자신)이 현재 상태와 변경 맥락을 빠르게 복원하도록 돕는 문서다.

- `current/`: 지금 코드가 실제로 어떻게 동작하는지 기록한다.
- `decisions/`: 왜 그런 방향을 선택했는지, 어떤 대안을 버렸는지 기록한다.
- `changelog.md`: 언제 무엇이 바뀌었는지 짧게 기록한다.

코드 수정 전에는 `current/`를 먼저 보고, 코드 수정 후 기능 단위 변화가 있으면 patchlog에 반영할지 확인한다.

## 사용 예시

코드 수정 후 기록을 남기고 싶으면 이렇게 요청한다:

```text
이번 수정 내용을 patchlog에 반영해줘.
```

그러면 현재 소스 코드를 다시 읽고, `current/`, `decisions/`, `changelog.md`에 반영할 내용을 확인한 뒤 `patch 내용을 log에 반영할까요?`라고 묻는다.

## current

`current/`는 현재 최종 변경 상태를 설명한다.

코드를 수정하기 전에 가장 먼저 확인해야 하는 문서 영역이다.

현재 코드와 문서가 다르면 현재 코드를 우선하되, 코드 수정이 끝난 뒤 문서를 다시 맞춘다.

`module-overview.md`는 현재 상태의 입구 문서로 사용한다.

저장소가 커져서 하나의 overview에 담기 어렵거나 특정 영역의 흐름, 성능, 외부 연동, 상태 관리가 복잡해지면 `current/` 아래에 별도 문서를 추가한다.

예시:
- `rendering-flow.md`
- `animation-flow.md`
- `config-flow.md`
- `performance-notes.md`

새 current 문서를 추가할 때는 `module-overview.md` 또는 `current/README.md`에서 찾을 수 있게 연결한다.

현재 동작 설명은 `decisions/`가 아니라 `current/`에 둔다.

## decisions

`decisions/`는 변경 이유, 판단 근거, 이전 상태의 내용을 md 파일로 남기는 영역이다.

이 문서들은 과거 상태나 변경 전 코드 내용을 포함할 수 있으므로 실제 현재 코드와 다를 수 있다.

현재 동작의 기준으로 사용하지 말고, 왜 지금 구조가 되었는지 이해하기 위한 참고 자료로 사용한다.

## changelog.md

`changelog.md`는 단순 변경 이력을 시간순으로 짧게 기록한다.

상세한 이유나 비교 내용은 `decisions/`에 남기고, 현재 동작 설명은 `current/`에 남긴다.

## Patchlog 반영 절차

patchlog는 코드 변경 내용을 현재 문서 체계에 반영하는 작업이다.

사용자가 수정된 내용에 대한 patchlog 기록을 요청했거나, 코드 수정 결과 기능 단위의 변화가 생긴 경우에는 먼저 `patch 내용을 log에 반영할까요?`라고 확인한다.

사용자가 `진행` 또는 명확한 긍정/승인 의사를 답하기 전까지는 `current/`, `decisions/`, `changelog.md`를 수정하지 않는다.

승인 후에는 메모리에 누적된 코드 내용에 의존하지 말고 현재 소스 코드를 다시 확인한다.

그 다음 최신 `changelog.md`와 관련 `decisions/*` 문서를 확인하고, 현재 소스 코드와 `current/*` 문서를 비교한다.

현재 동작이 달라졌으면 `current/*`를 현재 코드 기준으로 동기화한다.

변경 이유, 설계 판단, 트레이드오프, 이전 방식과의 차이가 남길 가치가 있으면 `decisions/*`에 기록한다.

patchlog 반영 이력은 `changelog.md`에 짧게 남긴다.
