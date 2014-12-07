#region License
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobOSDL
{
    internal struct SDL_Color
    {
        public byte r, g, b, a;
    }
    /// <summary>
    /// Represents an ARGB32 color
    /// </summary>
    public class SDLColor
    {
        /// <summary>
        /// Red value
        /// </summary>
        public byte R { get; set; }
        /// <summary>
        /// Green value
        /// </summary>
        public byte G { get; set; }
        /// <summary>
        /// Blue value
        /// </summary>
        public byte B { get; set; }
        /// <summary>
        /// Alpha value
        /// </summary>
        public byte A { get; set; }

        /// <summary>
        /// Black {0,0,0,255}
        /// </summary>
        public static SDLColor BLACK { get { return new SDLColor(0, 0, 0); } }

        /// <summary>
        /// Opaque color constructor (Alpha is 255)
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public SDLColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
            A = 255;
        }
        
        /// <summary>
        /// Transparent or semi-transparent color constructor
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public SDLColor(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        internal SDL_Color ToNativeColor()
        {
            SDL_Color native = new SDL_Color();
            native.r = R;
            native.g = G;
            native.b = B;
            native.a = A;
            return native;
        }
    }
}
