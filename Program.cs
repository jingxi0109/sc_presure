using System;

namespace sc_presure {
    class Program {
        static void Main (string[] args) {
            Console.WriteLine ("Hello World!");
            //IPMI_Access.Foreloop();    
            //  Console.WriteLine (LSEX ());
            //IPMI_Access.Foreloop();
            for (int i = 0; i < 20; i++)
            {
                 Console.WriteLine (common_cmd (" ls"));     
                 Console.WriteLine (common_cmd (" ifconfig"));     
            }
           
               }
        static string Ex () {

            string command = "ifconfig"; //"write your command here";
            string result = "";
            using (System.Diagnostics.Process proc = new System.Diagnostics.Process ()) {
                proc.StartInfo.FileName = "/bin/bash";
                proc.StartInfo.Arguments = "-c \" " + command + @" -a | grep -i  'inet ' ";
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
        static string LSEX () {

            string command = "ls"; //"write your command here";
            string result = "";
            using (System.Diagnostics.Process proc = new System.Diagnostics.Process ()) {
                proc.StartInfo.FileName = "/bin/bash";
                proc.StartInfo.Arguments = "-c " + command; //"-c  " + command;// + @" -a | grep -i  'inet ' ";
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
        static string common_cmd (string cmd) {

            string command = cmd; //"write your command here";
            string result = "";
            using (System.Diagnostics.Process proc = new System.Diagnostics.Process ()) {
                proc.StartInfo.FileName = "/bin/bash";
                proc.StartInfo.Arguments = "-c " + command; //"-c  " + command;// + @" -a | grep -i  'inet ' ";
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
}