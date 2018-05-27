using Microsoft.Xna.Framework;
using Microsoft.DirectX.DirectInput;

namespace Shu
{
    partial class Update
    {
        //***************************************************
        //********************テスト用***********************
        //***************************************************

        //(star == true でgccをgcに代入する。)
        int gcc = 0;
        /// <summary>
        /// 永続無敵
        /// </summary>
        bool star = false;

        /// <summary>
        /// グローバルカウンタ
        /// </summary>
        long gc = 0;


        //***************************************************
        //**********************状態*************************
        //***************************************************

        /// <summary>
        /// 一時無敵(半透明状態)
        /// </summary>
        bool muteki = true;

        // ボス中
        bool boss = false;

        // クリア中
        bool clear = false;

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

        //***************************************************
        //*****************ステージクリア********************
        //***************************************************

        // C = クリア
        int Cgc = 0;
        int Cstage = 0;

        int numclearStage = 0;
        int gameclear_wait = 90;

        //***************************************************
        //*****************コントローラー********************
        //***************************************************

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

        //***************************************************
        //********************アイテム***********************
        //***************************************************
        
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


        // ステージの総数。
        int stagenum = 5;

        // 画面
        int Width, Height;

        // キャラクタ出現管理。ch_appear[ch_num1 * 4] == gc でキャラ生成。
        int ch_num1 = 0;

        ///<summary>キャラ出現情報
        ///出現gc, 出現キャラクタ + アイテム,出現座標x,出現座標y
        ///コンストラクタで初期化
        ///</summary>
        long[][] ch_appear;


    }
}
