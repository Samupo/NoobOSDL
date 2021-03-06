﻿#region License
/* NoobOSDL License
 *
 * Copyright (c) 2014 Sergio Alonso
 *
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages arising from
 * the use of this software.
 * 
 * Thanks to Ethan Lee for his work on SDL2-C# port.
 *
 * Permission is granted to anyone to use this software for any non-commercial
 * project as long as the following requirements are met:
 *
 * 1. Sergio Alonso must be credited as the original author of NoobOSDL even
 * if you modify it's source files. Thanks must be given to Ethan Lee for his
 * work on SDL2-C# port.
 *
 * 2. Altered source versions must be plainly marked as such, and must not be
 * misrepresented as being the original software.
 *
 * 3. This notice may not be removed or altered from any source distribution.
 * 
 * If you want to use this software for a commercial project you need to ask
 * for permission to do so at samupo@noobogames.com
 *
 * Sergio "Samupo" Alonso <samupo@noobogames.com>
 * 
 * 
 * Ethan "flibitijibibo" Lee <flibitijibibo@flibitijibibo.com>
 *
 */
#endregion
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NoobOSDL
{
    /// <summary>
    /// An audio clip that can be played.
    /// </summary>
    public class Audio
    {
        internal IntPtr audioPtr { get; private set; }

        internal Audio(IntPtr audio)
        {
            audioPtr = audio;
        }

        /// <summary>
        /// Plays the audio clip.
        /// </summary>
        public void Play()
        {
            Mix_PlayChannelTimed(-1, audioPtr, 0, -1);
        }

        ~Audio()
        {
            Mix_FreeChunk(audioPtr);
        }

        [DllImport(SDL.MIXLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_PlayChannelTimed(int channel, IntPtr audio, int loops, int ticks);

        [DllImport(SDL.MIXLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Mix_FreeChunk(IntPtr audio);
    }
}
