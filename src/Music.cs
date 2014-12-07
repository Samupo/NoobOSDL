using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NoobOSDL
{
    /// <summary>
    /// This class manages background music. Only one music can be played at a time.
    /// </summary>
    public class Music
    {
        internal IntPtr musicPtr { get; private set; }
        /// <summary>
        /// Determines if any music is currently being played.
        /// </summary>
        public static bool Playing { get; private set; }
        /// <summary>
        /// Gets or sets the volume of the music.
        /// </summary>
        public static int Volume
        {
            get
            {
                return Mix_VolumeMusic(-1);
            }
            set
            {
                if (value >= 0) Mix_VolumeMusic(value);
            }
        }

        internal Music(IntPtr music)
        {
            musicPtr = music;
        }

        /// <summary>
        /// Plays this music.
        /// </summary>
        /// <param name="loops">Loops (-1 for unlimited)</param>
        public void Play(int loops = -1)
        {
            Mix_PlayMusic(musicPtr, loops);
            Playing = true;
        }

        /// <summary>
        /// Fades in this music while fading out previous one if any.
        /// </summary>
        /// <param name="fadeTime">Fade time in ms</param>
        /// <param name="loops">Loops (-1 for unlimited)</param>
        /// <param name="startPos">Staring position of the music</param>
        public void PlayFadeIn(int fadeTime, int loops = -1, double startPos = 0)
        {
            if (startPos == 0)
            {
                Mix_FadeInMusic(musicPtr, loops, fadeTime);
            }
            else
            {
                Mix_FadeOutMusic(fadeTime / 2);
                Mix_FadeInMusicPos(musicPtr, loops, fadeTime/2, startPos);
            }
            Playing = true;
        }

        /// <summary>
        /// Pauses the music.
        /// </summary>
        public static void Pause()
        {
            Mix_PauseMusic();
            Playing = false;
        }

        /// <summary>
        /// Resumes the music.
        /// </summary>
        public static void Resume()
        {
            Mix_ResumeMusic();
            Playing = true;
        }

        /// <summary>
        /// Returns the music to its start.
        /// </summary>
        public static void Rewind()
        {
            Mix_RewindMusic();
        }

        /// <summary>
        /// Moves the current music.
        /// </summary>
        /// <param name="amount">Amount to move</param>
        public static void MoveMusicPosition(double amount)
        {
            Mix_SetMusicPosition(amount);
        }

        /// <summary>
        /// Stops the music being played.
        /// </summary>
        public static void Stop()
        {
            Mix_HaltMusic();
            Playing = false;
        }

        ~Music()
        {
            Mix_FreeMusic(musicPtr);
        }

        [DllImport(SDL.MIXLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_PlayMusic(IntPtr music, int loops);

        [DllImport(SDL.MIXLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_FadeInMusic(IntPtr music, int loops, int ms);

        [DllImport(SDL.MIXLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_FadeInMusicPos(IntPtr music, int loops, int ms, double position);

        [DllImport(SDL.MIXLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_FadeOutMusic(int ms);

        [DllImport(SDL.MIXLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_VolumeMusic(int volume);

        [DllImport(SDL.MIXLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Mix_PauseMusic();

        [DllImport(SDL.MIXLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Mix_ResumeMusic();

        [DllImport(SDL.MIXLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Mix_RewindMusic();

        [DllImport(SDL.MIXLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_HaltMusic();

        [DllImport(SDL.MIXLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern int Mix_SetMusicPosition(double position);

        [DllImport(SDL.MIXLIB, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Mix_FreeMusic(IntPtr music);
    }
}
