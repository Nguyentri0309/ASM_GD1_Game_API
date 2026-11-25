using System.Windows.Forms;

namespace GameClient_WinForms;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new Forms.FrmLogin());
    }
}
