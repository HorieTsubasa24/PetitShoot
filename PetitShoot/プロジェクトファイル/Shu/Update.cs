using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Shu
{

    ///<summary>動作実装部
    ///キャラの実体を動かす部分
    ///グラフィック、オーディオの機能を
    ///使うときは、親クラスのGraphicsから使用。
    ///</summary>
    public partial class Update : Graphics
    {
        Game game;


        ///<summary>
        ///コンストラクタ
        ///</summary>
        public Update(Game ga, int w, int h) : base(ga, w, h)
        {
            game = ga;
            Width = w;
            Height = h;

            audio = new Audio(game);

            Load();
            ch_appearset();
        }


        ///<summary>
        ///ここからタイトル、ステージ、ゲームオーバーを呼び出す。
        ///</summary>
        public void update()
        {
            keya = keyb;
            keyb = Microsoft.Xna.Framework.Input.Keyboard.GetState();

            if (title != null) Console.WriteLine("存在");

            if (joystick != null) pada = padb;
            if (joystick != null) padb = joystick.CurrentJoystickState;

            if (scene == Scene.Title) Title();
            else if (scene == Scene.Stage) Stage();
            else if (scene == Scene.Gameover) Gameover();
            else if (scene == Scene.Ending) Ending();
        }

        /*************************************************
		 * 
		 * 
		 *			タイトルの処理を書いていく
		 * 
		 * 
		 * 
		 * ************************************************/


        public void Title()
        {
            switch (state)
            {
                case State.Init:
                    if (Fadein())
                    {
                        state = State.Active;
                    }
                    break;
                case State.Active:
                    bgtop.Move();
                    bgbottom.Move();
                    StageSelecting();
                    break;
                case State.Enter:
                    if (Fadeout())
                    {
                        StageSetWhenGameStart();

                        if (title != null)
                        {
                            refbreak(title);
                            title = null;
                        }
                        score = 0;
                    }
                    break;

            }
        }
        

        public void Stage()
        {
            switch (state)
            {
                case State.Init:
                    if (stage == stagenum + 1)
                    {
                        scene = Scene.Ending;
                        return;
                    }
                    if (Fadein())
                    {
                        if (star == true)
                        {
                            // テスト用
                            while (gc <= gcc)
                            {
                                Charaappear(true);
                                gc++;
                            }
                            player.PowerUp();
                            player.PowerUp();
                        }
                        // 中間
                        if (Cgc > 0 && Cstage == stage)
                        {
                            while (gc <= Cgc)
                            {
                                Charaappear(true);
                                gc++;
                            }
                        }
                        state = State.Active;
                    }
                    break;
                case State.Active:
                    if (title != null)
                    {
                        refbreak(title);
                        title = null;
                    }
                    Appear();

                    upMove();
                    Collision();

                    if (!boss) gc++;
                    break;
                case State.Enter:
                    upMove();
                    Collision();
                    break;
            }
        }
        
        public void Gameover()
        {
            switch (state)
            {
                //ゲームオーバー時
                case State.Init:
                    if (!Fadeout())
                        return;


                    Console.WriteLine("{0}  {1}", numclearStage, score);

                    Save();
                    Load();

                    CharadelAll();
                    //stage = 1;
                    charaload();
                    Chararefset(title);
                    temp.GOcount = 120;
                    for (long i = 0; i < 500000; i++) { }
                    gc = 0;
                    scene = Scene.Title;
                    state = State.Init;
                    audio.MusicStop();
                    break;
                //ステージクリア時
                case State.Active:
                    upMove();
                    if (gameclear_wait > 0)
                    {
                        gameclear_wait--;
                        return;
                    }

                    //ステージクリア時ゲームクリア時以外の時
                    if (stage != stagenum) audio.MusicPlay3();
                    state = State.Enter;
                    break;
                case State.Enter:
                    if (gameclear_wait == 0)
                    {
                        upMove();
                        if (keyb.IsKeyDown(Keys.Space) || (joystick != null && padb.GetButtons()[0] == 128))
                        {
                            gameclear_wait = 90;
                        }
                        return;
                    }
                    //ゲームクリア時以外の時
                    if (stage != stagenum)
                    {
                        if (Fadeout()) Stageup();
                    }
                    //ゲームクリア時
                    else
                    {
                        if (Fadeout(true)) Stageup();
                    }
                    break;
            }
        }


        void Ending()
        {
            switch (state)
            {
                case State.Init:
                    if (Fadein())
                    {
                        Chararef[Chararef.Length - 5] = new EBG(Chara.CN.haikei_u, charatex[(int)CN.endroll], new Vector2(Width / 4, Height), audio, game, null);

                        audio.MusicPlay4();
                        state = State.Active;
                    }
                    break;
                case State.Active:
                    var end = Chararef[Chararef.Length - 5].Move();
                    //高速移動
                    bgtop.Move();
                    bgbottom.Move();
                    bgtop.Move();
                    bgbottom.Move();
                    bgtop.Move();
                    bgbottom.Move();
                    bgtop.Move();
                    bgbottom.Move();
                    //エンディングのメッセージ
                    if (end != Chara.Cv.nul)
                    {
                        state = State.Enter;
                    }
                    break;
                case State.Enter:
                    player.upMove();
                    if (!Fadeout(true))
                        return;
                    CharadelAll();
                    stage = 1;
                    charaload();
                    Chararefset(title);
                    for (long i = 0; i < 500000; i++) { }
                    gc = 0;
                    scene = Scene.Title;
                    state = State.Init;
                    audio.MusicStop();
                    break;
            }
        }


        /// <summary>
        /// 主なゲーム進行
        /// </summary>
        void upMove()
        {
            //全キャラクタ動作
            for (int i = 0; i < Chararef.Length; i++)
                if (Chararef[i] != null && Chararef[i] != player && Chararef[i] != temp)
                //Chararef[i].Move()でキャラの行動を取得し、Enmoveで弾の生成とハンドル消去。
                {
                    Chararef[i].Animation();
                    if (Enmove(Chararef[i].Move(), Chararef[i])) refbreak(Chararef[i]);
                }

            //プレイヤー動作
            if ((state == State.Active || (scene == Scene.Gameover && state == State.Enter)) && player != null)
            {
                if (joystick != null) player.Jb = padb;
                player.Move();
                player.Animation();
                Toumei();
            }

            //プレイヤー消去
            if (scene == Scene.Stage && state == State.Enter && player != null)
                if (player.Move() == Chara.Cv.outview)
                {
                    refbreak(player);
                    player = null;
                }

            //ゲームオーバー遷移処理
            if (!clear && state == State.Enter && temp.Move() == Chara.Cv.outview)
            {
                if (keyb.IsKeyDown(Keys.Space) || (joystick != null && padb.GetButtons()[0] == 128))
                {
                    scene = Scene.Gameover;
                    state = State.Init;
                }
            }

        }

    }
}


