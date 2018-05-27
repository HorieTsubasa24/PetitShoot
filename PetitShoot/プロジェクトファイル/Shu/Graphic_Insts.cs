using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Shu
{
    // privateメソッド
    partial class Graphics
    {
        private void FontLoad()
        {
            spfont = game.Content.Load<SpriteFont>("spfont");
            spfontgameover = game.Content.Load<SpriteFont>("spfontg");
            sfd.timev = new Vector2(0, 0);
            sfd.gameoverv = new Vector2(213, 350);
            sfd.stagev = new Vector2(660, 115);
            sfd.scorev = new Vector2(640, 330);
        }

        private void ScoreFontReset()
        {
            for (int i = 0; i < SyometuDraw.SyometuScorenum; i++)
            {
                sd.Scorealpha[i] = 0;
                sd.Scorevec[i] = new Vector2(0, 0);
                sd.Score[i] = 0;
            }
        }

        private void AnimationDataLoad()
        {
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

            animeData[(int)CN.koiwa] = new Texture2D[6]; v = (int)CN.item1;
            v = (int)CN.koiwa;
            n = "小隕石";
            lp = 0;
            for (lp = 0; lp < 6; lp++)
            {
                animeData[v][lp] = game.Content.Load<Texture2D>(n + j + "小岩" + (lp + 1).ToString());
            }

            animeData[(int)CN.ooiwa] = new Texture2D[4];
            v = (int)CN.ooiwa;
            n = "隕石";
            lp = 0;
            for (lp = 0; lp < 4; lp++)
            {
                animeData[v][lp] = game.Content.Load<Texture2D>(n + j + "大岩" + (lp + 1).ToString());
            }

            animeData[(int)CN.houdai] = new Texture2D[9];
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
        }


        /// <summary>
        /// ハンドルChararef[Chara総数] にキャラを読み込み
        /// </summary>
        protected void charaload()
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

        private void ShadowDraw(int i)
        {
            if (stage != 1 && stage != 5 && stage != 6 && Chararef[i].Tag != Chara.CN.system)
                sp.Draw(Chararef[i].Gcharatex, Chararef[i].Gcvec + new Vector2(20, 20), null, new Color(32, 32, 32, 128), Chararef[i].Grot,
                    Chararef[i].Gorigin, Chararef[i].Gscale, SpriteEffects.None, 0);
        }

        private void RealBodyDraw(int i)
        {
            sp.Draw(Chararef[i].Gcharatex, Chararef[i].Gcvec, null, Chararef[i].Gcolor, Chararef[i].Grot,
                    Chararef[i].Gorigin, Chararef[i].Gscale, SpriteEffects.None, 0);
        }

        private void PlayerBombDraw(int p_bombnum)
        {
            for (int i = 0; i < p_bombnum; i++)
                if (player != null && player.P_bomb[i] != null)
                    sp.Draw(player.P_bomb[i].Gcharatex, player.P_bomb[i].Gcvec, null, player.P_bomb[i].Gcolor,
                            player.P_bomb[i].Grot, player.P_bomb[i].Gorigin, player.P_bomb[i].Gscale, SpriteEffects.None, 0);
        }


        /// <summary>
        /// 文字描画
        /// </summary>
        private void DrawSpritefont()
        {
            //sp.DrawString(spfont, "GC=" + gc, sfd.timev, Color.Red);        
            sp.DrawString(spfontgameover, stage.ToString(), sfd.stagev, color);
            ScoreDraw();
            SyometuScoreview();
            if (temp.GOcount == -1) sp.DrawString(spfontgameover, "Game Over", sfd.gameoverv, color);
            if (temp.GOcount == -2) sp.DrawString(spfontgameover, "Stage Clear", sfd.gameoverv, color);
        }

        private void ScoreDraw()
        {
            if (score > viewscore)
                viewscore += 100;
            else if (score != viewscore)
                viewscore = score;

            sp.DrawString(spfont, viewscore.ToString("00000000"), sfd.scorev, color);
        }


        private Color SyometuScoreColor(int i)
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
