# tggtmdx-scripts
The Golden Girls Take Manhattan Script Dump (2022)

~ Assets Used ~

WIDELY USED:
- Cinemachine (Used for camera play; it’s in the package manager in the Unity Registry category.)
- Visual Scripting (Available in the Unity Registry category as well. Used for the battle’s state machine as well as a few other things.)
- Odin Inspector https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041 (Used in MANY MANY places. This is probably the most important asset that I use.)
- DOTween Pro https://assetstore.unity.com/packages/tools/visual-scripting/dotween-pro-32416 (You can probably get away with just using the free version; I don’t think I use any of the Pro version’s functionality.)
- Super Text Mesh https://assetstore.unity.com/packages/p/super-text-mesh-57995 (My go to for text; used in MANY places.)

GENERALLY USED:
- Rewired https://assetstore.unity.com/packages/tools/utilities/rewired-21676 (Used for mapping controls.)
- Playmaker https://assetstore.unity.com/packages/tools/visual-scripting/playmaker-368 (Used for a few state machines.)
- Invector Third Person Controller (Melee Combat Template) https://assetstore.unity.com/packages/tools/game-toolkits/invector-third-person-controller-melee-combat-template-44227 (Used for third person sections. Not necessarily required but was used quite a bit.)

SPARINGLY USED/REPLACEABLE/NOT NECESSARY:
- RT-Voice Pro https://assetstore.unity.com/packages/tools/audio/rt-voice-pro-41068 (Used for the navigator. Should be easy to remove its uses if not wanted.)
- Camera Transitions https://assetstore.unity.com/packages/tools/camera/camera-transitions-36055 (This one is not available in the asset store anymore but it was only used for animations when entering battles or camera play in cutscenes. You can remove instances where it’s used and replace it with something else if you want later.)
- Legacy Image Effects https://assetstore.unity.com/packages/essentials/legacy-image-effects-83913 (This one is also no longer available but can be swapped out with something else as well.)
- SRDebugger https://assetstore.unity.com/packages/tools/gui/srdebugger-console-tools-on-device-27688 (Don’t remember what I was using this for; should be good to just remove references of.)
- uConsole https://assetstore.unity.com/packages/tools/game-toolkits/uconsole-103172 (Very good asset but also not required.)
- Dungeon Architect https://assetstore.unity.com/packages/tools/utilities/dungeon-architect-53895 (Used when I was seeing if it was good for generated dungeons but I ended up just making the tools myself.)
- 2DxFX https://assetstore.unity.com/packages/tools/sprite-management/2dxfx-2d-sprite-fx-42566 (Used for certain effects on sprites, usually animations in battles.)
- Camera Filter Pack https://assetstore.unity.com/packages/vfx/shaders/fullscreen-camera-effects/camera-filter-pack-18433 (Again, mostly for aesthetics and can be replaced.)
- Introloop Audio https://assetstore.unity.com/packages/tools/audio/introloop-51095 (Used for managing music playback; not exactly necessary but I stand by this package since it's a very good interface for defining how music tracks should be looped.)

You can definitely go without using some of these if you remove references where they're used (especially the ones no longer on the asset store since you will outright need to do that anyway.)
Hopefully I didn't forget anything but I went about this by using the repo on its own and reimporting everything that was throwing compilation errors so theoretically this should actually be it.
There are a few places where Playmaker/VisualScripting is used to drive logic (both are used because I was using the former before the latter became widely available and even then I still use the latter in some places by its original name Bolt) but the core functionality should otherwise all be in the scripts in the repo.
