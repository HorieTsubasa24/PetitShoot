using Microsoft.Xna.Framework;

namespace Shu
{
    partial class Update
    {
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

    }
}
