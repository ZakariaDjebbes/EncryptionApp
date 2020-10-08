using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EncryptionApp
{
    static class Program
    {
		/// <summary>
		/// Point d'entrée principal de l'application.
		/// </summary

        [STAThread]
        static void Main()
        {
			Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
