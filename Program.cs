using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.IO;

namespace sc_presure {
   class Program {
      static void Main (string[] args) {
         GGExe (Te ());

      }
      static GG_info Te () {
         GG_info GGG = new GG_info ();
         //    Console.WriteLine ("Hello World!");
         //IPMI_Access.Foreloop();
         //  Console.WriteLine (LSEX ());
         //IPMI_Access.Foreloop();
         //  for (int i = 0; i < 1; i++) {
         //  Console.WriteLine (common_cmd ("-c ls","/bin/bash"));
         //  Thread.Sleep (300);
         //  Console.WriteLine("----------------");
         //  Console.WriteLine (common_cmd ("-c ifconfig","/bin/bash"));
         //  Thread.Sleep (300);
         Console.WriteLine ("----------------");
         // Console.WriteLine (common_cmd ("-c  lan print 1", "/bin/ipmitool"));
         var res1 = Des_tor.RowRecord (common_cmd ("-c  lan print 1", "/bin/ipmitool"));
         var res2 = Des_tor.RowRecord (common_cmd ("-c  mc info", "/bin/ipmitool"));
         var res3 = Des_tor.RowRecord (common_cmd ("-c  fru", "/bin/ipmitool"));
         var res4 = Des_tor.RowRecord (common_cmd ("-c  dcmi power reading ", "/bin/ipmitool"));

         //3 elments in this list.
         var res5 = Des_tor.SDRRecord (common_cmd ("-c  sdr ", "/bin/ipmitool"));
         var res6 = Des_tor.RAWRecord (common_cmd ("  -t BIOS ", "/sbin/dmidecode"));
         var res7 = Des_tor.RAWRecord (common_cmd ("  -t system ", "/sbin/dmidecode"));
         var res8 = Des_tor.RAWRecord (common_cmd ("  -t baseboard ", "/sbin/dmidecode"));
         var res9 = Des_tor.RAWRecord (common_cmd ("  -t processor ", "/sbin/dmidecode"));

         var res10 = Des_tor.RAWRecord (common_cmd ("  -t memory ", "/sbin/dmidecode"));
         var res11 = Des_tor.RAWRecord (common_cmd ("  -t slot ", "/sbin/dmidecode"));
         var res12 = Des_tor.RAWRecord (common_cmd ("   ", "/sbin/dmidecode"));
         var res13 = Des_tor.RAWRecord (common_cmd ("   ", "/bin/lspci"));
         var res14 = Des_tor.RAWRecord (common_cmd (" -c ifconfig  ", "/bin/bash"));

         var slist1 = res1.Where (z => z[0] == "MAC Address" || z[0] == "IP Adress").ToList ();
         var slist2 = res2.Where (z => z[0] == "Firmware Revision" ||
            z[0] == "IPMI Version" ||
            z[0] == "Manufacturer ID" ||
            z[0] == "Manufacturer Name"
         ).ToList ();

         var slist3 = res3.Where (z =>
            z[0] == "Chassis Part Number" ||
            z[0] == "Chassis Serial" ||
            z[0] == "Board Mfg Date" ||
            z[0] == "Board Mfg" ||
            z[0] == "Board Product" ||
            z[0] == "Board Serial" ||
            z[0] == "Board Part Number" ||
            z[0] == "Product Manufacturer" ||
            z[0] == "Product Part Number" ||
            z[0] == "Product Serial"
         ).ToList ();

         var slist4 = res4.Where (z =>
            z[0] == "Instantaneous power reading" ||
            z[0] == "Minimum during sampling period" ||
            z[0] == "Maximum during sampling period" ||
            z[0] == "Average power reading over sample period"

         ).ToList ();
         //var Total_list=new List<string>();
         var llist1 = res6.Take (8).TakeLast (4);
         foreach (var re in llist1) {
            Console.WriteLine (re);
            GGG.BIOS.Add (re);
         }
         Console.WriteLine ("--System--------------");
         var llist2 = res7.Take (9).TakeLast (5);
         foreach (var re in llist2) {
            Console.WriteLine (re);
            GGG.Sysinfo.Add (re);
         }
         Console.WriteLine ("--BaseBoard--------------");
         var llist3 = res8.Take (9).TakeLast (5);
         foreach (var re in llist3) {
            Console.WriteLine (re);
            GGG.BaseBoard.Add (re);
         }
         Console.WriteLine ("--Processor--------------");
         var llist4 = res9.Where (z =>
            z.Contains ("Processor Information") ||
            z.Contains ("Socket Designation") ||
            z.Contains ("Version:") ||
            z.Contains ("Voltage:") ||
            z.Contains ("External Clock:") ||
            z.Contains ("Max Speed:") ||
            z.Contains ("Current Speed:")
         );
         foreach (var re in llist4) {
            Console.WriteLine (re);
            GGG.Processor.Add (re);
         }
         Console.WriteLine ("--Memory--------------");
         var llist5 = res10.Where (z =>
            z.Contains ("Memory Device") ||
            z.Contains ("Size:") ||
            z.Contains ("Locator:") ||
            z.Contains ("Speed:") ||
            z.Contains ("Configured Memory Speed:") ||
            z.Contains ("Manufacturer:") ||
            z.Contains ("Part Number:") ||
            z.Contains ("Configured Voltage:")
         );
         foreach (var re in llist5) {
            Console.WriteLine (re);
            GGG.Memory.Add (re);
         }
         Console.WriteLine ("--Slot--------------");
         var llist6 = res11.Where (z =>
            z.Contains ("System Slot Information") ||
            z.Contains ("Designation:") ||
            z.Contains ("Type:") ||
            z.Contains ("Current Usage:") ||
            z.Contains ("Length:") ||
            z.Contains ("ID:") ||
            z.Contains ("Characteristics:") ||
            z.Contains ("3.3 V is") ||
            z.Contains ("PME signal is") ||
            z.Contains ("Bus Address:")
         );
         foreach (var re in llist6) {
            Console.WriteLine (re);
            GGG.PCISlot.Add (re);
         }
         Console.WriteLine ("--Power--------------");
         var llist7 = res12.Where (z =>
            z.Contains ("Power") //||
            // z.Contains ("Designation:") ||
            // z.Contains ("Type:") ||
            // z.Contains ("Current Usage:") ||
            // z.Contains ("Length:") ||
            // z.Contains ("ID:") ||
            // z.Contains ("Characteristics:") ||
            // z.Contains("3.3 V is")||
            // z.Contains("PME signal is")||
            // z.Contains ("Bus Address:")
         );
         foreach (var re in llist7) {
            Console.WriteLine (re);
            GGG.PowerSuply.Add (re);
         }
         Console.WriteLine ("--EthNet--------------");
         var llist8 = res13.Where (z =>
            z.Contains ("Eth") //||
            // z.Contains ("Designation:") ||
            // z.Contains ("Type:") ||
            // z.Contains ("Current Usage:") ||
            // z.Contains ("Length:") ||
            // z.Contains ("ID:") ||
            // z.Contains ("Characteristics:") ||
            // z.Contains("3.3 V is")||
            // z.Contains("PME signal is")||
            // z.Contains ("Bus Address:")
         );
         foreach (var re in llist8) {
            Console.WriteLine (re);
            GGG.Ethernets.Add (re);
         }
         Console.WriteLine ("--------MELL--------");
         var llist9 = res13.Where (z =>
            z.Contains ("Mell") //||
            // z.Contains ("Designation:") ||
            // z.Contains ("Type:") ||
            // z.Contains ("Current Usage:") ||
            // z.Contains ("Length:") ||
            // z.Contains ("ID:") ||
            // z.Contains ("Characteristics:") ||
            // z.Contains("3.3 V is")||
            // z.Contains("PME signal is")||
            // z.Contains ("Bus Address:")
         );
         foreach (var re in llist9) {
            Console.WriteLine (re);
         }
         Console.WriteLine ("-----------LSI-----");
         var llist10 = res13.Where (z =>
            z.Contains ("LSI") //||
            // z.Contains ("Designation:") ||
            // z.Contains ("Type:") ||
            // z.Contains ("Current Usage:") ||
            // z.Contains ("Length:") ||
            // z.Contains ("ID:") ||
            // z.Contains ("Characteristics:") ||
            // z.Contains("3.3 V is")||
            // z.Contains("PME signal is")||
            // z.Contains ("Bus Address:")
         );
         foreach (var re in llist9) {
            Console.WriteLine (re);
         }
         Console.WriteLine ("---------LO-------");
         var llist11 = res14.Where (z =>
            !z.StartsWith (" ") && !z.Contains ("lo") //||
            // z.Contains ("Designation:") ||
            // z.Contains ("Type:") ||
            // z.Contains ("Current Usage:") ||
            // z.Contains ("Length:") ||
            // z.Contains ("ID:") ||
            // z.Contains ("Characteristics:") ||
            // z.Contains("3.3 V is")||
            // z.Contains("PME signal is")||
            // z.Contains ("Bus Address:")
         );
         foreach (var re in llist11) {
            Console.WriteLine (re.Split (": ") [0]);
            GGG.Lo.Add (re.Split (": ") [0]);
         }
         Console.WriteLine ("------IP----------");
         var llist12 = res14.Where (z =>
            !z.StartsWith (" ") && !z.Contains ("lo") //||
            // z.Contains ("Designation:") ||
            // z.Contains ("Type:") ||
            // z.Contains ("Current Usage:") ||
            // z.Contains ("Length:") ||
            // z.Contains ("ID:") ||
            // z.Contains ("Characteristics:") ||
            // z.Contains("3.3 V is")||
            // z.Contains("PME signal is")||
            // z.Contains ("Bus Address:")
         );

         foreach (var re in llist12) {
            //    Console.WriteLine (re);
            int index = res14.IndexOf (re);
            var str = res14[index + 1];
            var res = str.Split (' ', StringSplitOptions.RemoveEmptyEntries);
            Console.WriteLine (res[1]);
            GGG.IP.Add (res[1]);

         }

         Console.WriteLine ("--Others--------------");

         slist1.AddRange (slist2);
         slist1.AddRange (slist3);
         slist1.AddRange (slist4);
         foreach (var item in slist1) {
            Console.WriteLine (item[0] + ":" + item[1]);
            GGG.Others.Add (item[0] + ":" + item[1]);
         }
         return GGG;
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

      static void GGExe (GG_info info) {
         Console.WriteLine ("==============================");

       
         var glist = new List<G_info> ();

         while (info.Memory.Count > 0) {
            G_info g_ = new G_info (info.Memory.TakeLast (8).ToList (), info.Memory.Take (1).SingleOrDefault ());

            glist.Add (g_);

            info.Memory.RemoveRange (info.Memory.Count-9, 9);

         }

         foreach (var i in glist) {
            Console.WriteLine (i.Title);
            foreach (var s in i.info) {
               Console.WriteLine (s);
               // if (!s.StartsWith(" "))
               // {

               // }
               // else
               // {

               // }

            }
         }
         Console.WriteLine ("==============================");
      //   Console.WriteLine (glist.ToJson ());
        // var mlist=new List<Memory>();
      //   var doc =BsonDocument.Parse(glist.ToJson());
      List<Memory> mlist=Newtonsoft.Json.JsonConvert.DeserializeObject<List<Memory>>(glist.ToJson());
         
         Console.WriteLine(mlist.Where(z=>z.Info.Size=="16384 MB").Count());

      }

   }
   public class GG_info {

      public GG_info () {
         this.BIOS = new List<string> ();
         this.Sysinfo = new List<string> ();
         this.BaseBoard = new List<string> ();
         this.Processor = new List<string> ();
         this.Memory = new List<string> ();
         this.PCISlot = new List<string> ();
         this.PowerSuply = new List<string> ();
         this.Ethernets = new List<string> ();
         this.Lo = new List<string> ();
         this.IP = new List<string> ();
         this.Others = new List<string> ();

      }
      public List<string> BIOS { get; set; }
      public List<string> Sysinfo { get; set; }
      public List<string> BaseBoard { get; set; }
      public List<string> Processor { get; set; }
      public List<string> Memory { get; set; }
      public List<string> PCISlot { get; set; }
      public List<string> PowerSuply { get; set; }
      public List<string> Ethernets { get; set; }
      public List<string> Lo { get; set; }
      public List<string> IP { get; set; }
      public List<string> Others { get; set; }

   }
   public class G_info {
      public G_info (List<string> str, string Tit) {
         this.Title = Tit;
         this.info = new Dictionary<string, string> ();
         foreach (var item in str) {

            var s = item.Split (":", StringSplitOptions.RemoveEmptyEntries);
           // Console.WriteLine (s[0] + "--" + s[1]);
            this.info.Add (s[0].Trim ().Replace(" ",""), s[1].Trim ());

         }
      }

      public string Title { get; set; }
      public Dictionary<string, string> info { get; set; }
   }

   public class Des_tor {
      //处理由“:”进行分割的字符串，现用ipmitool ip测试 
      public static List<string[]> RowRecord (string raw_rec) {
         var sslist = new List<string[]> ();
         var res = raw_rec.Split (new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
         int i = 0;
         foreach (var re in res) {
            i++;
            //  Console.WriteLine(i.ToString()+" "+re);
            var t = re.Split (": ", StringSplitOptions.RemoveEmptyEntries).ToList ();
            t[0] = t[0].Trim ();
            //  if(t[1]!=null)
            try {
               t[1] = t[1].Trim ();

            } catch {
               t.Insert (1, "N/A");
               //t.Add("NA");
               //  Console.WriteLine(ex.Message);
            }

            sslist.Add (t.ToArray ());
            // Console.WriteLine(t[0]+"--"+t[1]);
         }
         //Console.WriteLine(raw_rec);

         // var z=sslist.Where(z=>z[0].ToString()=="MAC Address").FirstOrDefault()[1].ToString();
         //   Console.WriteLine("--------"+z);
         // foreach(var rr in sslist)
         // {
         //    Console.WriteLine(rr[0]+"-");
         // }
         return sslist;
      }
      public static List<string[]> SDRRecord (string raw_rec) {
         var sslist = new List<string[]> ();
         var res = raw_rec.Split (new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
         //  int i = 0;
         foreach (var re in res) {
            //  i++;
            //  Console.WriteLine(" "+re);
            var t = re.Split (",", StringSplitOptions.RemoveEmptyEntries).ToList ();
            t[0] = t[0].Trim ();
            //  if(t[1]!=null)
            try {
               t[1] = t[1].Trim ();

            } catch {
               t.Insert (1, "N/A");
            }
            try {
               t[2] = t[2].Trim ();
            } catch {
               t.Insert (2, "N/A");
            }
            try {
               t[3] = t[3].Trim ();
            } catch {
               t.Insert (3, "N/A");

            }

            sslist.Add (t.ToArray ());
            // Console.WriteLine(t[0]+"--"+t[1]);
         }
         //Console.WriteLine(raw_rec);

         // var z=sslist.Where(z=>z[0].ToString()=="MAC Address").FirstOrDefault()[1].ToString();
         //   Console.WriteLine("--------"+z);
         // foreach (var rr in sslist) {
         //    Console.WriteLine (rr[0] + "-" + rr[1] + "-" + rr[2] + "-" + rr[3]);
         // }
         return sslist;
      }

      public static List<string> RAWRecord (string raw_rec) {
         var sslist = new List<string> ();
         var res = raw_rec.Split (new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList ();
         //  int i = 0;

         sslist.AddRange (res);
         // Console.WriteLine(t[0]+"--"+t[1]);

         //Console.WriteLine(raw_rec);

         // var z=sslist.Where(z=>z[0].ToString()=="MAC Address").FirstOrDefault()[1].ToString();
         //   Console.WriteLine("--------"+z);
         // foreach (var rr in sslist) {
         //    Console.WriteLine (rr);
         // }
         return sslist;
      }
   }
    public partial class Memory
    {
        public string Title { get; set; }
        public Info Info { get; set; }
    }

    public partial class Info
    {
        public string Size { get; set; }
        public string Locator { get; set; }
        public string BankLocator { get; set; }
        public string Speed { get; set; }
        public string Manufacturer { get; set; }
        public string PartNumber { get; set; }
        public string ConfiguredMemorySpeed { get; set; }
        public string ConfiguredVoltage { get; set; }
    }

  //  public enum BankLocator { P1Node1Channel3Dimm1 };

 //   public enum ConfiguredMemorySpeed { Unknown };

 //   public enum Locator { P2Dimmh2 };

  //  public enum Manufacturer { NoDimm };

  //  public enum Size { NoModuleInstalled };

    //public enum Title { MemoryDevice };
}