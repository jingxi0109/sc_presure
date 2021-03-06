using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Security.AccessControl;
using System.Xml.Schema;
using MongoDB;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using Newtonsoft;
using RestSharp;
using sc_presure.sc_server;

namespace sc_presure {
   class Program {
      static void Main (string[] args) {

         string res = Newtonsoft.Json.JsonConvert.SerializeObject (Srv_Factory ());
         var client = new RestClient ("http://app.chinasupercloud.com:8088/support/api/ipmi");
         client.Timeout = -1;
         var request = new RestRequest (Method.POST);
         request.AddHeader ("Content-Type", "application/json");
         request.AddParameter ("application/json", res, ParameterType.RequestBody);
         IRestResponse response = client.Execute (request);
         Console.WriteLine (response.Content);

         //  UID_ON ();
      }
      static G_RAW_info Te () {
         G_RAW_info GGG = new G_RAW_info ();

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
         var res5 = Des_tor.RAWRecord (common_cmd ("-c  sdr ", "/bin/ipmitool"));



         var res6 = Des_tor.RAWRecord (common_cmd ("  -t BIOS ", "/sbin/dmidecode"));
         var res7 = Des_tor.RAWRecord (common_cmd ("  -t system ", "/sbin/dmidecode"));
         var res8 = Des_tor.RAWRecord (common_cmd ("  -t baseboard ", "/sbin/dmidecode"));
         var res9 = Des_tor.RAWRecord (common_cmd ("  -t processor ", "/sbin/dmidecode"));

         var res10 = Des_tor.RAWRecord (common_cmd ("  -t memory ", "/sbin/dmidecode"));
         var res11 = Des_tor.RAWRecord (common_cmd ("  -t slot ", "/sbin/dmidecode"));
         var res12 = Des_tor.RAWRecord (common_cmd ("   ", "/sbin/dmidecode"));





         var res13 = Des_tor.RAWRecord (common_cmd ("   ", "/bin/lspci"));


         var res14 = Des_tor.RAWRecord (common_cmd (" -c ifconfig  ", "/bin/bash"));

         var res15 = Des_tor.RAWRecord (common_cmd ("  -i 0 -q  ", "/bin/nvidia-smi"));
         var res16 = Des_tor.RAWRecord (common_cmd (" -a /dev/sda  ", "/sbin/smartctl"));


         var res17 = Des_tor.RAWRecord (common_cmd (" smart-log /dev/nvme0  ", "/sbin/nvme"));


         
         var res18 = Des_tor.RAWRecord (common_cmd (" /c0 show ", "/local/sbin/storcli"));
         var res19 = Des_tor.RAWRecord (common_cmd (" /c0/eall/sall show ", "/local/sbin/storcli"));
         var res20 = Des_tor.RAWRecord (common_cmd (" /c0/e252/s10 show all ", "/local/sbin/storcli"));

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
         var llist1 = res6; //.Take (8).TakeLast (4);
         foreach (var re in llist1) {
            Console.WriteLine (re);
            GGG.BIOS.Add (re);
         }
         Console.WriteLine ("--System--------------");
         var llist2 = res7; //.Take (9).TakeLast (5);
         foreach (var re in llist2) {
            Console.WriteLine (re);
            GGG.Sysinfo.Add (re);
         }
         Console.WriteLine ("--BaseBoard--------------");
         var llist3 = res8; //.Take (9).TakeLast (5);
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
            GGG.MELL.Add (re);
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
         foreach (var re in llist10) {
            Console.WriteLine (re);
            GGG.LSI.Add (re);
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
         Console.WriteLine ("---------GPU------");
         var llist13 = res15;
         foreach (var re in llist13) {
            Console.WriteLine (re);
            GGG.GPU.Add (re);
         }

         Console.WriteLine ("---------Disk-SDA-----");
         var llist14 = res16;
         foreach (var re in llist14) {
            Console.WriteLine (re);
            GGG.Disk_SDA.Add (re);
         }
         Console.WriteLine ("---------NVME-----");
         var llist15 = res17;
         foreach (var re in llist15) {
            Console.WriteLine (re);
            GGG.NVME.Add (re);
         }

         Console.WriteLine ("---/c0 show----Disks-----");
         var llist16 = res18; //.Where (z =>

         foreach (var re in llist16) {
            Console.WriteLine (re);
            GGG.Rcard.Add (re);
         }

         Console.WriteLine ("---/c0/eall/sall show----Disks-----");
         var llist17 = res19; //.Where (z =>

         foreach (var re in llist16) {
            Console.WriteLine (re);
            GGG.Rstatus.Add (re);
         }

         Console.WriteLine ("---/c0/e252/s10 show all----Disks-----");
         var llist18 = res20; //.Where (z =>

         foreach (var re in llist18) {
            Console.WriteLine (re);
            GGG.Rdisk.Add (re);
         }
         Console.WriteLine ("---Sensor Data-----");
         var llist19 = res5; //.Where (z =>

         foreach (var re in llist19) {
            Console.WriteLine (re);
            GGG.SDR.Add (re);
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
         if (File.Exists (filenamel)) {

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
         } else {
            filenamel = "/usr" + filename;
            if (File.Exists (filenamel)) {

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

            } else {
               throw new Exception (filenamel);
            }

         }

         return result;
      }
      public static void UID_ON () {
         common_cmd (" raw 0x30 0x0d", "/bin/ipmitool");
      }
      public static void UID_OFF () {
         common_cmd (" raw 0x30 0x0e", "/bin/ipmitool");
      }

      public static ServerInfo Srv_Factory () {
         G_RAW_info info = Te ();
         Console.WriteLine ("==============================");
         Server_info srv = new Server_info ();

         srv.Memory = Build_mem (info.Memory);
         srv.CPU = Build_cpu (info.Processor);
         srv.PCiSLOT = Build_slot (info.PCISlot);
         srv.BIOS = Build_Bios (info.BIOS);
         srv.Sysinfo = Build_Sysinfo (info.Sysinfo);
         srv.Base_Board = Build_BaseBoard (info.BaseBoard);
         srv.GPU = info.GPU;
         srv.DISK_SDA = info.Disk_SDA;
         srv.NVME = info.NVME;
         srv.LSI = info.LSI;
         srv.MELL = info.MELL;
         srv.Rstatus = info.Rstatus;
         srv.Rdisk = info.Rdisk;
         srv.Rcard = info.Rcard;
         srv.SDR=info.SDR;

         //            Console.WriteLine ("==============================");

         // foreach (var item in srv.Base_Board)
         // {
         //     Console.WriteLine(item);
         // }

         //              Console.WriteLine ("==============================");

         srv.others = info.Others;
         srv.EthNET = info.Ethernets;
         srv.LO = info.Lo;
         srv.IP = info.IP;

         // foreach (var item in bios) {
         //    Console.WriteLine (item);

         // }
         // Console.WriteLine ("==============================");

         // foreach (var item in sys) {
         //    Console.WriteLine (item);

         // }
         // Console.WriteLine ("==============================");

         // foreach (var item in bb) {
         //    Console.WriteLine (item);

         // }
         // Console.WriteLine ("==============================");

         // foreach (var item in slot) {

         string j = Newtonsoft.Json.JsonConvert.SerializeObject (srv);
         Console.WriteLine ("==============================");

         //foreach (var item in srv.Base_Board)
         //{
         Console.WriteLine (j);

         Console.WriteLine ("==============================");
         var sinfo = Newtonsoft.Json.JsonConvert.DeserializeObject<sc_server.ServerInfo> (j);
         Console.WriteLine (sinfo.ToJson ());

         // }

         Console.WriteLine ("==============================");
         return sinfo;

      }
      static List<string> Build_Bios (List<string> slist) {
         slist.RemoveAt (0);
         return slist;
      }
      static List<string> Build_Sysinfo (List<string> slist) {
         slist.RemoveAt (0);
         return slist;
      }
      static List<string> Build_BaseBoard (List<string> slist) {
         slist.RemoveAt (0);
         return slist;
      }

      static List<cpu.Cpu> Build_cpu (List<string> slist) {
         var glist = new List<G_info> ();

         while (slist.Count > 0) {
            G_info g_ = new G_info (slist.TakeLast (6).ToList (), slist.Take (1).SingleOrDefault ());

            glist.Add (g_);

            slist.RemoveRange (slist.Count - 7, 7);

         }
         //     foreach (var i in glist) {
         //    Console.WriteLine (i.Title);
         //    foreach (var s in i.info) {
         //       Console.WriteLine (s);
         //       if (!s.StartsWith(" "))
         //       {

         //       } 
         //       else
         //       {

         //       }

         //    }
         // }
         //   Console.WriteLine(glist.ToJson());
         //       Console.WriteLine ("==============================");
         //   Console.WriteLine (glist.ToJson ());
         // var mlist=new List<Memory>();
         //   var doc =BsonDocument.Parse(glist.ToJson());
         List<cpu.Cpu> mlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<cpu.Cpu>> (glist.ToJson ());

         //  Console.WriteLine(mlist.Where(z=>z.Info.SocketDesignation=="CPU1").Count());
         return mlist;
      }
      static List<mem.Memory> Build_mem (List<string> slist) {

         var glist = new List<G_info> ();

         while (slist.Count > 0) {
            G_info g_ = new G_info (slist.TakeLast (8).ToList (), slist.Take (1).SingleOrDefault ());

            glist.Add (g_);

            slist.RemoveRange (slist.Count - 9, 9);

         }

         // foreach (var i in glist) {
         //    Console.WriteLine (i.Title);
         //    foreach (var s in i.info) {
         //       Console.WriteLine (s);
         //       // if (!s.StartsWith(" "))
         //       // {

         //       // }
         //       // else
         //       // {

         //       // }

         //    }
         // }
         //  Console.WriteLine ("==============================");
         //   Console.WriteLine (glist.ToJson ());
         // var mlist=new List<Memory>();
         //   var doc =BsonDocument.Parse(glist.ToJson());
         List<mem.Memory> mlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<mem.Memory>> (glist.ToJson ());

         //    Console.WriteLine (mlist.Where (z => z.Info.Size == "16384 MB").Count ());
         return mlist;

      }
      static List<slot.Slot> Build_slot (List<string> slist) {
         var glist = new List<G_info> ();

         while (slist.Count > 0) {
            G_info g_ = new G_info (slist.TakeLast (9).ToList (), slist.Take (1).SingleOrDefault ());

            glist.Add (g_);

            slist.RemoveRange (slist.Count - 10, 10);

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
         //Console.WriteLine(glist.ToJson());

         List<slot.Slot> mlist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<slot.Slot>> (glist.ToJson ());

         //   Console.WriteLine (mlist.Where (z => z.Info.CurrentUsage == "In Use").Count ());
         return mlist;
         //  Console.WriteLine ("==============================");

      }

   }
   public class G_RAW_info {

      public G_RAW_info () {
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
         this.GPU = new List<string> ();
         this.LSI = new List<string> ();
         this.MELL = new List<string> ();
         this.NVME = new List<string> ();
         this.Others = new List<string> ();
         this.Disk_SDA = new List<string> ();
         this.Rcard = new List<string> ();
         this.Rdisk = new List<string> ();
         this.Rstatus = new List<string> ();
         this.SDR = new List<string> ();

      }
      public List<string> SDR { set; get; }
      public List<string> Rcard { set; get; }
      public List<string> Rstatus { set; get; }
      public List<string> Rdisk { set; get; }
      public List<string> Disk_SDA { set; get; }
      public List<string> MELL { set; get; }
      public List<string> LSI { set; get; }
      public List<string> NVME { set; get; }
      public List<string> GPU { get; set; }
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

            try {
               if (s.Length > 3) {
                  this.info.Add (s[0].Trim ().Replace (" ", ""), s[1].Trim () + ":" + s[2].Trim () + ":" + s[3].Trim ());
               } else {
                  //  if (s[0].Contains ("Characteristics")) {
                  //        this.info.Add (s[0].Trim ().Replace (" ", ""), "");
                  //    } else {
                  this.info.Add (s[0].Trim ().Replace (" ", ""), s[1].Trim ());
                  //  }
               }

            } catch (System.Exception) {

               // if (this.info["Characteristics"] == "") {
               //    this.info["Characteristics"] += s[0].Trim () + " / ";
               // } else {
               //    this.info["Characteristics"] += s[0].Trim ();

               // }

            }

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

   public class Server_info {
      public List<mem.Memory> Memory { get; set; }
      public List<cpu.Cpu> CPU { get; set; }
      public List<slot.Slot> PCiSLOT { get; set; }
      public List<string> BIOS { set; get; }
      public List<string> Sysinfo { set; get; }
      public List<string> MELL { set; get; }
      public List<string> Base_Board { set; get; }
      public List<string> others { set; get; }
      public List<string> EthNET { set; get; }
      public List<string> LO { set; get; }
      public List<string> GPU { set; get; }
      public List<string> DISK_SDA { set; get; }
      public List<string> LSI { set; get; }
      public List<string> NVME { set; get; }
      public List<string> IP { set; get; }
      public List<string> SDR { set; get; }
      public List<string> Rcard { set; get; }
      public List<string> Rstatus { set; get; }
      public List<string> Rdisk { set; get; }
   }
   //  public enum BankLocator { P1Node1Channel3Dimm1 };

   //   public enum ConfiguredMemorySpeed { Unknown };

   //   public enum Locator { P2Dimmh2 };

   //  public enum Manufacturer { NoDimm };

   //  public enum Size { NoModuleInstalled };

   //public enum Title { MemoryDevice };
   namespace mem {
      public partial class Memory {
         public string Title { get; set; }
         public Info Info { get; set; }
      }

      public partial class Info {
         public string Size { get; set; }
         public string Locator { get; set; }
         public string BankLocator { get; set; }
         public string Speed { get; set; }
         public string Manufacturer { get; set; }
         public string PartNumber { get; set; }
         public string ConfiguredMemorySpeed { get; set; }
         public string ConfiguredVoltage { get; set; }
      }

   }
   namespace cpu {
      public partial class Cpu {
         public string Title { get; set; }
         public Info Info { get; set; }
      }

      public partial class Info {
         public string SocketDesignation { get; set; }
         public string Version { get; set; }
         public string Voltage { get; set; }
         public string ExternalClock { get; set; }
         public string MaxSpeed { get; set; }
         public string CurrentSpeed { get; set; }
      }

   }
   namespace slot {
      public partial class Slot {
         public string Title { get; set; }
         public Info Info { get; set; }
      }

      public partial class Info {
         public string Designation { get; set; }
         public string Type { get; set; }
         public string CurrentUsage { get; set; }
         public string Length { get; set; }
         public long Id { get; set; }
         public string Characteristics { get; set; }
         public string BusAddress { get; set; }
      }
   }
   namespace sc_server {

      public partial class ServerInfo {
         public List<Memory> Memory { get; set; }
         public List<Cpu> Cpu { get; set; }
         public List<PCiSlot> PCiSlot { get; set; }
         public List<string> Bios { get; set; }
         public List<string> Sysinfo { get; set; }
         public List<string> Base_Board { get; set; }
         public List<string> Others { get; set; }
         public List<string> EthNet { get; set; }
         public List<string> Lo { get; set; }
         public List<string> Ip { get; set; }
         public List<string> MELL { set; get; }
         public List<string> GPU { get; set; }
         public List<string> DISK_SDA { set; get; }
         public List<string> NVME { set; get; }
         public List<string> LSI { set; get; }
         public List<string> SDR { set; get; }
         public List<string> Rcard { set; get; }
         public List<string> Rstatus { set; get; }
         public List<string> Rdisk { set; get; }
      }

      public partial class Cpu {
         public string Title { get; set; }
         public CpuInfo Info { get; set; }
      }

      public partial class CpuInfo {
         public string SocketDesignation { get; set; }
         public string Version { get; set; }
         public string Voltage { get; set; }
         public string ExternalClock { get; set; }
         public string MaxSpeed { get; set; }
         public string CurrentSpeed { get; set; }
      }

      public partial class Memory {
         public string Title { get; set; }
         public MemoryInfo Info { get; set; }
      }

      public partial class MemoryInfo {
         public string Size { get; set; }
         public string Locator { get; set; }
         public string BankLocator { get; set; }
         public string Speed { get; set; }
         public string Manufacturer { get; set; }
         public string PartNumber { get; set; }
         public string ConfiguredMemorySpeed { get; set; }
         public string ConfiguredVoltage { get; set; }
      }

      public partial class PCiSlot {
         public string Title { get; set; }
         public PCiSlotInfo Info { get; set; }
      }

      public partial class PCiSlotInfo {
         public string Designation { get; set; }
         public string Type { get; set; }
         public string CurrentUsage { get; set; }
         public string Length { get; set; }
         public long Id { get; set; }
         public string Characteristics { get; set; }
         public string BusAddress { get; set; }
      }

      //  public enum ConfiguredMemorySpeed { The1600MtS, Unknown };

      //  public enum ConfiguredVoltage { The2133MtS, Unknown };

      //  public enum Manufacturer { Micron, NoDimm };

      //  public enum PartNumber { NoDimm, The36Asf2G72Pz2G1A2 };

      //  public enum Size { NoModuleInstalled, The16384Mb };

      //  public enum Title { MemoryDevice };
   }
}