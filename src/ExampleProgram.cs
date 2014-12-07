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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoobOSDL;
using NoobOSDL.Events.Native;
using NoobOSDL.Events;

namespace TestProject
{
    class ExampleProgram
    {
        static void Main(string[] args)
        {
            //CSSDL.Initialize(CSSDL.InitializeModule.INIT_VIDEO);
            SDL.InitializeAll();
            Window window = SDL.CreateWindow("Title",640, 480, SDL.WindowMode.SDL_WINDOW_SHOWN);
            Renderer renderer = SDL.CreateRenderer(window, -1, SDL.RenderMode.SDL_RENDERER_ACCELERATED);
            renderer.DrawColor = SDLColor.BLACK;

            SDLEvents.MouseMovement += SDLEvents_MouseMovement;
            Music music =  ResourceManager.LoadMusic("music.mp3");
            music.PlayFadeIn(10000);

            Audio audio = ResourceManager.LoadAudio("audio.ogg");
            audio.Play();


            Texture texture = ResourceManager.LoadTexture("myimg.png");
            int x = -60;

            while (SDL.Running)
            {
                GC.Collect();

                List<SDL_Event> eventList = SDLEvents.PollAllEvents();

                x += 2;
                if (x > window.Width) x = -60;

                renderer.RenderClear();

                TextParameters tp = new TextParameters();
                tp.Color = new SDLColor(255,255,255,255);
                renderer.DrawText("Hello World!", "consola.ttf", tp);
                renderer.DrawTexture(texture, new Rect(0,0, texture.Width, texture.Height), new Rect(x, 80, 60, 60), x, RendererFlip.NONE);

                renderer.RenderPresent();
                SDL.Delay(8);
            }
            GC.Collect();
        }

        static void SDLEvents_MouseMovement(object sender, MouseMotionEventArgs e)
        {
            Console.WriteLine(e.X + " " + e.Y);
        }
    }
}
