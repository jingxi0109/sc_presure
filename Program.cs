using System;
using System.Threading;

namespace sc_presure {
   class Program {
      static void Main (string[] args) {
         Te ();

      }
      static void Te () {
         Console.WriteLine ("Hello World!");
         //IPMI_Access.Foreloop();
         //  Console.WriteLine (LSEX ());
         //IPMI_Access.Foreloop();
         for (int i = 0; i < 1; i++) {
            //  Console.WriteLine (common_cmd ("-c ls","/bin/bash"));
            //  Thread.Sleep (300);
            //  Console.WriteLine("----------------");
            //  Console.WriteLine (common_cmd ("-c ifconfig","/bin/bash"));
            //  Thread.Sleep (300);
            Console.WriteLine ("----------------");
            Console.WriteLine (common_cmd ("-c  lan print 1", "/bin/ipmitool"));
            Thread.Sleep (300);
            Console.WriteLine ("----------------");
            Console.WriteLine (common_cmd ("-c  mc info", "/bin/ipmitool"));
            Thread.Sleep (300);
            Console.WriteLine ("----------------");
            Console.WriteLine (common_cmd ("-c  fru ", "/bin/ipmitool"));
            Thread.Sleep (300);
            Console.WriteLine ("----------------");
            Console.WriteLine (common_cmd ("-c  dcmi power reading ", "/bin/ipmitool"));
            Thread.Sleep (300);
            Console.WriteLine ("----------------");
            Console.WriteLine (common_cmd ("-c  sdr ", "/bin/ipmitool"));
            Thread.Sleep (300);
            Console.WriteLine ("----------------");
            Console.WriteLine (common_cmd ("  -t BIOS ", "/sbin/dmidecode"));
            Thread.Sleep (300);
            Console.WriteLine ("----------------");
            Console.WriteLine (common_cmd ("  -t system ", "/sbin/dmidecode"));
            Thread.Sleep (300);
            Console.WriteLine ("----------------");
            Console.WriteLine (common_cmd ("  -t baseboard ", "/sbin/dmidecode"));
            Thread.Sleep (300);
            Console.WriteLine ("----------------");
            Console.WriteLine (common_cmd ("  -t processor ", "/sbin/dmidecode"));
            Thread.Sleep (300);
            Console.WriteLine ("----------------");
            Console.WriteLine (common_cmd ("  -t memory ", "/sbin/dmidecode"));
            Thread.Sleep (300);
            Console.WriteLine ("----------------");
            Console.WriteLine (common_cmd ("  -t slot ", "/sbin/dmidecode"));
            Thread.Sleep (300);
            Console.WriteLine ("----------------");
            //    Console.WriteLine (common_cmd ("  |grep -i power ","/sbin/dmidecode"));

         }
      }
  
      static string common_cmd (string cmd, string filename) {

         string command = cmd; //"write your command here";
         string filenamel = filename;
         string result = "";
         using (System.Diagnostics.Process proc = new System.Diagnostics.Process ()) {
            proc.StartInfo.FileName = filenamel; //"/bin/bash";
            proc.StartInfo.Arguments = "  " + command; //"-c  " + command;// + @" -a | grep -i  'inet ' ";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.Start ();

            result += proc.StandardOutput.ReadToEnd ();
            result += proc.StandardError.ReadToEnd ();

            proc.WaitForExit ();
         }
         return result;
      }

   }
   public class Distor{
      public static List<string,string> RowRecord(string raw_rec){
        var sslist =new List<string,string>();
        
         
      }
   }
}