//
//  Copyright(C) 2021 Sankyuri
//  自由にコピーや改変などをすることができます。
//  You can freely copy, modify, etc.
//


using System;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PathDelimiterAutoReplacer
{
    public class Resident : Form
    {

        [DllImport("user32.dll", SetLastError = true)]
        private extern static void AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        private extern static void RemoveClipboardFormatListener(IntPtr hwnd);


        private const int    WM_NCCREATE        = 0x81;
        private const int    WM_NCDESTROY       = 0x82;
        private const int    WM_CLIPBOARDUPDATE = 0x031D;




        private const string STR_PATH_START = "\".:\\\\"; // パス文字列の頭(正規表現)


        private NotifyIcon          m_icon;    // タスクトレイアイコン
        private ContextMenuStrip    m_menu;    // アイコンのメニュー

        private string m_strTitle = "パスの区切り文字自動置換器";
        private string m_strMenu1 = "区切り文字を設定(&S)";
        private string m_strMenu2 = "バルーンを表示(&B)";
        private string m_strMenu3 = "パスの区切り文字自動置換器について(&A)";
        private string m_strMenu4 = "終了(&Q)";

        private string m_strBlTitl = "コピーしたパスの区切り文字を変換しました。";
        private string m_strBlTxt1 = "\"";
        private string m_strBlTxt2 = "\" から \"";
        private string m_strBlTxt3 = "\" へ変換しました。";

        private string m_delimBefore = "\\"; // 置き換える区切り文字
        private string m_delimAfter  = "/";  // 区切り文字の代わりの文字
        
        private bool   m_isShowableBalloon = false; // バルーンを表示するか





        // 常駐アプリ
        public Resident()
        {
            InitializeComponent();
            Localization();
            CreateIcon();

            // クリップボード監視開始
            AddClipboardFormatListener(Handle);
        }




        private void InitializeComponent()
        {
            this.ShowInTaskbar = false;
//            timer = new Timer();
//            timer.Interval  = 100;
//            timer.Tick     += new EventHandler(Timer_Tick);
        }


        // 日本語以外だったら英語にする
        public void Localization()
        {
            if ("ja-JP" != CultureInfo.CurrentCulture.Name)
            {
                m_strTitle  = "Path Delimiter Auto Replacer";
                m_strMenu1  = "Setting Delimiter(&S)";
                m_strMenu2  = "Show Balloon(&B)";
                m_strMenu3  = "About(&A)";
                m_strMenu4  = "Quit(&Q)";

                m_strBlTitl = "Converted the copied path delimiter.";
                m_strBlTxt1 = "Converted From \"";
                m_strBlTxt2 = "\" to \"";
                m_strBlTxt3 = "\" .";
            }
        }




        // アイコンを作成
        private void CreateIcon()
        {
            // メニュー作成
            CreateIconMenu();
            // アイコン作成
            ResourceManager rm = Properties.Resources.ResourceManager;
            m_icon = new NotifyIcon()
            {
                Icon    = (Icon)rm.GetObject("icon"),
                Text    = m_strTitle,
                Visible = true,
            };
            // メニュー登録
            m_icon.ContextMenuStrip = m_menu;
        }


        // アイコンのメニューを作成
        private void CreateIconMenu()
        {
            m_menu = new ContextMenuStrip();
            AddIconMenu(m_strMenu1, new EventHandler(Icon_DelimiterSetter));
            AddIconMenu(m_strMenu2, new EventHandler(Icon_ShowBalloon));
            AddIconMenuSeparator();
            AddIconMenu(m_strMenu3, new EventHandler(Icon_ShowAbout));
            AddIconMenu(m_strMenu4, new EventHandler(Icon_Close));
        }


        // メニュー項目にセパレータを追加
        private void AddIconMenuSeparator()
        {
            m_menu.Items.Add(new ToolStripSeparator());
        }


        // メニュー項目を追加
        private void AddIconMenu(string a_text, EventHandler a_event)
        {
            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Text   = a_text;
            item.Click += a_event;
            m_menu.Items.Add(item);
        }




        // クリップボードの文字列を変更
        private void ChangeClipboardText()
        {
            var text = Clipboard.GetText();
            // 文字列がパスの場合は処理
            if (Regex.IsMatch(text, STR_PATH_START))
            {
                Clipboard.SetText(  text.Replace(m_delimBefore, m_delimAfter)  );
                ShowBalloon();
            }
        }


        // バルーン表示
        private void ShowBalloon()
        {
            if (m_isShowableBalloon)
            {
                m_icon.BalloonTipTitle = m_strBlTitl;
                m_icon.BalloonTipText  = GetBalloonText();
                m_icon.ShowBalloonTip(5000);
            }
        }


        // バルーンの文字列取得
        private string GetBalloonText()
        {
            return m_strBlTxt1 + m_delimBefore +
                   m_strBlTxt2 + m_delimAfter  +
                   m_strBlTxt3;
        }








        // ＊＊＊＊＊＊＊＊ フォームのイベントハンドラ ＊＊＊＊＊＊＊＊

        // ロード時
        protected override void OnLoad(EventArgs e)
        {
            //AddClipboardFormatListener(Handle);
            base.OnLoad(e);
        }


        // 破棄時
        protected override void Dispose(bool disposing)
        {
            // クリップボード監視終了
            RemoveClipboardFormatListener(Handle);
            base.Dispose(disposing);
        }


        // ウィンドウプロシージャ
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_CLIPBOARDUPDATE:
                    // クリップボード書き換え
                    ChangeClipboardText();
                    m.Result = IntPtr.Zero;
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }








        // ＊＊＊＊＊＊＊＊ メニュー項目のイベントハンドラ ＊＊＊＊＊＊＊＊

        // 区切り文字設定
        private void Icon_DelimiterSetter(object sender, EventArgs e)
        {
            var form = new DelimiterSetter(m_delimBefore, m_delimAfter);
            var text = form.ShowWindow();
            m_delimBefore = text[0];
            m_delimAfter  = text[1];
        }


        // バルーン表示切り替え
        private void Icon_ShowBalloon(object sender, EventArgs e)
        {
            m_isShowableBalloon = ( ! ((ToolStripMenuItem)sender).Checked);
            ((ToolStripMenuItem)sender).Checked = m_isShowableBalloon;
        }


        // アバウト表示
        private void Icon_ShowAbout(object sender, EventArgs e)
        {
            var form = new About();
            form.Show();
        }


        // 終了
        private void Icon_Close(object sender, EventArgs e)
        {
//            // クリップボード監視終了
//            RemoveClipboardFormatListener(Handle);
            Close();
            Application.Exit();
        }


    }
}
