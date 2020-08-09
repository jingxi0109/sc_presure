using System;

namespace sc_presure
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
  
 IPMI_Access.Foreloop();    
 //IPMI_Access.Foreloop();
 }
          static string Ex()
    {

string command = "write your command here";
string result = "";
using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
{
    proc.StartInfo.FileName = "/bin/bash";
    proc.StartInfo.Arguments = "-c \" " + command + " \"";
    proc.StartInfo.UseShellExecute = false;
    proc.StartInfo.RedirectStandardOutput = true;
    proc.StartInfo.RedirectStandardError = true;
    proc.Start();

    result += proc.StandardOutput.ReadToEnd();
    result += proc.StandardError.ReadToEnd();

    proc.WaitForExit();
}
return result;


    }
    }
}
