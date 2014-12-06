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
    public enum RendererFlip
    {
        NONE = 0x00000000,
        FLIP_HORIZONTAL = 0x00000001,
        FLIP_VERTICAL = 0x00000002,
        FLIP_BOTH = FLIP_HORIZONTAL | FLIP_VERTICAL
    }

    public class TextParameters
    {
        public int Size = 32;
        public SDLColor Color = SDLColor.BLACK;
        public enum TextFillType
        {
            TFT_NO_FIT, TFT_BEST_FIT, TFT_SCALED_FIT, TFT_KEEP_ASPECT_SCALED_FIT
        };
        public TextFillType TFT = TextFillType.TFT_NO_FIT;
        public Rect Rect = new Rect(0, 0, 320, 36);
    }
    #endregion

    public class Renderer
    {
        internal IntPtr rendererPtr { get; private set; }
        private SDLColor drawColor;
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

        public void RenderClear()
        {
            SDL_RenderClear(rendererPtr);
        }

        public void RenderPresent()
        {
            SDL_RenderPresent(rendererPtr);
        }
        #endregion

        #region TEXTURES
        public void DrawTexture(Texture texture)
        {
            SDL_RenderCopy(this.rendererPtr, texture.texturePtr, IntPtr.Zero, IntPtr.Zero);
        }

        public void DrawTexture(Texture texture, int x, int y)
        {
            SDL_Rect rect = new SDL_Rect();
            rect.x = x;
            rect.y = y;
            rect.w = texture.Width;
            rect.h = texture.Height;
            SDL_RenderCopy(this.rendererPtr, texture.texturePtr, IntPtr.Zero, ref rect);
        }

        public void DrawTexture(Texture texture, Rect destination)
        {
            SDL_RenderCopy(this.rendererPtr, texture.texturePtr, IntPtr.Zero, ref destination.rect);
        }

        public void DrawTexture(Texture texture, Rect source, Rect destination)
        {
            SDL_RenderCopy(this.rendererPtr, texture.texturePtr, ref source.rect, ref destination.rect);
        }

        public void DrawTexture(Texture texture, Rect source, Rect destination, double angle, RendererFlip flip)
        {
            SDL_RenderCopyEx(this.rendererPtr, texture.texturePtr, ref source.rect, ref destination.rect, angle, IntPtr.Zero, (uint)flip);
        }
        #endregion

        #region TEXT
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

        private Texture CreateTextTextureBestFit(string text, string fontPath, TextParameters textParams)
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
            SDL_Surface textSurface = new SDL_Surface();
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
                textSurface = (SDL_Surface)Marshal.PtrToStructure(SDL_CreateRGBSurface(0, (int)((float)textParams.Rect.Width * size), (int)((float)textParams.Rect.Height * size), 32, rmask, gmask, bmask, amask), typeof(SDL_Surface));
                int accWidth = 0;
                int line = 0;
                for (int i = 0; i < words.GetLength(0); i++)
                {
                    SDL_Surface sdl_surface = SDL_Surface.FromIntPtr(textures[i]);
                    if (accWidth + sdl_surface.w > textSurface.w)
                    {
                        line++;
                        accWidth = 0;
                    }
                    SDL_Rect r;
                    r.x = accWidth;
                    r.y = line * height;
                    r.w = sdl_surface.w;
                    r.h = sdl_surface.h;
                    SDL_BlitSurface(textures[i], IntPtr.Zero, textSurface, ref r);
                    accWidth += sdl_surface.w;
                }

                if (line * height + height < textSurface.h) done = true;
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
            Texture texture = new Texture(sdlTexture, textSurface.w, textSurface.h);
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

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SDL_BlitSurface(IntPtr surface, IntPtr srcrect, SDL_Surface dest, ref SDL_Rect destrect);

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
