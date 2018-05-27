using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
    public partial class Graphics : Game
    {
        private const int textotal = 36;
        private const int Blocktotal = 48;
        private const float speed = 5.0f;

        protected Texture2D[] charatex = new Texture2D[textotal];
        private string[] charaname =
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


        protected enum Scene
        {
            Title, Stage, Gameover, Ending

        }

        protected enum State
        {
            Init, Active, Enter

        }

        protected Scene scene = Scene.Title;
        protected State state = State.Init;

        /// <summary>
        /// アニメーションデータ
        /// </summary>
        protected Texture2D[][] animeData = new Texture2D[textotal][];


        // 操作クラス
        //クラス群
        public Audio audio;
        public Temp title;
        public Player player;
        public Temp temp;
        public BG bgtop;
        public BG bgbottom;
        //gc
        private SpriteFont spfont;
        //gameover
        private SpriteFont spfontgameover;
        //score
        protected ulong score = 0;
        protected ulong viewscore = 0;
        //stage
        public int stage = 1;

        private SpriteFontDraw sfd;
        private SyometuDraw sd;

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



        private GraphicsDeviceManager gdm;
        private SpriteBatch sp;
        private Game game;
        private Color color = new Color(0f, 0f, 0f, 1.0f);
        private float colorlight = 0f;
        private float lightspan = 0.06f;


        //ハンドル
        public const int Charanum = 1500;
        /// <summary>
        ///キャラクタのハンドル、Charaクラスのインスタンス参照
        /// </summary>
        public Chara[] Chararef = new Chara[Charanum];

        //コンストラクタで初期化
        private int Width;
        private int Height;


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
            FontLoad();

            // スコアフォントの初期化
            ScoreFontReset();

            // アニメーションデータ格納
            AnimationDataLoad();
            
            charaload();
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
            }
        }

        /// <summary>
        /// キャラハンドルの参照外し
        /// </summary>
        /// <param name="ch"></param>
        public void refbreak(Chara ch)
        {
            for (int i = 0; i < Charanum; i++)
                if (Chararef[i] == ch) Chararef[i] = null;
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
                    ShadowDraw(i);
                    //実像
                    RealBodyDraw(i);
                }

            //自機の弾
            PlayerBombDraw(p_bombnum);

            //sp.DrawString(spfont, "FPS=" + (1000 / gameTime.ElapsedGameTime.Milliseconds), spvectime, Color.Red);
            DrawSpritefont();

            sp.End();
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
        
    }
}
