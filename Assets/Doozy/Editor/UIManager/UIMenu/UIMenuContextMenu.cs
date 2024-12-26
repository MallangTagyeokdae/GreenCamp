// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//.........................
//.....Generated Class.....
//.........................
//.......Do not edit.......
//.........................

using UnityEditor;
// ReSharper disable All
namespace Doozy.Editor.UIManager.UIMenu
{
    public static class UIMenuContextMenu
    {
        private const int MENU_ITEM_PRIORITY = 7;
        private const string MENU_PATH = "GameObject/UIMenu";

        public static class Components
        {
            private const string TYPE_NAME = "Components";
            private const string TYPE_MENU_PATH = MENU_PATH + "/" + TYPE_NAME + "/";

            public static class ButtonBasic
            {
                private const string CATEGORY_NAME = "Button Basic";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Flex Button", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexButton(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexButton");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Icon Button", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexIconButton(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexIconButton");

                [MenuItem(CATEGORY_MENU_PATH + "Icon Button", false, MENU_ITEM_PRIORITY)]
                public static void CreateIconButton(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "IconButton");

                [MenuItem(CATEGORY_MENU_PATH + "Simple Button", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleButton(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleButton");

                [MenuItem(CATEGORY_MENU_PATH + "Simple Icon Button", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleIconButton(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleIconButton");
            }

            public static class ButtonDirection
            {
                private const string CATEGORY_NAME = "Button Direction";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Arrow Down", false, MENU_ITEM_PRIORITY)]
                public static void CreateArrowDown(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ArrowDown");

                [MenuItem(CATEGORY_MENU_PATH + "Arrow Left", false, MENU_ITEM_PRIORITY)]
                public static void CreateArrowLeft(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ArrowLeft");

                [MenuItem(CATEGORY_MENU_PATH + "Arrow Right", false, MENU_ITEM_PRIORITY)]
                public static void CreateArrowRight(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ArrowRight");

                [MenuItem(CATEGORY_MENU_PATH + "Arrow Up", false, MENU_ITEM_PRIORITY)]
                public static void CreateArrowUp(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ArrowUp");

                [MenuItem(CATEGORY_MENU_PATH + "Chevron Down", false, MENU_ITEM_PRIORITY)]
                public static void CreateChevronDown(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ChevronDown");

                [MenuItem(CATEGORY_MENU_PATH + "Chevron Left", false, MENU_ITEM_PRIORITY)]
                public static void CreateChevronLeft(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ChevronLeft");

                [MenuItem(CATEGORY_MENU_PATH + "Chevron Right", false, MENU_ITEM_PRIORITY)]
                public static void CreateChevronRight(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ChevronRight");

                [MenuItem(CATEGORY_MENU_PATH + "Chevron Up", false, MENU_ITEM_PRIORITY)]
                public static void CreateChevronUp(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ChevronUp");

                [MenuItem(CATEGORY_MENU_PATH + "Circle Arrow Down", false, MENU_ITEM_PRIORITY)]
                public static void CreateCircleArrowDown(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CircleArrowDown");

                [MenuItem(CATEGORY_MENU_PATH + "Circle Arrow Left", false, MENU_ITEM_PRIORITY)]
                public static void CreateCircleArrowLeft(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CircleArrowLeft");

                [MenuItem(CATEGORY_MENU_PATH + "Circle Arrow Right", false, MENU_ITEM_PRIORITY)]
                public static void CreateCircleArrowRight(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CircleArrowRight");

                [MenuItem(CATEGORY_MENU_PATH + "Circle Arrow Up", false, MENU_ITEM_PRIORITY)]
                public static void CreateCircleArrowUp(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CircleArrowUp");

                [MenuItem(CATEGORY_MENU_PATH + "Circle Chevron Down", false, MENU_ITEM_PRIORITY)]
                public static void CreateCircleChevronDown(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CircleChevronDown");

                [MenuItem(CATEGORY_MENU_PATH + "Circle Chevron Left", false, MENU_ITEM_PRIORITY)]
                public static void CreateCircleChevronLeft(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CircleChevronLeft");

                [MenuItem(CATEGORY_MENU_PATH + "Circle Chevron Right", false, MENU_ITEM_PRIORITY)]
                public static void CreateCircleChevronRight(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CircleChevronRight");

                [MenuItem(CATEGORY_MENU_PATH + "Circle Chevron Up", false, MENU_ITEM_PRIORITY)]
                public static void CreateCircleChevronUp(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CircleChevronUp");
            }

            public static class ButtonGeneral
            {
                private const string CATEGORY_NAME = "Button General";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "AddToCart", false, MENU_ITEM_PRIORITY)]
                public static void CreateAddToCart(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "AddToCart");

                [MenuItem(CATEGORY_MENU_PATH + "AddToCart Bag", false, MENU_ITEM_PRIORITY)]
                public static void CreateAddToCartBag(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "AddToCartBag");

                [MenuItem(CATEGORY_MENU_PATH + "AddToFavorites", false, MENU_ITEM_PRIORITY)]
                public static void CreateAddToFavorites(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "AddToFavorites");

                [MenuItem(CATEGORY_MENU_PATH + "Calendar", false, MENU_ITEM_PRIORITY)]
                public static void CreateCalendar(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Calendar");

                [MenuItem(CATEGORY_MENU_PATH + "Calendar Days", false, MENU_ITEM_PRIORITY)]
                public static void CreateCalendarDays(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CalendarDays");

                [MenuItem(CATEGORY_MENU_PATH + "Camera", false, MENU_ITEM_PRIORITY)]
                public static void CreateCamera(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Camera");

                [MenuItem(CATEGORY_MENU_PATH + "Close XMark", false, MENU_ITEM_PRIORITY)]
                public static void CreateCloseXMark(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CloseXMark");

                [MenuItem(CATEGORY_MENU_PATH + "Gamepad", false, MENU_ITEM_PRIORITY)]
                public static void CreateGamepad(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Gamepad");

                [MenuItem(CATEGORY_MENU_PATH + "Gift", false, MENU_ITEM_PRIORITY)]
                public static void CreateGift(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Gift");

                [MenuItem(CATEGORY_MENU_PATH + "Globe", false, MENU_ITEM_PRIORITY)]
                public static void CreateGlobe(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Globe");

                [MenuItem(CATEGORY_MENU_PATH + "Id Badge", false, MENU_ITEM_PRIORITY)]
                public static void CreateIdBadge(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "IdBadge");

                [MenuItem(CATEGORY_MENU_PATH + "Key", false, MENU_ITEM_PRIORITY)]
                public static void CreateKey(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Key");

                [MenuItem(CATEGORY_MENU_PATH + "Language", false, MENU_ITEM_PRIORITY)]
                public static void CreateLanguage(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Language");

                [MenuItem(CATEGORY_MENU_PATH + "Map", false, MENU_ITEM_PRIORITY)]
                public static void CreateMap(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Map");

                [MenuItem(CATEGORY_MENU_PATH + "Minus", false, MENU_ITEM_PRIORITY)]
                public static void CreateMinus(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Minus");

                [MenuItem(CATEGORY_MENU_PATH + "Plus", false, MENU_ITEM_PRIORITY)]
                public static void CreatePlus(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Plus");

                [MenuItem(CATEGORY_MENU_PATH + "Redo", false, MENU_ITEM_PRIORITY)]
                public static void CreateRedo(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Redo");

                [MenuItem(CATEGORY_MENU_PATH + "Refresh", false, MENU_ITEM_PRIORITY)]
                public static void CreateRefresh(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Refresh");

                [MenuItem(CATEGORY_MENU_PATH + "Reply", false, MENU_ITEM_PRIORITY)]
                public static void CreateReply(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Reply");

                [MenuItem(CATEGORY_MENU_PATH + "ReportBug", false, MENU_ITEM_PRIORITY)]
                public static void CreateReportBug(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ReportBug");

                [MenuItem(CATEGORY_MENU_PATH + "Search", false, MENU_ITEM_PRIORITY)]
                public static void CreateSearch(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Search");

                [MenuItem(CATEGORY_MENU_PATH + "SendEmail", false, MENU_ITEM_PRIORITY)]
                public static void CreateSendEmail(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SendEmail");

                [MenuItem(CATEGORY_MENU_PATH + "Share", false, MENU_ITEM_PRIORITY)]
                public static void CreateShare(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Share");

                [MenuItem(CATEGORY_MENU_PATH + "Share Nodes", false, MENU_ITEM_PRIORITY)]
                public static void CreateShareNodes(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ShareNodes");

                [MenuItem(CATEGORY_MENU_PATH + "Star", false, MENU_ITEM_PRIORITY)]
                public static void CreateStar(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Star");

                [MenuItem(CATEGORY_MENU_PATH + "Tag", false, MENU_ITEM_PRIORITY)]
                public static void CreateTag(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Tag");

                [MenuItem(CATEGORY_MENU_PATH + "Undo", false, MENU_ITEM_PRIORITY)]
                public static void CreateUndo(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Undo");

                [MenuItem(CATEGORY_MENU_PATH + "User", false, MENU_ITEM_PRIORITY)]
                public static void CreateUser(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "User");

                [MenuItem(CATEGORY_MENU_PATH + "Users", false, MENU_ITEM_PRIORITY)]
                public static void CreateUsers(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Users");
            }

            public static class ButtonMedia
            {
                private const string CATEGORY_NAME = "Button Media";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Backward", false, MENU_ITEM_PRIORITY)]
                public static void CreateBackward(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Backward");

                [MenuItem(CATEGORY_MENU_PATH + "Backward Step", false, MENU_ITEM_PRIORITY)]
                public static void CreateBackwardStep(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "BackwardStep");

                [MenuItem(CATEGORY_MENU_PATH + "Forward", false, MENU_ITEM_PRIORITY)]
                public static void CreateForward(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Forward");

                [MenuItem(CATEGORY_MENU_PATH + "Forward Step", false, MENU_ITEM_PRIORITY)]
                public static void CreateForwardStep(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "ForwardStep");

                [MenuItem(CATEGORY_MENU_PATH + "Pause", false, MENU_ITEM_PRIORITY)]
                public static void CreatePause(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Pause");

                [MenuItem(CATEGORY_MENU_PATH + "Play", false, MENU_ITEM_PRIORITY)]
                public static void CreatePlay(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Play");

                [MenuItem(CATEGORY_MENU_PATH + "PlayPause", false, MENU_ITEM_PRIORITY)]
                public static void CreatePlayPause(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "PlayPause");

                [MenuItem(CATEGORY_MENU_PATH + "Repeat", false, MENU_ITEM_PRIORITY)]
                public static void CreateRepeat(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Repeat");

                [MenuItem(CATEGORY_MENU_PATH + "Stop", false, MENU_ITEM_PRIORITY)]
                public static void CreateStop(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Stop");
            }

            public static class ProgressorBasic
            {
                private const string CATEGORY_NAME = "Progressor Basic";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Simple Progress Bar", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleProgressBar(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleProgressBar");

                [MenuItem(CATEGORY_MENU_PATH + "Simple Progress Text", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleProgressText(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleProgressText");
            }

            public static class ScrollbarBasic
            {
                private const string CATEGORY_NAME = "Scrollbar Basic";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Simple Scrollbar", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleScrollbar(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleScrollbar");
            }

            public static class SliderBasic
            {
                private const string CATEGORY_NAME = "Slider Basic";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Simple Slider", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleSlider(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleSlider");
            }

            public static class StepperBasic
            {
                private const string CATEGORY_NAME = "Stepper Basic";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Counter Stepper H", false, MENU_ITEM_PRIORITY)]
                public static void CreateCounterStepperH(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CounterStepperH");

                [MenuItem(CATEGORY_MENU_PATH + "Counter Stepper V", false, MENU_ITEM_PRIORITY)]
                public static void CreateCounterStepperV(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CounterStepperV");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Stepper H", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexStepperH(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexStepperH");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Stepper V", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexStepperV(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexStepperV");

                [MenuItem(CATEGORY_MENU_PATH + "Quanity Stepper H", false, MENU_ITEM_PRIORITY)]
                public static void CreateQuanityStepperH(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "QuanityStepperH");

                [MenuItem(CATEGORY_MENU_PATH + "Quantity Stepper V", false, MENU_ITEM_PRIORITY)]
                public static void CreateQuantityStepperV(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "QuantityStepperV");

                [MenuItem(CATEGORY_MENU_PATH + "Round Stepper H", false, MENU_ITEM_PRIORITY)]
                public static void CreateRoundStepperH(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "RoundStepperH");

                [MenuItem(CATEGORY_MENU_PATH + "Round Stepper V", false, MENU_ITEM_PRIORITY)]
                public static void CreateRoundStepperV(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "RoundStepperV");

                [MenuItem(CATEGORY_MENU_PATH + "Simple Stepper H", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleStepperH(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleStepperH");

                [MenuItem(CATEGORY_MENU_PATH + "Simple Stepper V", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleStepperV(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleStepperV");

                [MenuItem(CATEGORY_MENU_PATH + "Slider Stepper H", false, MENU_ITEM_PRIORITY)]
                public static void CreateSliderStepperH(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SliderStepperH");

                [MenuItem(CATEGORY_MENU_PATH + "Slider Stepper V", false, MENU_ITEM_PRIORITY)]
                public static void CreateSliderStepperV(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SliderStepperV");
            }

            public static class TabBasic
            {
                private const string CATEGORY_NAME = "Tab Basic";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Flex Icon Tab", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexIconTab(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexIconTab");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Tab", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexTab(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexTab");

                [MenuItem(CATEGORY_MENU_PATH + "Icon Tab", false, MENU_ITEM_PRIORITY)]
                public static void CreateIconTab(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "IconTab");

                [MenuItem(CATEGORY_MENU_PATH + "Simple Icon Tab", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleIconTab(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleIconTab");

                [MenuItem(CATEGORY_MENU_PATH + "Simple Tab", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleTab(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleTab");
            }

            public static class ToggleCheckbox
            {
                private const string CATEGORY_NAME = "Toggle Checkbox";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Checkbox", false, MENU_ITEM_PRIORITY)]
                public static void CreateCheckbox(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Checkbox");

                [MenuItem(CATEGORY_MENU_PATH + "Flex Checkbox", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexCheckbox(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexCheckbox");
            }

            public static class ToggleIcon
            {
                private const string CATEGORY_NAME = "Toggle Icon";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Icon Toggle", false, MENU_ITEM_PRIORITY)]
                public static void CreateIconToggle(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "IconToggle");
            }

            public static class ToggleRadio
            {
                private const string CATEGORY_NAME = "Toggle Radio";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Flex Radio", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexRadio(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexRadio");

                [MenuItem(CATEGORY_MENU_PATH + "Radio", false, MENU_ITEM_PRIORITY)]
                public static void CreateRadio(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Radio");
            }

            public static class ToggleSwitch
            {
                private const string CATEGORY_NAME = "Toggle Switch";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Flex Switch", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlexSwitch(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlexSwitch");

                [MenuItem(CATEGORY_MENU_PATH + "Switch", false, MENU_ITEM_PRIORITY)]
                public static void CreateSwitch(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Switch");
            }
        }

        public static class Containers
        {
            private const string TYPE_NAME = "Containers";
            private const string TYPE_MENU_PATH = MENU_PATH + "/" + TYPE_NAME + "/";

            public static class ContainerBasic
            {
                private const string CATEGORY_NAME = "Container Basic";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Simple Container", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleContainer(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleContainer");
            }

            public static class TabLayout
            {
                private const string CATEGORY_NAME = "Tab Layout";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Bottom Expanded", false, MENU_ITEM_PRIORITY)]
                public static void CreateBottomExpanded(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "BottomExpanded");

                [MenuItem(CATEGORY_MENU_PATH + "Bottom Left", false, MENU_ITEM_PRIORITY)]
                public static void CreateBottomLeft(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "BottomLeft");

                [MenuItem(CATEGORY_MENU_PATH + "Bottom Right", false, MENU_ITEM_PRIORITY)]
                public static void CreateBottomRight(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "BottomRight");

                [MenuItem(CATEGORY_MENU_PATH + "Left Bottom", false, MENU_ITEM_PRIORITY)]
                public static void CreateLeftBottom(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "LeftBottom");

                [MenuItem(CATEGORY_MENU_PATH + "Left Expanded", false, MENU_ITEM_PRIORITY)]
                public static void CreateLeftExpanded(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "LeftExpanded");

                [MenuItem(CATEGORY_MENU_PATH + "Left Top", false, MENU_ITEM_PRIORITY)]
                public static void CreateLeftTop(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "LeftTop");

                [MenuItem(CATEGORY_MENU_PATH + "Right Bottom", false, MENU_ITEM_PRIORITY)]
                public static void CreateRightBottom(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "RightBottom");

                [MenuItem(CATEGORY_MENU_PATH + "Right Expanded", false, MENU_ITEM_PRIORITY)]
                public static void CreateRightExpanded(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "RightExpanded");

                [MenuItem(CATEGORY_MENU_PATH + "Right Top", false, MENU_ITEM_PRIORITY)]
                public static void CreateRightTop(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "RightTop");

                [MenuItem(CATEGORY_MENU_PATH + "Top Expanded", false, MENU_ITEM_PRIORITY)]
                public static void CreateTopExpanded(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "TopExpanded");

                [MenuItem(CATEGORY_MENU_PATH + "Top Left", false, MENU_ITEM_PRIORITY)]
                public static void CreateTopLeft(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "TopLeft");

                [MenuItem(CATEGORY_MENU_PATH + "Top Right", false, MENU_ITEM_PRIORITY)]
                public static void CreateTopRight(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "TopRight");
            }

            public static class ViewBasic
            {
                private const string CATEGORY_NAME = "View Basic";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Simple View", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleView(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleView");
            }
        }

        public static class Content
        {
            private const string TYPE_NAME = "Content";
            private const string TYPE_MENU_PATH = MENU_PATH + "/" + TYPE_NAME + "/";

            public static class Basic
            {
                private const string CATEGORY_NAME = "Basic";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Dark Overlay", false, MENU_ITEM_PRIORITY)]
                public static void CreateDarkOverlay(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "DarkOverlay");

                [MenuItem(CATEGORY_MENU_PATH + "Light Overlay", false, MENU_ITEM_PRIORITY)]
                public static void CreateLightOverlay(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "LightOverlay");

                [MenuItem(CATEGORY_MENU_PATH + "Simple Card", false, MENU_ITEM_PRIORITY)]
                public static void CreateSimpleCard(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SimpleCard");
            }

            public static class ProductCard
            {
                private const string CATEGORY_NAME = "Product Card";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Card 001", false, MENU_ITEM_PRIORITY)]
                public static void CreateCard001(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Card001");

                [MenuItem(CATEGORY_MENU_PATH + "Card 002", false, MENU_ITEM_PRIORITY)]
                public static void CreateCard002(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Card002");

                [MenuItem(CATEGORY_MENU_PATH + "Card 003", false, MENU_ITEM_PRIORITY)]
                public static void CreateCard003(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Card003");

                [MenuItem(CATEGORY_MENU_PATH + "Card 004", false, MENU_ITEM_PRIORITY)]
                public static void CreateCard004(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Card004");

                [MenuItem(CATEGORY_MENU_PATH + "Card 005", false, MENU_ITEM_PRIORITY)]
                public static void CreateCard005(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Card005");
            }

            public static class SpinnerBasic
            {
                private const string CATEGORY_NAME = "Spinner Basic";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Circle Spinner", false, MENU_ITEM_PRIORITY)]
                public static void CreateCircleSpinner(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "CircleSpinner");

                [MenuItem(CATEGORY_MENU_PATH + "Rim Circle Spinner", false, MENU_ITEM_PRIORITY)]
                public static void CreateRimCircleSpinner(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "RimCircleSpinner");

                [MenuItem(CATEGORY_MENU_PATH + "Square Spinner", false, MENU_ITEM_PRIORITY)]
                public static void CreateSquareSpinner(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "SquareSpinner");
            }
        }

        public static class Layouts
        {
            private const string TYPE_NAME = "Layouts";
            private const string TYPE_MENU_PATH = MENU_PATH + "/" + TYPE_NAME + "/";

            public static class Basic
            {
                private const string CATEGORY_NAME = "Basic";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Grid", false, MENU_ITEM_PRIORITY)]
                public static void CreateGrid(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Grid");

                [MenuItem(CATEGORY_MENU_PATH + "Horizontal", false, MENU_ITEM_PRIORITY)]
                public static void CreateHorizontal(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Horizontal");

                [MenuItem(CATEGORY_MENU_PATH + "Radial", false, MENU_ITEM_PRIORITY)]
                public static void CreateRadial(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Radial");

                [MenuItem(CATEGORY_MENU_PATH + "Vertical", false, MENU_ITEM_PRIORITY)]
                public static void CreateVertical(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "Vertical");
            }
        }

        public static class Scripts
        {
            private const string TYPE_NAME = "Scripts";
            private const string TYPE_MENU_PATH = MENU_PATH + "/" + TYPE_NAME + "/";

            public static class Controller
            {
                private const string CATEGORY_NAME = "Controller";
                private const string CATEGORY_MENU_PATH = TYPE_MENU_PATH + CATEGORY_NAME + "/";

                [MenuItem(CATEGORY_MENU_PATH + "Flow Controller", false, MENU_ITEM_PRIORITY)]
                public static void CreateFlowController(MenuCommand command) => UIMenuUtils.AddToScene(TYPE_NAME, CATEGORY_NAME, "FlowController");
            }
        }        
    }
}
