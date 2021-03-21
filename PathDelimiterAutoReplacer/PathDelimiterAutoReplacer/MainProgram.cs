using System;
using System.Windows.Forms;

namespace PathDelimiterAutoReplacer
{
    static class MainProgram
    {

        [STAThread]
        static void Main()
        {
            var app = new Resident();
            Application.EnableVisualStyles();
            Application.Run(); // フォームを表示せずに実行
        }
    }
}
