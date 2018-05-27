using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.DirectX.DirectInput;

namespace Shu
{
    partial class Update
    {

        private void StageSelecting()
        {
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
        }

        private void StageSetWhenGameStart()
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
                FileStream fs = new FileStream(AppPath.path + "/save.bin", FileMode.OpenOrCreate, FileAccess.ReadWrite);
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
                FileStream fs = new FileStream(AppPath.path + "/save.bin", FileMode.OpenOrCreate, FileAccess.Read);
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
        /// 敵と自機弾
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="pl"></param>
        /// <returns></returns>
        public bool Collision(Chara ch, Player pl)
        {
            //判定しない判定。
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
                    bool v = ch.Hpdec();
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
        /// 敵弾の生成。Move()の返り値と敵キャラで生成する弾を変える。
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
                    Chararefset(new En8(Chara.CN.inseki + (int)Chara.CN.Item1, charatex[(int)CN.koiwa], vec, audio, game, animeData[(int)CN.koiwa]));
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


    }
}
