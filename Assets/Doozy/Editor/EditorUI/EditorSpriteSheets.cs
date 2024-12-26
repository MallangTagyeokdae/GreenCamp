// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//.........................
//.....Generated Class.....
//.........................
//.......Do not edit.......
//.........................

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Doozy.Editor.EditorUI.ScriptableObjects.SpriteSheets;
using UnityEngine;

namespace Doozy.Editor.EditorUI
{
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class EditorSpriteSheets
    {
        public static class EditorUI
        {
            public static class Arrows
            {
                private static EditorDataSpriteSheetGroup s_spriteSheetGroup;
                private static EditorDataSpriteSheetGroup spriteSheetGroup =>
                    s_spriteSheetGroup
                        ? s_spriteSheetGroup
                        : s_spriteSheetGroup = EditorDataSpriteSheetDatabase.GetSpriteSheetGroup("EditorUI","Arrows");

                public static List<Texture2D> GetTextures(SpriteSheetName sheetName) =>
                    spriteSheetGroup.GetTextures(sheetName.ToString());

                public enum SpriteSheetName
                {
                    ArrowDown,
                    ArrowLeft,
                    ArrowRight,
                    ArrowUp,
                    ChevronDown,
                    ChevronLeft,
                    ChevronRight,
                    ChevronUp
                }
                

                public static List<Texture2D> ArrowDown => GetTextures(SpriteSheetName.ArrowDown);
                public static List<Texture2D> ArrowLeft => GetTextures(SpriteSheetName.ArrowLeft);
                public static List<Texture2D> ArrowRight => GetTextures(SpriteSheetName.ArrowRight);
                public static List<Texture2D> ArrowUp => GetTextures(SpriteSheetName.ArrowUp);
                public static List<Texture2D> ChevronDown => GetTextures(SpriteSheetName.ChevronDown);
                public static List<Texture2D> ChevronLeft => GetTextures(SpriteSheetName.ChevronLeft);
                public static List<Texture2D> ChevronRight => GetTextures(SpriteSheetName.ChevronRight);
                public static List<Texture2D> ChevronUp => GetTextures(SpriteSheetName.ChevronUp);
                
            }

            public static class Components
            {
                private static EditorDataSpriteSheetGroup s_spriteSheetGroup;
                private static EditorDataSpriteSheetGroup spriteSheetGroup =>
                    s_spriteSheetGroup
                        ? s_spriteSheetGroup
                        : s_spriteSheetGroup = EditorDataSpriteSheetDatabase.GetSpriteSheetGroup("EditorUI","Components");

                public static List<Texture2D> GetTextures(SpriteSheetName sheetName) =>
                    spriteSheetGroup.GetTextures(sheetName.ToString());

                public enum SpriteSheetName
                {
                    CarretRightToDown,
                    Checkmark,
                    EditorColorPalette,
                    EditorFontFamily,
                    EditorLayoutGroup,
                    EditorMicroAnimationGroup,
                    EditorSelectableColorPalette,
                    EditorStyleGroup,
                    EditorTextureGroup,
                    LineMixedValues,
                    RadioCircle,
                    Switch
                }
                

                public static List<Texture2D> CarretRightToDown => GetTextures(SpriteSheetName.CarretRightToDown);
                public static List<Texture2D> Checkmark => GetTextures(SpriteSheetName.Checkmark);
                public static List<Texture2D> EditorColorPalette => GetTextures(SpriteSheetName.EditorColorPalette);
                public static List<Texture2D> EditorFontFamily => GetTextures(SpriteSheetName.EditorFontFamily);
                public static List<Texture2D> EditorLayoutGroup => GetTextures(SpriteSheetName.EditorLayoutGroup);
                public static List<Texture2D> EditorMicroAnimationGroup => GetTextures(SpriteSheetName.EditorMicroAnimationGroup);
                public static List<Texture2D> EditorSelectableColorPalette => GetTextures(SpriteSheetName.EditorSelectableColorPalette);
                public static List<Texture2D> EditorStyleGroup => GetTextures(SpriteSheetName.EditorStyleGroup);
                public static List<Texture2D> EditorTextureGroup => GetTextures(SpriteSheetName.EditorTextureGroup);
                public static List<Texture2D> LineMixedValues => GetTextures(SpriteSheetName.LineMixedValues);
                public static List<Texture2D> RadioCircle => GetTextures(SpriteSheetName.RadioCircle);
                public static List<Texture2D> Switch => GetTextures(SpriteSheetName.Switch);
                
            }

            public static class Icons
            {
                private static EditorDataSpriteSheetGroup s_spriteSheetGroup;
                private static EditorDataSpriteSheetGroup spriteSheetGroup =>
                    s_spriteSheetGroup
                        ? s_spriteSheetGroup
                        : s_spriteSheetGroup = EditorDataSpriteSheetDatabase.GetSpriteSheetGroup("EditorUI","Icons");

                public static List<Texture2D> GetTextures(SpriteSheetName sheetName) =>
                    spriteSheetGroup.GetTextures(sheetName.ToString());

                public enum SpriteSheetName
                {
                    Animator,
                    API,
                    ApplicationQuit,
                    Atom,
                    AudioMixer,
                    AudioMixerGroup,
                    AutoDisable,
                    Back,
                    Binoculars,
                    BookOpen,
                    Border,
                    ButtonClick,
                    ButtonDoubleClick,
                    ButtonLeftClick,
                    ButtonLongClick,
                    ButtonMiddleClick,
                    ButtonRightClick,
                    Camera,
                    CategoryPlus,
                    Clear,
                    Close,
                    Color,
                    ConnectedDisconnected,
                    Cooldown,
                    Copy,
                    Cut,
                    Debug,
                    DelayBetweenLoops,
                    Deselected,
                    Dice,
                    DisabledEnabled,
                    DisconnectedConnected,
                    Discord,
                    DoozyUI,
                    DotNet,
                    Drag,
                    Duration,
                    Edit,
                    EditorSettings,
                    EditorUI,
                    Email,
                    EmptyList,
                    EnabledDisabled,
                    EventsOnFinish,
                    EventsOnStart,
                    Export,
                    Facebook,
                    Feather,
                    Filter,
                    FirstFrame,
                    FixedUpdate,
                    Font,
                    GameObject,
                    GenericDatabase,
                    Hide,
                    Hourglass,
                    Idle,
                    Import,
                    Info,
                    Integrations,
                    Label,
                    Landscape,
                    Language,
                    LastFrame,
                    LateUpdate,
                    Link,
                    Load,
                    Location,
                    Locked,
                    LockedUnlocked,
                    Loop,
                    MagicWand,
                    Manual,
                    Minus,
                    More,
                    Music,
                    Navigation,
                    News,
                    OffOn,
                    OneShot,
                    OnOff,
                    Orientation,
                    Paste,
                    PingPong,
                    PingPongOnce,
                    Play,
                    PlayForward,
                    PlayPause,
                    PlayReverse,
                    PlayStop,
                    Plus,
                    PointerDown,
                    PointerEnter,
                    PointerExit,
                    PointerUp,
                    Popout,
                    Portrait,
                    Prefab,
                    QuestionMark,
                    RawImage,
                    Recent,
                    Redo,
                    Refresh,
                    Reset,
                    Reverse,
                    Save,
                    SaveAs,
                    Scripting,
                    Search,
                    Selectable,
                    SelectableColorGenerator,
                    SelectableStates,
                    Selected,
                    Settings,
                    Shake,
                    Show,
                    SortAz,
                    SortHue,
                    SortZa,
                    Sound,
                    SoundMute,
                    Spring,
                    Sprite,
                    SpriteRenderer,
                    SpriteSheet,
                    StartDelay,
                    Stop,
                    Store,
                    SupportRequest,
                    Texture,
                    TimeScale,
                    ToggleMixed,
                    ToggleOFF,
                    ToggleON,
                    Tooltip,
                    Twitter,
                    UIBehaviour,
                    UIMenu,
                    Undo,
                    Unity,
                    UnityEvent,
                    Unlink,
                    Unlocked,
                    VisibilityChanged,
                    Windows,
                    Youtube,
                    Zoom
                }
                

                public static List<Texture2D> Animator => GetTextures(SpriteSheetName.Animator);
                public static List<Texture2D> API => GetTextures(SpriteSheetName.API);
                public static List<Texture2D> ApplicationQuit => GetTextures(SpriteSheetName.ApplicationQuit);
                public static List<Texture2D> Atom => GetTextures(SpriteSheetName.Atom);
                public static List<Texture2D> AudioMixer => GetTextures(SpriteSheetName.AudioMixer);
                public static List<Texture2D> AudioMixerGroup => GetTextures(SpriteSheetName.AudioMixerGroup);
                public static List<Texture2D> AutoDisable => GetTextures(SpriteSheetName.AutoDisable);
                public static List<Texture2D> Back => GetTextures(SpriteSheetName.Back);
                public static List<Texture2D> Binoculars => GetTextures(SpriteSheetName.Binoculars);
                public static List<Texture2D> BookOpen => GetTextures(SpriteSheetName.BookOpen);
                public static List<Texture2D> Border => GetTextures(SpriteSheetName.Border);
                public static List<Texture2D> ButtonClick => GetTextures(SpriteSheetName.ButtonClick);
                public static List<Texture2D> ButtonDoubleClick => GetTextures(SpriteSheetName.ButtonDoubleClick);
                public static List<Texture2D> ButtonLeftClick => GetTextures(SpriteSheetName.ButtonLeftClick);
                public static List<Texture2D> ButtonLongClick => GetTextures(SpriteSheetName.ButtonLongClick);
                public static List<Texture2D> ButtonMiddleClick => GetTextures(SpriteSheetName.ButtonMiddleClick);
                public static List<Texture2D> ButtonRightClick => GetTextures(SpriteSheetName.ButtonRightClick);
                public static List<Texture2D> Camera => GetTextures(SpriteSheetName.Camera);
                public static List<Texture2D> CategoryPlus => GetTextures(SpriteSheetName.CategoryPlus);
                public static List<Texture2D> Clear => GetTextures(SpriteSheetName.Clear);
                public static List<Texture2D> Close => GetTextures(SpriteSheetName.Close);
                public static List<Texture2D> Color => GetTextures(SpriteSheetName.Color);
                public static List<Texture2D> ConnectedDisconnected => GetTextures(SpriteSheetName.ConnectedDisconnected);
                public static List<Texture2D> Cooldown => GetTextures(SpriteSheetName.Cooldown);
                public static List<Texture2D> Copy => GetTextures(SpriteSheetName.Copy);
                public static List<Texture2D> Cut => GetTextures(SpriteSheetName.Cut);
                public static List<Texture2D> Debug => GetTextures(SpriteSheetName.Debug);
                public static List<Texture2D> DelayBetweenLoops => GetTextures(SpriteSheetName.DelayBetweenLoops);
                public static List<Texture2D> Deselected => GetTextures(SpriteSheetName.Deselected);
                public static List<Texture2D> Dice => GetTextures(SpriteSheetName.Dice);
                public static List<Texture2D> DisabledEnabled => GetTextures(SpriteSheetName.DisabledEnabled);
                public static List<Texture2D> DisconnectedConnected => GetTextures(SpriteSheetName.DisconnectedConnected);
                public static List<Texture2D> Discord => GetTextures(SpriteSheetName.Discord);
                public static List<Texture2D> DoozyUI => GetTextures(SpriteSheetName.DoozyUI);
                public static List<Texture2D> DotNet => GetTextures(SpriteSheetName.DotNet);
                public static List<Texture2D> Drag => GetTextures(SpriteSheetName.Drag);
                public static List<Texture2D> Duration => GetTextures(SpriteSheetName.Duration);
                public static List<Texture2D> Edit => GetTextures(SpriteSheetName.Edit);
                public static List<Texture2D> EditorSettings => GetTextures(SpriteSheetName.EditorSettings);
                public static List<Texture2D> EditorUI => GetTextures(SpriteSheetName.EditorUI);
                public static List<Texture2D> Email => GetTextures(SpriteSheetName.Email);
                public static List<Texture2D> EmptyList => GetTextures(SpriteSheetName.EmptyList);
                public static List<Texture2D> EnabledDisabled => GetTextures(SpriteSheetName.EnabledDisabled);
                public static List<Texture2D> EventsOnFinish => GetTextures(SpriteSheetName.EventsOnFinish);
                public static List<Texture2D> EventsOnStart => GetTextures(SpriteSheetName.EventsOnStart);
                public static List<Texture2D> Export => GetTextures(SpriteSheetName.Export);
                public static List<Texture2D> Facebook => GetTextures(SpriteSheetName.Facebook);
                public static List<Texture2D> Feather => GetTextures(SpriteSheetName.Feather);
                public static List<Texture2D> Filter => GetTextures(SpriteSheetName.Filter);
                public static List<Texture2D> FirstFrame => GetTextures(SpriteSheetName.FirstFrame);
                public static List<Texture2D> FixedUpdate => GetTextures(SpriteSheetName.FixedUpdate);
                public static List<Texture2D> Font => GetTextures(SpriteSheetName.Font);
                public static List<Texture2D> GameObject => GetTextures(SpriteSheetName.GameObject);
                public static List<Texture2D> GenericDatabase => GetTextures(SpriteSheetName.GenericDatabase);
                public static List<Texture2D> Hide => GetTextures(SpriteSheetName.Hide);
                public static List<Texture2D> Hourglass => GetTextures(SpriteSheetName.Hourglass);
                public static List<Texture2D> Idle => GetTextures(SpriteSheetName.Idle);
                public static List<Texture2D> Import => GetTextures(SpriteSheetName.Import);
                public static List<Texture2D> Info => GetTextures(SpriteSheetName.Info);
                public static List<Texture2D> Integrations => GetTextures(SpriteSheetName.Integrations);
                public static List<Texture2D> Label => GetTextures(SpriteSheetName.Label);
                public static List<Texture2D> Landscape => GetTextures(SpriteSheetName.Landscape);
                public static List<Texture2D> Language => GetTextures(SpriteSheetName.Language);
                public static List<Texture2D> LastFrame => GetTextures(SpriteSheetName.LastFrame);
                public static List<Texture2D> LateUpdate => GetTextures(SpriteSheetName.LateUpdate);
                public static List<Texture2D> Link => GetTextures(SpriteSheetName.Link);
                public static List<Texture2D> Load => GetTextures(SpriteSheetName.Load);
                public static List<Texture2D> Location => GetTextures(SpriteSheetName.Location);
                public static List<Texture2D> Locked => GetTextures(SpriteSheetName.Locked);
                public static List<Texture2D> LockedUnlocked => GetTextures(SpriteSheetName.LockedUnlocked);
                public static List<Texture2D> Loop => GetTextures(SpriteSheetName.Loop);
                public static List<Texture2D> MagicWand => GetTextures(SpriteSheetName.MagicWand);
                public static List<Texture2D> Manual => GetTextures(SpriteSheetName.Manual);
                public static List<Texture2D> Minus => GetTextures(SpriteSheetName.Minus);
                public static List<Texture2D> More => GetTextures(SpriteSheetName.More);
                public static List<Texture2D> Music => GetTextures(SpriteSheetName.Music);
                public static List<Texture2D> Navigation => GetTextures(SpriteSheetName.Navigation);
                public static List<Texture2D> News => GetTextures(SpriteSheetName.News);
                public static List<Texture2D> OffOn => GetTextures(SpriteSheetName.OffOn);
                public static List<Texture2D> OneShot => GetTextures(SpriteSheetName.OneShot);
                public static List<Texture2D> OnOff => GetTextures(SpriteSheetName.OnOff);
                public static List<Texture2D> Orientation => GetTextures(SpriteSheetName.Orientation);
                public static List<Texture2D> Paste => GetTextures(SpriteSheetName.Paste);
                public static List<Texture2D> PingPong => GetTextures(SpriteSheetName.PingPong);
                public static List<Texture2D> PingPongOnce => GetTextures(SpriteSheetName.PingPongOnce);
                public static List<Texture2D> Play => GetTextures(SpriteSheetName.Play);
                public static List<Texture2D> PlayForward => GetTextures(SpriteSheetName.PlayForward);
                public static List<Texture2D> PlayPause => GetTextures(SpriteSheetName.PlayPause);
                public static List<Texture2D> PlayReverse => GetTextures(SpriteSheetName.PlayReverse);
                public static List<Texture2D> PlayStop => GetTextures(SpriteSheetName.PlayStop);
                public static List<Texture2D> Plus => GetTextures(SpriteSheetName.Plus);
                public static List<Texture2D> PointerDown => GetTextures(SpriteSheetName.PointerDown);
                public static List<Texture2D> PointerEnter => GetTextures(SpriteSheetName.PointerEnter);
                public static List<Texture2D> PointerExit => GetTextures(SpriteSheetName.PointerExit);
                public static List<Texture2D> PointerUp => GetTextures(SpriteSheetName.PointerUp);
                public static List<Texture2D> Popout => GetTextures(SpriteSheetName.Popout);
                public static List<Texture2D> Portrait => GetTextures(SpriteSheetName.Portrait);
                public static List<Texture2D> Prefab => GetTextures(SpriteSheetName.Prefab);
                public static List<Texture2D> QuestionMark => GetTextures(SpriteSheetName.QuestionMark);
                public static List<Texture2D> RawImage => GetTextures(SpriteSheetName.RawImage);
                public static List<Texture2D> Recent => GetTextures(SpriteSheetName.Recent);
                public static List<Texture2D> Redo => GetTextures(SpriteSheetName.Redo);
                public static List<Texture2D> Refresh => GetTextures(SpriteSheetName.Refresh);
                public static List<Texture2D> Reset => GetTextures(SpriteSheetName.Reset);
                public static List<Texture2D> Reverse => GetTextures(SpriteSheetName.Reverse);
                public static List<Texture2D> Save => GetTextures(SpriteSheetName.Save);
                public static List<Texture2D> SaveAs => GetTextures(SpriteSheetName.SaveAs);
                public static List<Texture2D> Scripting => GetTextures(SpriteSheetName.Scripting);
                public static List<Texture2D> Search => GetTextures(SpriteSheetName.Search);
                public static List<Texture2D> Selectable => GetTextures(SpriteSheetName.Selectable);
                public static List<Texture2D> SelectableColorGenerator => GetTextures(SpriteSheetName.SelectableColorGenerator);
                public static List<Texture2D> SelectableStates => GetTextures(SpriteSheetName.SelectableStates);
                public static List<Texture2D> Selected => GetTextures(SpriteSheetName.Selected);
                public static List<Texture2D> Settings => GetTextures(SpriteSheetName.Settings);
                public static List<Texture2D> Shake => GetTextures(SpriteSheetName.Shake);
                public static List<Texture2D> Show => GetTextures(SpriteSheetName.Show);
                public static List<Texture2D> SortAz => GetTextures(SpriteSheetName.SortAz);
                public static List<Texture2D> SortHue => GetTextures(SpriteSheetName.SortHue);
                public static List<Texture2D> SortZa => GetTextures(SpriteSheetName.SortZa);
                public static List<Texture2D> Sound => GetTextures(SpriteSheetName.Sound);
                public static List<Texture2D> SoundMute => GetTextures(SpriteSheetName.SoundMute);
                public static List<Texture2D> Spring => GetTextures(SpriteSheetName.Spring);
                public static List<Texture2D> Sprite => GetTextures(SpriteSheetName.Sprite);
                public static List<Texture2D> SpriteRenderer => GetTextures(SpriteSheetName.SpriteRenderer);
                public static List<Texture2D> SpriteSheet => GetTextures(SpriteSheetName.SpriteSheet);
                public static List<Texture2D> StartDelay => GetTextures(SpriteSheetName.StartDelay);
                public static List<Texture2D> Stop => GetTextures(SpriteSheetName.Stop);
                public static List<Texture2D> Store => GetTextures(SpriteSheetName.Store);
                public static List<Texture2D> SupportRequest => GetTextures(SpriteSheetName.SupportRequest);
                public static List<Texture2D> Texture => GetTextures(SpriteSheetName.Texture);
                public static List<Texture2D> TimeScale => GetTextures(SpriteSheetName.TimeScale);
                public static List<Texture2D> ToggleMixed => GetTextures(SpriteSheetName.ToggleMixed);
                public static List<Texture2D> ToggleOFF => GetTextures(SpriteSheetName.ToggleOFF);
                public static List<Texture2D> ToggleON => GetTextures(SpriteSheetName.ToggleON);
                public static List<Texture2D> Tooltip => GetTextures(SpriteSheetName.Tooltip);
                public static List<Texture2D> Twitter => GetTextures(SpriteSheetName.Twitter);
                public static List<Texture2D> UIBehaviour => GetTextures(SpriteSheetName.UIBehaviour);
                public static List<Texture2D> UIMenu => GetTextures(SpriteSheetName.UIMenu);
                public static List<Texture2D> Undo => GetTextures(SpriteSheetName.Undo);
                public static List<Texture2D> Unity => GetTextures(SpriteSheetName.Unity);
                public static List<Texture2D> UnityEvent => GetTextures(SpriteSheetName.UnityEvent);
                public static List<Texture2D> Unlink => GetTextures(SpriteSheetName.Unlink);
                public static List<Texture2D> Unlocked => GetTextures(SpriteSheetName.Unlocked);
                public static List<Texture2D> VisibilityChanged => GetTextures(SpriteSheetName.VisibilityChanged);
                public static List<Texture2D> Windows => GetTextures(SpriteSheetName.Windows);
                public static List<Texture2D> Youtube => GetTextures(SpriteSheetName.Youtube);
                public static List<Texture2D> Zoom => GetTextures(SpriteSheetName.Zoom);
                
            }

            public static class Placeholders
            {
                private static EditorDataSpriteSheetGroup s_spriteSheetGroup;
                private static EditorDataSpriteSheetGroup spriteSheetGroup =>
                    s_spriteSheetGroup
                        ? s_spriteSheetGroup
                        : s_spriteSheetGroup = EditorDataSpriteSheetDatabase.GetSpriteSheetGroup("EditorUI","Placeholders");

                public static List<Texture2D> GetTextures(SpriteSheetName sheetName) =>
                    spriteSheetGroup.GetTextures(sheetName.ToString());

                public enum SpriteSheetName
                {
                    ComingSoon,
                    Empty,
                    EmptyDatabase,
                    EmptyFile,
                    EmptyListView,
                    EmptyListViewSmall,
                    EmptySearch,
                    EmptySmall,
                    UnderConstruction
                }
                

                public static List<Texture2D> ComingSoon => GetTextures(SpriteSheetName.ComingSoon);
                public static List<Texture2D> Empty => GetTextures(SpriteSheetName.Empty);
                public static List<Texture2D> EmptyDatabase => GetTextures(SpriteSheetName.EmptyDatabase);
                public static List<Texture2D> EmptyFile => GetTextures(SpriteSheetName.EmptyFile);
                public static List<Texture2D> EmptyListView => GetTextures(SpriteSheetName.EmptyListView);
                public static List<Texture2D> EmptyListViewSmall => GetTextures(SpriteSheetName.EmptyListViewSmall);
                public static List<Texture2D> EmptySearch => GetTextures(SpriteSheetName.EmptySearch);
                public static List<Texture2D> EmptySmall => GetTextures(SpriteSheetName.EmptySmall);
                public static List<Texture2D> UnderConstruction => GetTextures(SpriteSheetName.UnderConstruction);
                
            }

            public static class Widgets
            {
                private static EditorDataSpriteSheetGroup s_spriteSheetGroup;
                private static EditorDataSpriteSheetGroup spriteSheetGroup =>
                    s_spriteSheetGroup
                        ? s_spriteSheetGroup
                        : s_spriteSheetGroup = EditorDataSpriteSheetDatabase.GetSpriteSheetGroup("EditorUI","Widgets");

                public static List<Texture2D> GetTextures(SpriteSheetName sheetName) =>
                    spriteSheetGroup.GetTextures(sheetName.ToString());

                public enum SpriteSheetName
                {
                    CircularGauge
                }
                

                public static List<Texture2D> CircularGauge => GetTextures(SpriteSheetName.CircularGauge);
                
            }


        }


        public static class Mody
        {
            public static class Effects
            {
                private static EditorDataSpriteSheetGroup s_spriteSheetGroup;
                private static EditorDataSpriteSheetGroup spriteSheetGroup =>
                    s_spriteSheetGroup
                        ? s_spriteSheetGroup
                        : s_spriteSheetGroup = EditorDataSpriteSheetDatabase.GetSpriteSheetGroup("Mody","Effects");

                public static List<Texture2D> GetTextures(SpriteSheetName sheetName) =>
                    spriteSheetGroup.GetTextures(sheetName.ToString());

                public enum SpriteSheetName
                {
                    Running
                }
                

                public static List<Texture2D> Running => GetTextures(SpriteSheetName.Running);
                
            }

            public static class Icons
            {
                private static EditorDataSpriteSheetGroup s_spriteSheetGroup;
                private static EditorDataSpriteSheetGroup spriteSheetGroup =>
                    s_spriteSheetGroup
                        ? s_spriteSheetGroup
                        : s_spriteSheetGroup = EditorDataSpriteSheetDatabase.GetSpriteSheetGroup("Mody","Icons");

                public static List<Texture2D> GetTextures(SpriteSheetName sheetName) =>
                    spriteSheetGroup.GetTextures(sheetName.ToString());

                public enum SpriteSheetName
                {
                    ModyAction,
                    ModyModule,
                    ModyTrigger
                }
                

                public static List<Texture2D> ModyAction => GetTextures(SpriteSheetName.ModyAction);
                public static List<Texture2D> ModyModule => GetTextures(SpriteSheetName.ModyModule);
                public static List<Texture2D> ModyTrigger => GetTextures(SpriteSheetName.ModyTrigger);
                
            }


        }


        public static class Nody
        {
            public static class Effects
            {
                private static EditorDataSpriteSheetGroup s_spriteSheetGroup;
                private static EditorDataSpriteSheetGroup spriteSheetGroup =>
                    s_spriteSheetGroup
                        ? s_spriteSheetGroup
                        : s_spriteSheetGroup = EditorDataSpriteSheetDatabase.GetSpriteSheetGroup("Nody","Effects");

                public static List<Texture2D> GetTextures(SpriteSheetName sheetName) =>
                    spriteSheetGroup.GetTextures(sheetName.ToString());

                public enum SpriteSheetName
                {
                    BackFlowIndicator,
                    NodeStateActive,
                    NodeStateRunning
                }
                

                public static List<Texture2D> BackFlowIndicator => GetTextures(SpriteSheetName.BackFlowIndicator);
                public static List<Texture2D> NodeStateActive => GetTextures(SpriteSheetName.NodeStateActive);
                public static List<Texture2D> NodeStateRunning => GetTextures(SpriteSheetName.NodeStateRunning);
                
            }

            public static class Icons
            {
                private static EditorDataSpriteSheetGroup s_spriteSheetGroup;
                private static EditorDataSpriteSheetGroup spriteSheetGroup =>
                    s_spriteSheetGroup
                        ? s_spriteSheetGroup
                        : s_spriteSheetGroup = EditorDataSpriteSheetDatabase.GetSpriteSheetGroup("Nody","Icons");

                public static List<Texture2D> GetTextures(SpriteSheetName sheetName) =>
                    spriteSheetGroup.GetTextures(sheetName.ToString());

                public enum SpriteSheetName
                {
                    ApplicationQuitNode,
                    BackButtonNode,
                    CustomNode,
                    DebugNode,
                    EnterNode,
                    ExitNode,
                    FlowController,
                    FlowGraph,
                    GraphController,
                    GroupNode,
                    Infinity,
                    MarkerNode,
                    Minimap,
                    Nody,
                    One,
                    PivotNode,
                    PortalNode,
                    RandomNode,
                    SignalNode,
                    SoundNode,
                    StartNode,
                    StickyNoteNode,
                    SwitchBackNode,
                    ThemeNode,
                    TimeScaleNode,
                    UINode
                }
                

                public static List<Texture2D> ApplicationQuitNode => GetTextures(SpriteSheetName.ApplicationQuitNode);
                public static List<Texture2D> BackButtonNode => GetTextures(SpriteSheetName.BackButtonNode);
                public static List<Texture2D> CustomNode => GetTextures(SpriteSheetName.CustomNode);
                public static List<Texture2D> DebugNode => GetTextures(SpriteSheetName.DebugNode);
                public static List<Texture2D> EnterNode => GetTextures(SpriteSheetName.EnterNode);
                public static List<Texture2D> ExitNode => GetTextures(SpriteSheetName.ExitNode);
                public static List<Texture2D> FlowController => GetTextures(SpriteSheetName.FlowController);
                public static List<Texture2D> FlowGraph => GetTextures(SpriteSheetName.FlowGraph);
                public static List<Texture2D> GraphController => GetTextures(SpriteSheetName.GraphController);
                public static List<Texture2D> GroupNode => GetTextures(SpriteSheetName.GroupNode);
                public static List<Texture2D> Infinity => GetTextures(SpriteSheetName.Infinity);
                public static List<Texture2D> MarkerNode => GetTextures(SpriteSheetName.MarkerNode);
                public static List<Texture2D> Minimap => GetTextures(SpriteSheetName.Minimap);
                public static List<Texture2D> Nody => GetTextures(SpriteSheetName.Nody);
                public static List<Texture2D> One => GetTextures(SpriteSheetName.One);
                public static List<Texture2D> PivotNode => GetTextures(SpriteSheetName.PivotNode);
                public static List<Texture2D> PortalNode => GetTextures(SpriteSheetName.PortalNode);
                public static List<Texture2D> RandomNode => GetTextures(SpriteSheetName.RandomNode);
                public static List<Texture2D> SignalNode => GetTextures(SpriteSheetName.SignalNode);
                public static List<Texture2D> SoundNode => GetTextures(SpriteSheetName.SoundNode);
                public static List<Texture2D> StartNode => GetTextures(SpriteSheetName.StartNode);
                public static List<Texture2D> StickyNoteNode => GetTextures(SpriteSheetName.StickyNoteNode);
                public static List<Texture2D> SwitchBackNode => GetTextures(SpriteSheetName.SwitchBackNode);
                public static List<Texture2D> ThemeNode => GetTextures(SpriteSheetName.ThemeNode);
                public static List<Texture2D> TimeScaleNode => GetTextures(SpriteSheetName.TimeScaleNode);
                public static List<Texture2D> UINode => GetTextures(SpriteSheetName.UINode);
                
            }


        }


        public static class Reactor
        {
            public static class Icons
            {
                private static EditorDataSpriteSheetGroup s_spriteSheetGroup;
                private static EditorDataSpriteSheetGroup spriteSheetGroup =>
                    s_spriteSheetGroup
                        ? s_spriteSheetGroup
                        : s_spriteSheetGroup = EditorDataSpriteSheetDatabase.GetSpriteSheetGroup("Reactor","Icons");

                public static List<Texture2D> GetTextures(SpriteSheetName sheetName) =>
                    spriteSheetGroup.GetTextures(sheetName.ToString());

                public enum SpriteSheetName
                {
                    AnimatorProgressTarget,
                    AudioMixerProgressTarget,
                    ColorAnimation,
                    ColorAnimator,
                    ColorTarget,
                    EditorHeartbeat,
                    Fade,
                    FloatAnimation,
                    FloatAnimator,
                    FrameByFrameAnimation,
                    FrameByFrameAnimator,
                    Heartbeat,
                    ImageProgressTarget,
                    IntAnimation,
                    IntAnimator,
                    Move,
                    Progressor,
                    ProgressorDatabase,
                    ProgressorGroup,
                    ProgressTarget,
                    ReactorController,
                    ReactorIcon,
                    ReactorIconToFull,
                    RectAnimation,
                    RectAnimator,
                    Rotate,
                    Scale,
                    SignalProgressTarget,
                    SpriteAnimation,
                    SpriteAnimator,
                    SpriteTarget,
                    TextMeshProProgressTarget,
                    TextProgressTarget,
                    UIAnimation,
                    UIAnimationPreset,
                    UIAnimator,
                    UnityEventProgressTarget,
                    Vector2Animation,
                    Vector2Animator,
                    Vector3Animation,
                    Vector3Animator,
                    Vector4Animation,
                    Vector4Animator
                }
                

                public static List<Texture2D> AnimatorProgressTarget => GetTextures(SpriteSheetName.AnimatorProgressTarget);
                public static List<Texture2D> AudioMixerProgressTarget => GetTextures(SpriteSheetName.AudioMixerProgressTarget);
                public static List<Texture2D> ColorAnimation => GetTextures(SpriteSheetName.ColorAnimation);
                public static List<Texture2D> ColorAnimator => GetTextures(SpriteSheetName.ColorAnimator);
                public static List<Texture2D> ColorTarget => GetTextures(SpriteSheetName.ColorTarget);
                public static List<Texture2D> EditorHeartbeat => GetTextures(SpriteSheetName.EditorHeartbeat);
                public static List<Texture2D> Fade => GetTextures(SpriteSheetName.Fade);
                public static List<Texture2D> FloatAnimation => GetTextures(SpriteSheetName.FloatAnimation);
                public static List<Texture2D> FloatAnimator => GetTextures(SpriteSheetName.FloatAnimator);
                public static List<Texture2D> FrameByFrameAnimation => GetTextures(SpriteSheetName.FrameByFrameAnimation);
                public static List<Texture2D> FrameByFrameAnimator => GetTextures(SpriteSheetName.FrameByFrameAnimator);
                public static List<Texture2D> Heartbeat => GetTextures(SpriteSheetName.Heartbeat);
                public static List<Texture2D> ImageProgressTarget => GetTextures(SpriteSheetName.ImageProgressTarget);
                public static List<Texture2D> IntAnimation => GetTextures(SpriteSheetName.IntAnimation);
                public static List<Texture2D> IntAnimator => GetTextures(SpriteSheetName.IntAnimator);
                public static List<Texture2D> Move => GetTextures(SpriteSheetName.Move);
                public static List<Texture2D> Progressor => GetTextures(SpriteSheetName.Progressor);
                public static List<Texture2D> ProgressorDatabase => GetTextures(SpriteSheetName.ProgressorDatabase);
                public static List<Texture2D> ProgressorGroup => GetTextures(SpriteSheetName.ProgressorGroup);
                public static List<Texture2D> ProgressTarget => GetTextures(SpriteSheetName.ProgressTarget);
                public static List<Texture2D> ReactorController => GetTextures(SpriteSheetName.ReactorController);
                public static List<Texture2D> ReactorIcon => GetTextures(SpriteSheetName.ReactorIcon);
                public static List<Texture2D> ReactorIconToFull => GetTextures(SpriteSheetName.ReactorIconToFull);
                public static List<Texture2D> RectAnimation => GetTextures(SpriteSheetName.RectAnimation);
                public static List<Texture2D> RectAnimator => GetTextures(SpriteSheetName.RectAnimator);
                public static List<Texture2D> Rotate => GetTextures(SpriteSheetName.Rotate);
                public static List<Texture2D> Scale => GetTextures(SpriteSheetName.Scale);
                public static List<Texture2D> SignalProgressTarget => GetTextures(SpriteSheetName.SignalProgressTarget);
                public static List<Texture2D> SpriteAnimation => GetTextures(SpriteSheetName.SpriteAnimation);
                public static List<Texture2D> SpriteAnimator => GetTextures(SpriteSheetName.SpriteAnimator);
                public static List<Texture2D> SpriteTarget => GetTextures(SpriteSheetName.SpriteTarget);
                public static List<Texture2D> TextMeshProProgressTarget => GetTextures(SpriteSheetName.TextMeshProProgressTarget);
                public static List<Texture2D> TextProgressTarget => GetTextures(SpriteSheetName.TextProgressTarget);
                public static List<Texture2D> UIAnimation => GetTextures(SpriteSheetName.UIAnimation);
                public static List<Texture2D> UIAnimationPreset => GetTextures(SpriteSheetName.UIAnimationPreset);
                public static List<Texture2D> UIAnimator => GetTextures(SpriteSheetName.UIAnimator);
                public static List<Texture2D> UnityEventProgressTarget => GetTextures(SpriteSheetName.UnityEventProgressTarget);
                public static List<Texture2D> Vector2Animation => GetTextures(SpriteSheetName.Vector2Animation);
                public static List<Texture2D> Vector2Animator => GetTextures(SpriteSheetName.Vector2Animator);
                public static List<Texture2D> Vector3Animation => GetTextures(SpriteSheetName.Vector3Animation);
                public static List<Texture2D> Vector3Animator => GetTextures(SpriteSheetName.Vector3Animator);
                public static List<Texture2D> Vector4Animation => GetTextures(SpriteSheetName.Vector4Animation);
                public static List<Texture2D> Vector4Animator => GetTextures(SpriteSheetName.Vector4Animator);
                
            }


        }


        public static class SceneManagement
        {
            public static class Icons
            {
                private static EditorDataSpriteSheetGroup s_spriteSheetGroup;
                private static EditorDataSpriteSheetGroup spriteSheetGroup =>
                    s_spriteSheetGroup
                        ? s_spriteSheetGroup
                        : s_spriteSheetGroup = EditorDataSpriteSheetDatabase.GetSpriteSheetGroup("SceneManagement","Icons");

                public static List<Texture2D> GetTextures(SpriteSheetName sheetName) =>
                    spriteSheetGroup.GetTextures(sheetName.ToString());

                public enum SpriteSheetName
                {
                    ActivateLoadedScenesNode,
                    LoadSceneNode,
                    SceneDirector,
                    SceneLoader,
                    UnloadSceneNode
                }
                

                public static List<Texture2D> ActivateLoadedScenesNode => GetTextures(SpriteSheetName.ActivateLoadedScenesNode);
                public static List<Texture2D> LoadSceneNode => GetTextures(SpriteSheetName.LoadSceneNode);
                public static List<Texture2D> SceneDirector => GetTextures(SpriteSheetName.SceneDirector);
                public static List<Texture2D> SceneLoader => GetTextures(SpriteSheetName.SceneLoader);
                public static List<Texture2D> UnloadSceneNode => GetTextures(SpriteSheetName.UnloadSceneNode);
                
            }


        }


        public static class Signals
        {
            public static class Icons
            {
                private static EditorDataSpriteSheetGroup s_spriteSheetGroup;
                private static EditorDataSpriteSheetGroup spriteSheetGroup =>
                    s_spriteSheetGroup
                        ? s_spriteSheetGroup
                        : s_spriteSheetGroup = EditorDataSpriteSheetDatabase.GetSpriteSheetGroup("Signals","Icons");

                public static List<Texture2D> GetTextures(SpriteSheetName sheetName) =>
                    spriteSheetGroup.GetTextures(sheetName.ToString());

                public enum SpriteSheetName
                {
                    MetaSignal,
                    MetaSignalOnOff,
                    Signal,
                    SignalBroadcaster,
                    SignalOnOff,
                    SignalProvider,
                    SignalReceiver,
                    SignalSender,
                    SignalStream,
                    StreamDatabase
                }
                

                public static List<Texture2D> MetaSignal => GetTextures(SpriteSheetName.MetaSignal);
                public static List<Texture2D> MetaSignalOnOff => GetTextures(SpriteSheetName.MetaSignalOnOff);
                public static List<Texture2D> Signal => GetTextures(SpriteSheetName.Signal);
                public static List<Texture2D> SignalBroadcaster => GetTextures(SpriteSheetName.SignalBroadcaster);
                public static List<Texture2D> SignalOnOff => GetTextures(SpriteSheetName.SignalOnOff);
                public static List<Texture2D> SignalProvider => GetTextures(SpriteSheetName.SignalProvider);
                public static List<Texture2D> SignalReceiver => GetTextures(SpriteSheetName.SignalReceiver);
                public static List<Texture2D> SignalSender => GetTextures(SpriteSheetName.SignalSender);
                public static List<Texture2D> SignalStream => GetTextures(SpriteSheetName.SignalStream);
                public static List<Texture2D> StreamDatabase => GetTextures(SpriteSheetName.StreamDatabase);
                
            }

            public static class Placeholders
            {
                private static EditorDataSpriteSheetGroup s_spriteSheetGroup;
                private static EditorDataSpriteSheetGroup spriteSheetGroup =>
                    s_spriteSheetGroup
                        ? s_spriteSheetGroup
                        : s_spriteSheetGroup = EditorDataSpriteSheetDatabase.GetSpriteSheetGroup("Signals","Placeholders");

                public static List<Texture2D> GetTextures(SpriteSheetName sheetName) =>
                    spriteSheetGroup.GetTextures(sheetName.ToString());

                public enum SpriteSheetName
                {
                    OfflineSignal,
                    OnlineSignal
                }
                

                public static List<Texture2D> OfflineSignal => GetTextures(SpriteSheetName.OfflineSignal);
                public static List<Texture2D> OnlineSignal => GetTextures(SpriteSheetName.OnlineSignal);
                
            }


        }


        public static class UIDesigner
        {
            public static class Icons
            {
                private static EditorDataSpriteSheetGroup s_spriteSheetGroup;
                private static EditorDataSpriteSheetGroup spriteSheetGroup =>
                    s_spriteSheetGroup
                        ? s_spriteSheetGroup
                        : s_spriteSheetGroup = EditorDataSpriteSheetDatabase.GetSpriteSheetGroup("UIDesigner","Icons");

                public static List<Texture2D> GetTextures(SpriteSheetName sheetName) =>
                    spriteSheetGroup.GetTextures(sheetName.ToString());

                public enum SpriteSheetName
                {
                    AlignModeCenter,
                    AlignModeInside,
                    AlignModeOutside,
                    DistributeSpacingHorizontal,
                    DistributeSpacingVertical,
                    HorizontalAlignCenter,
                    HorizontalAlignLeft,
                    HorizontalAlignRight,
                    HorizontalDistributeCenter,
                    HorizontalDistributeLeft,
                    HorizontalDistributeRight,
                    KeyObject,
                    Parent,
                    RectTransform,
                    RootCanvas,
                    Rotate,
                    RotateLeft,
                    RotateRight,
                    ScaleDecrease,
                    ScaleIncrease,
                    Selection,
                    SizeDecrease,
                    SizeIncrease,
                    UIDesigner,
                    UIRescaler,
                    VerticalAlignBottom,
                    VerticalAlignCenter,
                    VerticalAlignTop,
                    VerticalDistributeBottom,
                    VerticalDistributeCenter,
                    VerticalDistributeTop
                }
                

                public static List<Texture2D> AlignModeCenter => GetTextures(SpriteSheetName.AlignModeCenter);
                public static List<Texture2D> AlignModeInside => GetTextures(SpriteSheetName.AlignModeInside);
                public static List<Texture2D> AlignModeOutside => GetTextures(SpriteSheetName.AlignModeOutside);
                public static List<Texture2D> DistributeSpacingHorizontal => GetTextures(SpriteSheetName.DistributeSpacingHorizontal);
                public static List<Texture2D> DistributeSpacingVertical => GetTextures(SpriteSheetName.DistributeSpacingVertical);
                public static List<Texture2D> HorizontalAlignCenter => GetTextures(SpriteSheetName.HorizontalAlignCenter);
                public static List<Texture2D> HorizontalAlignLeft => GetTextures(SpriteSheetName.HorizontalAlignLeft);
                public static List<Texture2D> HorizontalAlignRight => GetTextures(SpriteSheetName.HorizontalAlignRight);
                public static List<Texture2D> HorizontalDistributeCenter => GetTextures(SpriteSheetName.HorizontalDistributeCenter);
                public static List<Texture2D> HorizontalDistributeLeft => GetTextures(SpriteSheetName.HorizontalDistributeLeft);
                public static List<Texture2D> HorizontalDistributeRight => GetTextures(SpriteSheetName.HorizontalDistributeRight);
                public static List<Texture2D> KeyObject => GetTextures(SpriteSheetName.KeyObject);
                public static List<Texture2D> Parent => GetTextures(SpriteSheetName.Parent);
                public static List<Texture2D> RectTransform => GetTextures(SpriteSheetName.RectTransform);
                public static List<Texture2D> RootCanvas => GetTextures(SpriteSheetName.RootCanvas);
                public static List<Texture2D> Rotate => GetTextures(SpriteSheetName.Rotate);
                public static List<Texture2D> RotateLeft => GetTextures(SpriteSheetName.RotateLeft);
                public static List<Texture2D> RotateRight => GetTextures(SpriteSheetName.RotateRight);
                public static List<Texture2D> ScaleDecrease => GetTextures(SpriteSheetName.ScaleDecrease);
                public static List<Texture2D> ScaleIncrease => GetTextures(SpriteSheetName.ScaleIncrease);
                public static List<Texture2D> Selection => GetTextures(SpriteSheetName.Selection);
                public static List<Texture2D> SizeDecrease => GetTextures(SpriteSheetName.SizeDecrease);
                public static List<Texture2D> SizeIncrease => GetTextures(SpriteSheetName.SizeIncrease);
                public static List<Texture2D> UIDesigner => GetTextures(SpriteSheetName.UIDesigner);
                public static List<Texture2D> UIRescaler => GetTextures(SpriteSheetName.UIRescaler);
                public static List<Texture2D> VerticalAlignBottom => GetTextures(SpriteSheetName.VerticalAlignBottom);
                public static List<Texture2D> VerticalAlignCenter => GetTextures(SpriteSheetName.VerticalAlignCenter);
                public static List<Texture2D> VerticalAlignTop => GetTextures(SpriteSheetName.VerticalAlignTop);
                public static List<Texture2D> VerticalDistributeBottom => GetTextures(SpriteSheetName.VerticalDistributeBottom);
                public static List<Texture2D> VerticalDistributeCenter => GetTextures(SpriteSheetName.VerticalDistributeCenter);
                public static List<Texture2D> VerticalDistributeTop => GetTextures(SpriteSheetName.VerticalDistributeTop);
                
            }


        }


        public static class UIManager
        {
            public static class Icons
            {
                private static EditorDataSpriteSheetGroup s_spriteSheetGroup;
                private static EditorDataSpriteSheetGroup spriteSheetGroup =>
                    s_spriteSheetGroup
                        ? s_spriteSheetGroup
                        : s_spriteSheetGroup = EditorDataSpriteSheetDatabase.GetSpriteSheetGroup("UIManager","Icons");

                public static List<Texture2D> GetTextures(SpriteSheetName sheetName) =>
                    spriteSheetGroup.GetTextures(sheetName.ToString());

                public enum SpriteSheetName
                {
                    BackButton,
                    InputToSignal,
                    KeyMapper,
                    MultiplayerInfo,
                    SignalListener,
                    SignalToAudioSource,
                    SignalToColorTarget,
                    SignalToSpriteTarget,
                    SpriteSwapper,
                    UIAlarm,
                    UIButton,
                    UIButtonDatabase,
                    UIButtonListener,
                    UIClock,
                    UIContainer,
                    UIDrawer,
                    UIDrawerListener,
                    UIManagerIcon,
                    UIPopup,
                    UIPopupDatabase,
                    UIPopupLink,
                    UIPopupManager,
                    UIRadialLayout,
                    UIScrollbar,
                    UISelectable,
                    UISelectableAnimator,
                    UISlider,
                    UISliderDatabase,
                    UIStepper,
                    UIStepperDatabase,
                    UIStopwatch,
                    UITab,
                    UITag,
                    UITagDatabase,
                    UITimer,
                    UIToggle,
                    UIToggleDatabase,
                    UIToggleGroup,
                    UIToggleListener,
                    UITooltip,
                    UITooltipDatabase,
                    UITooltipLink,
                    UIView,
                    UIViewDatabase,
                    UIViewListener,
                    ValueTrigger
                }
                

                public static List<Texture2D> BackButton => GetTextures(SpriteSheetName.BackButton);
                public static List<Texture2D> InputToSignal => GetTextures(SpriteSheetName.InputToSignal);
                public static List<Texture2D> KeyMapper => GetTextures(SpriteSheetName.KeyMapper);
                public static List<Texture2D> MultiplayerInfo => GetTextures(SpriteSheetName.MultiplayerInfo);
                public static List<Texture2D> SignalListener => GetTextures(SpriteSheetName.SignalListener);
                public static List<Texture2D> SignalToAudioSource => GetTextures(SpriteSheetName.SignalToAudioSource);
                public static List<Texture2D> SignalToColorTarget => GetTextures(SpriteSheetName.SignalToColorTarget);
                public static List<Texture2D> SignalToSpriteTarget => GetTextures(SpriteSheetName.SignalToSpriteTarget);
                public static List<Texture2D> SpriteSwapper => GetTextures(SpriteSheetName.SpriteSwapper);
                public static List<Texture2D> UIAlarm => GetTextures(SpriteSheetName.UIAlarm);
                public static List<Texture2D> UIButton => GetTextures(SpriteSheetName.UIButton);
                public static List<Texture2D> UIButtonDatabase => GetTextures(SpriteSheetName.UIButtonDatabase);
                public static List<Texture2D> UIButtonListener => GetTextures(SpriteSheetName.UIButtonListener);
                public static List<Texture2D> UIClock => GetTextures(SpriteSheetName.UIClock);
                public static List<Texture2D> UIContainer => GetTextures(SpriteSheetName.UIContainer);
                public static List<Texture2D> UIDrawer => GetTextures(SpriteSheetName.UIDrawer);
                public static List<Texture2D> UIDrawerListener => GetTextures(SpriteSheetName.UIDrawerListener);
                public static List<Texture2D> UIManagerIcon => GetTextures(SpriteSheetName.UIManagerIcon);
                public static List<Texture2D> UIPopup => GetTextures(SpriteSheetName.UIPopup);
                public static List<Texture2D> UIPopupDatabase => GetTextures(SpriteSheetName.UIPopupDatabase);
                public static List<Texture2D> UIPopupLink => GetTextures(SpriteSheetName.UIPopupLink);
                public static List<Texture2D> UIPopupManager => GetTextures(SpriteSheetName.UIPopupManager);
                public static List<Texture2D> UIRadialLayout => GetTextures(SpriteSheetName.UIRadialLayout);
                public static List<Texture2D> UIScrollbar => GetTextures(SpriteSheetName.UIScrollbar);
                public static List<Texture2D> UISelectable => GetTextures(SpriteSheetName.UISelectable);
                public static List<Texture2D> UISelectableAnimator => GetTextures(SpriteSheetName.UISelectableAnimator);
                public static List<Texture2D> UISlider => GetTextures(SpriteSheetName.UISlider);
                public static List<Texture2D> UISliderDatabase => GetTextures(SpriteSheetName.UISliderDatabase);
                public static List<Texture2D> UIStepper => GetTextures(SpriteSheetName.UIStepper);
                public static List<Texture2D> UIStepperDatabase => GetTextures(SpriteSheetName.UIStepperDatabase);
                public static List<Texture2D> UIStopwatch => GetTextures(SpriteSheetName.UIStopwatch);
                public static List<Texture2D> UITab => GetTextures(SpriteSheetName.UITab);
                public static List<Texture2D> UITag => GetTextures(SpriteSheetName.UITag);
                public static List<Texture2D> UITagDatabase => GetTextures(SpriteSheetName.UITagDatabase);
                public static List<Texture2D> UITimer => GetTextures(SpriteSheetName.UITimer);
                public static List<Texture2D> UIToggle => GetTextures(SpriteSheetName.UIToggle);
                public static List<Texture2D> UIToggleDatabase => GetTextures(SpriteSheetName.UIToggleDatabase);
                public static List<Texture2D> UIToggleGroup => GetTextures(SpriteSheetName.UIToggleGroup);
                public static List<Texture2D> UIToggleListener => GetTextures(SpriteSheetName.UIToggleListener);
                public static List<Texture2D> UITooltip => GetTextures(SpriteSheetName.UITooltip);
                public static List<Texture2D> UITooltipDatabase => GetTextures(SpriteSheetName.UITooltipDatabase);
                public static List<Texture2D> UITooltipLink => GetTextures(SpriteSheetName.UITooltipLink);
                public static List<Texture2D> UIView => GetTextures(SpriteSheetName.UIView);
                public static List<Texture2D> UIViewDatabase => GetTextures(SpriteSheetName.UIViewDatabase);
                public static List<Texture2D> UIViewListener => GetTextures(SpriteSheetName.UIViewListener);
                public static List<Texture2D> ValueTrigger => GetTextures(SpriteSheetName.ValueTrigger);
                
            }

            public static class UIMenu
            {
                private static EditorDataSpriteSheetGroup s_spriteSheetGroup;
                private static EditorDataSpriteSheetGroup spriteSheetGroup =>
                    s_spriteSheetGroup
                        ? s_spriteSheetGroup
                        : s_spriteSheetGroup = EditorDataSpriteSheetDatabase.GetSpriteSheetGroup("UIManager","UIMenu");

                public static List<Texture2D> GetTextures(SpriteSheetName sheetName) =>
                    spriteSheetGroup.GetTextures(sheetName.ToString());

                public enum SpriteSheetName
                {
                    AnimatedIcons,
                    AnimatedTitles,
                    Animations,
                    Button,
                    ButtonsPack,
                    Checkbox,
                    Components,
                    Containers,
                    Content,
                    Custom,
                    Dropdown,
                    GridLayout,
                    HorizontalLayout,
                    InputField,
                    Layouts,
                    Menus,
                    RadialLayout,
                    RadioButton,
                    Scollbar,
                    Scripts,
                    ScrollView,
                    Slider,
                    Switch,
                    TabButtonBottomLeft,
                    TabButtonBottomRight,
                    TabButtonLeftBottom,
                    TabButtonLeftFloating,
                    TabButtonLeftTop,
                    TabButtonMiddleFloating,
                    TabButtonMiddleLeft,
                    TabButtonMiddleRight,
                    TabButtonMiddleTop,
                    TabButtonRightBottom,
                    TabButtonRightFloating,
                    TabButtonRightTop,
                    TabButtonTopLeft,
                    TabButtonTopRight,
                    UIFX,
                    UIMenuHeader,
                    UIMenuItem,
                    UIPack,
                    VerticalLayout
                }
                

                public static List<Texture2D> AnimatedIcons => GetTextures(SpriteSheetName.AnimatedIcons);
                public static List<Texture2D> AnimatedTitles => GetTextures(SpriteSheetName.AnimatedTitles);
                public static List<Texture2D> Animations => GetTextures(SpriteSheetName.Animations);
                public static List<Texture2D> Button => GetTextures(SpriteSheetName.Button);
                public static List<Texture2D> ButtonsPack => GetTextures(SpriteSheetName.ButtonsPack);
                public static List<Texture2D> Checkbox => GetTextures(SpriteSheetName.Checkbox);
                public static List<Texture2D> Components => GetTextures(SpriteSheetName.Components);
                public static List<Texture2D> Containers => GetTextures(SpriteSheetName.Containers);
                public static List<Texture2D> Content => GetTextures(SpriteSheetName.Content);
                public static List<Texture2D> Custom => GetTextures(SpriteSheetName.Custom);
                public static List<Texture2D> Dropdown => GetTextures(SpriteSheetName.Dropdown);
                public static List<Texture2D> GridLayout => GetTextures(SpriteSheetName.GridLayout);
                public static List<Texture2D> HorizontalLayout => GetTextures(SpriteSheetName.HorizontalLayout);
                public static List<Texture2D> InputField => GetTextures(SpriteSheetName.InputField);
                public static List<Texture2D> Layouts => GetTextures(SpriteSheetName.Layouts);
                public static List<Texture2D> Menus => GetTextures(SpriteSheetName.Menus);
                public static List<Texture2D> RadialLayout => GetTextures(SpriteSheetName.RadialLayout);
                public static List<Texture2D> RadioButton => GetTextures(SpriteSheetName.RadioButton);
                public static List<Texture2D> Scollbar => GetTextures(SpriteSheetName.Scollbar);
                public static List<Texture2D> Scripts => GetTextures(SpriteSheetName.Scripts);
                public static List<Texture2D> ScrollView => GetTextures(SpriteSheetName.ScrollView);
                public static List<Texture2D> Slider => GetTextures(SpriteSheetName.Slider);
                public static List<Texture2D> Switch => GetTextures(SpriteSheetName.Switch);
                public static List<Texture2D> TabButtonBottomLeft => GetTextures(SpriteSheetName.TabButtonBottomLeft);
                public static List<Texture2D> TabButtonBottomRight => GetTextures(SpriteSheetName.TabButtonBottomRight);
                public static List<Texture2D> TabButtonLeftBottom => GetTextures(SpriteSheetName.TabButtonLeftBottom);
                public static List<Texture2D> TabButtonLeftFloating => GetTextures(SpriteSheetName.TabButtonLeftFloating);
                public static List<Texture2D> TabButtonLeftTop => GetTextures(SpriteSheetName.TabButtonLeftTop);
                public static List<Texture2D> TabButtonMiddleFloating => GetTextures(SpriteSheetName.TabButtonMiddleFloating);
                public static List<Texture2D> TabButtonMiddleLeft => GetTextures(SpriteSheetName.TabButtonMiddleLeft);
                public static List<Texture2D> TabButtonMiddleRight => GetTextures(SpriteSheetName.TabButtonMiddleRight);
                public static List<Texture2D> TabButtonMiddleTop => GetTextures(SpriteSheetName.TabButtonMiddleTop);
                public static List<Texture2D> TabButtonRightBottom => GetTextures(SpriteSheetName.TabButtonRightBottom);
                public static List<Texture2D> TabButtonRightFloating => GetTextures(SpriteSheetName.TabButtonRightFloating);
                public static List<Texture2D> TabButtonRightTop => GetTextures(SpriteSheetName.TabButtonRightTop);
                public static List<Texture2D> TabButtonTopLeft => GetTextures(SpriteSheetName.TabButtonTopLeft);
                public static List<Texture2D> TabButtonTopRight => GetTextures(SpriteSheetName.TabButtonTopRight);
                public static List<Texture2D> UIFX => GetTextures(SpriteSheetName.UIFX);
                public static List<Texture2D> UIMenuHeader => GetTextures(SpriteSheetName.UIMenuHeader);
                public static List<Texture2D> UIMenuItem => GetTextures(SpriteSheetName.UIMenuItem);
                public static List<Texture2D> UIPack => GetTextures(SpriteSheetName.UIPack);
                public static List<Texture2D> VerticalLayout => GetTextures(SpriteSheetName.VerticalLayout);
                
            }
        }


        
    }
}
