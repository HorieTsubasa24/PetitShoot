using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX.DirectInput;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;



namespace Shu
{
    /// <summary>
    /// キャラクタ情報を格納、コンストラクタで引数を変数にセット
    /// </summary>
    public abstract class Chara
    {
        protected const float Width = 800.0f;
        protected const float Height = 800.0f;
        protected const float Fieldl = 200.0f;
        protected const float Fieldr = 600.0f;


        /// <summary>
        /// キャラクタの行動状態。Charaクラス全体の返り値として使われ、Updateで処理される。
        /// Chara.stateの状態変数としても使う。
        /// </summary>
        public enum Cv
        {
            nul, outview, Boutview, boss_outview, z_bomb, en1_bomb, en2_bomb, en3_bomb, en12_bomb,
            engenerate, engenelatetwo, engenelatefinal, engenelatethree, engenelatefour,
            super_dan1, hyper_dan, syometu, Item
        }

        /// <summary>
        /// キャラクタナンバー。コンストラクタでキャラの種類ごとにChara.Textnumに代入される。
        /// キャラの識別に使う。
        /// </summary>
        public enum CN      //CharaName
        {
            nul, system, boss, Bboss, sruim, sara, ziki, zikidan, koiwa,
            ooiwa, tekiki, tekidan, haikei_u, haikei_b,
            h_temp, houdai, syometu, tekiki2,
            inseki, koinseki, hadou, ryusei,
            spiner, spiner2, tekiki3, houdai2, houdai3, houdai4, tekidan2,
            sara0, sara1, sara2, sara3,
            sara4, sara5, sara6, sara7,
            Bboss1, Bboss2, Bboss3, Bboss4,
            boss1, boss2, boss3, boss4, boss5,
            Item1 = 0x100
        }

        //キャラtag
        public CN Tag;
        //キャラ種類
        protected CN Textnum = 0;
        //アイテムの場合格納。
        protected CN Itemnum = 0;
        //キャラクタのテクスチャ
        protected Texture2D charatex;
        //消滅時のテクスチャ、消滅時画面に残る時間。
        protected Texture2D syometutex;
        protected Texture2D toumeitex;
        protected int syometucon = 50;
        //キャラのアニメデータ
        protected Texture2D[] animeData;
        //アニメデータの再生間隔
        protected int AnimeSpan = 5;
        //アニメデータの再生カウント
        protected int AnimeCount = 5;
        //アニメデータのポインタ
        protected int AnimePointa = 0;
        //体力
        protected uint hp = 255;
        //変位
        protected Vector2 cvec;
        //速度
        protected Vector2 vvec;
        //中心座標
        protected Vector2 origin;
        //拡大縮小
        protected Vector2 scale;
        //スコア
        protected int Score = 0;
        //色
        protected Color color = new Color((byte)0, (byte)0, (byte)0, (byte)0);

        protected Audio audio;
        protected Cv state = Cv.nul;
        //キャラの回転(rad)
        protected float rot = 0;

        public Texture2D[] GAnimeData
        {
            get { return animeData; }
        }

        public Color Gcolor
        {
            set { color = value; }
            get { return color; }
        }


        public int Gsyometucon
        {
            set { syometucon = value; }
            get { return syometucon; }
        }


        public int Gscore
        {
            get { return Score; }
        }


        public CN GTextnum
        {
            get { return Textnum; }
        }

        public CN GItemnum
        {
            get { return Itemnum; }
        }

        public Texture2D Gcharatex
        {
            set { charatex = value; }
            get { return this.charatex; }
        }

        public Vector2 Gcvec
        {
            get { return this.cvec; }
        }

        public Cv Gstate
        {
            set { state = value; }
            get { return this.state; }
        }

        public float Grot
        {
            get { return rot; }
        }


        public Vector2 Gorigin
        {
            get { return origin; }
        }

        public Vector2 Gscale
        {
            get { return scale; }
        }

        public uint Ghp
        {
            set { hp = value; }
            get { return hp; }
        }

        //Chara create 派生クラス用
        protected Chara(CN Tn, Texture2D t2, Vector2 v2, Audio ad, Game g, Texture2D[] anim)
        {
            Tag = CN.nul;
            charatex = t2;
            animeData = anim;
            if (anim == null && Tn != CN.tekidan && Tn != CN.tekidan2)
            {
                Console.WriteLine("透明 : " + Tn);
            }
            syometutex = g.Content.Load<Texture2D>("消滅");
            toumeitex = g.Content.Load<Texture2D>("透明");
            Itemnum = (CN)((int)Tn & 0xff00);
            Textnum = (CN)((int)Tn & 0xff);
            cvec = v2;
            audio = ad;
            origin = new Vector2(Gcharatex.Width / 2, Gcharatex.Height / 2);
            scale = new Vector2(1.0f, 1.0f);

        }

        /// <summary>
        /// 各派生クラスで各キャラクターの動作を設定する。
        /// </summary>
        /// <returns></returns>
        public abstract Cv move();

        /// <summary>
        /// 画面外判定。画面外の時Cv.outviewを返す。
        /// </summary>
        public Cv Outview()
        {
            if (cvec.Y > Height + 64.0f || cvec.Y < -64.0f || cvec.X > Fieldr + 64.0f || cvec.X < Fieldl - 64.0f) return Cv.outview;
            return Cv.nul;
        }

        /// <summary>
        /// Hpを減らしてHpゼロを判定。
        /// </summary>
        /// <returns></returns>
        public bool Hpdec()
        {
            hp--;
            return (hp == 0);
        }


        /// <summary>
        /// テクスチャに消滅グラフィック、消滅カウントを代入。
        /// 消滅時、アイテムを含むキャラの場合cnum, vecを返す。
        /// ただしボスはこのメソッドをスルーする。
        /// </summary>
        /// <param name="cnum"></param>
        /// <param name="vec"></param>
        public void syometu(out CN cnum, out Vector2 vec)
        {
            cnum = 0; vec = new Vector2();
            if (Tag == CN.boss)
            {
                //Textnum = CN.syometu;
                state = Cv.syometu;
                cnum = CN.nul;
                vec = new Vector2(0, 0);
                return;
            }

            audio.SoundPlay(Audio.SN.short_bomb);
            switch (Itemnum)
            {
                case CN.Item1:
                    charatex = toumeitex;
                    state = Cv.syometu;
                    cnum = CN.Item1;
                    vec = cvec;
                    break;
                default:
                    charatex = toumeitex;
                    state = Cv.syometu;
                    cnum = CN.nul;
                    vec = new Vector2(0, 0);
                    break;
            }
        }

        public void Animation()
        {
            //色を元に戻す
            var cn = color;
            for (int i = 0; i < 8; i++)
            {
                if (Textnum != CN.haikei_u && Textnum != CN.haikei_b && Textnum != CN.syometu &&
                   state != Cv.syometu && cn != Color.White && Itemnum != CN.Item1)
                {
                    if (cn.R < 255) cn.R++;
                    if (cn.G < 255) cn.G++;
                    if (cn.B < 255) cn.B++;
                    if (cn.A < 255) cn.A++;
                    color = cn;
                }
                else if (Itemnum != 0)
                {
                    if (cn.R < 255) cn.R++;
                    if (cn.G > 242) cn.G--;
                    if (cn.G < 242) cn.G++;
                    if (cn.B > 0) cn.B--;
                    if (cn.A < 255) cn.A++;
                    color = cn;
                }
            }

            //キャラ消滅アニメ
            if (state == Cv.syometu)
            {
                if (Textnum == CN.boss1 | Textnum == CN.boss2 | Textnum == CN.boss3)
                {
                    if (cn.R > 10) cn.R -= 3;
                    if (cn.G > 10) cn.G -= 3;
                    if (cn.B > 10) cn.B -= 3;
                    if (cn.A > 80) cn.A -= 3;
                    color = cn;
                    return;
                }
                if (cn.R > 5) cn.R -= 5;
                if (cn.G > 5) cn.G -= 5;
                if (cn.B > 5) cn.B -= 5;
                if (cn.A > 5) cn.A -= 5;
                color = cn;
                return;
            }

            //キャラ通常アニメ
            if (animeData != null)
            {
                //カウント減らし
                if (AnimeCount > 0)
                {
                    AnimeCount--;
                    return;
                }
                //アニメーション

                if (AnimePointa == animeData.Length)
                {
                    AnimePointa = 0;
                }
                charatex = animeData[AnimePointa];
                AnimePointa++;
                AnimeCount = AnimeSpan;
            }
        }
    }



    /// <summary>
    /// 背景の左右テンプレート
    /// </summary>
    public class Temp : Chara
    {
        /// <summary>
        /// ゲームオーバー時、ゲームクリア時にキャラクタオブジェクトに状態変数として、
        /// ゲームオーバー時GOcount = -1, ゲームクリア時GOcount = -2
        /// </summary>
		public int GOcount = 120;
        public Temp(CN Tn, Texture2D t2, Vector2 v2, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            Tag = CN.system;
            origin = new Vector2(0, 0);
        }
        /// <summary>
        /// player消滅処理時のみ実行。
        /// </summary>
        /// <returns></returns>
        public override Cv move()
        {
            GOcount--;
            if (GOcount == 0)
            {
                //audio.MusicPlay2();
                return Cv.nul;
            }
            if (GOcount <= 0)
            {
                GOcount = -1;
                return Cv.outview;
            }
            return Cv.nul;
        }
    }

    /// <summary>
    /// 背景。上下で分かれており、ずっとスクロールしている。
    /// </summary>
    public class BG : Chara
    {
        int Mode = 0;
        const int scr_UP_value = 60;
        int scr_UP_Count = 0;
        public int scr_Speed = 1;
        public float scr_PowerUp = 0;

        public BG(CN Tn, Texture2D t2, Vector2 v2, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            Tag = CN.system;
            origin = new Vector2(0, 0);
        }
        public override Cv move()
        {
            scr_Dash();
            Color_trans();
            if (cvec.Y >= 800.0f)
                cvec.Y = cvec.Y - 1600.0f;
            cvec.Y += 1.25f * (scr_Speed * 1.5f + scr_PowerUp * 4);
            return Cv.nul;
        }

        void scr_Dash()
        {
            if (Mode == 2 && scr_UP_Count == 0)
            {
                scr_PowerUp = 0;
                Mode = 0;
                color = Color.White;
                return;
            }
            if (Mode == 1)
            {
                if (scr_UP_Count % 2 == 0)
                {
                    color.R++;
                    color.G++;
                    color.B++;
                }
                if (scr_UP_Count > 40) scr_PowerUp += 0.5f;
                if (scr_UP_Count == 31)
                    Mode = 2;
            }
            else if (Mode == 2)
            {
                if (scr_UP_Count % 2 == 0)
                {
                    color.R++;
                    color.G++;
                    color.B++;
                }
                if (scr_UP_Count < 19) scr_PowerUp -= 0.5f;
            }

            scr_UP_Count--;
        }

        public void scr_up(uint p)
        {
            Mode = 1;
            scr_Speed = (int)p;
            scr_UP_Count = scr_UP_value;
        }

        public void scr_down(uint p)
        {
            color = Color.White;
            Mode = 0;
            scr_PowerUp = 0;
            scr_Speed = (int)p;
        }


        void Color_trans()
        {
            if (color.R > 192) color.R -= 2;
            else color.R += 2;
            if (color.G > 192) color.G -= 4;
            else color.G += 4;
            if (color.B > 192) color.B--;
            else color.B++;
        }

    }


    /// <summary>
    /// 背景。上下で分かれており、ずっとスクロールしている。
    /// </summary>
    public class EBG : Chara
    {

        public EBG(CN Tn, Texture2D t2, Vector2 v2, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            Tag = CN.system;
            color = Color.White;
            origin = new Vector2(0, 0);
        }
        public override Cv move()
        {
            if (cvec.Y <= -1900.0f)
                return Cv.Item;
            cvec.Y -= 0.75f;
            return Cv.nul;
        }
    }




    /// <summary>
    /// プレイヤーの自機。このインスタンスの中に、自機弾のインスタンス配列(ハンドル)も持っている。
    /// </summary>
    public class Player : Chara
    {
        Device joystick;

        const int p_bombnum = 60;
        public P_bomb[] P_bomb = new P_bomb[p_bombnum];

        //自機弾のテクスチャ
        Texture2D bombtx;
        Game game;
        private int m_span = 4;
        int bombmode = 1;
        int bombx = 13;
        int bombtim = 0;
        int bombspan = 5;

        Microsoft.DirectX.DirectInput.JoystickState jb;

        public Microsoft.DirectX.DirectInput.JoystickState Jb
        {
            set { jb = value; }
        }


        public Player(CN Tn, Texture2D t2, Vector2 v2, Audio ad, Game g, Device joy, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            joystick = joy;
            hp = 1;
            AnimeSpan = 10;
            game = g;
            bombtx = game.Content.Load<Texture2D>("自機弾");
        }

        /// <summary>
        /// 2段階までのレベルアップ。
        /// </summary>
        public void PowerUp()
        {
            audio.SoundPlay(Audio.SN.button02a);
            audio.SoundPlay(Audio.SN.button02a);
            switch (hp)
            {
                case 1:
                    hp++;
                    bombspan = 3;
                    break;
                case 2:
                    hp++;
                    bombspan = 4;
                    bombmode = 2;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Hpがゼロになるとtrueを返す。
        /// </summary>
        /// <returns></returns>
        public bool PowerDown()
        {
            switch (hp)
            {
                case 1:
                    hp--;
                    return true;
                case 2:
                    hp--;
                    bombspan = 5;
                    return false;
                case 3:
                    hp--;
                    bombspan = 3;
                    bombmode = 1;
                    return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Stageup()時の初期化
        /// </summary>
        public void init()
        {
            for (int i = 0; i < p_bombnum; i++)
                P_bomb[i] = null;
            cvec = new Vector2(Width / 2, Height * 5 / 7);
        }

        /// <summary>
        /// エンディング用のmove
        /// </summary>
        public void upmove()
        {
            cvec.Y -= 15.0f;
        }


        public override Cv move()
        {
            Microsoft.Xna.Framework.Input.KeyboardState key1 = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            //消滅処理
            if (state == Cv.syometu)
            {
                if (syometucon-- == 0) return Cv.outview;
                return Cv.nul;
            }

            //方向キー、スペースキー入力
            if (key1.IsKeyDown(Keys.Down) || joystick != null && jb.Y > 55535)
                this.cvec.Y += m_span;
            if (key1.IsKeyDown(Keys.Left) || joystick != null && jb.X < 10000)
                this.cvec.X -= m_span;
            if (key1.IsKeyDown(Keys.Right) || joystick != null && jb.X > 55535)
                this.cvec.X += m_span;
            if (key1.IsKeyDown(Keys.Up) || joystick != null && jb.Y < 10000)
                this.cvec.Y -= m_span;

            //自機弾生成
            if (key1.IsKeyDown(Keys.Space) || (joystick != null && jb.GetButtons()[0] == 128))
                P_bombcreate();

            //画面範囲判定
            if (cvec.X - origin.X < Fieldl) cvec.X = (float)Fieldl + origin.X;
            if (cvec.X - origin.X > Fieldr - charatex.Width) cvec.X = (float)Fieldr - charatex.Width + origin.X;
            if (cvec.Y - origin.Y < 0) cvec.Y = 0 + +origin.Y;
            if (cvec.Y - origin.Y > Width - 2 * charatex.Width) cvec.Y = Width - 2 * charatex.Width + origin.Y;
            //自機弾だけグラフィックハンドル＝＝P_Bomb[]なのでtrueの時はnullにする。
            for (int i = 0; i < p_bombnum; i++)
                if (P_bomb[i] != null && (P_bomb[i].move())) P_bomb[i] = null;

            //弾連射カウント減らし
            if (bombtim > 0) bombtim--;

            return Cv.nul;
        }

        /// <summary>
        /// 自機弾生成。
        /// </summary>
        void P_bombcreate()
        {
            for (int i = 0; i < P_bomb.Length - 1; i++)
                if (bombtim == 0 && P_bomb[i] == null && P_bomb[i + 1] == null)
                {
                    //Hp == 1, 2
                    if (bombmode == 1) P_bomb[i] = new P_bomb(CN.zikidan, 0, bombtx, this.cvec, audio, game, joystick, null);
                    //Hp == 3
                    if (bombmode == 2)
                    {
                        P_bomb[i] = new P_bomb(CN.zikidan, -bombx, bombtx, this.cvec, audio, game, joystick, null);
                        P_bomb[i + 1] = new P_bomb(CN.zikidan, bombx, bombtx, this.cvec, audio, game, joystick, null);
                    }
                    bombtim = bombspan;
                }
        }


    }

    /// <summary>
    /// 自機弾、移動するだけ。
    /// </summary>
    public class P_bomb : Player
    {
        private int m_span = 8;

        public P_bomb(CN Tn, int bombvecx, Texture2D t2, Vector2 v2, Audio ad, Game g, Device joy, Texture2D[] anim) : base(Tn, t2, v2, ad, g, joy, anim)
        {
            Tag = CN.system;
            color = Color.White;
            charatex = t2;
            cvec.Y -= 21f;
            cvec.X = cvec.X + bombvecx;
            if (bombvecx <= 0)
            {
                audio.SoundStop(Audio.SN.laser2);
                audio.SoundPlay(Audio.SN.laser2);
            }
        }
        //自機弾だけグラフィックハンドル＝＝P_Bomb[]なのでtrueの時はnullにする。
        public new bool move()
        {
            //move
            cvec.Y -= m_span;
            if (cvec.Y < -16.0f) return true;
            return false;
        }
    }

    /// <summary>
    /// ゆっくり動く弾。
    /// </summary>
    public class E_bomb1 : Chara
    {
        private int m_span = 1;

        /// <summary>
        /// 移動速度のr = sqr(x2+y2)成分。
        /// </summary>
        private double R_player_dis = 2.4;

        //自機の方向にベクトルセット。
        public E_bomb1(CN Tn, Texture2D t2, Vector2 v2, Vector2 pv2, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            hp = 1;
            Score = 100;

            Tag = CN.system;
            R_player_dis = R_player_dis / Math.Sqrt((pv2.X - cvec.X) * (pv2.X - cvec.X) + (pv2.Y - cvec.Y) * (pv2.Y - cvec.Y));
            vvec.X = (float)((pv2.X - cvec.X) * R_player_dis);
            vvec.Y = (float)((pv2.Y - cvec.Y) * R_player_dis);
        }
        //指定された回転方向にベクトルセット。
        public E_bomb1(CN Tn, Texture2D t2, Vector2 v2, float rt, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            hp = 1;
            Score = 200;

            Tag = CN.system;
            rot += rt + (float)Math.PI / 2;
            vvec = new Vector2((float)(R_player_dis * Math.Cos(rot)), (float)(m_span + R_player_dis * Math.Sin(rot)));
        }

        //指定された回転方向にベクトルセット。速度指定
        public E_bomb1(CN Tn, Texture2D t2, Vector2 v2, float rt, Audio ad, Game g, Texture2D[] anim, float sp) : base(Tn, t2, v2, ad, g, anim)
        {
            hp = 1;
            Score = 200;

            Tag = CN.system;
            rot += rt + (float)Math.PI / 2;
            R_player_dis = sp;
            vvec = new Vector2((float)(R_player_dis * Math.Cos(rot)), (float)(m_span + R_player_dis * Math.Sin(rot)));
        }

        public override Cv move()
        {
            //Cv.outviewで参照外し nulで無視
            cvec.X += vvec.X;
            cvec.Y += vvec.Y;
            return Outview();
        }
    }

    public class E_bomb2 : Chara
    {
        private int m_span = 8;
        private double R = 6.5;


        //自機の方向にベクトルセット。
        public E_bomb2(CN Tn, Texture2D t2, Vector2 v2, Vector2 pv2, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            hp = 1;
            Score = 200;

            Tag = CN.system;
            R = R / Math.Sqrt((pv2.X - cvec.X) * (pv2.X - cvec.X) + (pv2.Y - cvec.Y) * (pv2.Y - cvec.Y));
            vvec.X = (float)((pv2.X - cvec.X) * R);
            vvec.Y = (float)((pv2.Y - cvec.Y) * R);
        }

        public override Cv move()
        {
            //Cv.outviewで参照外し nulで無視
            cvec.X += vvec.X;
            cvec.Y += vvec.Y;
            return Outview();
        }
    }





    /// <summary>
    /// スライム
    /// </summary>
    public class En1 : Chara
    {
        Random rand;
        Vector2 Basevec;
        float x = -0.5f;
        float A = 175.0f;
        float movespan = 1.7f;
        int dan_span = 0;

        //キャラクタの初期位置によってsin(x)のxをセット。
        public En1(CN Tn, Texture2D t2, Vector2 v2, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            Score = 100;
            hp = 1;
            AnimeSpan = 3;
            x += (v2.X - 222.0f) / ((Fieldr - Fieldl) - 10.0f);
            rand = new Random();
            Basevec = v2;
            Basevec.X = Basevec.X - A * (float)Math.Sin(x * Math.PI);
            dan_span = rand.Next() % 90 + 75;
        }

        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                if (syometucon-- == 0) return Cv.outview;
                return Cv.nul;
            }

            //en1_bombで弾生成 nulで無視
            cvec.X = Basevec.X + A * (float)Math.Sin(x * Math.PI);
            x = x + 0.01f;
            if (x >= 2.0f) x = 0;
            cvec.Y += movespan;
            if (dan_span-- == 0)
            {
                dan_span = rand.Next() % 30 + 75;
                return Cv.en1_bomb;
            }
            return Outview();
        }

    }

    /// <summary>
    /// 敵機。
    /// </summary>
    public class En2 : Chara
    {
        Random rand;
        private int m_span = 4;
        //速度ベクトルのr成分
        private double R_player_dis = 2.7;
        int dan_span = 62;

        //自機の方向に敵機を飛ばす。
        public En2(CN Tn, Texture2D t2, Vector2 v2, Player pl, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            Score = 200;
            hp = 1;
            AnimeSpan = 7;
            Vector2 pv2;
            pv2.X = pl.Gcvec.X; pv2.Y = pl.Gcvec.Y;

            //ベクトルセット
            R_player_dis = R_player_dis / Math.Sqrt((pv2.X - cvec.X) * (pv2.X - cvec.X) + (pv2.Y - cvec.Y) * (pv2.Y - cvec.Y));
            vvec.X = (float)((pv2.X - cvec.X) * R_player_dis);
            vvec.Y = (float)((pv2.Y - cvec.Y) * R_player_dis);
            rot = (float)(Math.Asin((cvec.X - pv2.X) / Math.Sqrt((pv2.X - cvec.X) * (pv2.X - cvec.X) + (pv2.X - cvec.Y) * (pv2.X - cvec.Y))));
            rand = new Random();
            dan_span = rand.Next() % 30 + 65;
        }

        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                if (syometucon-- == 0) return Cv.outview;
                return Cv.nul;
            }
            if (dan_span-- == 0)
            {
                dan_span = rand.Next() % 30 + 65;
                return Cv.en2_bomb;
            }
            cvec.Y += vvec.Y;
            cvec.X += vvec.X;
            return Outview();
        }

    }

    /// <summary>
    /// 大砲。
    /// </summary>
    public class En3 : Chara
    {
        Player pl;
        Random rand;
        int ID;
        float m_span = 1.25f;
        int dan_span = 75;
        int rotspan = 1;

        public En3(CN Tn, Texture2D t2, Vector2 v2, Player pl, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            Score = 300;
            hp = 1;
            AnimeSpan = 4;
            rand = new Random();
            this.pl = pl;
            if (cvec.X < 400.0f) { rot = MathHelper.ToRadians(-90); ID = 1; }//弾は左上
            if (cvec.X > 400.0f) { rot = MathHelper.ToRadians(90); ID = 2; }//弾は右上
        }


        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                if (syometucon-- == 0) return Cv.outview;
                return Cv.nul;
            }
            if (dan_span-- == 0)
            {
                dan_span = rand.Next() % 30 + 50;
                return Cv.en2_bomb;
            }
            else
            {
                //自機の方向に徐々に向いてくる。

                //ベクトル
                var dx = pl.Gcvec.X - cvec.X;
                var dy = pl.Gcvec.Y - cvec.Y;
                float deg = 0;
                //var deg = Math.Asin(dx / Math.Sqrt(dx*dx+dy*dy));
                if (dy > 0) deg = (float)Math.Atan(dy / dx);
                if (dy < 0) deg = (float)Math.Atan(-dx / dy);

                //大砲が左
                if (ID == 1)
                {
                    if (dy <= 0)
                        if (deg > rot - MathHelper.ToRadians(-180))
                            rot += MathHelper.ToRadians(rotspan);
                        else
                            rot -= MathHelper.ToRadians(rotspan);
                    if (dy > 0)
                        if (deg > rot - MathHelper.ToRadians(-90))
                            rot += MathHelper.ToRadians(rotspan);
                        else
                            rot -= MathHelper.ToRadians(rotspan);
                }

                //大砲が右
                else if (ID == 2)
                {
                    if (dy <= 0)
                        if (deg > rot - MathHelper.ToRadians(180))
                            rot += MathHelper.ToRadians(rotspan);
                        else
                            rot -= MathHelper.ToRadians(rotspan);
                    if (dy > 0)
                        if (deg > rot - MathHelper.ToRadians(90))
                            rot += MathHelper.ToRadians(rotspan);
                        else
                            rot -= MathHelper.ToRadians(rotspan);
                }


            }
            cvec.Y += m_span;
            return Outview();
        }

    }

    /// <summary>
    /// 向かってくる敵機。
    /// </summary>
    public class En4 : Chara
    {

        Player pl;
        Random rand;
        private int m_span = 4;
        //速度ベクトルr成分
        private double R_player_dis_ps = 2.7;
        private double R_player_dis = 2.7;
        int dan_span = 62;
        //回転速度
        int rotspan = 1;

        //プレイヤーの方向に進む。
        public En4(CN Tn, Texture2D t2, Vector2 v2, Player pl, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            Score = 500;
            hp = 2;
            AnimeSpan = 2;
            this.pl = pl;
            Vector2 pv2;
            pv2.X = pl.Gcvec.X; pv2.Y = pl.Gcvec.Y;

            R_player_dis = R_player_dis / Math.Sqrt((pv2.X - cvec.X) * (pv2.X - cvec.X) + (pv2.Y - cvec.Y) * (pv2.Y - cvec.Y));
            vvec.X = (float)((pv2.X - cvec.X) * R_player_dis);
            vvec.Y = (float)((pv2.Y - cvec.Y) * R_player_dis);
            rot = (float)(Math.Asin((cvec.X - pv2.X) / Math.Sqrt((pv2.X - cvec.X) * (pv2.X - cvec.X) + (pv2.X - cvec.Y) * (pv2.X - cvec.Y))));
            rand = new Random();
            dan_span = rand.Next() % 30 + 65;
        }

        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                if (syometucon-- == 0) return Cv.outview;
                return Cv.nul;
            }

            if (dan_span-- == 0)
            {
                dan_span = rand.Next() % 30 + 65;
                return Cv.en2_bomb;
            }

            //回転のセット
            Rotset();


            cvec.Y += vvec.Y;
            cvec.X += vvec.X;
            return Outview();
        }

        ///回転のセット。
        void Rotset()
        {
            //ベクトル
            var dx = pl.Gcvec.X - cvec.X;
            var dy = pl.Gcvec.Y - cvec.Y;
            //角度
            float deg = 0;
            //var deg = Math.Asin(dx / Math.Sqrt(dx*dx+dy*dy));
            if (dy > 0) deg = (float)Math.Atan(dy / dx);
            if (dy < 0) deg = (float)Math.Atan(-dx / dy);

            //プレイヤーの左側にいる。
            if (cvec.X < pl.Gcvec.X)
            {
                if (dy <= 0)
                    if (deg > rot - MathHelper.ToRadians(-180))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
                if (dy > 0)
                    if (deg > rot - MathHelper.ToRadians(-90))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
            }

            //プレイヤーの右側にいる。
            if (pl.Gcvec.X < cvec.X)
            {
                if (dy <= 0)
                    if (deg > rot - MathHelper.ToRadians(180))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
                if (dy > 0)
                    if (deg > rot - MathHelper.ToRadians(90))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
            }
            ///一回ごとに速度ベクトルをセット。
            R_player_dis = R_player_dis_ps;
            R_player_dis = R_player_dis / Math.Sqrt((pl.Gcvec.X - cvec.X) * (pl.Gcvec.X - cvec.X) + (pl.Gcvec.Y - cvec.Y) * (pl.Gcvec.Y - cvec.Y));
            vvec.X = (float)((pl.Gcvec.X - cvec.X) * R_player_dis);
            vvec.Y = (float)Math.Abs(((pl.Gcvec.Y - cvec.Y) * R_player_dis));
        }
    }

    /// <summary>
    /// 岩
    /// </summary>
    public class En5 : Chara
    {
        Random rand;
        float movespan = 1.0f;
        float movespanx = 1.0f;
        int movex_count;
        int movex_span_base;


        public En5(CN Tn, Texture2D t2, Vector2 v2, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            Score = 1000;
            hp = 32;
            AnimeSpan = 5;
            if (GTextnum == CN.koiwa) { hp = 4; }
            rand = new Random();
            movex_span_base = rand.Next() % 8 + 24;
            movex_count = movex_span_base;
            vvec = new Vector2(movespanx, movespan);
        }

        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                if (syometucon-- == 0) return Cv.outview;
                return Cv.nul;
            }

            movex_count--;
            if (movex_count <= 0)
            {
                var v = rand.Next();
                movex_count = v % movex_span_base;
                if (v % 3 == 0) vvec.X = vvec.X / 2;
                else if (cvec.X < 220f || v % 3 == 1) vvec.X = movespanx;
                else if (cvec.X > 580f || v % 3 == 2) vvec.X = -movespanx;

            }
            cvec.X += vvec.X;
            cvec.Y += vvec.Y;
            return Outview();
        }

    }


    /// <summary>
    /// 隕石
    /// </summary>
    public class En6 : Chara
    {
        float movespan = 2.50f;


        public En6(CN Tn, Texture2D t2, Vector2 v2, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            hp = 10;
            Score = 1000;
            AnimeSpan = 3;
        }

        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                if (syometucon-- == 0) return Cv.outview;
                return Cv.nul;
            }


            cvec.Y += movespan;
            return Outview();
        }

    }


    /// <summary>
    /// 小隕石
    /// </summary>
    public class En7 : Chara
    {
        float movespan = 7.50f;


        public En7(CN Tn, Texture2D t2, Vector2 v2, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            hp = 2;
            AnimeSpan = 1;
            Score = 1000;
        }

        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                if (syometucon-- == 0) return Cv.outview;
                return Cv.nul;
            }


            cvec.Y += movespan;
            return Outview();
        }

    }

    /// <summary>
    /// 皿
    /// </summary>
    public class En8 : Chara
    {
        int rand;
        int mode = 0;
        int movedirection;
        Vector2 basevec;

        public En8(CN Tn, Texture2D t2, Vector2 v2, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            movedirection = Moveset(Tn);
            Random ra = new Random((int)v2.X * 48);
            rand = ra.Next() % 500;
            hp = 1;
            AnimeSpan = 3;
            basevec = v2;
            Score = 100;
            vvec.Y = 5.0f;
        }

        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                if (syometucon-- == 0) return Cv.outview;
                return Cv.nul;
            }

            if (mode == 0 && (int)cvec.Y > rand + 250)
                mode = 1;

            MoveForDirection(movedirection);

            return Outview();
        }

        int Moveset(CN Tn)
        {
            return (int)Tn - (int)CN.sara0;
        }


        void MoveForDirection(int d)
        {

            if (mode == 1)
            {
                switch (d)
                {
                    //0~7 is direction
                    case 0:
                        vvec = new Vector2(0.0f, -6.0f);
                        break;
                    case 1:
                        vvec = new Vector2(6.0f, -6.0f);
                        break;
                    case 2:
                        vvec = new Vector2(6.0f, 0f);
                        break;
                    case 3:
                        vvec = new Vector2(6.0f, 6.0f);
                        break;
                    case 4:
                        vvec = new Vector2(0.0f, 6.0f);
                        break;
                    case 5:
                        vvec = new Vector2(-6.0f, 6.0f);
                        break;
                    case 6:
                        vvec = new Vector2(-6.0f, 0.0f);
                        break;
                    case 7:
                        vvec = new Vector2(-6.0f, -6.0f);
                        break;
                }
            }

            cvec.X += vvec.X;
            cvec.Y += vvec.Y;
            return;
        }
    }

    /// <summary>
    /// スピナー
    /// </summary>
    public class En9 : Chara
    {
        float rotspan;
        float movespan;

        int dan_span = 12;
        Random rand;

        float R;
        float R_rot;
        float R_rotspan;
        Vector2 basevec;
        Vector2 rotvec;

        public En9(CN Tn, Texture2D t2, Vector2 v2, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            hp = 5;
            Score = 1000;

            rand = new Random();
            rotspan = 5.0f;
            movespan = 2f;
            R_rotspan = -2.0f;

            R = 48.0f;
            basevec = cvec;
            R_rot = 270f;
            rotvec = new Vector2(0, -R);
        }

        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                if (syometucon-- == 0) return Cv.outview;
                return Cv.nul;
            }

            if (dan_span-- == 0)
            {
                dan_span = rand.Next() % 15 + 18;
                return Cv.en2_bomb;
            }

            R_rot += R_rotspan;
            basevec.Y += movespan;
            rotvec = new Vector2(R * (float)Math.Cos(MathHelper.ToRadians(R_rot)), R * (float)Math.Sin((MathHelper.ToRadians(R_rot))));

            cvec = basevec + rotvec;
            rot += rotspan;

            return Outview();
        }

    }

    /// <summary>
    /// スピナー2
    /// </summary>
    public class En10 : Chara
    {
        float x = -0.5f;
        float A = 100.0f;

        float rotspan;
        float movespan;

        Random rand;
        int dan_span = 8;

        float R;
        float R_rot;
        float R_rotspan;
        Vector2 basevec;
        Vector2 rotvec;

        public En10(CN Tn, Texture2D t2, Vector2 v2, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            hp = 15;
            Score = 1000;

            rand = new Random();
            x += (v2.X - 222.0f) / ((Fieldr - Fieldl) - 10.0f);

            rotspan = 5.0f;
            movespan = 1f;
            R_rotspan = -2.0f;

            R = 18.0f;
            basevec = cvec;
            R_rot = 270f;
            rotvec = new Vector2(0, -R);
        }

        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                if (syometucon-- == 0) return Cv.outview;
                return Cv.nul;
            }

            if (dan_span-- == 0)
            {
                dan_span = rand.Next() % 8 + 4;
                return Cv.en2_bomb;
            }

            x = x + 0.01f;
            if (x >= 2.0f) x = 0;

            R_rot += R_rotspan;
            basevec.Y += movespan;
            rotvec = new Vector2(R * (float)Math.Cos(MathHelper.ToRadians(R_rot)), R * (float)Math.Sin((MathHelper.ToRadians(R_rot))));

            cvec = basevec + rotvec;
            cvec.X = basevec.X + A * (float)Math.Sin(x * Math.PI);
            rot += rotspan;

            return Outview();
        }

    }


    /// <summary>
    /// 敵機3
    /// </summary>
    public class En11 : Chara
    {
        Player pl;
        Random rand;
        //速度ベクトルr成分
        private double R_player_dis_ps = 2.7;
        private double R_player_dis = 2.7;
        int dan_span = 32;
        float rotspan = 1.0f;
        float xspan = 0.015f;

        float x = -0.5f;
        float A = 100.0f;

        float movespan;

        float R;
        float R_rot;
        float R_rotspan;
        Vector2 basevec;
        Vector2 rotvec;

        public En11(CN Tn, Texture2D t2, Vector2 v2, Player pl, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            hp = 6;
            Score = 1000;

            rand = new Random();
            x += (v2.X - 222.0f) / ((Fieldr - Fieldl) - 10.0f);

            movespan = 1.5f;
            R_rotspan = -0.05f;

            R = 18.0f;
            basevec = cvec;
            R_rot = 270f;
            rotvec = new Vector2(0, -R);

            AnimeSpan = 2;
            this.pl = pl;
            Vector2 pv2;
            pv2.X = pl.Gcvec.X; pv2.Y = pl.Gcvec.Y;

            R_player_dis = R_player_dis / Math.Sqrt((pv2.X - cvec.X) * (pv2.X - cvec.X) + (pv2.Y - cvec.Y) * (pv2.Y - cvec.Y));
            vvec.X = (float)((pv2.X - cvec.X) * R_player_dis);
            vvec.Y = (float)((pv2.Y - cvec.Y) * R_player_dis);
            rot = (float)(Math.Asin((cvec.X - pv2.X) / Math.Sqrt((pv2.X - cvec.X) * (pv2.X - cvec.X) + (pv2.X - cvec.Y) * (pv2.X - cvec.Y))));
            rand = new Random();
            dan_span = rand.Next() % 30 + 35;
        }

        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                if (syometucon-- == 0) return Cv.outview;
                return Cv.nul;
            }

            if (dan_span-- == 0)
            {
                dan_span = rand.Next() % 30 + 65;
                return Cv.en2_bomb;
            }

            x = x + xspan;
            if (x >= 2.0f)
            {
                A = rand.Next() % 75 + 25;
                x = 0;
            }

            R_rot += R_rotspan;
            basevec.Y += movespan;

            cvec = basevec + rotvec;
            cvec.X = basevec.X + A * (float)Math.Sin(x * Math.PI);


            //回転のセット
            Rotset();

            cvec.Y += vvec.Y;
            cvec.X += vvec.X;

            return Outview();
        }

        ///回転のセット。
        void Rotset()
        {
            //ベクトル
            var dx = pl.Gcvec.X - cvec.X;
            var dy = pl.Gcvec.Y - cvec.Y;
            //角度
            float deg = 0;
            //var deg = Math.Asin(dx / Math.Sqrt(dx*dx+dy*dy));
            if (dy > 0) deg = (float)Math.Atan(dy / dx);
            if (dy < 0) deg = (float)Math.Atan(-dx / dy);

            //プレイヤーの左側にいる。
            if (cvec.X < pl.Gcvec.X)
            {
                if (dy <= 0)
                    if (deg > rot - MathHelper.ToRadians(-180))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
                if (dy > 0)
                    if (deg > rot - MathHelper.ToRadians(-90))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
            }

            //プレイヤーの右側にいる。
            if (pl.Gcvec.X < cvec.X)
            {
                if (dy <= 0)
                    if (deg > rot - MathHelper.ToRadians(180))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
                if (dy > 0)
                    if (deg > rot - MathHelper.ToRadians(90))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
            }
            ///一回ごとに速度ベクトルをセット。
            R_player_dis = R_player_dis_ps;
            R_player_dis = R_player_dis / Math.Sqrt((pl.Gcvec.X - cvec.X) * (pl.Gcvec.X - cvec.X) + (pl.Gcvec.Y - cvec.Y) * (pl.Gcvec.Y - cvec.Y));
            vvec.X = (float)((pl.Gcvec.X - cvec.X) * R_player_dis);
            vvec.Y = (float)Math.Abs(((pl.Gcvec.Y - cvec.Y) * R_player_dis));
        }
    }

    /// <summary>
    /// 大砲ゴーランド
    /// </summary>
    public class En12 : Chara
    {
        float rotspan;
        float movespan;

        int dan_span = 18;
        Random rand;

        public En12(CN Tn, Texture2D t2, Vector2 v2, Player pl, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            hp = 35;
            Score = 5000;

            rand = new Random();
            rotspan = 0.03f;
            movespan = 1.25f;

        }

        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                if (syometucon-- == 0) return Cv.outview;
                return Cv.nul;
            }

            if (dan_span-- == 0)
            {
                dan_span = rand.Next() % 20 + 25;
                return Cv.en12_bomb;
            }

            cvec.Y += movespan;
            rot += rotspan;

            return Outview();
        }

    }


    /// <summary>
    /// 大砲ゴーランド2
    /// </summary>
    public class En13 : Chara
    {
        float rotspan;
        float movespan;

        int dan_span = 18;
        Random rand;

        public En13(CN Tn, Texture2D t2, Vector2 v2, Player pl, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            hp = 24;
            Score = 5000;

            rand = new Random();
            rotspan = 0.03f;
            movespan = 1.25f;

        }

        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                if (syometucon-- == 0) return Cv.outview;
                return Cv.nul;
            }

            if (dan_span-- == 0)
            {
                dan_span = rand.Next() % 20 + 25;
                if (dan_span > 42) return Cv.super_dan1;
                return Cv.en12_bomb;
            }

            cvec.Y += movespan;
            rot += rotspan;

            return Outview();
        }

    }

    /// <summary>
    /// 流星
    /// </summary>
    public class En14 : Chara
    {
        private int m_span = 1;
        float rotspan = 4.0f;

        /// <summary>
        /// 移動速度のr = sqr(x2+y2)成分。
        /// </summary>
        private double R_player_dis = 8.2f;

        //自機の方向にベクトルセット。
        public En14(CN Tn, Texture2D t2, Vector2 v2, Vector2 pv2, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {

            //ベクトル
            var dx = pv2.X - cvec.X;
            var dy = pv2.Y - cvec.Y;
            float deg = 0;

            //var deg = Math.Asin(dx / Math.Sqrt(dx*dx+dy*dy));
            if (dy > 0) deg = (float)Math.Atan(dy / dx);
            if (dy < 0) deg = (float)Math.Atan(-dx / dy);

            rot = deg;
            if (pv2.X < cvec.X) rot += (float)Math.PI;

            hp = 30;
            Score = 200;

            Tag = CN.system;
            R_player_dis = R_player_dis / Math.Sqrt((pv2.X - cvec.X) * (pv2.X - cvec.X) + (pv2.Y - cvec.Y) * (pv2.Y - cvec.Y));
            vvec.X = (float)((pv2.X - cvec.X) * R_player_dis);
            vvec.Y = (float)((pv2.Y - cvec.Y) * R_player_dis);
        }
        //指定された回転方向にベクトルセット。
        public En14(CN Tn, Texture2D t2, Vector2 v2, float rt, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            hp = 30;
            Score = 200;

            Tag = CN.system;
            rot = rt - (float)Math.PI / 2;
            vvec = new Vector2((float)(R_player_dis * Math.Cos(rot)), (float)(m_span + R_player_dis * Math.Sin(rot)));
        }

        //指定された回転方向にベクトルセット。速度指定
        public En14(CN Tn, Texture2D t2, Vector2 v2, float rt, Audio ad, Game g, Texture2D[] anim, float sp) : base(Tn, t2, v2, ad, g, anim)
        {
            hp = 30;
            Score = 200;

            Tag = CN.system;
            rot = rt - (float)Math.PI / 2;
            R_player_dis = sp;
            vvec = new Vector2((float)(R_player_dis * Math.Cos(rot)), (float)(m_span + R_player_dis * Math.Sin(rot)));
        }

        public override Cv move()
        {
            //Cv.outviewで参照外し nulで無視
            if (Textnum == CN.tekidan) rot += rotspan;
            if (rot > 360) rot = 0;
            cvec.X += vvec.X;
            cvec.Y += vvec.Y;
            return Outview();
        }
    }



    /// <summary>
    /// 中ボススライム。
    /// </summary>
    public class BEn1 : Chara
    {
        Random rand;
        Player pl;
        const float movespanconst = 1f;
        float movespan = 1f;
        float rotspan = 0.5f;
        int dan_span = 2;
        double R_player_dis_ps = 8.0f;
        double R_player_dis = 8.0f;

        float gr = 0.1f;
        float upper_sp = 4.0f;

        //自機の方向に敵機を飛ばす。
        public BEn1(CN Tn, Texture2D t2, Vector2 v2, Player pl, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            Tag = CN.Bboss;
            Score = 5000;
            this.pl = pl;
            rand = new Random();
            vvec = new Vector2(0, 7.5f);
            hp = 360;
        }

        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                if (syometucon-- == 0) return Cv.Boutview;
                return Cv.nul;
            }
            if (dan_span-- == 0)
            {
                dan_span = rand.Next() % 24 + 2;
                if (dan_span < 12) return Cv.engenerate;
                return Cv.en3_bomb;
            }

            cvec.X += movespan;

            vvec.Y += gr;
            cvec.Y += vvec.Y;

            if (cvec.Y > 200)
            {
                var v = rand.Next() % 100;
                vvec.Y = -Math.Abs(upper_sp + 0.02f * v);
                cvec.Y -= 20;
                movespan = Math.Sign(movespan) * (movespanconst + v / 200);
            }
            if (cvec.X > 500 | cvec.X < 300)
                movespan = -movespan;

            //回転設定
            Rotset();
            return Cv.nul;
        }
        

        ///回転のセット。
        void Rotset()
        {
            //ベクトル
            var dx = pl.Gcvec.X - cvec.X;
            var dy = pl.Gcvec.Y - cvec.Y;
            //角度
            float deg = 0;
            //var deg = Math.Asin(dx / Math.Sqrt(dx*dx+dy*dy));
            if (dy > 0) deg = (float)Math.Atan(dy / dx);
            if (dy < 0) deg = (float)Math.Atan(-dx / dy);

            //プレイヤーの左側にいる。
            if (cvec.X < pl.Gcvec.X)
            {
                if (dy <= 0)
                    if (deg > rot - MathHelper.ToRadians(-180))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
                if (dy > 0)
                    if (deg > rot - MathHelper.ToRadians(-90))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
            }

            //プレイヤーの右側にいる。
            if (pl.Gcvec.X < cvec.X)
            {
                if (dy <= 0)
                    if (deg > rot - MathHelper.ToRadians(180))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
                if (dy > 0)
                    if (deg > rot - MathHelper.ToRadians(90))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
            }
            ///一回ごとに速度ベクトルをセット。
            R_player_dis = R_player_dis_ps;
            R_player_dis = R_player_dis / Math.Sqrt((pl.Gcvec.X - cvec.X) * (pl.Gcvec.X - cvec.X) + (pl.Gcvec.Y - cvec.Y) * (pl.Gcvec.Y - cvec.Y));

        }

    }



    /// <summary>
    /// 大砲ゴーランド中ボス
    /// </summary>
    public class BEn2 : Chara
    {
        float rotspan;
        float movespan;

        int dan_span = 6;
        Random rand;

        public BEn2(CN Tn, Texture2D t2, Vector2 v2, Player pl, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            Tag = CN.Bboss;
            hp = 400;
            Score = 5000;

            rand = new Random();
            rotspan = 0.15f;
            movespan = 0;

        }

        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                if (syometucon-- == 0) return Cv.Boutview;
                return Cv.nul;
            }

            if (dan_span-- == 0)
            {
                dan_span = rand.Next() % 10;
                if (dan_span > 9) return Cv.super_dan1;
                if (dan_span > 6) return Cv.engenelatefinal;
                return Cv.en2_bomb;
            }

            cvec.Y += movespan;
            rot += rotspan;

            return Cv.nul;
        }

    }


    /// <summary>
    /// スピナー中ボス
    /// </summary>
    public class BEn3 : Chara
    {
        Player pl;
        float x = -0.5f;
        float A = 100.0f;
        float basY;

        float rotspan;

        Random rand;
        int dan_span = 8;

        Vector2 basevec;
        Vector2 avec;

        public BEn3(CN Tn, Texture2D t2, Vector2 v2, Player p, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            pl = p;
            hp = 282;
            Score = 1000;

            rand = new Random();
            x += (v2.X - 222.0f) / ((Fieldr - Fieldl) - 10.0f);

            rotspan = 6.0f;


            basevec = cvec;
            avec = new Vector2();
            vvec = new Vector2();
            basY = cvec.Y;
        }

        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                if (syometucon-- == 0) return Cv.Boutview;
                return Cv.nul;
            }

            if (dan_span-- == 0)
            {
                dan_span = rand.Next() % 5 + 5;
                if (rand.Next() % 6 == 4) return Cv.engenelatefour;
                return Cv.en2_bomb;
            }

            x = x + 0.03f;
            if (x >= 2.0f) x = 0;


            //basevec.Y += movespan;
            //rotvec = new Vector2(R * (float)Math.Cos(MathHelper.ToRadians(R_rot)), R * (float)Math.Sin((MathHelper.ToRadians(R_rot))));


            if (vvec.Y == 0 && (pl.Gcvec.X < cvec.X + 2f) & (pl.Gcvec.X > cvec.X - 2f))
            {
                avec.Y = 0.44f;
                vvec += avec;
                cvec.Y += vvec.Y;
            }
            if (cvec.Y > basY)
            {
                vvec += avec;
                cvec.Y += vvec.Y;
                avec.Y -= 0.011f;
                x -= 0.015f;
            }
            else if (cvec.Y != basY)
            {
                vvec = new Vector2();
                cvec.Y = basY;
            }


            //cvec = basevec + rotvec;
            cvec.X = 400.0f + A * (float)Math.Sin(x * Math.PI);
            rot += rotspan;

            return Cv.nul;
        }

    }


    /// <summary>
    /// 敵機。
    /// </summary>
    public class Boss1 : Chara
    {
        Random rand;
        Player pl;
        float movespan = 2;
        float rotspan = 0.5f;
        int dan_span = 2;
        bool Move_arrow = true;
        private double R_player_dis_ps = 2.7;
        private double R_player_dis = 2.7;

        //自機の方向に敵機を飛ばす。
        public Boss1(CN Tn, Texture2D t2, Vector2 v2, Player pl, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            Tag = CN.boss;
            syometucon = 360;
            Score = 5000;
            this.pl = pl;
            rand = new Random();
            vvec = new Vector2(0, 2.5f);
            hp = 255;
        }
        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                return Cv.boss_outview;
            }
            if (dan_span-- == 0)
            {
                dan_span = rand.Next() % 24 + 2;
                return Cv.en3_bomb;
            }

            //向き反転
            Move_mir();
            //回転設定
            Rotset();

            return Cv.nul;
        }

        void Move_mir()
        {
            //向き反転
            if (Move_arrow)
            {
                if (cvec.Y < 200)
                {
                    cvec.Y += movespan;
                }
                else
                {
                    movespan = -movespan;
                    Move_arrow = false;
                }
            }
            else
            {
                if (cvec.Y > -8)
                {
                    cvec.Y += movespan;
                }
                else
                {
                    movespan = -movespan;
                    Move_arrow = true;
                }
            }
        }

        ///回転のセット。
        void Rotset()
        {
            //ベクトル
            var dx = pl.Gcvec.X - cvec.X;
            var dy = pl.Gcvec.Y - cvec.Y;
            //角度
            float deg = 0;
            //var deg = Math.Asin(dx / Math.Sqrt(dx*dx+dy*dy));
            if (dy > 0) deg = (float)Math.Atan(dy / dx);
            if (dy < 0) deg = (float)Math.Atan(-dx / dy);

            //プレイヤーの左側にいる。
            if (cvec.X < pl.Gcvec.X)
            {
                if (dy <= 3)
                    if (deg > rot - MathHelper.ToRadians(-180))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
                if (dy > -3)
                    if (deg > rot - MathHelper.ToRadians(-90))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
            }

            //プレイヤーの右側にいる。
            if (pl.Gcvec.X < cvec.X)
            {
                if (dy <= 3)
                    if (deg > rot - MathHelper.ToRadians(180))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
                if (dy > -3)
                    if (deg > rot - MathHelper.ToRadians(90))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
            }
            ///一回ごとに速度ベクトルをセット。
            R_player_dis = R_player_dis_ps;
            R_player_dis = R_player_dis / Math.Sqrt((pl.Gcvec.X - cvec.X) * (pl.Gcvec.X - cvec.X) + (pl.Gcvec.Y - cvec.Y) * (pl.Gcvec.Y - cvec.Y));
            vvec.X = (float)((pl.Gcvec.X - cvec.X) * R_player_dis);
            vvec.Y = (float)Math.Abs(((pl.Gcvec.Y - cvec.Y) * R_player_dis));
        }
    }
    
    /// <summary>
    /// 敵機。
    /// </summary>
    public class Boss2 : Chara
    {
        Random rand;
        Player pl;
        const float movespanconst = 1f;
        float movespan = 1f;
        float rotspan = 0.5f;
        int dan_span = 2;
        double R_player_dis_ps = 8.0f;
        double R_player_dis = 8.0f;

        float gr = 0.1f;
        float upper_sp = 4.0f;

        //自機の方向に敵機を飛ばす。
        public Boss2(CN Tn, Texture2D t2, Vector2 v2, Player pl, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            Tag = CN.boss;
            syometucon = 360;
            Score = 5000;
            this.pl = pl;
            rand = new Random();
            vvec = new Vector2(0, 7.5f);
            hp = 510;
        }
        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                return Cv.boss_outview;
            }
            if (dan_span-- == 0)
            {
                dan_span = rand.Next() % 24 + 2;
                if (dan_span < 6) return Cv.engenerate;
                return Cv.en3_bomb;
            }

            cvec.X += movespan;

            vvec.Y += gr;
            cvec.Y += vvec.Y;

            if (cvec.Y > 200)
            {
                var v = rand.Next() % 100;
                vvec.Y = -Math.Abs(upper_sp + 0.02f * v);
                cvec.Y -= 20;
                movespan = Math.Sign(movespan) * (movespanconst + v / 200);
            }
            if (cvec.X > 500 | cvec.X < 300)
                movespan = -movespan;

            //回転設定
            Rotset();

            return Cv.nul;
        }

        ///回転のセット。
        void Rotset()
        {
            //ベクトル
            var dx = pl.Gcvec.X - cvec.X;
            var dy = pl.Gcvec.Y - cvec.Y;
            //角度
            float deg = 0;
            //var deg = Math.Asin(dx / Math.Sqrt(dx*dx+dy*dy));
            if (dy > 0) deg = (float)Math.Atan(dy / dx);
            if (dy < 0) deg = (float)Math.Atan(-dx / dy);

            //プレイヤーの左側にいる。
            if (cvec.X < pl.Gcvec.X)
            {
                if (dy <= 0)
                    if (deg > rot - MathHelper.ToRadians(-180))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
                if (dy > 0)
                    if (deg > rot - MathHelper.ToRadians(-90))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
            }

            //プレイヤーの右側にいる。
            if (pl.Gcvec.X < cvec.X)
            {
                if (dy <= 0)
                    if (deg > rot - MathHelper.ToRadians(180))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
                if (dy > 0)
                    if (deg > rot - MathHelper.ToRadians(90))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
            }
            ///一回ごとに速度ベクトルをセット。
            R_player_dis = R_player_dis_ps;
            R_player_dis = R_player_dis / Math.Sqrt((pl.Gcvec.X - cvec.X) * (pl.Gcvec.X - cvec.X) + (pl.Gcvec.Y - cvec.Y) * (pl.Gcvec.Y - cvec.Y));

        }
    }

    /// <summary>
    /// 敵機。
    /// </summary>
    public class Boss3 : Chara
    {
        Random rand;
        Player pl;
        float movespan = 4;
        float rotspan = 0.5f;
        int dan_span = 1;
        bool Move_arrow = true;
        private double R_player_dis_ps = 8.0f;
        private double R_player_dis = 8.0f;

        //自機の方向に敵機を飛ばす。
        public Boss3(CN Tn, Texture2D t2, Vector2 v2, Player pl, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            Tag = CN.boss;
            syometucon = 360;
            Score = 5000;
            this.pl = pl;
            rand = new Random();
            vvec = new Vector2(0, 15f);
            hp = 1100;
        }
        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                return Cv.boss_outview;
            }
            if (dan_span-- == 0)
            {
                dan_span = rand.Next() % 24 + 4;
                if (dan_span < 16) return Cv.engenelatefinal;
                return Cv.en3_bomb;
            }

            //向き反転
            Move_mir();
            //回転設定
            Rotset();

            return Cv.nul;
        }

        void Move_mir()
        {
            //向き反転
            if (Move_arrow)
            {
                if (cvec.Y < 200)
                {
                    cvec.Y += movespan;
                }
                else
                {
                    movespan = -movespan;
                    Move_arrow = false;
                }
            }
            else
            {
                if (cvec.Y > -8)
                {
                    cvec.Y += movespan;
                }
                else
                {
                    movespan = -movespan;
                    Move_arrow = true;
                }
            }
        }

        ///回転のセット。
        void Rotset()
        {
            //ベクトル
            var dx = pl.Gcvec.X - cvec.X;
            var dy = pl.Gcvec.Y - cvec.Y;
            //角度
            float deg = 0;
            //var deg = Math.Asin(dx / Math.Sqrt(dx*dx+dy*dy));
            if (dy > 0) deg = (float)Math.Atan(dy / dx);
            if (dy < 0) deg = (float)Math.Atan(-dx / dy);

            //プレイヤーの左側にいる。
            if (cvec.X < pl.Gcvec.X)
            {
                if (dy <= 0)
                    if (deg > rot - MathHelper.ToRadians(-180))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
                if (dy > 0)
                    if (deg > rot - MathHelper.ToRadians(-90))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
            }

            //プレイヤーの右側にいる。
            if (pl.Gcvec.X < cvec.X)
            {
                if (dy <= 0)
                    if (deg > rot - MathHelper.ToRadians(180))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
                if (dy > 0)
                    if (deg > rot - MathHelper.ToRadians(90))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
            }
            ///一回ごとに速度ベクトルをセット。
            R_player_dis = R_player_dis_ps;
            R_player_dis = R_player_dis / Math.Sqrt((pl.Gcvec.X - cvec.X) * (pl.Gcvec.X - cvec.X) + (pl.Gcvec.Y - cvec.Y) * (pl.Gcvec.Y - cvec.Y));
            vvec.X = (float)((pl.Gcvec.X - cvec.X) * R_player_dis);
            vvec.Y = (float)Math.Abs(((pl.Gcvec.Y - cvec.Y) * R_player_dis));
        }

    }


    /// <summary>
    /// スピナー
    /// </summary>
    public class Boss4 : Chara
    {
        Vector2 basevvec;
        Random rand;
        Player pl;
        float rotspan = 2.5f;
        int dan_span = 1;
        int super_dan_span = 1000;
        private double R_player_dis_ps = 1.0f;
        private double R_player_dis = 1.0f;

        //自機の方向に敵機を飛ばす。
        public Boss4(CN Tn, Texture2D t2, Vector2 v2, Player pl, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            Tag = CN.boss;
            syometucon = 360;
            Score = 5000;
            this.pl = pl;
            rand = new Random();
            vvec = new Vector2(0, 15f);
            basevvec = new Vector2(0, 0);
            hp = 650;
        }
        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                return Cv.boss_outview;
            }
            if (dan_span-- == 0)
            {
                dan_span = rand.Next() % 30 + 10;
                if (dan_span < 25) return Cv.engenelatetwo;
                return Cv.en1_bomb;
            }
            if (super_dan_span-- == 0)
            {
                super_dan_span = rand.Next() % 650 + 700;
                if (dan_span < 25) return Cv.super_dan1;
                return Cv.en1_bomb;
            }

            //向き反転
            Move_mir();
            //回転設定
            Rotset();
            basevvec.X += 0.001f * (rand.Next() % 100) - 0.05f;
            basevvec.Y += 0.001f * (rand.Next() % 100) - 0.05f;

            if (Math.Abs(basevvec.X) > 3.0f)
                basevvec.X *= 0.5f;

            if (Math.Abs(basevvec.Y) > 3.0f)
                basevvec.X *= 0.5f;

            cvec += vvec + basevvec;

            return Cv.nul;
        }

        void Move_mir()
        {
            if (cvec.X < 230)
            {
                vvec.X = -vvec.X;
                basevvec = -basevvec;
            }
            else if (cvec.X > 570)
            {
                vvec.X = -vvec.X;
                basevvec = -basevvec;
            }
            if (cvec.Y < 50)
            {
                vvec.Y = -vvec.Y;
                basevvec = -basevvec;
            }
            else if (cvec.Y > 650)
            {
                vvec.Y = -vvec.Y;
                basevvec = -basevvec;
            }

            if (cvec.Y > pl.Gcvec.Y)
            {
                vvec.Y = -1.0f;
                basevvec.Y = 0;
            }
        }

        ///回転のセット。
        void Rotset()
        {
            rot += rotspan;
            R_player_dis = R_player_dis_ps;
            R_player_dis = R_player_dis / Math.Sqrt((pl.Gcvec.X - cvec.X) * (pl.Gcvec.X - cvec.X) + (pl.Gcvec.Y - cvec.Y) * (pl.Gcvec.Y - cvec.Y));
            vvec.X = (float)((pl.Gcvec.X - cvec.X) * R_player_dis);
            vvec.Y = (float)(((pl.Gcvec.Y - cvec.Y) * R_player_dis));
        }
    }

    /// <summary>
    /// 自機ボス
    /// </summary>
    public class Boss5 : Chara
    {
        int ID;
        Vector2 basevvec;
        Random rand;
        Player pl;
        float rotspan = 1.0f;
        int dan_span = 1;
        private double R_player_dis_ps = 1.0f;
        private double R_player_dis = 1.0f;

        //自機の方向に敵機を飛ばす。
        public Boss5(CN Tn, Texture2D t2, Vector2 v2, Player pl, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            Tag = CN.boss;
            syometucon = 360;
            Score = 5000;
            this.pl = pl;
            rand = new Random();
            vvec = new Vector2(0, 15f);
            basevvec = new Vector2(0, 0);
            hp = 1100;
            ID = 0;
        }
        public override Cv move()
        {
            //消滅処理
            if (state == Cv.syometu)
            {
                return Cv.boss_outview;
            }
            if (dan_span-- == 0)
            {
                if (hp > 600)
                {
                    dan_span = rand.Next() % 30 + 10;
                    if (dan_span > 38) return Cv.super_dan1;
                    if (dan_span < 29) return Cv.engenelatethree;
                    return Cv.en2_bomb;
                }
                else
                {
                    dan_span = rand.Next() % 30;
                    if (dan_span < 8) return Cv.engenelatefour;
                    if (rand.Next() % 16 == 0) return Cv.hyper_dan;
                    return Cv.en12_bomb;
                }
            }

            //向き反転
            Move_mir();
            //回転設定
            Rotset();
            basevvec.X += 0.001f * (rand.Next() % 100) - 0.05f;
            basevvec.Y += 0.001f * (rand.Next() % 100) - 0.05f;

            if (Math.Abs(basevvec.X) > 3.0f)
                basevvec.X *= 0.5f;

            if (Math.Abs(basevvec.Y) > 3.0f)
                basevvec.X *= 0.5f;

            cvec += vvec + basevvec;

            if (cvec.X < pl.Gcvec.X) { ID = 1; }//弾は左上
            else { ID = 2; }//弾は右上

            return Cv.nul;
        }

        void Move_mir()
        {
            if (cvec.X < 230)
            {
                vvec.X = -vvec.X;
                basevvec = -basevvec;
            }
            else if (cvec.X > 570)
            {
                vvec.X = -vvec.X;
                basevvec = -basevvec;
            }
            if (cvec.Y < 50)
            {
                vvec.Y = -vvec.Y;
                basevvec = -basevvec;
            }
            else if (cvec.Y > 650)
            {
                vvec.Y = -vvec.Y;
                basevvec = -basevvec;
            }

            if (cvec.Y > pl.Gcvec.Y)
            {
                vvec.Y = -1.0f;
                basevvec.Y = 0;
            }
        }

        ///回転のセット。
        void Rotset()
        {//ベクトル
            var dx = pl.Gcvec.X - cvec.X;
            var dy = pl.Gcvec.Y - cvec.Y;
            //角度
            float deg = 0;
            //var deg = Math.Asin(dx / Math.Sqrt(dx*dx+dy*dy));
            if (dy > 0) deg = (float)Math.Atan(dy / dx);
            if (dy < 0) deg = (float)Math.Atan(-dx / dy);

            //自機の方向に徐々に向いてくる。

            //大砲が左
            if (ID == 1)
            {
                if (dy <= 0)
                    if (deg > rot - MathHelper.ToRadians(-180))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
                if (dy > 0)
                    if (deg > rot - MathHelper.ToRadians(-90))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
            }

            //大砲が右
            else if (ID == 2)
            {
                if (dy <= 0)
                    if (deg > rot - MathHelper.ToRadians(180))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
                if (dy > 0)
                    if (deg > rot - MathHelper.ToRadians(90))
                        rot += MathHelper.ToRadians(rotspan);
                    else
                        rot -= MathHelper.ToRadians(rotspan);
            }
            R_player_dis = R_player_dis_ps;
            R_player_dis = R_player_dis / Math.Sqrt((pl.Gcvec.X - cvec.X) * (pl.Gcvec.X - cvec.X) + (pl.Gcvec.Y - cvec.Y) * (pl.Gcvec.Y - cvec.Y));
            vvec.X = (float)((pl.Gcvec.X - cvec.X) * R_player_dis);
            vvec.Y = (float)(((pl.Gcvec.Y - cvec.Y) * R_player_dis));
        }

    }

    /// <summary>
    /// パワーアップアイテム。
    /// </summary>
    public class Item1 : Chara
    {

        public Item1(CN Tn, Texture2D t2, Vector2 v2, Audio ad, Game g, Texture2D[] anim) : base(Tn, t2, v2, ad, g, anim)
        {
            Tag = CN.system;
            Textnum = Tn;
            Itemnum = Tn;
        }

        public override Cv move()
        {

            //消滅処理
            if (state == Cv.syometu)
            {
                return Cv.outview;
                //sound
            }

            cvec.Y++;

            return Cv.nul;
        }

    }



    public class Syometu : Chara
    {
        const double R = 2.0f;
        public Syometu(CN Tn, Texture2D t2, Vector2 v2, Audio ad, Game g, Texture2D[] anim, int num) : base(Tn, t2, v2, ad, g, anim)
        {
            Random r = new Random();
            var v = 1.0f + 0.01f * (r.Next() % 100);
            scale.X = v;
            scale.Y = v;
            v = r.Next() % 360;
            Tag = CN.system;
            color = Color.White;
            //ad.SoundPlay(Audio.SN.misairubakuhatu);

            switch (num)
            {
                case 0:
                    rot = 20f + v;
                    var rotR = MathHelper.ToRadians(rot);
                    var vx = R * Math.Cos(rotR);
                    var vy = R * -Math.Sin(rotR);
                    vvec = new Vector2((float)vx, (float)vy);
                    break;
                case 1:
                    rot = 92f + v;
                    var rotR1 = MathHelper.ToRadians(rot);
                    var vx1 = R * Math.Cos(rotR1);
                    var vy1 = R * -Math.Sin(rotR1);
                    vvec = new Vector2((float)vx1, (float)vy1);
                    break;
                case 2:
                    rot = 164f + v;
                    var rotR2 = MathHelper.ToRadians(rot);
                    var vx2 = R * Math.Cos(rotR2);
                    var vy2 = R * -Math.Sin(rotR2);
                    vvec = new Vector2((float)vx2, (float)vy2);
                    break;
                case 3:
                    rot = 236f + v;
                    var rotR3 = MathHelper.ToRadians(rot);
                    var vx3 = R * Math.Cos(rotR3);
                    var vy3 = R * -Math.Sin(rotR3);
                    vvec = new Vector2((float)vx3, (float)vy3);
                    break;
                case 4:
                    rot = 308f + v;
                    var rotR4 = MathHelper.ToRadians(rot);
                    var vx4 = R * Math.Cos(rotR4);
                    var vy4 = R * -Math.Sin(rotR4);
                    vvec = new Vector2((float)vx4, (float)vy4);
                    break;
            }
        }
        public override Cv move()
        {
            //消滅処理
            syometucon--;
            if (syometucon == 0)
                return Cv.outview;

            cvec += vvec;
            color.A -= 15;
            scale.X -= 0.02f;
            scale.Y -= 0.02f;

            return Cv.nul;
        }
    }

}

