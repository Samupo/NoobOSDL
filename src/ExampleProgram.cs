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
using NoobOSDL;
using NoobOSDL.Events.Native;
using NoobOSDL.Events;

namespace TestProject
{
    class ExampleProgram
    {
        static void Main(string[] args)
        {
            // INITIALIZATION
            // SDL initializaton
            SDL.InitializeAll();
            Window window = SDL.CreateWindow("Title", 640, 480, SDL.WindowMode.SDL_WINDOW_SHOWN);
            Renderer renderer = SDL.CreateRenderer(window, -1, SDL.RenderMode.SDL_RENDERER_ACCELERATED);
            renderer.DrawColor = SDLColor.BLACK;

            // Event initialization
            SDLEvents.MouseMovement += SDLEvents_MouseMovement;
            SDLEvents.Quit += SDLEvents_Quit;

            // Play music
            Music music = ResourceManager.LoadMusic("music.mp3");
            music.PlayFadeIn(10000);

            // Play audio
            Audio audio = ResourceManager.LoadAudio("audio.ogg");
            audio.Play();

            // Texture loading
            Texture texture = ResourceManager.LoadTexture("myimg.png");
            Texture texture2 = ResourceManager.LoadTexture("myimg2.png");

            // Texture setup
            texture.BlendMode = Texture.BlendModeEnum.ALPHA_BLEND;
            texture.Opacity = 128;

            // X position and rotation where we will draw texture
            int x = -60;

            // We use this three variables to calculate how much time we have to delay each cycle
            uint desiredDelay = 1000 / 120;
            long start; // Number of ms at the start of each cycle of the loop
            uint delay; // Delay after each cycle of the loop

            // MAIN LOOP
            while (SDL.Running)
            {
                start = SDL.GetTicks();

                // Parse all events if needed
                List<SDL_Event> eventList = SDLEvents.PollAllEvents();
                // Breaks the loop if SDL has stopped
                if (!SDL.Running) break;

                // Update game logic
                x += 2;
                if (x > window.Width) x = -60;

                // Start of drawing
                renderer.RenderClear();

                // The draw order is important
                TextParameters tp = new TextParameters();
                tp.Color = new SDLColor(255, 255, 255, 255);
                renderer.DrawTexture(texture2);
                renderer.DrawText("Hello World!", "consola.ttf", tp);
                renderer.DrawTexture(texture, new Rect(0, 0, texture.Width, texture.Height), new Rect(x, 80, 60, 60), x, RendererFlip.NONE);

                renderer.RenderPresent();
                // End of drawing

                // Optional call to garbage collector
                GC.Collect();

                // Delay next cycle if needed to maintain desired FPS rate
                delay = (uint)(SDL.GetTicks() - start);
                if (delay < desiredDelay)
                {
                    SDL.Delay(desiredDelay - delay);
                }
            }
        }

        static void SDLEvents_Quit(object sender, QuitEventArgs e)
        {
            SDL.Quit();
        }

        static void SDLEvents_MouseMovement(object sender, MouseMotionEventArgs e)
        {
            Console.WriteLine(e.X + " " + e.Y);
        }
    }
}
