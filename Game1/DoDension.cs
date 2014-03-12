using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using Game1.Model;

namespace Game1
{
    public class DoDension
    {
        public static SoundEffectInstance sei;
        public static SoundEffectInstance mainBGM;


        public static void playBackGroundMusic(string sourceName)
        {
            if (!Config.IsMusicOpen)
            {
                return;
            }
            if (mainBGM != null)
            {
                mainBGM.Stop();
            }

            string path = "Assets/Sounds/" + sourceName;
            using (var stream = TitleContainer.OpenStream(path))
            {
                var effect = SoundEffect.FromStream(stream);
                mainBGM = effect.CreateInstance();
                mainBGM.IsLooped = true;
                FrameworkDispatcher.Update();
                mainBGM.Volume = (float)0.5;
                mainBGM.Play();
            }
        }

        public static void playSound(string sourceName, bool isLoop)
        {
            if (!Config.IsSoundOpen)
            {
                return;
            }
            if (sei != null)
            {
                sei.Stop();
            }

            string path = "Assets/Sounds/" + sourceName;
            using (var stream = TitleContainer.OpenStream(path))
            {
                var effect = SoundEffect.FromStream(stream);
                sei = effect.CreateInstance();
                sei.IsLooped = isLoop;
                FrameworkDispatcher.Update();
                sei.Volume = 1;
                sei.Play();
            }
        }

        public static void stopDension()
        {
            if (sei != null)
            {
                sei.Stop();
            }
            if (mainBGM != null)
            {
                mainBGM.Stop();
            }
        }

        public static void pauseDension()
        {
            if (sei != null)
            {
                sei.Pause();
            }
            if (mainBGM != null)
            {
                mainBGM.Pause();
            }
        }

        public static void resumeDension()
        {
            
            if (sei != null)
            {
                if (Config.IsSoundOpen && sei.State == SoundState.Paused)
                    sei.Resume();
            }
            if (mainBGM != null)
            {
                if (Config.IsMusicOpen && mainBGM.State == SoundState.Paused)
                    mainBGM.Resume();
            }
        }
    }
}
