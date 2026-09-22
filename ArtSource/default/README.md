# ArtSource/default

이 폴더는 펫 이미지 제작 과정에서 쓰는 작업용 원본 보관소다. 앱 배포 대상이 아니며 빌드 결과물에 포함되지 않는다.

`DESKTOP_PET_ASSET_GUIDE.md` 규격에 따라 아래 파일을 이 폴더에 순서대로 쌓는다.

- `reference.png` — 보존한 원본 캐릭터 이미지 (AI로 생성한 기준 이미지)
- `character-notes.md` — 색·비율·특징·좌우 반전 가능 여부 기록
- `prompts.md` — 실제 사용한 이미지 생성 요청문 기록
- `keyposes/` — 동작별 대표 자세 (idle-key, walk-key, rest-key, sleep-key, react-key, drag-key)
- `frames/` — 검수를 마친 개별 애니메이션 프레임

검수와 패키징이 끝난 최종 파일만 `Assets/Pets/default/`로 옮겨 앱이 실제로 읽게 한다. 원본(`reference.png`)은 덮어쓰지 않는다.
