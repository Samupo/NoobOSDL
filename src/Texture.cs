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
using SDL2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NoobOSDL
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SDL_Surface
    {
        public uint flags;
        public IntPtr format;
        public int w;
        public int h;
        public int pitch;
        public IntPtr pixels;
        public IntPtr userdata;
        public int locked;
        public IntPtr lock_data;
        public SDL_Rect clip_rect;
        public IntPtr map;
        public int refcount;

        public static SDL_Surface FromIntPtr(IntPtr data)
        {
            return (SDL_Surface)Marshal.PtrToStructure(data, typeof(SDL_Surface));
        }
    }

    /// <summary>
    /// Represents a texture that is stored in memory.
    /// </summary>
    public class Texture : IDisposable
    {
        /// <summary>
        /// Texture blending mode
        /// </summary>
        public enum BlendModeEnum
        {
            /// <summary>
            /// Completely opaque
            /// </summary>
            NONE = 0x00000000,
            /// <summary>
            /// Uses texture alpha channel to draw
            /// </summary>
            ALPHA_BLEND = 0x00000001,
            /// <summary>
            /// Additive blending [dstRGB = (srcRGB*srcA) + dstRGB)]
            /// </summary>
            ADDITIVE = 0x00000002,
            /// <summary>
            /// Color modulate [dstRGB = srcRGB * dstRGB]
            /// </summary>
            MODULATE = 0x00000004
        }


        private bool disposed = false;
        internal IntPtr texturePtr { get; private set; }
        //internal string rid;
        /// <summary>
        /// Gets the texture width
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// Gets the texture height
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// Gets or sets the opacity (0 is fully transparent and 255 is opaque)
        /// </summary>
        public byte Opacity { get { byte b; SDL_GetTextureAlphaMod(texturePtr, out b); return b; } set { SDL_SetTextureAlphaMod(texturePtr, value); } }
        /// <summary>
        /// Blending mode
        /// </summary>
        public BlendModeEnum BlendMode { get { uint mode; SDL_GetTextureBlendMode(texturePtr, out mode); return (BlendModeEnum)mode; } set { SDL_SetTextureBlendMode(texturePtr, (uint)value); } }

        internal Texture(IntPtr texture, int w, int h)
        {
            //this.rid = resourceID;
            texturePtr = texture;
            this.Width = w;
            this.Height = h;
        }

        ~Texture()
        {
            SDL_DestroyTexture(texturePtr);
            Dispose(false);
        }


        /// <summary>
        /// Disposes the texture and frees its memory
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {

                SDL_DestroyTexture(texturePtr);
            }
            disposed = true;
        }


        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_DestroyTexture(IntPtr texture);

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetTextureAlphaMod(IntPtr texture, byte alpha);

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetTextureAlphaMod(IntPtr texture, out byte alpha);

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetTextureBlendMode(IntPtr texture, uint alpha);

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_GetTextureBlendMode(IntPtr texture, out uint alpha);
    }

}
