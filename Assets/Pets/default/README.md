# Assets/Pets/default

이 폴더는 앱이 런타임에 실제로 읽는 최종 펫 스킨 파일이 위치하는 곳이다.

빌드 시 `DesktopPet.csproj`의 `CopyToOutputDirectory` 설정에 따라 이 폴더 전체가 빌드 결과물(exe)과 같은 경로에 복사된다. 즉 배포된 `DesktopPet.exe` 옆에서도 `Assets/Pets/default/`를 직접 찾을 수 있다.

이 폴더는 임베디드 리소스가 아니라 느슨한 파일이다. 사용자가 exe 옆 이 폴더를 열어 이미지나 `character.json`을 직접 교체하면 커스텀 스킨으로 바뀐다 (로더가 구현된 이후 기준).

## 넣어야 할 파일 (`DESKTOP_PET_ASSET_GUIDE.md` 규격)

```text
character.json
idle.png     1024x256 / 4프레임
walk.png     1536x256 / 6프레임
rest.png     512x256  / 2프레임
sleep.png    1024x256 / 4프레임
react.png    1024x256 / 4프레임
drag.png     256x256  / 1프레임
```

`ArtSource/default/`에서 제작·검수한 프레임을 위 규격대로 패키징한 뒤 이 폴더에 넣는다.

현재 이 폴더에는 최종 파일이 없다. `PetAssetLoader` 및 관련 로딩 코드도 아직 구현되지 않았다.
