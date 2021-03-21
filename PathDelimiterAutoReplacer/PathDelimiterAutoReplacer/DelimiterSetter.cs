//
//  Copyright(C) 2021 Sankyuri
//  自由にコピーや改変などをすることができます。
//  You can freely copy, modify, etc.
//


using System;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;

namespace PathDelimiterAutoReplacer
{
    // 区切り文字設定フォーム
    public partial class DelimiterSetter : Form
    {

        string[] m_res;


    public DelimiterSetter(string a_bef, string a_aft)
        {
            InitializeComponent();
            Localization();
            ResourceManager rm = Properties.Resources.ResourceManager;
            this.Icon = (Icon)rm.GetObject("icon");

            // 戻り値仮設定
            m_res = new string[] { a_bef, a_aft };

            // テキストボックス内のテキストを設定
            TXBTarget.Text    = a_bef;
            TXBDelimiter.Text = a_aft;
        }


        // 日本語以外だったら英語にする
        public void Localization()
        {
            if ("ja-JP" != CultureInfo.CurrentCulture.Name)
            {
                this.Text      = "Delimiter Setting";
                label1.Text    = "Target";
                label2.Text    = "Delimiter";
                BTNSubmit.Text = "Submit";
            }
        }




        public string[] ShowWindow()
        {
            this.ShowDialog();
            return m_res;
        }


        private void BTNSubmit_Click(object sender, EventArgs e)
        {
            m_res[0] = TXBTarget.Text;
            m_res[1] = TXBDelimiter.Text;
            Close();
        }
    }
}
