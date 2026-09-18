# Module Patchlog Structure

이 문서는 `XenoLink_application` 아래 모듈에 patchlog system을 처음 구축할 때 사용하는 기본 템플릿이다.

이 템플릿은 `hpt.linschedulemanager`에 적용한 구조를 기준으로 한다.

일반 코드 수정 작업마다 반드시 읽어야 하는 문서는 아니다. 이미 patchlog system이 구축된 모듈에서는 해당 모듈의 `AGENTS.md`와 관련 `patchlog/current/*`를 우선 확인한다.

## Patchlog란?

patchlog는 코드 변경의 현재 상태와 변경 이유를 함께 남기는 모듈별 기록 체계다.

일반 README처럼 사용법 전체를 설명하는 문서가 아니라, 코드가 바뀌었을 때 다음 작업자가 현재 상태와 변경 맥락을 빠르게 복원하도록 돕는 문서다.

- `current/`: 지금 코드가 실제로 어떻게 동작하는지 기록한다.
- `decisions/`: 왜 그런 방향을 선택했는지, 어떤 대안을 버렸는지 기록한다.
- `changelog.md`: 언제 무엇이 바뀌었는지 짧게 기록한다.

코드 수정 전에는 `current/`를 먼저 보고, 코드 수정 후 기능 단위 변화가 있으면 patchlog에 반영할지 확인한다.

## 사용 예시

새 모듈에 patchlog system을 만들고 싶으면 이렇게 요청한다:

```text
XenoLink_application의 module-patchlog-structure.md를 읽고,
<module> 모듈에 patchlog system을 만들어줘.
```

코드 수정 후 기록을 남기고 싶으면 이렇게 요청한다:

```text
이번 수정 내용을 patchlog에 반영해줘.
```

그러면 Codex는 현재 소스 코드를 다시 읽고, `current/`, `decisions/`, `changelog.md`에 반영할 내용을 확인한 뒤 사용자에게 `patch 내용을 log에 반영할까요?`라고 묻는다.

## 적용 모듈 목록

이 템플릿이 적용된 모듈은 아래 목록으로 추적한다.

현재 추적 대상은 각 모듈의 루트 `AGENTS.md`만 포함한다. `patchlog/*` 내부 문서는 각 모듈의 `AGENTS.md` 규칙에 따라 자율적으로 관리한다.

- `hpt.linschedulemanager/AGENTS.md`
- `hpt.hwdriver/AGENTS.md`
- `hpt.trace/AGENTS.md`

새 모듈에 patchlog system을 구축할 때는 해당 모듈의 루트 `AGENTS.md`를 이 목록에 추가한다.

해당 모듈의 루트 `AGENTS.md`를 이 목록에 추가하기 전까지는 새 모듈 patchlog system 구축이 완료된 것으로 보고하지 않는다.

## Codex에게 요청하는 방법

새 모듈에 이 구조를 만들고 싶으면 다음처럼 요청한다:

```text
XenoLink_application의 module-patchlog-structure.md를 읽고,
<module> 모듈에 같은 patchlog 구조를 만들어줘.
모듈 AGENTS.md와 patchlog/current, patchlog/decisions, patchlog/changelog.md까지 생성해줘.
가능하면 현재 코드 상태를 읽고 patchlog/current/module-overview.md도 초기 생성해줘.
module-patchlog-structure.md의 적용 모듈 목록에도 <module>/AGENTS.md를 추가해줘.
```

기존 모듈의 patchlog 구조를 바꾸고 싶으면 다음처럼 요청한다:

```text
<module>의 patchlog 구조를 변경하고 싶어.
module-patchlog-structure.md와 달라지는 부분을 먼저 알려주고,
계속 진행할지 물어봐줘.
```

## 기본 구조

새 모듈에 patchlog system을 구축할 때는 기본적으로 다음 구조를 만든다.

```text
<module>/
  AGENTS.md
  patchlog/
    README.md
    current/
      README.md
      module-overview.md
    decisions/
      README.md
    changelog.md
```

`patchlog/current/module-overview.md`는 가능하면 현재 코드 상태를 확인한 뒤 생성한다.

대상 모듈 루트에 `README.md` 또는 `readme.md`가 있으면, `module-overview.md` 생성 전에 함께 읽고 참고자료로 활용한다.

루트 README는 현재 코드와 다를 수 있으므로 현재 동작의 기준으로 단정하지 않는다. 현재 코드와 README가 충돌하면 현재 코드를 우선하고, 필요한 경우 `Needs Confirmation`에 남긴다.

초기 구축 시점에 현재 코드 분석이 작업 범위를 벗어나거나 사용자 승인이 필요하면, `module-overview.md` 생성을 별도 단계로 제안하고 사용자 확인을 받는다.

## 기존 모듈에 AGENTS.md가 있는 경우

대상 모듈 루트에 이미 `AGENTS.md`가 있으면 통째로 덮어쓰지 않는다.

기존 `AGENTS.md`를 먼저 읽고 기존 지침을 보존한 상태에서 patchlog 관련 섹션만 추가하거나 병합한다.

기존 지침과 이 템플릿의 patchlog 규칙이 충돌하면 사용자에게 충돌 내용을 설명하고 계속 진행할지 확인한다.

사용자가 `진행` 또는 명확한 긍정/승인 의사를 답하기 전까지 기존 `AGENTS.md`를 구조적으로 재작성하거나 기존 지침을 삭제하지 않는다.

## 파일 역할

### AGENTS.md

모듈 내부 작업 규칙을 정의한다.

코드 수정 전에 어떤 문서를 읽어야 하는지, `patchlog/current`, `patchlog/decisions`, `patchlog/changelog.md`를 어떻게 활용해야 하는지 명시한다.

상위 `XenoLink_application\AGENTS.md`를 먼저 읽고, 모듈의 `AGENTS.md`를 추가로 적용한다.

### patchlog/README.md

모듈 `patchlog` 디렉터리의 역할을 설명하는 안내 문서다.

`current/`, `decisions/`, `changelog.md`가 각각 어떤 의미인지 짧게 설명한다.

### patchlog/current/README.md

현재 최종 동작과 구조를 기록하는 영역의 안내 문서다.

코드 수정 전 가장 먼저 확인해야 하는 문서 영역임을 명시한다.

현재 코드와 문서가 다르면 현재 코드를 우선하고, 수정이 끝난 뒤 문서를 현재 코드에 맞게 갱신한다.

### patchlog/current/module-overview.md

모듈의 현재 코드 기준 개요를 기록하는 초기 current 문서다.

가능하면 patchlog system을 처음 구축할 때 현재 코드를 읽고 함께 생성한다.

대상 모듈 루트에 `README.md` 또는 `readme.md`가 있으면 함께 읽고 참고자료로 활용한다.

루트 README는 현재 코드와 다를 수 있으므로 현재 동작의 기준으로 단정하지 않는다. 현재 코드와 README가 충돌하면 현재 코드를 우선하고, 필요한 경우 `Needs Confirmation`에 남긴다.

이 문서에는 다음 내용을 짧게 기록한다:
- 모듈의 책임과 범위
- 주요 디렉터리와 레이어
- 주요 실행 흐름
- 외부 모듈 또는 장비 연동 지점
- 아직 확인되지 않은 부분

이미 patchlog system이 구축된 모듈에서 `patchlog/current/`에 `README.md` 외의 current 문서가 없다면, 코드 수정 전에 `module-overview.md` 생성을 제안한다.

사용자가 `진행` 또는 명확한 긍정/승인 의사를 답하기 전까지는 current 문서를 자동 생성하지 않는다.

### patchlog/decisions/README.md

변경 결정과 판단 근거를 기록하는 영역의 안내 문서다.

이 영역의 문서는 이전 revision의 내용, 당시 고려한 대안, 변경 이유를 포함할 수 있으므로 현재 코드와 다를 수 있음을 명시한다.

현재 동작의 기준은 `patchlog/current/*`와 현재 코드로 판단한다.

### patchlog/changelog.md

단순 변경 이력을 시간순으로 짧게 기록한다.

상세한 이유나 비교 내용은 `patchlog/decisions/`에 남기고, 현재 동작 설명은 `patchlog/current/`에 남긴다.

## 생성 템플릿

새 모듈에 patchlog system을 구축할 때는 아래 템플릿을 사용한다.

`<module>`은 실제 모듈 디렉터리명으로 바꾼다.

대상 모듈에 이미 `AGENTS.md`가 있으면 아래 템플릿으로 통째로 교체하지 않는다. 기존 지침을 보존하고 patchlog 관련 섹션만 추가하거나 병합한다.

### `<module>/AGENTS.md`

```md
# AGENTS.md

## 1. 적용 범위

이 파일은 `<module>` 모듈 내부 작업에 적용한다.

상위 `XenoLink_application\AGENTS.md`를 먼저 읽고, 이 파일의 지침을 추가로 적용한다.

모듈 고유 작업 방식은 이 파일을 우선하되, Wiki 동기화 개발·성능 지침과 상위 공통 안전 규칙은 상위 `XenoLink_application\AGENTS.md`를 우선한다.

## 2. 상위 적용 모듈 목록 등록 확인

이 모듈에 patchlog system을 처음 구축한 경우, 상위 `XenoLink_application\module-patchlog-structure.md`의 적용 모듈 목록에 `<module>/AGENTS.md`가 등록되어 있는지 확인한다.

등록되어 있지 않다면 patchlog system 구축이 완료된 것으로 보고하지 않는다.

## 3. 처음 읽을 때 개발 계획서 확인

Codex가 이 파일을 처음 읽는 경우, Wiki MCP 또는 로컬 문서에서 이 모듈과 관련된 개발 계획서를 최소한 한 번 검색한다.

검색 키워드 예:
- `<module>`
- `<module display name>`
- `<module feature name>`
- `<module development plan title>`

찾은 개발 계획서는 모듈의 배경과 설계 의도를 이해하기 위한 참고자료로만 사용한다.

개발 계획서는 이전 계획일 수 있으므로 현재 코드와 다를 수 있다.

현재 동작은 현재 코드와 `patchlog/current/*`를 우선한다.

개발 계획서와 현재 코드 또는 `patchlog/current/*`가 충돌하고 그 차이가 작업 판단에 영향을 준다면 사용자에게 확인한다.

## 4. 코드 수정 전 확인 문서

코드를 수정하기 전에는 `patchlog/README.md`를 먼저 읽어 patchlog 구조와 역할을 확인한 뒤, 관련 `patchlog/current/*`를 확인한다.

`patchlog/current/*`는 이 모듈의 현재 최종 동작과 구조를 설명하는 기준 문서다.

`patchlog/decisions/*`는 변경 결정, 이전 revision의 판단, 과거 맥락을 기록한 문서다. 이전 상태나 변경 전 코드 내용이 포함될 수 있으므로 현재 코드와 다를 수 있다.

현재 사실을 판단할 때는 다음 순서를 따른다:
- 현재 코드
- `patchlog/current/*`
- 관련 `patchlog/decisions/*`
- `patchlog/changelog.md`

## 5. 문서 업데이트 규칙

코드 수정으로 현재 동작, 구조, 책임 범위, 외부 연동 방식이 바뀌면 관련 `patchlog/current/*`를 업데이트한다.

변경 이력은 `patchlog/changelog.md`에 짧게 남긴다.

왜 그렇게 바꿨는지, 어떤 대안을 버렸는지, 이전 revision과 비교해 어떤 판단이 있었는지는 필요할 때 `patchlog/decisions/*`에 별도 md 파일로 남긴다.

문서를 업데이트할 때는 과거 로그만 보고 쓰지 말고 반드시 현재 코드 상태를 먼저 확인한다.

## 6. Patchlog 반영 승인 절차

사용자가 수정된 내용에 대한 patchlog 기록을 요청했거나, Codex가 코드 수정 결과 기능 단위의 변화가 생겼다고 판단한 경우, 바로 patchlog를 수정하지 않는다.

먼저 사용자에게 `patch 내용을 log에 반영할까요?`라고 묻는다.

사용자가 `진행` 또는 명확한 긍정/승인 의사를 답하기 전까지는 `patchlog/current/*`, `patchlog/decisions/*`, `patchlog/changelog.md`를 수정하지 않는다.

승인 후에는 다음 순서로 진행한다:
- 메모리에 누적된 코드 내용에 의존하지 말고 현재 소스 코드를 다시 읽는다.
- `patchlog/README.md`를 확인한다.
- 관련 `patchlog/current/*`와 현재 소스 코드를 비교한다.
- 현재 동작이 달라졌으면 `patchlog/current/*`를 현재 코드 기준으로 동기화한다.
- 변경 이유, 설계 판단, 트레이드오프, 이전 방식과의 차이가 남길 가치가 있으면 `patchlog/decisions/*`에 기록한다.
- patchlog 반영 이력은 `patchlog/changelog.md`에 짧게 남긴다.
```

### `<module>/patchlog/README.md`

```md
# Patchlog Map

이 디렉터리는 `<module>` 모듈의 현재 동작, 변경 결정, 변경 이력을 patchlog로 관리한다.

## Patchlog란?

patchlog는 코드 변경의 현재 상태와 변경 이유를 함께 남기는 모듈별 기록 체계다.

일반 README처럼 사용법 전체를 설명하는 문서가 아니라, 코드가 바뀌었을 때 다음 작업자가 현재 상태와 변경 맥락을 빠르게 복원하도록 돕는 문서다.

- `current/`: 지금 코드가 실제로 어떻게 동작하는지 기록한다.
- `decisions/`: 왜 그런 방향을 선택했는지, 어떤 대안을 버렸는지 기록한다.
- `changelog.md`: 언제 무엇이 바뀌었는지 짧게 기록한다.

코드 수정 전에는 `current/`를 먼저 보고, 코드 수정 후 기능 단위 변화가 있으면 patchlog에 반영할지 확인한다.

## 사용 예시

코드 수정 후 기록을 남기고 싶으면 이렇게 요청한다:

```text
이번 수정 내용을 patchlog에 반영해줘.
```

그러면 Codex는 현재 소스 코드를 다시 읽고, `current/`, `decisions/`, `changelog.md`에 반영할 내용을 확인한 뒤 사용자에게 `patch 내용을 log에 반영할까요?`라고 묻는다.

## current

`current/`는 현재 최종 변경 상태를 설명한다.

코드를 수정하기 전에 가장 먼저 확인해야 하는 문서 영역이다.

현재 코드와 문서가 다르면 현재 코드를 우선하되, 코드 수정이 끝난 뒤 문서를 다시 맞춘다.

`module-overview.md`는 현재 상태의 입구 문서로 사용한다.

모듈이 커서 하나의 overview에 담기 어렵거나 특정 영역의 흐름, 성능, 외부 연동, 상태 관리가 복잡하면 `current/` 아래에 별도 문서를 추가한다.

예시:
- `rendering-flow.md`
- `data-binding-flow.md`
- `communication-flow.md`
- `performance-notes.md`
- `state-management.md`

새 current 문서를 추가할 때는 `module-overview.md` 또는 `current/README.md`에서 찾을 수 있게 연결한다.

현재 동작 설명은 `decisions/`가 아니라 `current/`에 둔다.

## decisions

`decisions/`는 변경 이유, 판단 근거, 이전 revision의 내용을 md 파일로 남기는 영역이다.

이 문서들은 과거 상태나 변경 전 코드 내용을 포함할 수 있으므로 실제 현재 코드와 다를 수 있다.

현재 동작의 기준으로 사용하지 말고, 왜 지금 구조가 되었는지 이해하기 위한 참고 자료로 사용한다.

## changelog.md

`changelog.md`는 단순 변경 이력을 시간순으로 짧게 기록한다.

상세한 이유나 비교 내용은 `decisions/`에 남기고, 현재 동작 설명은 `current/`에 남긴다.

## Patchlog 반영 절차

patchlog는 코드 변경 내용을 현재 문서 체계에 반영하는 작업이다.

사용자가 수정된 내용에 대한 patchlog 기록을 요청했거나, 코드 수정 결과 기능 단위의 변화가 생긴 경우에는 먼저 사용자에게 `patch 내용을 log에 반영할까요?`라고 확인한다.

사용자가 `진행` 또는 명확한 긍정/승인 의사를 답하기 전까지는 `current/`, `decisions/`, `changelog.md`를 수정하지 않는다.

승인 후에는 메모리에 누적된 코드 내용에 의존하지 말고 현재 소스 코드를 다시 확인한다.

그 다음 최신 `changelog.md`와 관련 `decisions/*` 문서를 확인하고, 현재 소스 코드와 `current/*` 문서를 비교한다.

현재 동작이 달라졌으면 `current/*`를 현재 코드 기준으로 동기화한다.

변경 이유, 설계 판단, 트레이드오프, 이전 방식과의 차이가 남길 가치가 있으면 `decisions/*`에 기록한다.

patchlog 반영 이력은 `changelog.md`에 짧게 남긴다.
```

### `<module>/patchlog/current/README.md`

```md
# Current Behavior

이 디렉터리는 `<module>` 모듈의 현재 최종 동작과 구조를 기록한다.

코드를 수정하기 전에는 이 디렉터리의 관련 문서를 먼저 확인한다.

현재 코드와 문서가 다르면 현재 코드를 우선하고, 수정이 끝난 뒤 문서를 현재 코드에 맞게 갱신한다.
```

### `<module>/patchlog/current/module-overview.md`

```md
# <module> Module Overview

Last verified: <YYYY-MM-DD>

This document describes the current code state of `<module>`.

Reference material used for background only:
- <development plan or related reference>
- <module README.md or readme.md, if present>

The current code and this `patchlog/current/*` area take priority over older plans or decision logs.

## Responsibility

<Summarize this module's responsibility from current source code.>

## Main Layers

~~~text
<module>/
  <main directories>
  patchlog/
~~~

## Key Components

<List important classes, services, presenters, views, models, adapters, or entry points.>

## Current Execution Flow

<Describe the main runtime flow based on current code.>

## External Dependencies

<List project references, important interfaces, external services, hardware APIs, or UI frameworks.>

## Confirmed Constraints

<List facts confirmed from current source code.>

## Needs Confirmation

<List important uncertainties without treating them as facts.>
```

### `<module>/patchlog/decisions/README.md`

```md
# Decisions

이 디렉터리는 `<module>` 모듈의 변경 결정과 판단 근거를 기록한다.

각 문서는 이전 revision의 내용, 당시 고려한 대안, 변경 이유를 포함할 수 있다.

따라서 이 문서의 내용은 현재 코드와 다를 수 있으며, 현재 동작의 기준은 `patchlog/current/*`와 현재 코드로 판단한다.
```

### `<module>/patchlog/changelog.md`

```md
# Changelog

## <YYYY-MM-DD>

- Added module patchlog structure with `current/`, `decisions/`, and `changelog.md`.
- Added module-level `AGENTS.md` for `<module>` patchlog and code-change rules.
```

## 구조 변경 시 주의

이 문서는 최초 구축용 기본 템플릿이다.

적용 모듈 목록에 있는 모듈의 `AGENTS.md`를 수정하는 경우, 그 변경이 기본 patchlog 구조나 운영 방식에 영향을 주는지 확인한다.

기존 모듈의 patchlog system 구조를 변경하려는 경우에는 먼저 이 기본 구조와 비교한다.

변경하려는 구조가 이 템플릿과 달라진다면, 사용자에게 기본 템플릿과 달라진다는 점을 명시하고 계속 진행할지 확인한다.

기본 템플릿과 달라지는 변경이라면, 사용자에게 `module-patchlog-structure.md`를 변경할지 아니면 해당 모듈만 예외로 둘지 확인한다.

`module-patchlog-structure.md` 자체는 사용자가 명시적으로 요청한 경우에만 수정한다.
