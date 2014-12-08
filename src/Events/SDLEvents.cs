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
using NoobOSDL.Events.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NoobOSDL.Events
{
    public class KeyboardEventArgs : EventArgs
    {
        public enum Mode
        {
            PRESSED,
            RELEASED
        }
        public SDL_Keycode KeyCode { get; private set; }
        public SDL_Keymod KeyMod { get; private set; }
        public Mode PressMode { get; private set; }
        public bool Repeat { get; private set; }

        public KeyboardEventArgs(Mode mode, SDL_Keycode keycode, SDL_Keymod keymod, bool repeat)
        {
            this.PressMode = mode;
            this.KeyCode = keycode;
            this.KeyMod = KeyMod;
            this.Repeat = repeat;
        }
    }

    public class MouseMotionEventArgs : EventArgs
    {

        public int X { get; private set; }
        public int Y { get; private set; }
        public uint WindowID { get; private set; }
        public uint Which { get; private set; }

        public MouseMotionEventArgs(int x, int y, uint windowID, uint which)
        {
            this.X = x;
            this.Y = y;
            this.WindowID = windowID;
            this.Which = which;
        }
    }

    public class MouseButtonEventArgs : EventArgs
    {
        public enum Mode
        {
            PRESSED,
            RELEASED
        }
        public byte Button { get; private set; }
        public Mode PressMode { get; private set; }
        public int Clicks { get; private set; }
        public uint WindowID { get; private set; }
        public uint Which { get; private set; }

        public MouseButtonEventArgs(Mode mode, byte button, uint windowID, uint which, int clicks)
        {
            this.PressMode = mode;
            this.Button = button;
            this.WindowID = windowID;
            this.Which = which;
        }
    }

    public class QuitEventArgs : EventArgs
    {
        public uint Timestamp { get; private set; }

        public QuitEventArgs(uint timestamp)
        {
            this.Timestamp = timestamp;
        }
    }

    public abstract class SDLEvents
    {
        public delegate void KeyboardEventHandler(object sender, KeyboardEventArgs e);
        public delegate void MouseMovementHandler(object sender, MouseMotionEventArgs e);
        public delegate void MouseButtonHandler(object sender, MouseButtonEventArgs e);
        public delegate void QuitHandler(object sender, QuitEventArgs e);

        public static event KeyboardEventHandler Keyboard;
        public static event MouseButtonHandler MouseButton;
        public static event MouseMovementHandler MouseMovement;
        public static event QuitHandler Quit;

        public static List<SDL_Event> PollAllEvents()
        {
            List<SDL_Event> list = new List<SDL_Event>();
            SDL_Event e;
            while (SDL_PollEvent(out e) != 0)
            {
                list.Add(e);
                if (e.type == SDL_EventType.SDL_KEYDOWN)
                {
                    if (Keyboard != null)
                        Keyboard(null, new KeyboardEventArgs(KeyboardEventArgs.Mode.PRESSED, e.key.keysym.sym, e.key.keysym.mod, e.key.repeat > 0));
                }
                else if (e.type == SDL_EventType.SDL_KEYUP)
                {
                    if (Keyboard != null)
                        Keyboard(null, new KeyboardEventArgs(KeyboardEventArgs.Mode.RELEASED, e.key.keysym.sym, e.key.keysym.mod, e.key.repeat > 0));
                }
                else if (e.type == SDL_EventType.SDL_MOUSEMOTION)
                {
                    if (MouseMovement != null)
                        MouseMovement(null, new MouseMotionEventArgs(e.motion.x, e.motion.y, e.motion.windowID, e.motion.which));
                }
                else if (e.type == SDL_EventType.SDL_MOUSEBUTTONUP)
                {
                    if (MouseButton != null)
                        MouseButton(null, new MouseButtonEventArgs(MouseButtonEventArgs.Mode.RELEASED, e.button.button, e.button.windowID, e.button.which, e.button.clicks));
                }
                else if (e.type == SDL_EventType.SDL_MOUSEBUTTONDOWN)
                {
                    if (MouseButton != null)
                        MouseButton(null, new MouseButtonEventArgs(MouseButtonEventArgs.Mode.PRESSED, e.button.button, e.button.windowID, e.button.which, e.button.clicks));
                }
                else if (e.type == SDL_EventType.SDL_QUIT)
                {
                    if (Quit != null)
                        Quit(null, new QuitEventArgs(e.quit.timestamp));
                }
            }
            return list;
        }

        [DllImport(SDL.NATIVELIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SDL_PollEvent(out SDL_Event _event);
    }
}
