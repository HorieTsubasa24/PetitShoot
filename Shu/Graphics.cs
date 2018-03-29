using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Shu
{

    /*		グラフィック実装部
	 *			
	 *		グラフィックのテンプレートと、
	 *		キャラクタの実体が入っています。	
	 *		
	 *		他グラフィック系のメソッド。
	 * 
	 *		子クラスのUpdateから使用。
	 *								*/
    /// <summary>
    /// テクスチャの実体とグラフィック命令。
    /// </summary>
    public class Graphics : Game
    {

        public long gc = 0;

        const int textotal = 36;
        const int Blocktotal = 48;
        const float speed = 5.0f;

        public Texture2D[] charatex = new Texture2D[textotal];
        public string[] charaname =
        {
            "スライム", "皿", "自機", "自機弾", "小岩",
            "大岩", "敵機", "敵機", "敵弾", "背景", "背景下",
            "砂漠", "砂漠下", "氷上", "氷上下", "火星", "火星下", "背景", "背景下",
            "背景テンプレート", "砲台", "消滅", "Item1", "透明",
            "スピナー", "砲台複数", "敵弾黄色","波動弾","流星",
            "隕石","タイトル","Endroll",
            "敵機ボス", "スライムボス", "スピナーボス", "自機ボス"
        };

        public enum CN      //CharaName
        {
            sruim, sara, ziki, zikidan, koiwa,
            ooiwa, tekiki, tekiki2, tekidan, haikei_u, haikei_b,
            haikei_u2, haikei_b2, haikei_u3, haikei_b3, haikei_u4, haikei_b4, haikei_u5, haikei_b5,
            h_temp, houdai, syometu, item1, toumei,
            spiner, houdai2, tekidan2, hadou, ryusei,
            inseki, title, endroll,
            boss1, sruimboss, spinerboss, zikiboss
        }


        public enum Scene
        {
            Title, Stage, Gameover, Ending

        }

        public enum State
        {
            Init, Active, Enter

        }

        protected Scene scene = Scene.Title;
        protected State state = State.Init;

        /// <summary>
        /// アニメーションデータ
        /// </summary>
        public Texture2D[][] animeData = new Texture2D[textotal][];


        /*
		 
			操作クラス 

			 
			 */
        //クラス群
        public Audio audio;
        public Temp title;
        public Player player;
        public Temp temp;
        public BG bgtop;
        public BG bgbottom;
        //gc
        SpriteFont spfont;
        //gameover
        SpriteFont spfontgameover;
        //score
        protected ulong score = 0;
        protected ulong viewscore = 0;
        //stage
        public int stage = 1;

        SpriteFontDraw sfd;
        SyometuDraw sd;

        public class SpriteFontDraw
        {
            public Vector2 timev;
            public Vector2 gameoverv;
            public Vector2 stagev;
            public Vector2 scorev;
        }
        public class SyometuDraw
        {
            public const int SyometuScorenum = 300;
            public Vector2[] Scorevec = new Vector2[SyometuScorenum];
            public float[] Scorealpha = new float[SyometuScorenum];
            public int[] Score = new int[SyometuScorenum];
        }



        GraphicsDeviceManager gdm;
        SpriteBatch sp;
        Game game;
        Color color = new Color(0f, 0f, 0f, 1.0f);
        float colorlight = 0f;
        float lightspan = 0.06f;


        //ハンドル
        public const int Charanum = 1500;
        /// <summary>
        ///キャラクタのハンドル、Charaクラスのインスタンス参照
        /// </summary>
        public Chara[] Chararef = new Chara[Charanum];

        //コンストラクタで初期化
        int Width;
        int Height;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="ga"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        public Graphics(Game ga, int w, int h)
        {
            Width = w; Height = h;
            game = ga;
            gdm = new GraphicsDeviceManager(game);
            gdm.PreferredBackBufferWidth = w;//604, 667;
            gdm.PreferredBackBufferHeight = h;
            sfd = new SpriteFontDraw();
            sd = new SyometuDraw();


            for (int i = 0; i < Charanum; i++)
                Chararef[i] = null;
        }

        /// <summary>
        /// スクリーン横幅取得
        /// </summary>
        public int SCRWID
        {
            set {; }
            get { return Width/*gdm.GraphicsDevice.Viewport.Width*/; }
        }

        /// <summary>
        /// スクリーン縦幅取得
        /// </summary>
        public int SCRHEI
        {
            set {; }
            get { return Height/*gdm.GraphicsDevice.Viewport.Height*/; }
        }

        /// <summary>
        /// スプライトフォント、キャラ読み込み
        /// </summary>
        public void loadcontent()
        {
            sp = new SpriteBatch(gdm.GraphicsDevice);

            //文字スプライトの為にフォントの準備。
            spfont = game.Content.Load<SpriteFont>("spfont");
            spfontgameover = game.Content.Load<SpriteFont>("spfontg");
            sfd.timev = new Vector2(0, 0);
            sfd.gameoverv = new Vector2(213, 350);
            sfd.stagev = new Vector2(660, 115);
            sfd.scorev = new Vector2(640, 330);
            for (int i = 0; i < SyometuDraw.SyometuScorenum; i++)
            {
                sd.Scorealpha[i] = 0;
                sd.Scorevec[i] = new Vector2(0, 0);
                sd.Score[i] = 0;
            }

            //アニメーションデータ格納
            var j = "アニメーション/";

            animeData[(int)CN.sruim] = new Texture2D[6];
            var v = (int)CN.sruim;
            var n = "スライム";
            var lp = 0;
            for (lp = 0; lp < 6; lp++)
            {
                animeData[v][lp] = game.Content.Load<Texture2D>(n + j + n + (lp + 1).ToString());
            }

            animeData[(int)CN.item1] = new Texture2D[6];
            v = (int)CN.item1;
            n = "パワーアップ";
            lp = 0;
            for (lp = 0; lp < 6; lp++)
            {
                animeData[v][lp] = game.Content.Load<Texture2D>(n + j + "Item" + (lp + 1).ToString());
            }


            animeData[(int)CN.sara] = new Texture2D[9];
            v = (int)CN.sara;
            n = "皿";
            lp = 0;
            for (lp = 0; lp < 9; lp++)
            {
                animeData[v][lp] = game.Content.Load<Texture2D>(n + j + n + (lp + 1).ToString());
            }

            animeData[(int)CN.ziki] = new Texture2D[5];
            v = (int)CN.ziki;
            n = "自機";
            lp = 0;
            for (lp = 0; lp < 5; lp++)
            {
                animeData[v][lp] = game.Content.Load<Texture2D>(n + j + n + (lp + 1).ToString());
            }

            animeData[(int)CN.koiwa] = new Texture2D[6]; v = (int)CN.item1; ;
            v = (int)CN.koiwa;
            n = "小隕石";
            lp = 0;
            for (lp = 0; lp < 6; lp++)
            {
                animeData[v][lp] = game.Content.Load<Texture2D>(n + j + "小岩" + (lp + 1).ToString());
            }

            animeData[(int)CN.ooiwa] = new Texture2D[4]; ;
            v = (int)CN.ooiwa;
            n = "隕石";
            lp = 0;
            for (lp = 0; lp < 4; lp++)
            {
                animeData[v][lp] = game.Content.Load<Texture2D>(n + j + "大岩" + (lp + 1).ToString());
            }

            animeData[(int)CN.houdai] = new Texture2D[9]; ;
            v = (int)CN.houdai;
            n = "大砲";
            lp = 0;
            for (lp = 0; lp < 9; lp++)
            {
                animeData[v][lp] = game.Content.Load<Texture2D>(n + j + "砲台" + (lp + 1).ToString());
            }

            animeData[(int)CN.tekiki] = new Texture2D[9];
            v = (int)CN.tekiki;
            n = "敵機";
            lp = 0;
            for (lp = 0; lp < 9; lp++)
            {
                animeData[v][lp] = game.Content.Load<Texture2D>(n + "1" + j + n + (lp + 1).ToString());
            }

            animeData[(int)CN.tekiki2] = new Texture2D[9]; v = (int)CN.tekiki;
            v = (int)CN.tekiki2;
            n = "敵機";
            lp = 0;
            for (lp = 0; lp < 9; lp++)
            {
                animeData[v][lp] = game.Content.Load<Texture2D>(n + "2" + j + n + (lp + 1).ToString());
            }

            animeData[(int)CN.boss1] = new Texture2D[12];
            v = (int)CN.boss1;
            n = "敵機ボス";
            lp = 0;
            for (lp = 0; lp < 12; lp++)
            {
                var h = n + j + n + (lp + 1).ToString();
                animeData[v][lp] = game.Content.Load<Texture2D>(n + j + n + (lp + 1).ToString());
            }

            animeData[(int)CN.syometu] = new Texture2D[8];
            v = (int)CN.syometu;
            n = "消滅";
            lp = 0;
            for (lp = 0; lp < 8; lp++)
            {
                var h = n + j + n + (lp + 1).ToString();
                animeData[v][lp] = game.Content.Load<Texture2D>(n + j + n + (lp + 1).ToString());
            }

            animeData[(int)CN.spiner] = new Texture2D[8];
            v = (int)CN.spiner;
            n = "スピナー";
            lp = 0;
            for (lp = 0; lp < 8; lp++)
            {
                var h = n + j + n + (lp + 1).ToString();
                animeData[v][lp] = game.Content.Load<Texture2D>(n + j + n + (lp + 1).ToString());
            }

            animeData[(int)CN.spinerboss] = new Texture2D[8];
            v = (int)CN.spinerboss;
            n = "スピナーボス";
            lp = 0;
            for (lp = 0; lp < 8; lp++)
            {
                var h = n + j + n + (lp + 1).ToString();
                animeData[v][lp] = game.Content.Load<Texture2D>(n + j + n + (lp + 1).ToString());
            }

            animeData[(int)CN.sruimboss] = new Texture2D[6];
            v = (int)CN.sruimboss;
            n = "スライムボス";
            lp = 0;
            for (lp = 0; lp < 6; lp++)
            {
                var h = n + j + n + (lp + 1).ToString();
                animeData[v][lp] = game.Content.Load<Texture2D>(n + j + n + (lp + 1).ToString());
            }

            animeData[(int)CN.zikiboss] = new Texture2D[5];
            v = (int)CN.zikiboss;
            n = "自機ボス";
            lp = 0;
            for (lp = 0; lp < 5; lp++)
            {
                var h = n + j + n + (lp + 1).ToString();
                animeData[v][lp] = game.Content.Load<Texture2D>(n + j + n + (lp + 1).ToString());
            }

            animeData[(int)CN.hadou] = new Texture2D[4];
            v = (int)CN.hadou;
            n = "波動弾";
            lp = 0;
            for (lp = 0; lp < 4; lp++)
            {
                var h = n + j + n + (lp + 1).ToString();
                animeData[v][lp] = game.Content.Load<Texture2D>(n + j + n + (lp + 1).ToString());
            }

            charaload();
        }



        /// <summary>
        /// ハンドルChararef[Chara総数] にキャラを読み込み
        /// </summary>
        public void charaload()
        {
            //グラフィックス保持
            for (int i = 0; i < textotal; i++)
                charatex[i] = game.Content.Load<Texture2D>(charaname[i]);

            //背景セット
            if (temp == null) temp = new Temp(Chara.CN.h_temp, charatex[(int)CN.h_temp], new Vector2(0f, 0f), audio, game, null);
            if (bgtop == null) bgtop = new BG(Chara.CN.haikei_u, charatex[(int)CN.haikei_u + 2 * (stage - 1)], new Vector2(Width / 4, -Height), audio, game, null);
            if (bgbottom == null) bgbottom = new BG(Chara.CN.haikei_b, charatex[(int)CN.haikei_b + 2 * (stage - 1)], new Vector2(Width / 4, 0f), audio, game, null);


            bgtop.Gcharatex = charatex[(int)CN.haikei_u + 2 * (stage - 1)];
            bgbottom.Gcharatex = charatex[(int)CN.haikei_b + 2 * (stage - 1)];
            if (title == null) title = new Temp(Chara.CN.system, charatex[(int)CN.title], new Vector2(200f, 0f), audio, game, null);


            if (stage == 6)
            {
                bgtop.Gcharatex = charatex[(int)CN.haikei_u];
                bgbottom.Gcharatex = charatex[(int)CN.haikei_b];
            }

            Chararef[0] = bgtop;
            Chararef[1] = bgbottom;
            if (state == State.Init && (scene == Scene.Title ||
                scene == Scene.Gameover || scene == Scene.Ending)) Chararef[Chararef.Length - 2] = title;
            //テンプレートは最前面に
            Chararef[Chararef.Length - 1] = temp;
        }


        /// <summary>
        /// ハンドルにキャラを登録(フォント以外のキャラ)
        /// </summary>
        /// <param name="ch"></param>
        public void Chararefset(Chara ch)
        {
            for (int i = 0; i < Chararef.Length; i++)
            {
                //キャラ登録
                if (Chararef[i] == null)
                {
                    Chararef[i] = ch;

                    Console.WriteLine(i);
                    return;
                }
                if (i >= Chararef.Length)
                {
                    Console.WriteLine("キャラ数の際限を超えました。");
                    game.Exit();
                }
                /*
                if (Chararef[i] != null) Console.WriteLine("Textnum" + Chararef[i].GTextnum);
                if (Chararef[i] != null)
                {
                    if (Chararef[i].GAnimeData != null) Console.WriteLine("an" + Chararef[i].GAnimeData.ToString());
                    else Console.WriteLine("Noan");
                }
                */
            }
        }


        /// <summary>
        /// キャラハンドルの参照外し
        /// </summary>
        /// <param name="ch"></param>
        public void refbreak(Chara ch)
        {
            for (int i = 0; i < Charanum; i++)
            {
                if (Chararef[i] == ch) Chararef[i] = null;
            }
        }

        /// <summary>
        /// フェードインが完了したらtrue
        /// </summary>
        /// <returns></returns>
        public bool Fadein()
        {
            colorlight += lightspan;
            System.Console.WriteLine(colorlight);
            if (colorlight > 1.0f)
            {
                colorlight = 1.0f;
                color = new Color(colorlight, colorlight, colorlight, 1.0f);
                for (int i = 0; i < Chararef.Length; i++)
                    if (Chararef[i] != null) Chararef[i].Gcolor = color;
                return true;
            }
            color = new Color(colorlight, colorlight, colorlight, 1.0f);
            for (int i = 0; i < Chararef.Length; i++)
                if (Chararef[i] != null) Chararef[i].Gcolor = color;
            return false;
        }

        /// <summary>
        /// フェードアウトが完了したらtrue
        /// </summary>
        /// <returns></returns>
        public bool Fadeout()
        {
            colorlight -= lightspan;
            if (colorlight < 0.0f)
            {
                colorlight = 0.0f;
                color = new Color(colorlight, colorlight, colorlight, 1.0f);
                for (int i = 0; i < Chararef.Length; i++)
                    if (Chararef[i] != null) Chararef[i].Gcolor = color;
                return true;
            }
            color = new Color(colorlight, colorlight, colorlight, 1.0f);
            for (int i = 0; i < Chararef.Length; i++)
                if (Chararef[i] != null) Chararef[i].Gcolor = color;
            return false;
        }

        /// <summary>
        /// ゆっくりフェードアウト
        /// </summary>
        /// <param name="bl"></param>
        /// <returns></returns>
        public bool Fadeout(bool bl)
        {
            if (bl == false) return Fadeout();

            colorlight -= lightspan / 4;
            if (colorlight < 0.0f)
            {
                colorlight = 0.0f;
                color = new Color(colorlight, colorlight, colorlight, 1.0f);
                for (int i = 0; i < Chararef.Length; i++)
                    if (Chararef[i] != null) Chararef[i].Gcolor = color;
                return true;
            }
            color = new Color(colorlight, colorlight, colorlight, 1.0f);
            for (int i = 0; i < Chararef.Length; i++)
                if (Chararef[i] != null) Chararef[i].Gcolor = color;
            return false;
        }


        /// <summary>
        /// ハンドルのグラフィックと自機弾、スプライトフォントの描画
        /// </summary>
        /// <param name="gameTime"></param>
        public new void Draw(GameTime gameTime)
        {
            const int p_bombnum = 60;
            game.GraphicsDevice.Clear(Color.Black); //毎フレーム画面を単色でクリアする。


            sp.Begin();


            //キャラクタ全体
            for (int i = 0; i < Charanum; i++)
                if (Chararef[i] != null)
                {
                    //影
                    if (stage != 1 && stage != 5 && stage != 6 && Chararef[i].Tag != Chara.CN.system)
                        sp.Draw(Chararef[i].Gcharatex, Chararef[i].Gcvec + new Vector2(20, 20), null, new Color(32, 32, 32, 128), Chararef[i].Grot,
                            Chararef[i].Gorigin, Chararef[i].Gscale, SpriteEffects.None, 0);
                    //実像
                    sp.Draw(Chararef[i].Gcharatex, Chararef[i].Gcvec, null, Chararef[i].Gcolor, Chararef[i].Grot,
                            Chararef[i].Gorigin, Chararef[i].Gscale, SpriteEffects.None, 0);
                }

            //自機の弾
            for (int i = 0; i < p_bombnum; i++)
                if (player != null && player.P_bomb[i] != null)
                    sp.Draw(player.P_bomb[i].Gcharatex, player.P_bomb[i].Gcvec, null, player.P_bomb[i].Gcolor,
                            player.P_bomb[i].Grot, player.P_bomb[i].Gorigin, player.P_bomb[i].Gscale, SpriteEffects.None, 0);


            //sp.DrawString(spfont, "FPS=" + (1000 / gameTime.ElapsedGameTime.Milliseconds), spvectime, Color.Red);
            DrawSpritefont();

            sp.End();
        }


        /// <summary>
        /// 文字描画
        /// </summary>
        void DrawSpritefont()
        {
            //sp.DrawString(spfont, "GC=" + gc, sfd.timev, Color.Red);        
            sp.DrawString(spfontgameover, stage.ToString(), sfd.stagev, color);
            ScoreDraw();
            SyometuScoreview();
            if (temp.GOcount == -1) sp.DrawString(spfontgameover, "Game Over", sfd.gameoverv, color);
            if (temp.GOcount == -2) sp.DrawString(spfontgameover, "Stage Clear", sfd.gameoverv, color);
        }

        void ScoreDraw()
        {
            if (score > viewscore)
            {
                viewscore += 100;
            }
            else if (score != viewscore)
                viewscore = score;
            sp.DrawString(spfont, viewscore.ToString("00000000"), sfd.scorev, color);
        }

        /// <summary>
        /// 消滅時のスコアセット
        /// Updateから呼び出し
        /// </summary>
        /// <param name="ch"></param>
        protected void SyometuScoreset(Chara ch)
        {
            for (int i = 0; i < sd.Score.Length; i++)
            {
                if (sd.Scorealpha[i] <= 0)
                {
                    sd.Score[i] = ch.Gscore;
                    sd.Scorevec[i] = ch.Gcvec;
                    sd.Scorealpha[i] = 1.0f;
                    return;
                }
            }
        }

        /// <summary>
        /// 消滅時のスコア表示
        /// Draw()から呼び出し
        /// </summary>
        /// <param name="ch"></param>
        protected void SyometuScoreview()
        {
            for (int i = 0; i < sd.Score.Length; i++)
            {
                if (sd.Scorealpha[i] > 0)
                {
                    var v = SyometuScoreColor(i);
                    sp.DrawString(spfont, sd.Score[i].ToString(), sd.Scorevec[i], v);
                    sd.Scorealpha[i] -= 0.05f;
                    sd.Scorevec[i].Y -= 1.2f;
                }
            }
            return;
        }

        Color SyometuScoreColor(int i)
        {
            switch (sd.Score[i])
            {
                case 100:
                    return new Color(Color.White, sd.Scorealpha[i]);
                case 200:
                    return new Color(Color.Blue, sd.Scorealpha[i]);
                case 300:
                    return new Color(Color.Green, sd.Scorealpha[i]);
                case 500:
                    return new Color(Color.YellowGreen, sd.Scorealpha[i]);
                case 1000:
                    return new Color(Color.Yellow, sd.Scorealpha[i]);
                default:
                    return new Color(Color.PowderBlue, sd.Scorealpha[i]);

            }
        }


    }
}
