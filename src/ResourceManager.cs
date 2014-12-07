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
    internal class StoredResource<T>
    {
        public T Resource;
        public int Uses;

        public StoredResource(T resource)
        {
            this.Resource = resource;
            this.Uses = 1;
        }
    }

    /// <summary>
    /// Manages resource loading, storing and unloading. For every Load method a Unload one must be used.
    /// </summary>
    public static class ResourceManager
    {
        private static Renderer renderer;
        /// <summary>
        /// Gets or sets the renderer the ResourceManager uses to create textures
        /// </summary>
        public static Renderer AssignedRenderer
        {
            get
            {
                if (renderer == null)
                {
                    if (SDL.Renderers.GetLength(0) > 0)
                        renderer = SDL.Renderers[0];
                } return renderer;
            }
            set
            {
                renderer = value;
            }
        }

        #region TEXTURES
        private static Dictionary<string, StoredResource<Texture>> storedTextures = new Dictionary<string, StoredResource<Texture>>();

        /// <summary>
        /// Loads a texture. If not unloaded later a memory leak will appear.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Texture LoadTexture(string file)
        {
            StoredResource<Texture> sr;
            if (storedTextures.ContainsKey(file))
            {
                storedTextures.TryGetValue(file, out sr);
                if (sr != null)
                {
                    sr.Uses++;
                    return sr.Resource;
                }
            }
            sr = new StoredResource<Texture>(DoLoadTexture(file));
            storedTextures.Add(file, sr);
            return sr.Resource;
        }

        /// <summary>
        /// Unloads a texture. It's mandatory to call a Unload for each Load method used previously.
        /// </summary>
        /// <param name="file"></param>
        public static void UnloadTexture(string file)
        {
            StoredResource<Texture> sr;
            storedTextures.TryGetValue(file, out sr);
            if (sr != null)
            {
                sr.Uses--;
                if (sr.Uses <= 0)
                {
                    storedTextures.Remove(file);
                }
            }
        }

        private static Texture DoLoadTexture(string file)
        {
            IntPtr texturePtr = IntPtr.Zero;

            IntPtr loadedSurface = IMG_Load(file);
            if (loadedSurface == IntPtr.Zero)
            {
                throw new SDLException("Unable to load image " + file + "\r\n" + IMG_GetError());
            }
            texturePtr = SDL_CreateTextureFromSurface(AssignedRenderer.rendererPtr, loadedSurface);
            if (texturePtr == IntPtr.Zero)
            {
                throw new SDLException("Unable to load image " + file + "\r\n" + IMG_GetError());
            }

            SDL_Surface surfaceStruct = (SDL_Surface)Marshal.PtrToStructure(loadedSurface, typeof(SDL_Surface));
            Texture texture = new Texture(texturePtr, surfaceStruct.w, surfaceStruct.h);
            SDL_FreeSurface(loadedSurface);
            return texture;

        }
        #endregion

        #region AUDIO
        private static Dictionary<string, StoredResource<Audio>> storedAudio = new Dictionary<string, StoredResource<Audio>>();

        /// <summary>
        /// Loads an audio clip. If not unloaded later a memory leak will appear.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Audio LoadAudio(string file)
        {
            StoredResource<Audio> sr;
            if (storedAudio.ContainsKey(file))
            {
                storedAudio.TryGetValue(file, out sr);
                if (sr != null)
                {
                    sr.Uses++;
                    return sr.Resource;
                }
            }
            sr = new StoredResource<Audio>(DoLoadAudio(file));
            storedAudio.Add(file, sr);
            return sr.Resource;
        }

        /// <summary>
        /// Unloads an audio clip. It's mandatory to call a Unload for each Load method used previously.
        /// </summary>
        /// <param name="file"></param>
        public static void UnloadAudio(string file)
        {
            StoredResource<Audio> sr;
            storedAudio.TryGetValue(file, out sr);
            if (sr != null)
            {
                sr.Uses--;
                if (sr.Uses <= 0)
                {
                    storedAudio.Remove(file);
                }
            }
        }

        private static Audio DoLoadAudio(string file)
        {
            return new Audio(Mix_LoadWAV_RW(SDL_RWFromFile(file, "rb"), 1));
        }
        #endregion


        #region MUSIC
        private static Dictionary<string, StoredResource<Music>> storedMusic = new Dictionary<string, StoredResource<Music>>();
        /// <summary>
        /// Loads a music. If not unloaded later a memory leak will appear.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Music LoadMusic(string file)
        {
            StoredResource<Music> sr;
            if (storedMusic.ContainsKey(file))
            {
                storedMusic.TryGetValue(file, out sr);
                if (sr != null)
                {
                    sr.Uses++;
                    return sr.Resource;
                }
            }
            sr = new StoredResource<Music>(DoLoadMusic(file));
            storedMusic.Add(file, sr);
            return sr.Resource;
        }

        /// <summary>
        /// Unloads a music. It's mandatory to call a Unload for each Load method used previously.
        /// </summary>
        /// <param name="file"></param>
        public static void UnloadMusic(string file)
        {
            StoredResource<Music> sr;
            storedMusic.TryGetValue(file, out sr);
            if (sr != null)
            {
                sr.Uses--;
                if (sr.Uses <= 0)
                {
                    storedMusic.Remove(file);
                }
            }
        }

        private static Music DoLoadMusic(string file)
        {
            return new Music(Mix_LoadMUS(file));

        }
        #endregion

        #region NATIVE
        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_RWFromFile(
        [In()] [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler))]
string file,
        [In()] [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler))]
string mode
        );

        [DllImport(SDL.MIXLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Mix_LoadWAV_RW(
        IntPtr src,
        int freesrc
        );

        [DllImport(SDL.MIXLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Mix_LoadWAV([In()] [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler))] string file);

        [DllImport(SDL.MIXLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Mix_LoadMUS([In()] [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler))] string file);

        [DllImport(SDL.IMGLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr IMG_Load([In()] [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler))] string file);

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_FreeSurface(IntPtr surface);

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateTextureFromSurface(IntPtr renderer, IntPtr surface);

        [DllImport(SDL.IMGLIB, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler), MarshalCookie = LPUtf8StrMarshaler.LeaveAllocated)]
        private static extern string IMG_GetError();


        [DllImport(SDL.MIXLIB, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler), MarshalCookie = LPUtf8StrMarshaler.LeaveAllocated)]
        private static extern string Mix_GetError();
        #endregion
    }
}
