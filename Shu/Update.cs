using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.DirectX.DirectInput;

namespace Shu
{

    ///<summary>動作実装部
    ///キャラの実体を動かす部分
    ///グラフィック、オーディオの機能を
    ///使うときは、親クラスのGraphicsから使用。
    ///</summary>
    public class Update : Graphics
    {
        /// <summary>
        /// 永続無敵
        /// </summary>
        bool star = false;
        /// <summary>
        /// 一時無敵
        /// </summary>
        bool muteki = true;
        /// <summary>
        /// グローバルカウンタ
        /// </summary>
        int gcc = 0;

        bool boss = false;
        bool clear = false;
        int Cgc = 0;
        int Cstage = 0;

        int numclearStage = 0;


        int gameclear_wait = 90;
        int Width, Height;

        const float speed = 5.0f;



        Game game;
        Microsoft.Xna.Framework.Input.KeyboardState keya;
        Microsoft.Xna.Framework.Input.KeyboardState keyb;
        Microsoft.DirectX.DirectInput.JoystickState pada;
        Microsoft.DirectX.DirectInput.JoystickState padb;

        /// <summary>
        /// コントローラーデバイス
        /// </summary>
        Device joystick = null;
        /// <summary>
        /// デバイスのリスト
        /// </summary>
        DeviceList devList;



        int stagenum = 5;
        int ch_num1 = 0;

        ///<summary>キャラ出現情報
        ///出現gc, 出現キャラクタ + アイテム,出現座標x,出現座標y
        ///コンストラクタで初期化
        ///</summary>
		long[][] ch_appear;


        //アイテム定義
        public const int Itbufnum = 11;
        /// <summary>
        /// アイテムのバッファ。画面内に10個まで表示できる。
        /// </summary>
        Chara.CN[] Itbuf = new Chara.CN[Itbufnum];
        /// <summary>
        /// アイテムのバッファ初期出現位置。画面内に10個まで表示できる。
        /// </summary>
        Vector2[] Itvec = new Vector2[Itbufnum];

        //透明定義
        /// <summary>
        /// ダメージを負うと75フレーム間無敵となる。
        /// </summary>
        bool toumei = false;
        /// <summary>
        /// ダメージを負うと75フレーム間無敵となる。
        /// </summary>
        int toumeitim = 0;
        int toumeiset = 75;


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

        /// <summary>
        /// ゲームパッド取得
        /// </summary>
        public void GamePadInit()
        {
            try
            {
                devList = Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);
                foreach (DeviceInstance dev in devList)
                {
                    joystick = new Device(dev.InstanceGuid);
                    break;
                }
                if (joystick != null) joystick.SetDataFormat(DeviceDataFormat.Joystick);
                if (joystick != null) joystick.Acquire();
            }
            catch
            {
                joystick = null;
            }
        }

        /// <summary>
        /// キャラクタ出現変数セット
        /// </summary>
        void ch_appearset()
        {
            CharaAppear ca = new CharaAppear();
            ch_appear = new long[stagenum][];
            ch_appear[0] = ca.Stage1;
            ch_appear[1] = ca.Stage2;
            ch_appear[2] = ca.Stage3;
            ch_appear[3] = ca.Stage4;
            ch_appear[4] = ca.Stage5;
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
            if (scene == Scene.Stage) Stage();
            if (scene == Scene.Gameover) Gameover();
            if (scene == Scene.Ending) Ending();
        }

        ///<summary>
        ///キャラクタの参照Chararef[charanum]にnullを入れる。
        ///出現管理変数ch_num1の初期化
        ///player, temp, bgtop, bgbottomにnullをいれる。
        ///</summary>
        public void CharadelAll()
        {
            for (int i = 0; i < Chararef.Length; i++)
                Chararef[i] = null;

            for (int i = 0; i < Itbuf.Length; i++)
                Itbuf[i] = 0;
            for (int i = 0; i < Itvec.Length; i++)
                Itvec[i] = new Vector2(0, 0);
            ch_num1 = 0;
            //player = null;
            //temp = null;
            //bgtop = null;
            //bgbottom = null;

            gc = 0;
            boss = false;
            clear = false;
        }

        /// <summary>
        /// ステージアップ
        /// </summary>
        void Stageup()
        {
            numclearStage = stage;

            Console.WriteLine("{0}  {1}", numclearStage, score);

            Save();
            gameclear_wait = 90;
            player.init();
            CharadelAll();
            scene = Scene.Stage;
            state = State.Init;
            Chararef[Charanum - 2] = player;
            temp.GOcount = 120;
            stage++;
            audio.MusicPlay(stage);
            charaload();
            Cgc = 0;
            Cstage = 0;
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
                    bgtop.move();
                    bgbottom.move();


                    if (keyb.IsKeyDown(Keys.Space) || (joystick != null && padb.GetButtons()[0] == 128))
                    {
                        audio.SoundPlay(Audio.SN.button02a);
                        state = State.Enter;
                    }


                    if ((keyb.IsKeyDown(Keys.Up) && keya != keyb) || (joystick != null && (padb.Y == 0 && padb.Y != pada.Y)))
                    {
                        if (stage <= numclearStage) stage++;
                    }
                    if ((keyb.IsKeyDown(Keys.Down) && keya != keyb) || (joystick != null && (padb.Y == 65535 && padb.Y != pada.Y)))
                    {
                        if (stage > 1) stage--;
                    }
                    break;
                case State.Enter:
                    if (Fadeout())

                    {
                        player = new Player(Chara.CN.ziki, charatex[(int)CN.ziki], new Vector2(Width / 2, Height * 5 / 7), audio, game, joystick, animeData[(int)CN.ziki]);
                        Chararef[Charanum - 2] = player;
                        bgtop.scr_down(player.Ghp);
                        bgbottom.scr_down(player.Ghp);
                        scene = Scene.Stage;
                        state = State.Init;
                        if (stage < 2) stage = 1;
                        audio.MusicPlay(stage);
                        charaload();

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



        /*************************************************
		 * 
		 * 
		 *			メインの処理を書いていく
		 * 
		 * 
		 * 
		 * ************************************************/


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
                            while (gc <= gcc)
                            {
                                Charaappear(true);
                                gc++;
                            }
                            player.PowerUp();
                            player.PowerUp();
                        }
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
                    GC.Collect();
                    Appear();

                    Move();
                    Collision();

                    if (!boss) gc++;



                    break;
                case State.Enter:
                    Move();
                    Collision();
                    break;
            }
        }

        /*************************************************
		 * 
		 * 
		 *			ゲームオーバーの処理を書いていく
		 * 
		 * 
		 * 
		 * ************************************************/

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
                    Move();
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
                        Move();
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



        /*************************************************
		 * 
		 * 
		 *		     	雑多な処理を書いていく
		 * 
		 * 
		 * 
		 * ************************************************/

        /*
         * Save1データの構成
         * クリアステージ数(Int32),
         * 最高Score(Int64)
        */
        /// <summary>
        /// Save
        /// </summary>
        void Save()
        {
            try
            {
                FileStream fs = new FileStream("save.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite);
                BinaryReader br = new BinaryReader(fs);
                BinaryWriter bw = new BinaryWriter(fs);

                fs.Seek(0, SeekOrigin.Begin);

                int st = 0;
                ulong sc = 0;

                //ファイルがある場合
                if (fs.Length != 0)
                {
                    st = br.ReadInt32();
                    sc = (ulong)br.ReadUInt64();
                }

                Console.WriteLine("{0}  {1}", numclearStage, score);



                //ファイル書き込み処理
                fs.Seek(0, SeekOrigin.Begin);
                if (st < numclearStage)
                    bw.Write((int)numclearStage);
                else
                    fs.Seek(4, SeekOrigin.Current);

                if ((ulong)sc < (ulong)score)
                    bw.Write((ulong)score);
                //else
                //    bw.Write((ulong)sc);

                bw.Flush();
                br.Close();
                bw.Close();
                fs.Close();
            }
            catch
            {

            }
        }
        /// <summary>
        /// Load
        /// </summary>
        void Load()
        {
            try
            {
                FileStream fs = new FileStream("save.bin", FileMode.OpenOrCreate, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);

                fs.Seek(0, SeekOrigin.Begin);

                //ファイルが空の場合
                if (fs.Length == 0)
                {
                    br.Close();
                    fs.Close();
                    return;
                }

                int st = br.ReadInt32();
                ulong sc = (ulong)br.ReadUInt64();

                //ファイル読み込み処理
                fs.Seek(0, SeekOrigin.Begin);
                numclearStage = st;
                score = sc;


                viewscore = score;

                br.Close();
                fs.Close();
            }
            catch
            {

            }
        }

        /// <summary>
        /// プレイヤー消去。体力が残ると透明モードになる。
        /// </summary>
        public void playerdel()
        {
            audio.SoundPlay(Audio.SN.explosion1);
            //体力が残った
            if (!player.PowerDown())
            {
                SyometuSet(player.Gcvec);
                bgtop.scr_down(player.Ghp);
                bgbottom.scr_down(player.Ghp);
                toumei = true;
                toumeitim = toumeiset;
                return;
            }
            SyometuSet(player.Gcvec);
            audio.MusicStop();
            player.syometu(out Itbufck(Itbuf), out Itvec[Itbufcknum(Itbuf)]);
            state = State.Enter;
            gameclear_wait = 90;
        }

        /// <summary>
        /// キャラの透明解除判定、グラフィック変更。PowerDown()してtrue(hp > 0)なら実行される。
        /// </summary>
        void Toumei()
        {
            if (toumeitim > 0)
            {
                toumeitim--;
                if (toumeitim % 2 == 0)
                    player.Gcharatex = charatex[(int)CN.ziki];
                else
                    player.Gcharatex = charatex[(int)CN.toumei];

                return;
            }
            toumei = false;
            return;
        }

        /// <summary>
        /// フリーのアイテムバッファの参照を返す。空きがないときは配列の最後の参照を返す。
        /// </summary>
        /// <param name="It"></param>
        /// <returns></returns>
        ref Chara.CN Itbufck(Chara.CN[] It)
        {
            for (int i = 0; i < It.Length; i++)
                if (It[i] == 0)
                    return ref It[i];
            return ref It[It.Length - 1];
        }

        /// <summary>
        /// 値を格納しているアイテムバッファを返す。全て空きの時は配列の最後の参照を返す。
        /// </summary>
        /// <param name="It"></param>
        /// <returns></returns>
        ref Chara.CN Itbufckt(Chara.CN[] It)
        {
            for (int i = 0; i < It.Length; i++)
                if (It[i] != 0)
                    return ref It[i];
            return ref It[It.Length - 1];
        }


        /// <summary>
        /// フリーのアイテムバッファの配列の添え数を返す。空きがないときは配列の最後の添え字を返す。
        /// </summary>
        /// <param name="It"></param>
        /// <returns></returns>
        int Itbufcknum(Chara.CN[] It)
        {
            for (int i = 0; i < It.Length; i++)
                if (It[i] == 0)
                    return i;
            return Itbufnum - 1;
        }

        /// <summary>
        /// スコア加算、画面表示
        /// </summary>
        void ScoreUp(Chara ch)
        {
            score += (ulong)ch.Gscore;
            SyometuScoreset(ch);
        }


        void Move()
        {
            //全キャラクタ動作
            for (int i = 0; i < Chararef.Length; i++)
                if (Chararef[i] != null && Chararef[i] != player && Chararef[i] != temp)
                //Chararef[i].move()でキャラの行動を取得し、Enmoveで弾の生成とハンドル消去。
                {
                    Chararef[i].Animation();
                    if (Enmove(Chararef[i].move(), Chararef[i])) refbreak(Chararef[i]);
                }

            //プレイヤー動作
            if ((state == State.Active || (scene == Scene.Gameover && state == State.Enter)) && player != null)
            {
                if (joystick != null) player.Jb = padb;
                player.move();
                player.Animation();
                Toumei();
            }

            //プレイヤー消去
            if (scene == Scene.Stage && state == State.Enter && player != null)
                if (player.move() == Chara.Cv.outview)
                {
                    refbreak(player);
                    player = null;
                }

            //ゲームオーバー遷移処理
            if (!clear && state == State.Enter && temp.move() == Chara.Cv.outview)
            {
                if (keyb.IsKeyDown(Keys.Space) || (joystick != null && padb.GetButtons()[0] == 128))
                {
                    scene = Scene.Gameover;
                    state = State.Init;
                }
            }

        }

        /// <summary>
        /// 衝突判定全般
        /// </summary>
        void Collision()
        {
            //敵と自機弾の当たり判定
            for (int i = 0; i < Chararef.Length; i++)
                //ハンドルがnull キャラが敵の弾、アイテム、プレイヤー以外の時に衝突判定。 
                if (Collision(Chararef[i], player))
                {
                    Chararef[i].syometu(out Itbufck(Itbuf), out Itvec[Itbufcknum(Itbuf)]);
                    SyometuSet(Chararef[i].Gcvec);
                }

            if (scene == Scene.Stage)
            {
                //自機と敵弾と敵の当たり判定
                if (Collision(player, Chararef))
                    playerdel();

                //アイテムと自機の判定、v = Itemname
                var v = CollisionItem(player, Chararef);
                if (v != Chara.CN.nul)
                {
                    playerpowerup(v);
                    toumei = true;
                    toumeitim = toumeiset;
                }
            }
        }

        /// <summary>
        ///	アイテムパワーアップ
        /// </summary>
        /// <param name="Itemname"></param>
        void playerpowerup(Chara.CN Itemname)
        {
            switch (Itemname)
            {
                case Chara.CN.Item1:
                    player.PowerUp();
                    player.Gcolor = Color.Blue;
                    bgtop.scr_Speed++;
                    bgbottom.scr_Speed++;
                    bgtop.scr_up(player.Ghp);
                    bgbottom.scr_up(player.Ghp);
                    break;
            }
        }


        /// <summary>
        /// キャラクタ出現管理。ch_appear[ch_num1 * 4] == gc でキャラ生成。
        /// </summary>
        public void Appear()
        {
            //キャラクタ出現と、ハンドルの登録

            //データ終わりの時ch_num1 = 0
            if (ch_appear[stage - 1][ch_num1 * 4] == -1)
            {
                ch_num1 = 0;
                gc = 0;
                return;
            }

            //アイテムバッファからアイテムセット
            Itemappear();
            //ch_appear[ch_num1 * 4] == gc でキャラ生成。
            Charaappear(false);

        }

        void Itemappear()
        {
            for (int i = 0; i < Itbuf.Length; i++)
            {
                if (Itbuf[i] != 0)
                {
                    Chararefset(new Item1(Chara.CN.Item1, charatex[(int)CN.item1], Itvec[i], audio, game, animeData[(int)CN.item1]));

                    Itbuf[i] = 0;
                    Itvec[i] = new Vector2(0, 0);
                    break;
                }
            }
        }

        void Charaappear(bool test)
        {
            //ch_appear[ch_num1 * 4] == gc でキャラ生成。
            if (ch_appear[stage - 1][ch_num1 * 4] == (int)gc)
            {
                if (clear) return;
                long s = ch_appear[stage - 1][ch_num1 * 4 + 1] & 0xff;
                var v = (Chara.CN)ch_appear[(stage - 1)][(ch_num1 * 4 + 1)];
                var v2 = new Vector2(ch_appear[stage - 1][ch_num1 * 4 + 2], ch_appear[stage - 1][ch_num1 * 4 + 3]);


                if (test == false)
                {
                    switch (s)
                    {
                        case (int)Chara.CN.sruim:
                            Chararefset(new En1(v, charatex[(int)CN.sruim], v2, audio, game, animeData[(int)CN.sruim]));
                            break;
                        case (int)Chara.CN.tekiki:
                            Chararefset(new En2(v, charatex[(int)CN.tekiki], v2, player, audio, game, animeData[(int)CN.tekiki]));
                            break;
                        case (int)Chara.CN.houdai:
                            Chararefset(new En3(v, charatex[(int)CN.houdai], v2, player, audio, game, animeData[(int)CN.houdai]));
                            break;
                        case (int)Chara.CN.tekiki2:
                            Chararefset(new En4(v, charatex[(int)CN.tekiki], v2, player, audio, game, animeData[(int)CN.tekiki2]));
                            break;
                        case (int)Chara.CN.koiwa:
                            Chararefset(new En5(v, charatex[(int)CN.koiwa], v2, audio, game, animeData[(int)CN.koiwa]));
                            break;
                        case (int)Chara.CN.ooiwa:
                            Chararefset(new En5(v, charatex[(int)CN.ooiwa], v2, audio, game, animeData[(int)CN.ooiwa]));
                            break;
                        case (int)Chara.CN.inseki:
                            Chararefset(new En6(v, charatex[(int)CN.inseki], v2, audio, game, animeData[(int)CN.ooiwa]));
                            break;
                        case (int)Chara.CN.koinseki:
                            Chararefset(new En7(v, charatex[(int)CN.koiwa], v2, audio, game, animeData[(int)CN.koiwa]));
                            break;
                        case (int)Chara.CN.sara0:
                            Chararefset(new En8(v, charatex[(int)CN.sara], v2, audio, game, animeData[(int)CN.sara]));
                            break;
                        case (int)Chara.CN.sara1:
                            Chararefset(new En8(v, charatex[(int)CN.sara], v2, audio, game, animeData[(int)CN.sara]));
                            break;
                        case (int)Chara.CN.sara2:
                            Chararefset(new En8(v, charatex[(int)CN.sara], v2, audio, game, animeData[(int)CN.sara]));
                            break;
                        case (int)Chara.CN.sara3:
                            Chararefset(new En8(v, charatex[(int)CN.sara], v2, audio, game, animeData[(int)CN.sara]));
                            break;
                        case (int)Chara.CN.sara4:
                            Chararefset(new En8(v, charatex[(int)CN.sara], v2, audio, game, animeData[(int)CN.sara]));
                            break;
                        case (int)Chara.CN.sara5:
                            Chararefset(new En8(v, charatex[(int)CN.sara], v2, audio, game, animeData[(int)CN.sara]));
                            break;
                        case (int)Chara.CN.sara6:
                            Chararefset(new En8(v, charatex[(int)CN.sara], v2, audio, game, animeData[(int)CN.sara]));
                            break;
                        case (int)Chara.CN.sara7:
                            Chararefset(new En8(v, charatex[(int)CN.sara], v2, audio, game, animeData[(int)CN.sara]));
                            break;
                        case (int)Chara.CN.spiner:
                            Chararefset(new En9(v, charatex[(int)CN.spiner], v2, audio, game, animeData[(int)CN.spiner]));
                            break;
                        case (int)Chara.CN.spiner2:
                            Chararefset(new En10(v, charatex[(int)CN.spiner], v2, audio, game, animeData[(int)CN.spiner]));
                            break;
                        case (int)Chara.CN.tekiki3:
                            Chararefset(new En11(v, charatex[(int)CN.tekiki2], v2, player, audio, game, animeData[(int)CN.tekiki2]));
                            break;
                        case (int)Chara.CN.houdai2:
                            Chararefset(new En12(v, charatex[(int)CN.houdai2], v2, player, audio, game, animeData[(int)CN.houdai2]));
                            break;
                        case (int)Chara.CN.houdai3:
                            var houdai3 = new En3(v, charatex[(int)CN.houdai], v2, player, audio, game, animeData[(int)CN.houdai]);
                            houdai3.Ghp = 5;
                            Chararefset(houdai3);
                            break;
                        case (int)Chara.CN.houdai4:
                            Chararefset(new En13(v, charatex[(int)CN.houdai2], v2, player, audio, game, animeData[(int)CN.houdai2]));
                            break;
                        case (int)Chara.CN.hadou:
                            Chararefset(new En14(Chara.CN.tekidan2, charatex[(int)CN.hadou], v2, player.Gcvec, audio, game, animeData[(int)CN.hadou]));
                            break;
                        case (int)Chara.CN.ryusei:
                            Chararefset(new En14(Chara.CN.tekidan, charatex[(int)CN.ryusei], v2, player.Gcvec, audio, game, null));
                            break;
                        case (int)Chara.CN.Bboss1:
                            Chararefset(new BEn1(v, charatex[(int)CN.sruimboss], v2, player, audio, game, animeData[(int)CN.sruimboss]));
                            boss = true;
                            break;
                        case (int)Chara.CN.Bboss2:
                            Chararefset(new BEn2(v, charatex[(int)CN.houdai2], v2, player, audio, game, animeData[(int)CN.houdai2]));
                            boss = true;
                            break;
                        case (int)Chara.CN.Bboss3:
                            Chararefset(new BEn3(v, charatex[(int)CN.spiner], v2, player, audio, game, animeData[(int)CN.spiner]));
                            boss = true;
                            break;
                        case (int)Chara.CN.boss1:
                            Chararefset(new Boss1(v, charatex[(int)CN.boss1], v2, player, audio, game, animeData[(int)CN.boss1]));
                            boss = true;
                            break;
                        case (int)Chara.CN.boss2:
                            Chararefset(new Boss2(v, charatex[(int)CN.sruimboss], v2, player, audio, game, animeData[(int)CN.sruimboss]));
                            boss = true;
                            break;
                        case (int)Chara.CN.boss3:
                            Chararefset(new Boss3(v, charatex[(int)CN.boss1], v2, player, audio, game, animeData[(int)CN.boss1]));
                            boss = true;
                            break;
                        case (int)Chara.CN.boss4:
                            Chararefset(new Boss4(v, charatex[(int)CN.spinerboss], v2, player, audio, game, animeData[(int)CN.spinerboss]));
                            boss = true;
                            break;
                        case (int)Chara.CN.boss5:
                            Chararefset(new Boss5(v, charatex[(int)CN.zikiboss], v2, player, audio, game, animeData[(int)CN.zikiboss]));
                            boss = true;
                            break;
                            //case nn:
                    }
                }
                ch_num1++;
            }
            //同じgcにほかの出現キャラがないか判定。あれば再帰呼び出し。
            if (ch_appear[stage - 1][ch_num1 * 4] == (int)gc) Charaappear(test);
            return;
        }


        /// <summary>
        /// 敵と自機弾
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="pl"></param>
        /// <returns></returns>
        public bool Collision(Chara ch, Player pl)
        {
            //判定しないぜ判定。
            if (ch == null || pl == null || ch.GTextnum == Chara.CN.syometu ||
                ch.GTextnum == Chara.CN.tekidan ||
                ch.GTextnum == Chara.CN.Item1 ||
                ch == player || ch == bgbottom || ch == bgtop || ch == temp ||
                ch.Gstate == Chara.Cv.syometu) return false;


            for (int i = 0; i < pl.P_bomb.Length; i++)
            {
                if (pl.P_bomb[i] == null) continue;

                //敵のテクスチャの領域に入ると自機弾消去&true
                if (pl.P_bomb[i].Gcvec.X + pl.P_bomb[i].Gorigin.X > ch.Gcvec.X - (ch.Gorigin.X * 3 / 4) &&
                    pl.P_bomb[i].Gcvec.X - pl.P_bomb[i].Gorigin.X < ch.Gcvec.X + (ch.Gorigin.X * 3 / 4) &&
                    pl.P_bomb[i].Gcvec.Y + pl.P_bomb[i].Gorigin.Y > ch.Gcvec.Y - (ch.Gorigin.Y * 3 / 4) &&
                    pl.P_bomb[i].Gcvec.Y - pl.P_bomb[i].Gorigin.Y < ch.Gcvec.Y + (ch.Gorigin.X * 3 / 4))
                {
                    refbreak(pl.P_bomb[i]);
                    pl.P_bomb[i] = null;
                    //Hpを減らす。ゼロになったらtrue
                    var v = ch.Hpdec();
                    if (!v)
                    {
                        audio.SoundPlay(Audio.SN.misairubakuhatu);
                        ch.Gcolor = new Color(255, 64, 64, 255);
                    }
                    else
                        ScoreUp(ch);
                    return v;
                }
            }
            return false;
        }

        /// <summary>
        /// 自機と敵,敵弾
        /// </summary>
        /// <param name="pl"></param>
        /// <param name="eb"></param>
        /// <returns></returns>
        public bool Collision(Player pl, Chara[] eb)
        {
            //無敵判定
            if ((star & muteki) || pl == null || toumei || pl.Gstate == Chara.Cv.syometu) return false;

            for (int i = 0; i < eb.Length; i++)
            {
                //判定しない判定
                if (eb[i] == player || eb[i] == bgbottom || eb[i] == bgtop || eb[i] == temp ||
                    eb[i] == title || eb[i] == null ||
                    eb[i].GTextnum == Chara.CN.syometu ||
                    (int)eb[i].GTextnum > 0xff || eb[i].Gstate == Chara.Cv.syometu) continue;

                //自機の中心から4/8の領域に入るとtrue
                if (eb[i].Gcvec.X + eb[i].Gorigin.X > pl.Gcvec.X - (pl.Gorigin.X * 2 / 4) &&
                    eb[i].Gcvec.X - eb[i].Gorigin.Y < pl.Gcvec.X + (pl.Gorigin.X * 2 / 4) &&
                    eb[i].Gcvec.Y + eb[i].Gorigin.Y > pl.Gcvec.Y - (pl.Gorigin.Y * 2 / 4) &&
                    eb[i].Gcvec.Y - eb[i].Gorigin.Y < pl.Gcvec.Y + (pl.Gorigin.Y * 2 / 4))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 自機とアイテム
        /// </summary>
        /// <param name="pl"></param>
        /// <param name="it"></param>
        /// <returns></returns>
        Chara.CN CollisionItem(Player pl, Chara[] it)
        {
            for (int i = 0; i < it.Length; i++)
            {
                //判定しない判定
                if (it[i] == null || pl == null || pl.Gstate == Chara.Cv.syometu || (int)it[i].GTextnum <= 0xff) continue;
                //自機の中心から4/8の領域に入るとtrue
                if (it[i].Gcvec.X + it[i].Gorigin.X > pl.Gcvec.X - (pl.Gorigin.X * 2 / 4) &&
                    it[i].Gcvec.X - it[i].Gorigin.Y < pl.Gcvec.X + (pl.Gorigin.X * 2 / 4) &&
                    it[i].Gcvec.Y + it[i].Gorigin.Y > pl.Gcvec.Y - (pl.Gorigin.Y * 2 / 4) &&
                    it[i].Gcvec.Y - it[i].Gorigin.Y < pl.Gcvec.Y + (pl.Gorigin.Y * 2 / 4))
                {
                    var v = it[i].GItemnum;
                    refbreak(it[i]);
                    return v;
                }
            }
            return Chara.CN.nul;
        }

        /// <summary>
        /// 敵弾の生成。move()の返り値と敵キャラで生成する弾を変える。
        /// </summary>
        /// <param name="cv"></param>
        /// <param name="ch"></param>
        //引数　(キャラクター固有の(生成)アクション, 生成元のキャラ座標（を含んだ情報）)
        public void E_bombcreate(Chara.Cv cv, Chara ch)
        {
            if (player == null) return;
            switch (cv)
            {
                case Chara.Cv.en1_bomb:
                    Chararefset(new E_bomb1(Chara.CN.tekidan, charatex[(int)CN.tekidan], ch.Gcvec, player.Gcvec, audio, game, null));
                    return;
                case Chara.Cv.en2_bomb:
                    Chararefset(new E_bomb1(Chara.CN.tekidan, charatex[(int)CN.tekidan], ch.Gcvec, ch.Grot, audio, game, null));
                    return;
                case Chara.Cv.en3_bomb:
                    Chararefset(new E_bomb2(Chara.CN.tekidan, charatex[(int)CN.tekidan2], ch.Gcvec, player.Gcvec, audio, game, null));
                    return;
                case Chara.Cv.en12_bomb:
                    Random rand = new Random();
                    var val = rand.Next();
                    if (val % 2 == 0) Chararefset(new E_bomb1(Chara.CN.tekidan2, charatex[(int)CN.tekidan2], ch.Gcvec, ch.Grot + ((float)Math.PI / 2) * (rand.Next() % 4), audio, game, null, 2.0f));
                    if (val % 2 == 1) Chararefset(new E_bomb1(Chara.CN.tekidan, charatex[(int)CN.tekidan], ch.Gcvec, ch.Grot + ((float)Math.PI / 2) * (rand.Next() % 4), audio, game, null, 2.0f));
                    return;
                case Chara.Cv.engenerate:
                    Random r = new Random();
                    var vec = new Vector2(r.Next() % 400 + 200, r.Next() % 16);
                    Chararefset(new En8(Chara.CN.inseki, charatex[(int)CN.koiwa], vec, audio, game, animeData[(int)CN.koiwa]));
                    if (r.Next() % 5 > 2) Chararefset(new En14(Chara.CN.tekidan2, charatex[(int)CN.hadou], ch.Gcvec, player.Gcvec, audio, game, animeData[(int)CN.hadou]));
                    Bgflash();
                    return;
                case Chara.Cv.engenelatetwo:
                    Random r2 = new Random();
                    var vec2 = new Vector2(r2.Next() % 400 + 200, r2.Next() % 16);
                    Chararefset(new En2(Chara.CN.tekiki, charatex[(int)CN.tekiki], vec2, player, audio, game, animeData[(int)CN.tekiki]));
                    Chararefset(new E_bomb1(Chara.CN.tekidan, charatex[(int)CN.tekidan], ch.Gcvec, ch.Grot, audio, game, null));

                    if (r2.Next() % 8 == 0) Chararefset(new En9(Chara.CN.spiner, charatex[(int)CN.spiner], vec2, audio, game, animeData[(int)CN.spiner]));
                    Bgflash();
                    if (r2.Next() % 12 == 0)
                    {
                        Chararefset(new En8((Chara.CN.inseki + (int)Chara.CN.Item1), charatex[(int)CN.koiwa], vec2, audio, game, animeData[(int)CN.koiwa]));
                        return;
                    }
                    return;
                case Chara.Cv.super_dan1:
                    ch.Gcolor = Color.Yellow;
                    for (float i = 0; i < 360f; i += 10.0f)
                    {
                        if (i == 60f) i = 80f;
                        if (i == 160f) i = 180f;
                        if (i == 280f) i = 300f;
                        Chararefset(new E_bomb1(Chara.CN.tekidan2, charatex[(int)CN.tekidan2], ch.Gcvec, MathHelper.ToRadians(i), audio, game, null, 4.0f));
                    }
                    return;
                case Chara.Cv.hyper_dan:
                    ch.Gcolor = Color.Blue;
                    for (float i = 0; i < 360f; i += 40.0f)
                    {
                        Chararefset(new En14(Chara.CN.tekidan2, charatex[(int)CN.hadou], ch.Gcvec, MathHelper.ToRadians(i), audio, game, animeData[(int)CN.hadou], 4.7f));
                    }
                    return;
                case Chara.Cv.engenelatefinal:
                    Random f = new Random();
                    var vecf = new Vector2(f.Next() % 400 + 200, f.Next() % 16);
                    Chararefset(new En8(Chara.CN.inseki, charatex[(int)CN.koiwa], vecf, audio, game, animeData[(int)CN.koiwa]));

                    vecf = new Vector2(f.Next() % 400 + 200, f.Next() % 16);

                    Chararefset(new E_bomb1(Chara.CN.tekidan, charatex[(int)CN.tekidan2], ch.Gcvec, ch.Grot, audio, game, null, 4.0f));
                    Chararefset(new E_bomb1(Chara.CN.tekidan, charatex[(int)CN.tekidan2], ch.Gcvec, MathHelper.ToRadians(f.Next() % 360), audio, game, null, 4.0f));
                    if (f.Next() % 4 == 0) Chararefset(new En5(Chara.CN.ooiwa, charatex[(int)CN.ooiwa], vecf, audio, game, animeData[(int)CN.ooiwa]));
                    if (f.Next() % 8 == 0) Chararefset(new Item1(Chara.CN.Item1, charatex[(int)CN.item1], vecf, audio, game, animeData[(int)CN.item1]));

                    Bgflash();
                    return;
                case Chara.Cv.engenelatethree:
                    Random f3 = new Random();
                    var vecf3 = new Vector2(f3.Next() % 400 + 200, f3.Next() % 16);
                    Chararefset(new E_bomb1(Chara.CN.tekidan, charatex[(int)CN.tekidan], ch.Gcvec, ch.Grot + MathHelper.ToRadians((float)(f3.Next() % 20 - 10)), audio, game, null, 2.5f));
                    if (f3.Next() % 2 == 0) Chararefset(new E_bomb1(Chara.CN.tekidan, charatex[(int)CN.tekidan2], ch.Gcvec, MathHelper.ToRadians(f3.Next() % 360), audio, game, null, 2.5f));
                    if (f3.Next() % 2 == 0) Chararefset(new E_bomb1(Chara.CN.tekidan, charatex[(int)CN.tekidan2], ch.Gcvec, MathHelper.ToRadians(f3.Next() % 360), audio, game, null, 2.5f));


                    vecf = new Vector2(f3.Next() % 400 + 200, f3.Next() % 16);
                    if (f3.Next() % 8 == 0) Chararefset(new En7(Chara.CN.koiwa + (int)Chara.CN.Item1, charatex[(int)CN.koiwa], vecf, audio, game, animeData[(int)CN.koiwa]));
                    if (f3.Next() % 48 == 6) Chararefset(new En9(Chara.CN.spiner, charatex[(int)CN.spiner], vecf3, audio, game, animeData[(int)CN.spiner]));
                    if (f3.Next() % 8 == 3) Chararefset(new En2(Chara.CN.tekiki, charatex[(int)CN.tekiki2], vecf3, player, audio, game, animeData[(int)CN.tekiki2]));
                    if (f3.Next() % 8 == 5) Chararefset(new En1(Chara.CN.sruim, charatex[(int)CN.tekiki2], vecf3, audio, game, animeData[(int)CN.sruim]));

                    Bgflash();
                    return;
                case Chara.Cv.engenelatefour:
                    Random f4 = new Random();
                    var vecf4 = new Vector2(f4.Next() % 400 + 200, f4.Next() % 16);

                    vecf4 = new Vector2(f4.Next() % 400 + 200, f4.Next() % 16);
                    Chararefset(new E_bomb1(Chara.CN.tekidan, charatex[(int)CN.tekidan], ch.Gcvec, ch.Grot, audio, game, null, 3.0f));

                    if (f4.Next() % 16 == 2) Chararefset(new En5(Chara.CN.ooiwa, charatex[(int)CN.ooiwa], vecf4, audio, game, animeData[(int)CN.ooiwa]));
                    if (f4.Next() % 5 == 0) Chararefset(new En7(Chara.CN.koiwa + (int)Chara.CN.Item1, charatex[(int)CN.koiwa], vecf4, audio, game, animeData[(int)CN.koiwa]));
                    if (f4.Next() % 48 == 6)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            vecf4.Y += i * 2;
                            Chararefset(new En2(Chara.CN.tekiki, charatex[(int)CN.tekiki], vecf4, player, audio, game, animeData[(int)CN.tekiki2]));
                        }
                    }
                    if (f4.Next() % 8 == 3) Chararefset(new En2(Chara.CN.tekiki, charatex[(int)CN.tekiki2], vecf4, player, audio, game, animeData[(int)CN.tekiki2]));
                    if (f4.Next() % 8 == 5) Chararefset(new En1(Chara.CN.sruim, charatex[(int)CN.tekiki2], vecf4, audio, game, animeData[(int)CN.sruim]));

                    Bgflash();
                    return;

                    //case Chara.Cv.~~
                    //return;
            }
        }

        void Bgflash()
        {
            Random r = new Random();
            if (r.Next() % 16 == 0)
            {
                bgtop.Gcolor = Color.Yellow;
                bgbottom.Gcolor = Color.Yellow;
            }
        }

        /// <summary>
        /// 消滅アニメーションセット
        /// </summary>
        /// <param name="v"></param>
        void SyometuSet(Vector2 v)
        {
            audio.SoundPlay(Audio.SN.misairubakuhatu);
            for (int i = 0; i < 5; i++)
                Chararefset(new Syometu(Chara.CN.syometu, charatex[(int)CN.syometu], v, audio, game, animeData[(int)CN.syometu], i));
        }


        /// <summary>
        /// 敵キャラの行動管理。画面外の時true(ハンドル消去)、Cvの値で敵弾生成。
        /// </summary>
        /// <param name="cv"></param>
        /// <param name="ch"></param>
        /// <returns></returns>
        public bool Enmove(Chara.Cv cv, Chara ch)
        {
            if (cv == Chara.Cv.boss_outview)
            {
                BossOut(ch);
                return false;
            }
            if (cv == Chara.Cv.Boutview)
            {
                boss = false;
                Cgc = (int)gc;
                Cstage = stage;
                return true;
            }
            if (cv == Chara.Cv.outview) return true;
            if (cv == Chara.Cv.nul) return false;
            //弾生成時、呼び出し
            E_bombcreate(cv, ch);
            return false;
        }

        /// <summary>
        /// ボス退場、爆発
        /// </summary>
        void BossOut(Chara ch)
        {
            Random rand = new Random();
            ch.Gsyometucon--;
            if (ch.Gsyometucon % 5 == 0)
            {
                var v = new Vector2(rand.Next() % 100 - 50 + ch.Gcvec.X, rand.Next() % 200 - 100 + ch.Gcvec.Y);
                SyometuSet(v);
            }

            if (ch.Gsyometucon <= 0)
            {
                refbreak(ch);
                boss = false;
                StageClearTrigger();
            }
        }


        /// <summary>
        /// ステージクリアのトリガー
        /// </summary>
        void StageClearTrigger()
        {
            if (player == null || player.Gstate == Chara.Cv.syometu) return;
            audio.MusicStop();
            state = State.Active;
            scene = Scene.Gameover;
            clear = true;
            temp.GOcount = -2;
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
                    var end = Chararef[Chararef.Length - 5].move();
                    //高速移動
                    bgtop.move();
                    bgbottom.move();
                    bgtop.move();
                    bgbottom.move();
                    bgtop.move();
                    bgbottom.move();
                    bgtop.move();
                    bgbottom.move();
                    //エンディングのメッセージ
                    if (end != Chara.Cv.nul)
                    {
                        state = State.Enter;
                    }
                    break;
                case State.Enter:
                    player.upmove();
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

    }
}


