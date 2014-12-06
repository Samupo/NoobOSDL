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
 * Serio "Samupo" Alonso <samupo@noobogames.com>
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
    
    public class Texture : IDisposable
    {
        private bool disposed = false;
        internal IntPtr texturePtr { get; private set; }
        //internal string rid;
        public int Width { get; private set; }
        public int Height { get; private set; }

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

    }

}
