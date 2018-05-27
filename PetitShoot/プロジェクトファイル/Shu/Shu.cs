using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Shu
{
    /// <summary>
    /// ゲーム・メイン部分
    /// </summary>
    public class Shu : Game
    {
        /// <summary>
        /// ゲーム
        /// </summary>
        Update update;
        
        /// <summary>
        /// テクスチャー
        /// </summary>
        //文字表示用スプライト定義。
        private Vector2 spVecTime = new Vector2(0, 0);  //フレームの経過時間などの表示座標用
        
        private int scrWidth = 800;   //ゲーム画面の幅を入れる変数。
        private int scrHeight = 800;  //ゲーム画面の高さを入れる変数。

        //コンストラクタ。
        public Shu()
        {
            AppPath.SetAppPath();
            Content.RootDirectory = "Content";
            update = new Update(this, scrWidth, scrHeight);

            this.IsMouseVisible = true;
        }

        /// <summary>
        /// ゲームの初期化並びにMonoGameの機能読込
        /// </summary>
        protected override void Initialize()
        {
            // 初期化コード。
            update.GamePadInit();
            base.Initialize();
        }

        /// <summary>
        /// コンテンツを読み込む
        /// </summary>
        protected override void LoadContent()
        {
            update.loadcontent();
        }
        

        /// <summary>
        /// ゲーム全体の更新
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            update.update();

            base.Update(gameTime);
        }

        /// <summary>
        /// ゲーム画面の描画
        /// </summary>
        /// <param name="gameTime">タイミング値のスナップショットを提供します。</param>
        protected override void Draw(GameTime gameTime)
        {
            update.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
