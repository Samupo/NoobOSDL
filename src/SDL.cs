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
    public static class SDL
    {
        internal const string NATIVELIB = "SDL2.dll";
        internal const string IMGLIB = "SDL2_image.dll";
        internal const string TTFLIB = "SDL2_ttf.dll";
        internal const string MIXLIB = "SDL2_mixer.dll";

        #region STATIC VARS
        private static List<Window> windows = new List<Window>();
        private static List<Renderer> renderers = new List<Renderer>();
        public static Window[] Windows { get { return windows.ToArray(); } }
        public static Renderer[] Renderers { get { return renderers.ToArray(); } }
        public static bool Running { get; private set; }
        #endregion

        #region SDL ENUMS & SDL VARS
        internal enum InitializeModule
        {
            INIT_TIMER = 0x00000001,
            INIT_AUDIO = 0x00000010,
            INIT_VIDEO = 0x00000020
        }

        /// <summary>
        /// For a normal Window you should use SDL_WINDOW_SHOWN
        /// </summary>
        public enum WindowMode
        {
            SDL_WINDOW_FULLSCREEN = 0x00000001,
            SDL_WINDOW_OPENGL = 0x00000002,
            SDL_WINDOW_SHOWN = 0x00000004,
            SDL_WINDOW_HIDDEN = 0x00000008,
            SDL_WINDOW_BORDERLESS = 0x00000010,
            SDL_WINDOW_RESIZABLE = 0x00000020,
            SDL_WINDOW_MINIMIZED = 0x00000040,
            SDL_WINDOW_MAXIMIZED = 0x00000080,
            SDL_WINDOW_INPUT_GRABBED = 0x00000100,
            SDL_WINDOW_INPUT_FOCUS = 0x00000200,
            SDL_WINDOW_MOUSE_FOCUS = 0x00000400,
            SDL_WINDOW_FULLSCREEN_DESKTOP =
            (SDL_WINDOW_FULLSCREEN | 0x00001000),
            SDL_WINDOW_FOREIGN = 0x00000800,
            SDL_WINDOW_ALLOW_HIGHDPI = 0x00002000
        }

        /// <summary>
        /// Mode of rendering
        /// </summary>
        public enum RenderMode
        {
            SDL_RENDERER_SOFTWARE = 0x00000001,
            SDL_RENDERER_ACCELERATED = 0x00000002,
            SDL_RENDERER_PRESENTVSYNC = 0x00000004,
            SDL_RENDERER_TARGETTEXTURE = 0x00000008
        }

        internal enum ImageMode
        {
            IMG_INIT_JPG = 0x00000001,
            IMG_INIT_PNG = 0x00000002,
            IMG_INIT_TIF = 0x00000004,
            IMG_INIT_WEBP = 0x00000008
        }

        internal enum MIX_InitFlags
        {
            MIX_INIT_FLAC = 0x00000001,
            MIX_INIT_MOD = 0x00000002,
            MIX_INIT_MP3 = 0x00000004,
            MIX_INIT_OGG = 0x00000008,
            MIX_INIT_FLUIDSYNTH = 0x00000010,
            MIX_INIT_ALL = MIX_INIT_FLAC | MIX_INIT_FLUIDSYNTH | MIX_INIT_MOD | MIX_INIT_MP3 | MIX_INIT_OGG
        }

        public const ushort DEFAULT_AUDIO_FORMAT = 0x8010;


        #endregion

        #region NATIVE CALLS

        [DllImport(NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_Init(uint flags);

        [DllImport(NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        [return : MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler), MarshalCookie = LPUtf8StrMarshaler.LeaveAllocated)]
        private static extern string SDL_GetError();

        [DllImport(NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateWindow([In()] [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler))] string title, int x, int y, int w, int h, uint flags);

        [DllImport(NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateRenderer(IntPtr window, int index, uint flags);

        [DllImport(NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_DestroyWindow(IntPtr window);

        [DllImport(NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_DestroyRenderer(IntPtr renderer);

        [DllImport(NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_Quit();

        [DllImport(NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_Delay(UInt32 ms);

        [DllImport(NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern UInt32 SDL_GetTicks();

        [DllImport(IMGLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int IMG_Init(uint flags);

        [DllImport(TTFLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TTF_Init();

        [DllImport(MIXLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_OpenAudio(int frequency, UInt16 format, int channels, int chunksize);

        [DllImport(MIXLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_Init(uint flags);
        #endregion

        #region SIMPLIFIED CALLS
        private static void Initialize(InitializeModule init)
        {
            if (SDL_Init((uint)init) < 0)
            {
                throw new SDLException("Couldn't initialize CSSDL\r\n" + SDL_GetError());
            }
        }

        /// <summary>
        /// Initializes the SDL engine. Should be called before using anything related to SDL.
        /// </summary>
        public static void InitializeAll()
        {
            Initialize(InitializeModule.INIT_VIDEO | InitializeModule.INIT_AUDIO);
            IMG_Init((uint)ImageMode.IMG_INIT_PNG);
            TTF_Init();
            Console.WriteLine(Mix_Init((uint)MIX_InitFlags.MIX_INIT_ALL));
            Mix_OpenAudio(44100, DEFAULT_AUDIO_FORMAT, 2, 2048);
            Running = true;
        }

        /// <summary>
        /// Creates a Window on the specified position with the specified flags
        /// </summary>
        /// <param name="title"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="modes"></param>
        /// <returns></returns>
        public static Window CreateWindow(string title, int x, int y, int width, int height, params WindowMode[] modes)
        {
            uint flags = 0;
            foreach (WindowMode mode in modes)
            {
                flags |= (uint)mode;
            }
            IntPtr windowPtr = SDL_CreateWindow(title, x, y, width, height, flags);
            if (windowPtr == IntPtr.Zero) throw new SDLException("Couldn't create window\r\n" + SDL_GetError());
            Window window = new Window(windowPtr);
            windows.Add(window);
            return window;
        }

        /// <summary>
        /// Creates a Window with the specified flags
        /// </summary>
        /// <param name="title"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="modes"></param>
        /// <returns></returns>
        public static Window CreateWindow(string title, int width, int height, params WindowMode[] modes) {
            int x = 0x1FFF0000;
            int y = 0x1FFF0000;
            return CreateWindow(title, x, y, width, height, modes);
        }

        /// <summary>
        /// Creates a Renderer to draw in the specified window
        /// </summary>
        /// <param name="window"></param>
        /// <param name="index"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static Renderer CreateRenderer(Window window, int index, RenderMode mode)
        {
            IntPtr rendererPtr = SDL_CreateRenderer(window.windowPtr, index, (uint)mode);
            if (rendererPtr == IntPtr.Zero)
            {
                throw new SDLException("Couldn't start renderer\r\n" + SDL_GetError());
            }
            Renderer renderer = new Renderer(rendererPtr);
            renderers.Add(renderer);
            return renderer;
        }

        /// <summary>
        /// Stops all SDL functionality
        /// </summary>
        public static void Quit()
        {
            foreach (Renderer renderer in renderers) {
                SDL_DestroyRenderer(renderer.rendererPtr);
            }
            renderers.Clear();
            foreach (Window window in windows) {
                SDL_DestroyWindow(window.windowPtr);
            }
            windows.Clear();
            // TODO: Stop IMG
            SDL_Quit();

            Running = false;
        }

        /// <summary>
        /// Delays the code by a certain amount
        /// </summary>
        /// <param name="ms"></param>
        public static void Delay(UInt32 ms)
        {
            SDL_Delay(ms);
        }

        /// <summary>
        /// Gets the number of milliseconds elapsed since SDL initialization
        /// </summary>
        /// <returns></returns>
        public static UInt32 GetTicks()
        {
            return SDL_GetTicks();
        }
        #endregion
    }
}
