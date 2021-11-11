using KargoDagitimSistemi._1.GUI;
using System;
using System.Windows.Forms;

namespace KargoDagitimSistemi
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GirisFrm());
        }
    }
}
