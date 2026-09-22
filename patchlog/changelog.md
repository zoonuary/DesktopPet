# Changelog

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
