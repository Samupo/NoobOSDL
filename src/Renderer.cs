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
    #region UTILS
    /// <summary>
    /// Flip mode
    /// </summary>
    public enum RendererFlip
    {
        /// <summary>
        /// Not flipped
        /// </summary>
        NONE = 0x00000000,
        /// <summary>
        /// Flipped horizontally
        /// </summary>
        FLIP_HORIZONTAL = 0x00000001,
        /// <summary>
        /// Flipped vertically
        /// </summary>
        FLIP_VERTICAL = 0x00000002,
        /// <summary>
        /// Flipped both horizontally and vertically
        /// </summary>
        FLIP_BOTH = FLIP_HORIZONTAL | FLIP_VERTICAL
    }

    /// <summary>
    /// Parameters that are needed to draw the text.
    /// </summary>
    public class TextParameters
    {
        /// <summary>
        /// Font size
        /// </summary>
        public int Size = 32;
        /// <summary>
        /// Font color
        /// </summary>
        public SDLColor Color = SDLColor.BLACK;
        /// <summary>
        /// Determines how the text will be drawn in the rectangle
        /// </summary>
        public enum TextFillType
        {
            /// <summary>
            /// Ignores the rectangle's width and height, only cares for the position. Current Rect will be overwritten to the new one.
            /// </summary>
            TFT_NO_FIT,
            /// <summary>
            /// Tries to make the text fit the designed Rect. It's very slow and resource consuming, avoid if possible.
            /// </summary>
            TFT_BEST_FIT,
            /// <summary>
            /// Scales the text to the rectangle.
            /// </summary>
            TFT_SCALED_FIT,
            /// <summary>
            /// Scales the text to the rectangle but without distorting it.
            /// </summary>
            TFT_KEEP_ASPECT_SCALED_FIT
        };
        /// <summary>
        /// Determines how the text will be drawn in the rectangle
        /// </summary>
        public TextFillType TFT = TextFillType.TFT_NO_FIT;
        /// <summary>
        /// Rectangle where the text is going to be drawn
        /// </summary>
        public Rect Rect = new Rect(0, 0, 320, 36);
    }
    #endregion

    /// <summary>
    /// A Renderer that draws in a Window.
    /// </summary>
    public class Renderer
    {
        internal IntPtr rendererPtr { get; private set; }
        private SDLColor drawColor;

        /// <summary>
        /// Clear color of the Renderer
        /// </summary>
        public SDLColor DrawColor
        {
            get { return drawColor; }
            set
            {
                SetDrawColor(value);
            }
        }

        #region RENDERER
        internal Renderer(IntPtr rendererPtr)
        {
            this.rendererPtr = rendererPtr;
            SetDrawColor(SDLColor.BLACK);
        }

        private void SetDrawColor(SDLColor color)
        {
            drawColor = color;
            SDL_SetRenderDrawColor(rendererPtr, color.R, color.G, color.B, color.A);
        }

        /// <summary>
        /// Destroys the renderer
        /// </summary>
        public void Destroy()
        {
            SDL.DoDestroyRenderer(this);
        }

        /// <summary>
        /// Clears the screen filling it with the DrawColor
        /// </summary>
        public void RenderClear()
        {
            SDL_RenderClear(rendererPtr);
        }

        /// <summary>
        /// Renders the screen to the assigned Window
        /// </summary>
        public void RenderPresent()
        {
            SDL_RenderPresent(rendererPtr);
        }
        #endregion

        #region TEXTURES
        /// <summary>
        /// Draws a texture
        /// </summary>
        /// <param name="texture"></param>
        public void DrawTexture(Texture texture)
        {
            SDL_RenderCopy(this.rendererPtr, texture.texturePtr, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// Draws a texture in the specified position
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void DrawTexture(Texture texture, int x, int y)
        {
            SDL_Rect rect = new SDL_Rect();
            rect.x = x;
            rect.y = y;
            rect.w = texture.Width;
            rect.h = texture.Height;
            SDL_RenderCopy(this.rendererPtr, texture.texturePtr, IntPtr.Zero, ref rect);
        }

        /// <summary>
        /// Draws a texture scaled in the specified destination
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="destination"></param>
        public void DrawTexture(Texture texture, Rect destination)
        {
            SDL_RenderCopy(this.rendererPtr, texture.texturePtr, IntPtr.Zero, ref destination.rect);
        }

        /// <summary>
        /// Draws a part of the texture in the specified destination
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public void DrawTexture(Texture texture, Rect source, Rect destination)
        {
            SDL_RenderCopy(this.rendererPtr, texture.texturePtr, ref source.rect, ref destination.rect);
        }

        /// <summary>
        /// Draws a rotated and/or flipped part of a texture in the specified destination
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="angle"></param>
        /// <param name="flip"></param>
        public void DrawTexture(Texture texture, Rect source, Rect destination, double angle, RendererFlip flip)
        {
            SDL_RenderCopyEx(this.rendererPtr, texture.texturePtr, ref source.rect, ref destination.rect, angle, IntPtr.Zero, (uint)flip);
        }
        #endregion

        #region TEXT
        /// <summary>
        /// Draws a text on the screen
        /// </summary>
        /// <param name="text">String to draw</param>
        /// <param name="fontPath">Relative path to the font</param>
        /// <param name="textParams">Text params, cannot be null</param>
        public void DrawText(string text, string fontPath, TextParameters textParams)
        {
            using (Texture texture = CreateTextTexture(text, fontPath, textParams))
            {
                DrawTexture(texture, textParams.Rect);
            }

        }

        private Texture CreateTextTexture(string text, string fontPath, TextParameters textParams)
        {
            Texture texture;
            switch (textParams.TFT)
            {

                case TextParameters.TextFillType.TFT_NO_FIT:
                    {
                        texture = CreateTextTextureNoFit(text, fontPath, textParams);
                        textParams.Rect.Height = texture.Height;
                        textParams.Rect.Width = texture.Width;
                        return texture;
                    }

                case TextParameters.TextFillType.TFT_SCALED_FIT:
                    {
                        texture = CreateTextTextureNoFit(text, fontPath, textParams);
                        return texture;
                    }

                case TextParameters.TextFillType.TFT_KEEP_ASPECT_SCALED_FIT:
                    {
                        texture = CreateTextTextureNoFit(text, fontPath, textParams);
                        float wvar = (float)textParams.Rect.Width / (float)texture.Width;
                        float hvar = (float)textParams.Rect.Height / (float)texture.Height;
                        float w = texture.Width;
                        float h = texture.Height;

                        if (wvar < hvar)
                        {
                            w *= wvar;
                            h *= wvar;
                        }
                        else
                        {
                            w *= hvar;
                            h *= hvar;
                        }

                        textParams.Rect.Width = (int)w;
                        textParams.Rect.Height = (int)h;

                        return texture;
                    }

                case TextParameters.TextFillType.TFT_BEST_FIT:
                    {
                        texture = CreateTextTextureBestFit(text, fontPath, textParams);
                        return texture;
                    }
            }
            return null;
        }

        private IntPtr CreateTextSurface(string text, string fontPath, TextParameters textParams)
        {
            IntPtr textSurface = IntPtr.Zero;
            // Open the font
            IntPtr gFont = TTF_OpenFont(fontPath, textParams.Size);
            if (gFont == IntPtr.Zero)
            {
                throw new SDLException("Failed to load font.\r\n" + TTF_GetError());
            }
            else
            { //Render text
                //Render text surface
                textSurface = TTF_RenderText_Blended(gFont, text, textParams.Color.ToNativeColor());
                if (textSurface == IntPtr.Zero)
                {
                    throw new SDLException("Unable to render text surface.\r\n" + TTF_GetError());
                }
            }
            TTF_CloseFont(gFont);

            return textSurface;

        }

        private Texture CreateTextTextureNoFit(string text, string fontPath, TextParameters textParams)
        {
            IntPtr surface = CreateTextSurface(text, fontPath, textParams);

            IntPtr sdlTexture = SDL_CreateTextureFromSurface(this.rendererPtr, surface);
            if (sdlTexture == IntPtr.Zero)
            {
                throw new SDLException("Unable to create texture from rendered text." + SDL_GetError());
            }

            //Get image dimensions
            SDL_Surface sdl_surface = SDL_Surface.FromIntPtr(surface);
            Texture texture = new Texture(sdlTexture, sdl_surface.w, sdl_surface.h);
            //Get rid of old surface
            SDL_FreeSurface(surface);
            return texture;
        }

        private unsafe Texture CreateTextTextureBestFit(string text, string fontPath, TextParameters textParams)
        {
            String[] words = text.Split(' ');
            IntPtr[] textures = new IntPtr[words.GetLength(0)];

            // Get raw word textures
            for (int i = 0; i < words.GetLength(0); i++)
            {
                textures[i] = CreateTextSurface(words[i] + " ", fontPath, textParams);
            }

#if SDL_BYTEORDER == SDL_BIG_ENDIAN
            UInt32 rmask = 0xff000000;
            UInt32 gmask = 0x00ff0000;
            UInt32 bmask = 0x0000ff00;
            UInt32 amask = 0x000000ff;
#else
	        UInt32 rmask = 0x000000ff;
	        UInt32 gmask = 0x0000ff00;
	        UInt32 bmask = 0x00ff0000;
	        UInt32 amask = 0xff000000;
#endif

            // Create surface
            IntPtr textSurface = IntPtr.Zero;
            // Get line height
            int height = 0;
            for (int i = 0; i < words.GetLength(0); i++)
            {
                SDL_Surface sdl_surface = SDL_Surface.FromIntPtr(textures[i]);
                if (sdl_surface.h > height) height = sdl_surface.h;
            }

            bool done = false;
            float size = 1;
            while (!done)
            {
                textSurface = SDL_CreateRGBSurface(0, (int)((float)textParams.Rect.Width * size), (int)((float)textParams.Rect.Height * size), 32, rmask, gmask, bmask, amask);
                int accWidth = 0;
                int line = 0;
                for (int i = 0; i < words.GetLength(0); i++)
                {
                    SDL_Surface sdl_surface = SDL_Surface.FromIntPtr(textures[i]);
                    if (accWidth + sdl_surface.w > textParams.Rect.Width)
                    {
                        line++;
                        accWidth = 0;
                    }
                    SDL_Rect r;
                    r.x = accWidth;
                    r.y = line * height;
                    r.w = sdl_surface.w;
                    r.h = sdl_surface.h;
                    SDL_BlitSurface(textures[i], ref sdl_surface.clip_rect, textSurface, ref r);
                    accWidth += sdl_surface.w;
                }

                if (line * height + height < textParams.Rect.Height) done = true;
                if (!done)
                {
                    size += 0.1f;
                }
            }

            // Free textures
            for (int i = 0; i < words.GetLength(0); i++)
            {
                SDL_FreeSurface(textures[i]);
            }

            IntPtr sdlTexture = SDL_CreateTextureFromSurface(rendererPtr, textSurface);
            if (sdlTexture == IntPtr.Zero)
            {
                throw new SDLException("Unable to create texture from rendered text.\r\n" + SDL_GetError());
            }

            //Get image dimensions
            Texture texture = new Texture(sdlTexture, textParams.Rect.Width, textParams.Rect.Height);
            //Get rid of old surface
            SDL_FreeSurface(textSurface);
            return texture;
        }

        #endregion

        #region NATIVE
        [DllImport(SDL.TTFLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr TTF_OpenFont([In()] [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler))] string file, int size);

        [DllImport(SDL.TTFLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern void TTF_CloseFont(IntPtr font);

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler), MarshalCookie = LPUtf8StrMarshaler.LeaveAllocated)]
        private static extern string SDL_GetError();

        [DllImport(SDL.TTFLIB, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler), MarshalCookie = LPUtf8StrMarshaler.LeaveAllocated)]
        private static extern string TTF_GetError();

        [DllImport(SDL.TTFLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr TTF_RenderText_Blended(IntPtr font, [In()] [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(LPUtf8StrMarshaler))] string text, SDL_Color color);

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateTextureFromSurface(IntPtr renderer, IntPtr surface);

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateTextureFromSurface(IntPtr renderer, SDL_Surface surface);

        [DllImport(SDL.NATIVELIB, EntryPoint = "SDL_UpperBlit", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_BlitSurface(IntPtr src, ref SDL_Rect srcrect, IntPtr dst, ref SDL_Rect dstrect);

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr SDL_CreateRGBSurface(UInt32 flags, int width, int height, int depth, UInt32 Rmask, UInt32 Gmask, UInt32 Bmask, UInt32 Amask);

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_FreeSurface(IntPtr surface);

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_FreeSurface(SDL_Surface surface);

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_SetRenderDrawColor(IntPtr renderer, byte r, byte g, byte b, byte a);

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderClear(IntPtr renderer);

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderPresent(IntPtr renderer);


        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopy(IntPtr renderer, IntPtr texture, IntPtr srcrect, IntPtr dstrect);

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopy(IntPtr renderer, IntPtr texture, IntPtr srcrect, ref SDL_Rect dstrect);

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopy(IntPtr renderer, IntPtr texture, ref SDL_Rect srcrect, ref SDL_Rect dstrect);

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_RenderCopyEx(IntPtr renderer, IntPtr texture, ref SDL_Rect srcrect, ref SDL_Rect dstrect, double angle, IntPtr center, uint flip);
        #endregion
    }
}
