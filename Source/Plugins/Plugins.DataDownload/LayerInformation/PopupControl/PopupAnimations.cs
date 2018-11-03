﻿#region License LGPL 3
// Copyright © Łukasz Świątkowski 2007–2010.
// http://www.lukesw.net/
//
// This library is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this library.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;

namespace HydroDesktop.Plugins.DataDownload.LayerInformation.PopupControl
{
    /// <summary>
    /// Types of animation of the pop-up window.
    /// </summary>
    [Flags]
    public enum PopupAnimations
    {
        /// <summary>
        /// Uses no animation.
        /// </summary>
        None = 0,
        /// <summary>
        /// Animates the window from left to right. This flag can be used with roll or slide animation.
        /// </summary>
        LeftToRight = 0x00001,
        /// <summary>
        /// Animates the window from right to left. This flag can be used with roll or slide animation.
        /// </summary>
        RightToLeft = 0x00002,
        /// <summary>
        /// Animates the window from top to bottom. This flag can be used with roll or slide animation.
        /// </summary>
        TopToBottom = 0x00004,
        /// <summary>
        /// Animates the window from bottom to top. This flag can be used with roll or slide animation.
        /// </summary>
        BottomToTop = 0x00008,
        /// <summary>
        /// Makes the window appear to collapse inward if it is hiding or expand outward if the window is showing.
        /// </summary>
        Center = 0x00010,
        /// <summary>
        /// Uses a slide animation.
        /// </summary>
        Slide = 0x40000,
        /// <summary>
        /// Uses a fade effect.
        /// </summary>
        Blend = 0x80000,
        /// <summary>
        /// Uses a roll animation.
        /// </summary>
        Roll = 0x100000,
        /// <summary>
        /// Uses a default animation.
        /// </summary>
        SystemDefault = 0x200000,
    }
}
