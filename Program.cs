using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Enchante_Membership
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {            
            // Syncfusion Licensing
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzE4MDQxOEAzMjM1MmUzMDJlMzBCVk9VWndoK3VaMnRFVTdvWThXV3FlWTlaQ0Rpd0lWTUFkZU1kRG9sbGdBPQ==");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new EnchanteMembership());
        }
    }
}
