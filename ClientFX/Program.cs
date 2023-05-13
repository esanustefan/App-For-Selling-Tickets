using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using Networking.rpProtocols;
using Services;

namespace Baschet_Server_Client3
{
  static class Program
  {
    private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      try
      {
        log.Info("Entering application.");
        ServiceInterface service = new ServiceRpcProxy("127.0.0.1", 55557);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Console.WriteLine("Deschide form1");
        var form2 = new Form2(service);
        Application.Run(new Form1(service, form2));
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message);
      }
    }
  }
}
