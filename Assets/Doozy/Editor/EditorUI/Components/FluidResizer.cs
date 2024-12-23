// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Colors;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.EditorUI.Components
{
    public class FluidResizer : VisualElement
    {
        private const int DEFAULT_SIZE = 3;

        public enum Position { Top, Right, Bottom, Left }

        private Position currentPosition { get; set; } = Position.Right;

        public UnityAction<PointerEnterEvent> onPointerEnter { get; set; }
        public UnityAction<PointerLeaveEvent> onPointerLeave { get; set; }
        public UnityAction<PointerDownEvent> onPointerDown { get; set; }
        public UnityAction<PointerUpEvent> onPointerUp { get; set; }
        public UnityAction<PointerMoveEvent> onPointerMoveEvent { get; set; }

        public UnityAction onResized { get; set; }

        public FluidResizer(Position position) : this()
        {
            schedule.Execute(() =>
            {
                SetPosition(currentPosition);
            });
        }

        private FluidResizer()
        {
            this
                .SetStyleFlexGrow(1)
                .SetStyleFlexShrink(0)
                .SetPosition(currentPosition)
                .SetStyleBackgroundColor(Color.clear);



            RegisterCallback<PointerEnterEvent>(evt =>
            {
                this.SetStyleBackgroundColor(EditorColors.Default.BoxBackground.WithAlpha(0.5f));
                ApplyCursor();
                onPointerEnter?.Invoke(evt);
            });

            RegisterCallback<PointerLeaveEvent>(evt =>
            {
                this.SetStyleBackgroundColor(Color.clear);
                onPointerLeave?.Invoke(evt);
            });

            RegisterCallback<PointerDownEvent>(evt =>
            {
                this.CaptureMouse();
                onPointerDown?.Invoke(evt);
            });

            RegisterCallback<PointerUpEvent>(evt =>
            {
                this.ReleaseMouse();
                onPointerUp?.Invoke(evt);
                onResized?.Invoke();
            });

            RegisterCallback<PointerMoveEvent>(evt =>
            {
                if (!this.HasMouseCapture()) return;
                onPointerMoveEvent?.Invoke(evt);
            });
        }

        public FluidResizer SetPosition(Position position)
        {
            this.ClearMargins()
                .ResetStyleSize()
                .ResetStyleMinSize()
                .ResetStyleMaxSize();

            schedule.Execute(() =>
            {
                switch (position)
                {
                    case Position.Top:
                        this.SetStyleHeight(DEFAULT_SIZE, DEFAULT_SIZE, DEFAULT_SIZE);
                        this.SetStyleMarginBottom(-DEFAULT_SIZE);
                        break;
                    case Position.Right:
                        this.SetStyleWidth(DEFAULT_SIZE, DEFAULT_SIZE, DEFAULT_SIZE);
                        this.SetStyleMarginLeft(-DEFAULT_SIZE);
                        break;
                    case Position.Bottom:
                        this.SetStyleHeight(DEFAULT_SIZE, DEFAULT_SIZE, DEFAULT_SIZE);
                        this.SetStyleMarginTop(-DEFAULT_SIZE);
                        break;
                    case Position.Left:
                        this.SetStyleWidth(DEFAULT_SIZE, DEFAULT_SIZE, DEFAULT_SIZE);
                        this.SetStyleMarginRight(-DEFAULT_SIZE);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(position), position, null);
                }
            });

            return this;
        }

        private void ApplyCursor()
        {
            switch (currentPosition)
            {
                case Position.Top:
                case Position.Bottom:
                    this.SetStyleCursor(EditorTextures.EditorUI.Cursors.ArrowsUpDown);
                    break;
                case Position.Right:
                case Position.Left:
                    this.SetStyleCursor(EditorTextures.EditorUI.Cursors.ArrowsLeftRight);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


    }
}
