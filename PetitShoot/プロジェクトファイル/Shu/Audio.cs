using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Shu
{
    /// <summary>
    /// サウンドとミュージックの再生。
    /// </summary>
    public class Audio
    {
        SoundEffect music1;
        SoundEffect music2;
        SoundEffect music3;
        SoundEffect music4;
        SoundEffect music5;
        SoundEffect music6;
        SoundEffect music7;

        SoundEffectInstance SEI;

        const int Soundnum = 9;

        //		SoundEffectInstance[] sei = new SoundEffectInstance[Soundnum];
        SoundEffect[] sound = new SoundEffect[Soundnum];
        string[] Sounddata =
        {
            "button02a",
            "laser2",
            "shoot1",
            "short_bomb",
            "explosion1",
            "explosion2",
            "destruction1",
            "misairubakuhatu(ボス爆発)",
            "damage3(連続爆発)"
        };

        public enum SN
        {
            button02a,
            laser2,
            shoot1,
            short_bomb,
            explosion1,
            explosion2,
            destruction1,
            misairubakuhatu,
            damage3
        };



        /// <summary>
        /// コンストラクタ。メディアの読み出し。
        /// </summary>
        /// <param name="g"></param>
        public Audio(Game g)
        {
            //gr = g;
            music1 = g.Content.Load<SoundEffect>("4_Space(Stage1)");
            music2 = g.Content.Load<SoundEffect>("3_Meka(Stage2)");
            music3 = g.Content.Load<SoundEffect>("5_Gosulori(Stage3)");
            music4 = g.Content.Load<SoundEffect>("2_Sea(Stage4)");
            music5 = g.Content.Load<SoundEffect>("6_Mystery(Stage5)");
            music6 = g.Content.Load<SoundEffect>("ファンファーレは誰のために？");
            music7 = g.Content.Load<SoundEffect>("bgm_maoudamashii_acoustic52");


            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.1f;
            SoundEffect.MasterVolume = 0.2f;


            for (int i = 0; i < Soundnum; i++)
            {
                sound[i] = g.Content.Load<SoundEffect>(Sounddata[i]);
                //sei[i] = sound[i].CreateInstance();
                //sei[i].Volume = 0.6f;
            }

        }

        /// <summary>
        /// メインテーマ再生。
        /// </summary>
        public void MusicPlay()
        {
            if (SEI != null) SEI.Stop();
            SEI.IsLooped = true; //ループする場合はtrue
            SEI = music1.CreateInstance();
            SEI.Play();
        }
        /// <summary>
        /// ステージごとに再生
        /// </summary>
        public void MusicPlay(int st)
        {
            if (SEI != null) SEI.Stop();
            var v = music1;
            switch (st)
            {
                case 1:
                    break;
                case 2:
                    v = music2;
                    break;
                case 3:
                    v = music3;
                    break;
                case 4:
                    v = music4;
                    break;
                case 5:
                    v = music5;
                    break;
            }
            SEI = v.CreateInstance();
            SEI.IsLooped = true; //ループする場合はtrue
            SEI.Play();
        }
        /// <summary>
        /// GameOver再生。
        /// </summary>
        public void MusicPlay2()
        {
            if (SEI != null) SEI.Stop();
            SEI = music2.CreateInstance();
            SEI.IsLooped = false; //ループする場合はtrue
            SEI.Play();
        }
        /// <summary>
        /// ステージクリア再生。
        /// </summary>
        public void MusicPlay3()
        {
            if (SEI != null) SEI.Stop();
            SEI = music6.CreateInstance();
            SEI.IsLooped = false; //ループする場合はtrue
            SEI.Play();
        }
        /// <summary>
        /// エンディング再生。
        /// </summary>
        public void MusicPlay4()
        {
            if (SEI != null) SEI.Stop();
            SEI = music7.CreateInstance();
            SEI.IsLooped = false; //ループする場合はtrue
            SEI.Play();
        }
        /// <summary>
        /// Musicを全て停止。
        /// </summary>
        public void MusicStop()
        {
            SEI.Stop();
        }

        /// <summary>
        /// 再生するサウンドファイルを指定して再生。
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool SoundPlay(SN num)
        {
            ///*if (sei[(int)num].State == SoundState.Stopped)*/ sei[(int)num].Play();
            sound[(int)num].Play();
            //Console.WriteLine(sei[(int)num].State);
            return true;
        }

        /// <summary>
        /// すべてのサウンドをストップ
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public bool SoundStop(SN num)
        {
            //sei[(int)num].Stop();

            //Console.WriteLine(sei[(int)num].State);
            return true;
        }
    }
}
