# Changelog

## 2026-09-30 (2)

- Hid the pet window from Alt+Tab by setting `WS_EX_TOOLWINDOW` (and clearing `WS_EX_APPWINDOW`) on the extended window style via `user32.dll` P/Invoke in a new `MainWindow_SourceInitialized` handler. `ShowInTaskbar=False` alone doesn't remove a WPF window from the Alt+Tab switcher — this is the standard fix.
- Verified: `dotnet build` clean; user confirmed the app no longer appears in Alt+Tab and both context menus still work normally.
- Updated `patchlog/current/module-overview.md` with the new SourceInitialized behavior.

## 2026-09-30

- Added a WPF `Window.ContextMenu` directly on `MainWindow` (일반 모드/여기서 쉬기/집중 모드 + 구분선 + 종료), so the pet can be closed or its mode changed by right-clicking it directly instead of hunting for the tray icon.
- Refactored mode changes to a single entry point, `ApplyModeSelection(PetMode)`, called by both the tray menu and the new pet context menu; it updates behavior, refreshes the visual, and syncs check marks on both menus. Added `TrayIconService.SyncMode(PetMode)` so the tray menu can be kept in sync when the mode changes from the other menu.
- Verified: `dotnet build` clean; user confirmed the pet context menu opens on right-click, mode switches and exit work from it, and both menus' checkmarks stay in sync.
- Updated `patchlog/current/module-overview.md`: new context menu behavior, `ApplyModeSelection` as the mode-change entry point, and fixed a stale line claiming the sprite loader wasn't wired yet (it has been since the previous sprite-loading changelog entry).

## 2026-09-29 (4)

- Decided to use a portable self-contained single-file `dotnet publish` build for cross-PC testing instead of a full installer (still deferred to the real deployment stage). See `patchlog/decisions/2026-09-29-portable-publish.md` for the exact command and output layout.
- Verified: `dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true` produces a working `DesktopPet.exe` (~170MB) with `Assets/Pets/default/` copied alongside it automatically; ran the published exe standalone without crashing.

## 2026-09-29 (3)

- Fixed Walk immediately reverting to Idle: `PetBehaviorController.Tick()` now takes `nearLeftEdge`/`nearRightEdge` bools (plain bools, no WPF types) and `EnterWalk()` chooses an inward facing when already near an edge instead of a fully random one, per `DESKTOP_PET_BEHAVIOR_SPEC.md`'s "경계에 있다면 안쪽을 선택" rule.
- Fixed the same symptom on multi-monitor setups: `SystemParameters.WorkArea` only ever returns the primary monitor's work area, so a pet on a secondary monitor had its coordinates compared against the wrong bounds and hit "boundary" almost instantly. Added `MainWindow.GetCurrentWorkArea()`, which finds the actual monitor via `System.Windows.Forms.Screen.FromHandle()` and converts pixels to DIPs via `VisualTreeHelper.GetDpi()`, falling back to `SystemParameters.WorkArea` if no screen is found.
- `MainWindow_Loaded` now explicitly centers the window horizontally on the current monitor's work area instead of relying on OS default placement, which could land near an edge.
- Verified: `dotnet build` clean; user confirmed Walk now moves for several seconds before stopping, and that dragging the pet to a second monitor no longer causes Walk to end instantly.
- Updated `patchlog/current/module-overview.md` with both fixes.

## 2026-09-29 (2)

- Added `Animation/PetAssetDefinition.cs` (`PetClipDefinition`, `PetCharacterDefinition`, `PetSkin` records), `Animation/PetAssetLoader.cs` (`LoadSkin`: parses/validates `character.json`, rejects paths escaping the skin directory, checks each clip image's actual size against `frameCount × frameSizePx`, loads each PNG once via `BitmapImage`/`Freeze()`), and `Animation/PetAnimationPlayer.cs` (`SetClip`/`Tick(elapsedMs)` → `FrameIndex`, loop-or-hold on last frame).
- Wired sprite rendering into `MainWindow`: added an `Image` (`PetImage`) with a `ScaleTransform` for mirroring, tried loading `Assets/Pets/default` in the constructor, and fall back to the existing placeholder `Ellipse` on `PetAssetLoadException`/`IOException`/`JsonException`. Per-tick, `UpdateAnimationFrame` maps `PetStateId` to a clip name, advances `PetAnimationPlayer`, and only rebuilds the `CroppedBitmap` when the clip or frame index actually changes.
- Resized `MainWindow` from 200x200 to 160x160 (BEHAVIOR_SPEC's suggested display canvas), and changed initial placement to use the anchor-ratio formula (`Top = WorkArea.Bottom - AnchorYPx/FrameHeightPx * Height`) when a skin is loaded, so the character's feet land on the work area bottom instead of the window's bottom edge.
- Replaced direct `UpdatePlaceholderColor()` calls in the drag handler and tray mode callback with `RefreshVisual()`, which updates the sprite frame immediately when a skin is loaded or falls back to the color update otherwise.
- Verified: `dotnet build` clean; app runs without crash; user confirmed the real sprite renders, animates per state, and mirrors during Walk.
- Updated `patchlog/current/module-overview.md` extensively: new Animation layer, revised MainWindow/execution-flow description, window size, and constraints (React still unconnected to input, fallback path untested against an actual load failure).

## 2026-09-29

- Added final AI-generated sprite assets for all six clips (idle 4f, walk 6f, rest 2f, sleep 4f, react 4f, drag 1f) to `Assets/Pets/default/`, plus `character.json` per the `DESKTOP_PET_ASSET_GUIDE.md` schema (frameSizePx 256x256, anchorPx (128,232), allowMirror true).
- Verified programmatically: all frames are 256x256 with fully transparent corners (alpha=0), and per-frame foot-contact Y position is consistent across every clip (bounding box minY/maxY identical or near-identical in all frames).
- Verified: `dotnet build` clean; confirmed all 6 PNGs + character.json + README.md copy to `bin/Debug/net10.0-windows/Assets/Pets/default/`.
- No loader/animation code yet — `MainWindow` still renders the placeholder `Ellipse`. Next blocker is `PetAssetLoader` + `PetAnimationPlayer`.
- Updated `patchlog/current/module-overview.md`: asset blocker resolved, new blocker is the loading/animation code; flagged `allowMirror: true` as an unverified assumption.

## 2026-09-22 (4)

- Wired Stay/Focus/Normal mode switching into `TrayIconService`: constructor now takes an initial mode and a mode-selected callback, and the context menu shows "일반 모드"/"여기서 쉬기"/"집중 모드" as checked radio-style items above a separator and "종료".
- Fixed: `DispatcherTimer` was created with `DispatcherPriority.Render`, which is tied to WPF's render pass and didn't fire reliably without user interaction — autonomous state transitions (Idle→Walk/Rest etc.) effectively stalled until something forced a render (e.g. a drag). Changed to `DispatcherPriority.Normal` so ticks run consistently regardless of rendering.
- Fixed: clicking a tray mode menu item called `_behavior.SetMode()` but left the placeholder color update to the next timer tick, so clicks appeared to do nothing. The mode-selected callback in `MainWindow` now calls `UpdatePlaceholderColor()` directly, matching how Drag already gives immediate feedback.
- Verified: `dotnet build` clean, app runs without crash; user confirmed mode menu clicks change the placeholder color immediately.
- Updated `patchlog/current/module-overview.md` with the tray mode menu, the timer priority fix, and the direct color-update fix.

## 2026-09-22 (3)

- Added `Behavior/PetState.cs` (`PetStateId`, `PetFacing`, `PetMode`) and `Behavior/PetBehaviorController.cs`, implementing the Idle/Walk/Rest/Sleep autonomous cycle and probability rules from `DESKTOP_PET_BEHAVIOR_SPEC.md` §2, plus `EnterDrag()`/`ExitDrag()` for the Drag state.
- Refactored `PetWanderMovement.GetNextLeft` to take `directionX` as a parameter and return `(Left, HitBoundary)` instead of owning direction internally — direction ownership now belongs to `PetBehaviorController.Facing`, matching the "책임 경계" note in the behavior spec. Renamed `SpeedPixelsPerSecond` to `SpeedDipPerSecond`.
- Wired `MainWindow`'s tick loop to drive the behavior controller (elapsed time clamped to 100ms), move only during `Walk`, and report boundary hits back to the controller. Left-click now calls `_behavior.EnterDrag()`/`ExitDrag()` around `DragMove()`.
- Added a debug-only placeholder color per state (Idle/Walk/Rest/Sleep/Drag) on the `Ellipse` so behavior can be verified without real sprites.
- Verified: `dotnet build` clean, app runs without crash; user confirmed drag turns the placeholder purple and reverts on release, and confirmed color changes over time reflect state transitions.
- Updated `patchlog/current/module-overview.md` for the new Behavior layer and revised execution flow.

## 2026-09-22 (2)

- Adopted `DESKTOP_PET_BEHAVIOR_SPEC.md` and `DESKTOP_PET_ASSET_GUIDE.md` as the v1 detailed design reference; `DESKTOP_PET_IDEAS.md` kept as non-binding roadmap. See `patchlog/decisions/2026-09-22-detailed-design-and-asset-root.md`.
- Decided pet skins live as loose (non-embedded) files copied next to the built exe, not WPF embedded resources, so users can swap skins without rebuilding. Added `<None Update="Assets\**\*">` with `CopyToOutputDirectory=PreserveNewest` to `DesktopPet.csproj`.
- Scaffolded `Assets/Pets/default/` (final runtime skin folder) and `ArtSource/default/` (+ `keyposes/`, `frames/`) working art source folders, both currently containing only README.md guides.
- Verified: `dotnet build` clean; confirmed `Assets/Pets/default/README.md` is copied to `bin/Debug/net10.0-windows/Assets/Pets/default/`.
- Updated `patchlog/current/module-overview.md` to reference the new design docs and asset folder layout; flagged missing reference character image as the current blocker for sprite production.

## 2026-09-22

- Added `TrayIconService` (`System.Windows.Forms.NotifyIcon`) with a right-click "종료" menu item wired to `System.Windows.Application.Current.Shutdown()`. Enabled `UseWindowsForms` in the csproj and fully qualified `Application` in `App.xaml.cs`/`MainWindow.xaml.cs` to resolve the WPF/WinForms namespace clash.
- Deferred the "설정" tray menu item until a real settings screen exists, per plan agreed with the user.
- `MainWindow.Closed` now stops the wander timer and disposes the tray icon.
- Verified: `dotnet build` clean, app runs without crash, user confirmed the tray icon appears and right-click 종료 exits the app.
- Updated `patchlog/current/module-overview.md` for tray icon behavior and the WPF/WinForms dependency note.

## 2026-09-18 (4)

- Added `PetWanderMovement`, a pure logic class computing left/right auto-wander position with edge bounce.
- Wired a `DispatcherTimer` (16ms) into `MainWindow` to drive auto-wander, pausing while the user drags the window.
- Verified: `dotnet build` clean, app runs without crash, user confirmed auto-wander/pause-on-drag behavior by running it directly.
- Updated `patchlog/current/module-overview.md` to describe wander movement.

## 2026-09-18 (3)

- Implemented v1's first feature: transparent, topmost, borderless `MainWindow` (200x200) with mouse-drag move via `DragMove()`. Placeholder `Ellipse` used to visually verify transparency/topmost behavior.
- Verified: `dotnet build` clean, app runs without crash, user confirmed drag/transparent/topmost behavior by running it directly.
- Updated `patchlog/current/module-overview.md` to describe the new window behavior.

## 2026-09-18 (2)

- Decided v1 scope: sprite-sheet visuals, transparent/topmost draggable window, walk animation, tray icon + context menu, MSI/Installer distribution. See `patchlog/decisions/2026-09-18-v1-scope.md`.
- Updated `patchlog/current/module-overview.md` to reflect resolved v1 scope decisions.

## 2026-09-18

- Added repository-level `AGENTS.md` for patchlog and code-change rules, adapted from `module-patchlog-structure.md` for a single-module repository (dropped multi-module tracking and parent-project references).
- Added patchlog structure with `current/`, `decisions/`, and `changelog.md`.
- Added initial `patchlog/current/module-overview.md` based on current WPF scaffold code.
