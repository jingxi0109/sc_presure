using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;

namespace sc_presure
{
    public   class IPMI_Access
    {
        List<ipmi_info> ilist;
        public string IP_ipmi { get; set; }
        public string IPMI_URL { get; set; }
        public IPMI_Access(string ip)
        {
            this.IP_ipmi = ip;
            this.IPMI_URL = UrlEn(ip);
            this.ilist = new List<ipmi_info>();

        }
        public List<ipmi_info> Get_last_ilist()
        {
            this.ilist.Clear();
            var client = new RestClient(IPMI_URL);
            //Console.WriteLine(UrlEn("192.168.7.219"));
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            // request.AddHeader("Cookie", "SID=zohrbitjnbdlgnws");
            IRestResponse response = client.Execute(request);
            //   Console.WriteLine(response.Content);
            //   System.Xml.XmlDocument doc = new System.Xml.XmlDocument();//新建对象

            //       doc.LoadXml(response.Content);//符合xml格式的字符串

            System.Data.DataSet ds = StringToDataSet(response.Content);
            foreach (DataRow node in ds.Tables[1].Rows)
            {
                //bool tag = (int.Parse(node["UNIT1"].ToString(), NumberStyles.HexNumber) >> 6) == 0x02;
                ////Console.WriteLine($"L:{node["L"].ToString()} type is {node["STYPE"].ToString()} SFormula {node["Option"].ToString()}");
                ////Console.WriteLine($" UNR -{node["UNR"].ToString()}-->{ToSigned(int.Parse(node["UNR"].ToString(), NumberStyles.HexNumber), 8)}");
                ////Console.WriteLine($" UC -{node["UC"].ToString()}-->{ToSigned(int.Parse(node["UC"].ToString(), NumberStyles.HexNumber), 8)}");
                ////Console.WriteLine($" UNC -{node["UNC"].ToString()}-->{ToSigned(int.Parse(node["UNC"].ToString(), NumberStyles.HexNumber), 8)}");
                ////Console.WriteLine($" LNC-{node["LNC"].ToString()}-->{ToSigned(int.Parse(node["LNC"].ToString(), NumberStyles.HexNumber), 8)}");
                ////Console.WriteLine($" LC -{node["LC"].ToString()}-->{ToSigned(int.Parse(node["LC"].ToString(), NumberStyles.HexNumber), 8)}");
                ////Console.WriteLine($" LNR -{node["LNR"].ToString()}-->{ToSigned(int.Parse(node["LNR"].ToString(), NumberStyles.HexNumber), 8)}");
                ////Console.WriteLine("----------------------");
                //Console.WriteLine("Raw reading==>" + RawReading);
                //Console.WriteLine("Raw reading formart==>" + ReadingDataFormat);
                //Console.WriteLine($"UNR ==> {afterFuncSensor(node, "UNR", tag)}");
                //Console.WriteLine($"UC ==> {afterFuncSensor(node, "UC", tag)}");
                //Console.WriteLine($"UNC ==> {afterFuncSensor(node, "UNC", tag)}");
                //Console.WriteLine($"LNC ==> {afterFuncSensor(node, "LNC", tag)}");
                //Console.WriteLine($"LC ==> {afterFuncSensor(node, "LC", tag)}");
                //Console.WriteLine($"LNR ==> {afterFuncSensor(node, "LNR", tag)}");

                //////        Console.WriteLine($"--{node["READING"].ToString().Substring(2,2)}--");
                string RawReading = node["READING"].ToString().Substring(0, 2);
                string ReadingDataFormat = tenToSixteen(ToSigned(int.Parse(RawReading, NumberStyles.HexNumber), 8).ToString());
                //var res = float.Parse(SensorFunc(ReadingDataFormat, node["M"].ToString(), node["B"].ToString(), node["RB"].ToString()).ToString(), NumberStyles.Integer);
                var AfterFuncSensorReading = SensorFunc(RawReading, node["M"].ToString(), node["B"].ToString(), node["RB"].ToString());
                var UnitType1 = int.Parse(node["UNIT1"].ToString(), NumberStyles.HexNumber);
                var UnitType = int.Parse(node["UNIT"].ToString(), NumberStyles.HexNumber);
                string Unit = "";
                switch (UnitType)
                {
                    case 0x01:
                        Unit = "degrees C";
                        break;
                    case 0x02:
                        Unit = " degrees F";
                        break;
                    case 0x03:
                        Unit = " degrees K";
                        break;
                    case 0x04:
                        Unit = " Volts";
                        break;
                    case 0x05:
                        Unit = " Amps";
                        break;
                    case 0x06:
                        Unit = " Watts";
                        break;
                    case 0x07:
                        Unit = " Joules";
                        break;
                    case 0x12:
                        Unit = " R.P.M";
                        break;
                    case 0x13:
                        Unit = " Hz";
                        break;
                    default:
                        break;
                }
                //    Unit_ = Unit;
                //  var AfterFuncSensorReading = float.Parse(, NumberStyles.Integer);
                //  Console.WriteLine(node["UNIT"].ToString());
                if (node["UNIT"].ToString() == "04")
                {//电压
                    //    AfterFuncSensorReading =float.Parse(SensorFunc(ReadingDataFormat, node["M"].ToString(), node["B"].ToString(), node["RB"].ToString()).ToString(), NumberStyles.Integer);
               //     Console.WriteLine($"{node[0]}- {node[1]} - Raw: {node[2]}: {AfterFuncSensorReading.ToString("0.000")} {Unit}");
                    this.ilist.Add(new ipmi_info()
                    {
                        i_index = node["ID"].ToString(),
                         i_Name=node["NAME"].ToString(),
                          Raw_value=node["READING"].ToString(),
                           Unit=Unit,
                            Value= AfterFuncSensorReading.ToString("0.000")
                    }) ;
                }
                else if (node["UNIT1"].ToString() == "c0")//温度
                {
              //      Console.WriteLine($"{node[0]}- {node[1]}  - Raw:  {node[2]}: {AfterFuncSensorReading} {Unit}");
                    this.ilist.Add(new ipmi_info()
                    {
                        i_index = node["ID"].ToString(),
                        i_Name = node["NAME"].ToString(),
                        Raw_value = node["READING"].ToString(),
                        Unit = Unit,
                        Value = AfterFuncSensorReading.ToString()
                    });
                }
                else if (node["UNIT"].ToString() == "12")//风扇
                {
                    //   Console.WriteLine($"{node[0]}- {node[1]}  - Raw:  {node[2]}: {Math.Pow(AfterFuncSensorReading, 2)} {Unit}");
                    if (node["L"].ToString()=="08")//数据需要进行特殊处理
                    {
                        this.ilist.Add(new ipmi_info()
                        {
                            i_index = node["ID"].ToString(),
                            i_Name = node["NAME"].ToString(),
                            Raw_value = node["READING"].ToString(),
                            Unit = Unit,
                            Value = Math.Pow(AfterFuncSensorReading, 2).ToString()//AfterFuncSensorReading.ToString("0.000")
                        });
                    }
                    else
                    {
                        this.ilist.Add(new ipmi_info()
                        {
                            i_index = node["ID"].ToString(),
                            i_Name = node["NAME"].ToString(),
                            Raw_value = node["READING"].ToString(),
                            Unit = Unit,
                            Value =AfterFuncSensorReading.ToString()//, 2).ToString()//AfterFuncSensorReading.ToString("0.000")
                        });
                    }
         
                }


                else if (node["Option"].ToString() == "00")
                {//不存在数据 
              //      Console.WriteLine($"{node[0]}- {node[1]} - Raw:   {node[2]}: Not Present!");//{AfterFuncSensorReading / 1000}");
                    this.ilist.Add(new ipmi_info()
                    {
                        i_index = node["ID"].ToString(),
                        i_Name = node["NAME"].ToString(),
                        Raw_value = node["READING"].ToString(),
                        Unit = Unit,
                        Value ="Not Present!"//Math.Pow(AfterFuncSensorReading, 2).ToString()//AfterFuncSensorReading.ToString("0.000")
                    });
                }
                else
                {//温度
                   // Console.WriteLine($"{node[0]}- {node[1]}  - Raw:  {node[2]}: {AfterFuncSensorReading} {Unit}");
                    this.ilist.Add(new ipmi_info()
                    {
                        i_index = node["ID"].ToString(),
                        i_Name = node["NAME"].ToString(),
                        Raw_value = node["READING"].ToString(),
                        Unit = Unit,
                        Value = AfterFuncSensorReading.ToString()
                    });
                }
            }
            return this.ilist;

            }
        public DataTable Get_sensor_Table()
        {
            var client = new RestClient(IPMI_URL);
            //Console.WriteLine(UrlEn("192.168.7.219"));
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            // request.AddHeader("Cookie", "SID=zohrbitjnbdlgnws");
            IRestResponse response = client.Execute(request);
            //   Console.WriteLine(response.Content);
            //   System.Xml.XmlDocument doc = new System.Xml.XmlDocument();//新建对象

            //       doc.LoadXml(response.Content);//符合xml格式的字符串

            System.Data.DataSet ds = StringToDataSet(response.Content);

            return ds.Tables[1];
        }
        // static void Main()
        // {
        //     for (int i = 0; i < 20; i++)
        //     {
        //         Foreloop();
        //     }
         
        // }

        public static void Foreloop()
        {
            IPMI_Access access = new IPMI_Access("192.168.7.191");
            var v = access.Get_last_ilist();
            foreach (var i in v)
            {
                Console.WriteLine($"[rec] {i.i_index} {i.i_Name} {i.Value}/{i.Unit}");
            }
            //   throw new NotImplementedException();
        }

        static void Mainn(string[] args)
        {
            Console.WriteLine("Hello World!");

            var client = new RestClient(UrlEn("192.168.7.219"));
            Console.WriteLine(UrlEn("192.168.7.219"));
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
           // request.AddHeader("Cookie", "SID=zohrbitjnbdlgnws");
            IRestResponse response = client.Execute(request);
            //   Console.WriteLine(response.Content);
            //   System.Xml.XmlDocument doc = new System.Xml.XmlDocument();//新建对象

            //       doc.LoadXml(response.Content);//符合xml格式的字符串

            System.Data.DataSet ds = StringToDataSet(response.Content);
            //   Console.WriteLine(tenToSixteen("15"));
            // Console.WriteLine(ds.Tables.Count);
       //     ds.Tables[0].WriteXml("otmep.xml");
        //    DataTable dt = ds.Tables[1].Clone();
            //    Console.WriteLine(ds.Tables[1].Rows.Count);
            //   Console.WriteLine(ds.Tables[0].Rows.Count);
            foreach (DataRow node in ds.Tables[1].Rows)
            {
                //////        Console.WriteLine($"--{node["READING"].ToString().Substring(2,2)}--");
                string RawReading = node["READING"].ToString().Substring(0, 2);
                string ReadingDataFormat = tenToSixteen(ToSigned(int.Parse(RawReading, NumberStyles.HexNumber), 8).ToString());
                //var res = float.Parse(SensorFunc(ReadingDataFormat, node["M"].ToString(), node["B"].ToString(), node["RB"].ToString()).ToString(), NumberStyles.Integer);
                var AfterFuncSensorReading = SensorFunc(RawReading, node["M"].ToString(), node["B"].ToString(), node["RB"].ToString());
                var UnitType1 = int.Parse(node["UNIT1"].ToString(), NumberStyles.HexNumber);
                var UnitType = int.Parse(node["UNIT"].ToString(), NumberStyles.HexNumber);
                string Unit = "";
                switch (UnitType)
                {
                    case 0x01:
                        Unit = "degrees C";
                        break;
                    case 0x02:
                        Unit = " degrees F";
                        break;
                    case 0x03:
                        Unit = " degrees K";
                        break;
                    case 0x04:
                        Unit = " Volts";
                        break;
                    case 0x05:
                        Unit = " Amps";
                        break;
                    case 0x06:
                        Unit = " Watts";
                        break;
                    case 0x07:
                        Unit = " Joules";
                        break;
                    case 0x12:
                        Unit = " R.P.M";
                        break;
                    case 0x13:
                        Unit = " Hz";
                        break;
                    default:
                        break;
                }
                //    Unit_ = Unit;
                //  var AfterFuncSensorReading = float.Parse(, NumberStyles.Integer);
              //  Console.WriteLine(node["UNIT"].ToString());
                if (node["UNIT"].ToString() == "04")
                {//电压
                    //    AfterFuncSensorReading =float.Parse(SensorFunc(ReadingDataFormat, node["M"].ToString(), node["B"].ToString(), node["RB"].ToString()).ToString(), NumberStyles.Integer);
                    Console.WriteLine($"{node[0]}- {node[1]} - Raw: {node[2]}: {AfterFuncSensorReading.ToString("0.000")} {Unit}");
                }else if(node["UNIT1"].ToString() == "c0")//温度
                {
                    Console.WriteLine($"{node[0]}- {node[1]}  - Raw:  {node[2]}: {AfterFuncSensorReading} {Unit}");
                }
                else if (node["UNIT"].ToString() == "12")//风扇
                {
                    Console.WriteLine($"{node[0]}- {node[1]}  - Raw:  {node[2]}: {Math.Pow(AfterFuncSensorReading,2)} {Unit}");
                }
                

                else if(node["Option"].ToString()=="00")
                {//不存在数据 
                    Console.WriteLine($"{node[0]}- {node[1]} - Raw:   {node[2]}: Not Present!");//{AfterFuncSensorReading / 1000}");
                }
                else
                {//温度
                    Console.WriteLine($"{node[0]}- {node[1]}  - Raw:  {node[2]}: {AfterFuncSensorReading} {Unit}");
                }
                //   Console.WriteLine($"==={SensorFunc(RawReading,node["M"].ToString(),node["B"].ToString(),node["RB"].ToString())}");
                //Console.WriteLine((int.Parse(node["UNIT1"].ToString(), NumberStyles.HexNumber) >> 6) == 0x02);
                //bool tag = (int.Parse(node["UNIT1"].ToString(), NumberStyles.HexNumber) >> 6) == 0x02;
                ////Console.WriteLine($"L:{node["L"].ToString()} type is {node["STYPE"].ToString()} SFormula {node["Option"].ToString()}");
                ////Console.WriteLine($" UNR -{node["UNR"].ToString()}-->{ToSigned(int.Parse(node["UNR"].ToString(), NumberStyles.HexNumber), 8)}");
                ////Console.WriteLine($" UC -{node["UC"].ToString()}-->{ToSigned(int.Parse(node["UC"].ToString(), NumberStyles.HexNumber), 8)}");
                ////Console.WriteLine($" UNC -{node["UNC"].ToString()}-->{ToSigned(int.Parse(node["UNC"].ToString(), NumberStyles.HexNumber), 8)}");
                ////Console.WriteLine($" LNC-{node["LNC"].ToString()}-->{ToSigned(int.Parse(node["LNC"].ToString(), NumberStyles.HexNumber), 8)}");
                ////Console.WriteLine($" LC -{node["LC"].ToString()}-->{ToSigned(int.Parse(node["LC"].ToString(), NumberStyles.HexNumber), 8)}");
                ////Console.WriteLine($" LNR -{node["LNR"].ToString()}-->{ToSigned(int.Parse(node["LNR"].ToString(), NumberStyles.HexNumber), 8)}");
                ////Console.WriteLine("----------------------");
                //Console.WriteLine("Raw reading==>" + RawReading);
                //Console.WriteLine("Raw reading formart==>" + ReadingDataFormat);
                //Console.WriteLine($"UNR ==> {afterFuncSensor(node, "UNR", tag)}");
                //Console.WriteLine($"UC ==> {afterFuncSensor(node, "UC", tag)}");
                //Console.WriteLine($"UNC ==> {afterFuncSensor(node, "UNC", tag)}");
                //Console.WriteLine($"LNC ==> {afterFuncSensor(node, "LNC", tag)}");
                //Console.WriteLine($"LC ==> {afterFuncSensor(node, "LC", tag)}");
                //Console.WriteLine($"LNR ==> {afterFuncSensor(node, "LNR", tag)}");
            }



            Console.WriteLine("-----------------");

            //foreach (DataRow n in ds.Tables[1].Rows)
            //{



            //    //   //////        Console.WriteLine($"--{node["READING"].ToString().Substring(2,2)}--");
            //    //   string RawReading = node["READING"].ToString().Substring(0, 2);
            //    //string ReadingDataFormat = tenToSixteen(ToSigned(int.Parse(RawReading, NumberStyles.HexNumber), 8).ToString());
            //    //   //var res = float.Parse(SensorFunc(ReadingDataFormat, node["M"].ToString(), node["B"].ToString(), node["RB"].ToString()).ToString(), NumberStyles.Integer);

            //    //   var AfterFuncSensorReading = float.Parse(SensorFunc(RawReading, node["M"].ToString(), node["B"].ToString(), node["RB"].ToString()).ToString(), NumberStyles.Integer);
            //    //   if (node[15].ToString()=="04")
            //    //   {
            //    //   //    AfterFuncSensorReading =float.Parse(SensorFunc(ReadingDataFormat, node["M"].ToString(), node["B"].ToString(), node["RB"].ToString()).ToString(), NumberStyles.Integer);
            //    //       Console.WriteLine($"{node[0]}-{node[1]}   {node[2]}: {AfterFuncSensorReading/1000}");
            //    //   }
            //    //   else
            //    //   {
            //    //       Console.WriteLine($"{node[0]}-{node[1]}   {node[2]}: {AfterFuncSensorReading}");
            //    //   }  



            //    //int sr = (int.Parse(Math.Pow(AfterFuncSensorReading, 2).ToString()) * SensorReadingScale) / SensorReadingScale;
            //    //int re = 0;
            //    //try
            //    //{
            //    //    re = int.Parse((float.Parse(SensorReadingScale.ToString()) / AfterFuncSensorReading).ToString()) / SensorReadingScale;
            //    //}
            //    //catch (Exception)
            //    //{

            //    //    //  throw;

            //    //}

            //    //float er = float.Parse((AfterFuncSensorReading * float.Parse(SensorReadingScale.ToString())).ToString()) / SensorReadingScale;
            //    ////       int re = int.Parse((float.Parse(SensorReadingScale.ToString()) / AfterFuncSensorReading).ToString()) / SensorReadingScale;
            //    ////    int sr = (int.Parse(Math.Pow(AfterFuncSensorReading, 2).ToString()) * SensorReadingScale) / SensorReadingScale;
            //    //Console.WriteLine($"int:{res}/{sr}/{re}/{er}   RAW:{RawReading} :{ReadingDataFormat}");

            //    //        //-00------------------------------------------------------

            //    //      float re = float.Parse((AfterFuncSensorReading * float.Parse(SensorReadingScale.ToString())).ToString()) / SensorReadingScale;

            //    //     Console.WriteLine($"{m.ItemArray[0]} {m.ItemArray[1]}   {m.ItemArray[2]}  - {covert16( m.ItemArray[3].ToString(),m.ItemArray[4].ToString())}" +
            //    //      $"-{m.ItemArray[4]}- { m.ItemArray[5]} {m.ItemArray[6]} { m.ItemArray[7]} { m.ItemArray[8]}"+
            //    //    $"===={m.ItemArray.Length}");
            //    //            Console.WriteLine(    DiscreteSensor(m));
            //    //   Console.WriteLine(covert16(m["READING"].ToString(), m["STYPE"].ToString()));
            //    DataRow dr = dt.NewRow();
            //    SensorFormula(n, dr);

            //    for (int i = 0; i < n.ItemArray.Length; i++)
            //    {
            //        //      object item = m.ItemArray[i];
            //        object drr = dr.ItemArray[i];

            //        Console.Write(":" + i.ToString() + ":" + drr.ToString() + " ");

            //    }
            //    Console.WriteLine();
            //    //dt.Rows.Add(dr);
            //    //  dr.AcceptChanges();
            //    //    Console.WriteLine(DiscreteSensor(m));
            //    //foreach (var item in dr.ItemArray)
            //    //{
            //    ////    Console.Write(item.ToString()+" ");
            //    //}



            //    //          Console.WriteLine(System.Convert.ToBoolean((int.Parse("C0", NumberStyles.HexNumber) & 0x40)));
            //    //Console.WriteLine(!0);
            //    //     Console.WriteLine(HttpUtility.UrlDecode("?SENSOR_INFO_FOR_SYS_HEALTH.XML=(1%2Cff)&time_stamp=Fri%20Jul%2017%202020%2009%3A08%3A16%20GMT%2B0800%20(China%20Standard%20Time)&_="));
            //    //string str= "SENSOR_INFO_FOR_SYS_HEALTH.XML=(1%2Cff)&time_stamp=Fri%20Jul%2017%202020%2009%3A08%3A16%20GMT%2B0800%20(China%20Standard%20Time)";
            //    //    //  HttpUtility.UrlDecode("?SENSOR_INFO_FOR_SYS_HEALTH.XML=(1%2Cff)&time_stamp=Fri%20Jul%2017%202020%2009%3A08%3A16%20GMT%2B0800%20(China%20Standard%20Time)&_=");
            //    //    Console.WriteLine(str);
            //    //    Console.WriteLine(HttpUtility.UrlDecode( str));
            //    //    string dstr = "SENSOR_INFO_FOR_SYS_HEALTH.XML=(1,ff)&time_stamp=Fri Jul 17 2020 14:01:16 GMT+0800 (China Standard Time)";// HttpUtility.UrlDecode(str);
            //    //    Console.WriteLine(HttpUtility.UrlEncode(dstr,Encoding.Default ));


            //    //     Console.WriteLine(UrlEn("192.168.7.234"));


            //}
      //      dt.AcceptChanges();
        //    dt.WriteXml("tem.xml");
        }
        static string DiscreteSensor(DataRow row)
        {
            string sType = row["STYPE"].ToString();
            string sReadingObj = row["READING"].ToString();
            int sOption = int.Parse(row["OPTION"].ToString(), NumberStyles.HexNumber);
            string sRaw = sReadingObj.Substring(0, 2);
            int sSensorD = int.Parse(sReadingObj.Substring(2, 2), NumberStyles.HexNumber);
            int sDMSB = int.Parse(sReadingObj.Substring(4, 2), NumberStyles.HexNumber);
            string sStatus = "";

            if (System.Convert.ToBoolean(sOption & 0x40))
            {
                row[2] = "Not Present";
                row[9] = "bgcolor=white";
            }
            else
            {
                if ((sRaw == "0" && sType != "c0") && sSensorD == 0 && sDMSB == 0 && sType != "05")
                {
                    row[2] = "Not Present";
                    row[9] = "bgcolor=white";

                }
                else if (sType == "05")
                {
                    sStatus = @"show disc state api("",Raw) ";
                    row[9] = "showdiscstateapi.sensorhealth";//TODO :: stateApi
                    if (0x04 != 0x04)//TODO PlatformCapability
                    {
                        if (sRaw == "0")
                        {
                            //TODO: ButtonChassisIntrusionObj.style.visibility='hidden';
                        }
                        else
                        {
                            //     ButtonChassisIntrusionObj.style.visibility = 'visible';
                            //    ButtonChassisIntrusionObj.disabled = false;
                        }

                    }
                    row[1] = "&nbsp;";
                    row[2] = sStatus;
                }
                else if (sType == "08" || sType == "c0" || sType == "c2")
                {
                    sStatus = @"show disc state api(stype,SensorD) ";
                    row[1] = "&nbsp;";
                    row[2] = sStatus;
                    row[9] = "showdiscstateapi.sensorhealth";//TODO :: stateApi

                }
                else
                {
                    row[1] = "Not Supported";
                    row[9] = "bgcolor=white";
                }

            }
            //return sType+" "+sReadingObj+" "+sOption.ToString()+" "+sRaw;
            return $"{sType} {sReadingObj} {sOption.ToString()} {sRaw} {sSensorD} {sDMSB}";
        }
        static string covert16(string vars, string s_type)
        {
            if (s_type == "01")
            {
                int s = int.Parse(vars.Substring(0, 2), NumberStyles.HexNumber);

                return s.ToString() + " 度";
            }
            else if (s_type == "02")
            {
                int s = int.Parse(vars.Substring(0, 2), NumberStyles.HexNumber);

                return ToSigned(s, 8).ToString() + " V"; //TODO mark
            }
            else if (s_type == "04")
            {
                int s = int.Parse(vars.Substring(0, 2), NumberStyles.HexNumber);

                return s.ToString() + " rpm";
            }
            else
            {
                return vars + " others ";
            }

        }

        static int ToSigned(int Num, int signedbitB)
        {
            if (signedbitB > 0)
            {
                if ((Num % (0x01 << signedbitB) / (0x01 << (signedbitB - 1))) < 1)
                {
                    return Num % (0x01 << signedbitB - 1);
                }
                else
                {
                    int tmp = (Num % (0x01 << signedbitB - 1)) ^ ((0x01 << signedbitB - 1) - 1);
                    return (-1 - tmp);
                }




            }
            else
            {
                return Num;
            }

        }
        private static DataSet StringToDataSet(string str)
        {
            StringReader sr = new StringReader(str);
            DataSet ds = new DataSet();
            ds.ReadXml(sr);
            return ds;
        }

         static string UrlEn(string ip)
        {
            string iphead = "http://" + ip + "/cgi/ipmi.cgi?";
            string e_time = DateTime.Now.ToString("ddd MMM yyyy HH:mm:ss", CultureInfo.CreateSpecificCulture("en-GB")).ToString();
            string dstr = "SENSOR_INFO_FOR_SYS_HEALTH.XML=(1,ff)&time_stamp=" + e_time + " GMT+0800 (China Standard Time)";// 


            string url = HttpUtility.UrlEncode(dstr, Encoding.Default).Replace("+", "%20").Replace("%3d", "=").Replace("%26", "&");

            return iphead + url;
        }

        static float afterFuncSensor(DataRow dr , string mark ,bool tag)
        {
            float res;
            if (tag)
            {
                string rm = tenToSixteen(ToSigned(int.Parse(dr[mark].ToString(), NumberStyles.HexNumber), 8).ToString());
                res = float.Parse(SensorFunc(rm, dr["M"].ToString(), dr["B"].ToString(), dr["RB"].ToString()).ToString());
            }
            else
            {
           res= float.Parse(SensorFunc(dr[mark].ToString(), dr["M"].ToString(), dr["B"].ToString(), dr["RB"].ToString()).ToString());
            }

            return res;
        }
        static int NeedCompare = 0;
        static string Unit_ = "";
        static void SensorFormula(DataRow node,DataRow SensorTableArray)
        {
            string SensorType = node["STYPE"].ToString();
            var SensorReadingObj = node["READING"].ToString();
            var RawReading = SensorReadingObj.Substring(0, 2);
            var Option = int.Parse (node["OPTION"].ToString(), NumberStyles.HexNumber);
            string SFormula = node["L"].ToString();
            var UnitType1 = int.Parse(node["UNIT1"].ToString(), NumberStyles.HexNumber);
            var UnitType = int .Parse(node["UNIT"].ToString(), NumberStyles.HexNumber);
            string Unit="";
            var AnalogDataFormat = UnitType1 >> 6;
            string UNR, UC, UNC, LNC, LC, LNR;
            string ReadingDataFormat;
            NeedCompare = 0;

            switch (UnitType)
            {
                case 0x01:
                    Unit = "degrees C";
                    break;
                case 0x02:
                    Unit = " degrees F";
                    break;
                case 0x03:
                    Unit = " degrees K";
                    break;
                case 0x04:
                    Unit = " Volts";
                    break;
                case 0x05:
                    Unit = " Amps";
                    break;
                case 0x06:
                    Unit = " Watts";
                    break;
                case 0x07:
                    Unit = " Joules";
                    break;
                case 0x12:
                    Unit = " R.P.M";
                    break;
                case 0x13:
                    Unit = " Hz";
                    break;
                default:
                    break;
            }
            Unit_ = Unit;
            if (AnalogDataFormat == 0x02)
            {
                ReadingDataFormat =tenToSixteen (ToSigned(int.Parse(RawReading, NumberStyles.HexNumber), 8).ToString());
                UNR = tenToSixteen( ToSigned(int.Parse(node["UNR"].ToString(), NumberStyles.HexNumber), 8).ToString());
                UC = tenToSixteen(ToSigned(int.Parse(node["UC"].ToString(), NumberStyles.HexNumber), 8).ToString());//ToSigned(parseInt(node.getAttribute("UC"), 16), 8).toString(16);
                UNC = tenToSixteen(ToSigned(int.Parse(node["UNC"].ToString(), NumberStyles.HexNumber), 8).ToString());//ToSigned(parseInt(node.getAttribute("UNC"), 16), 8).toString(16);
                LNC = tenToSixteen(ToSigned(int.Parse(node["LNC"].ToString(), NumberStyles.HexNumber), 8).ToString());// ToSigned(parseInt(node.getAttribute("LNC"), 16), 8).toString(16);
                LC = tenToSixteen(ToSigned(int.Parse(node["LC"].ToString(), NumberStyles.HexNumber), 8).ToString());// ToSigned(parseInt(node.getAttribute("LC"), 16), 8).toString(16);
                LNR = tenToSixteen(ToSigned(int.Parse(node["LNR"].ToString(), NumberStyles.HexNumber), 8).ToString());//ToSigned(parseInt(node.getAttribute("LNR"), 16), 8).toString(16);
            }
            else
            {
                ReadingDataFormat = RawReading;
                UNR = node["UNR"].ToString();
                UC = node["UC"].ToString();//node.getAttribute("UC");
                UNC = node["UNC"].ToString();//node.getAttribute("UNC");
                LNC = node["LNC"].ToString();//node.getAttribute("LNC");
                LC = node["LC"].ToString(); //node.getAttribute("LC");
                LNR = node["LNR"].ToString(); //node.getAttribute("LNR");
            }

            var AfterFuncSensorReading = float.Parse(SensorFunc(ReadingDataFormat, node["M"].ToString(), node["B"].ToString(), node["RB"].ToString()).ToString(),NumberStyles.Integer);

        var AfterFuncSensorUNR = float.Parse(SensorFunc(UNR, node["M"].ToString(), node["B"].ToString(), node["RB"].ToString()).ToString() , NumberStyles.Integer);
            var AfterFuncSensorUC = float.Parse(SensorFunc(UC, node["M"].ToString(), node["B"].ToString(), node["RB"].ToString()).ToString(), NumberStyles.Integer);
            var AfterFuncSensorUNC = float.Parse(SensorFunc(UNC, node["M"].ToString(), node["B"].ToString(), node["RB"].ToString()).ToString(), NumberStyles.Integer);// parseFloat(SensorFunc(UNC, node.getAttribute("M"), node.getAttribute("B"), node.getAttribute("RB")), 10);
            var AfterFuncSensorLNC = float.Parse(SensorFunc(LNC, node["M"].ToString(), node["B"].ToString(), node["RB"].ToString()).ToString(), NumberStyles.Integer);//parseFloat(SensorFunc(LNC, node.getAttribute("M"), node.getAttribute("B"), node.getAttribute("RB")), 10);
            var AfterFuncSensorLC = float.Parse(SensorFunc(LC, node["M"].ToString(), node["B"].ToString(), node["RB"].ToString()).ToString(), NumberStyles.Integer);// parseFloat(SensorFunc(LC, node.getAttribute("M"), node.getAttribute("B"), node.getAttribute("RB")), 10);
            var AfterFuncSensorLNR = float.Parse(SensorFunc(LNR, node["M"].ToString(), node["B"].ToString(), node["RB"].ToString()).ToString(), NumberStyles.Integer);// parseFloat(SensorFunc(LNR, node.getAttribute("M"), node.getAttribute("B"), node.getAttribute("RB")), 10);
            if (!System.Convert.ToBoolean(Option & 0x40))
            {
                if (SensorTableArray!=null)
                {
                    SensorTableArray[2] = "Not Present!";
                    SensorTableArray[9] = "bgcolor=white";

                }
            }
            else
            {
                if (SFormula == "00")
                {
               //     var SFunction = SFuction1(0);

                    if (SensorTableArray != null)
                    {
                        SensorTableArray[8] = SFuction1(AfterFuncSensorUNR);//SensorFormula.SensorUNR;
                        SensorTableArray[7] = SFuction1(AfterFuncSensorUC);// SensorFormula.SensorUC;
                        SensorTableArray[6] = SFuction1(AfterFuncSensorUNC);//SensorFormula.SensorUNC;
                        SensorTableArray[5] = SFuction1(AfterFuncSensorLNC);// SensorFormula.SensorLNC;
                        SensorTableArray[4] = SFuction1(AfterFuncSensorLC);// SensorFormula.SensorLC;
                        SensorTableArray[3] = SFuction1(AfterFuncSensorLNR); //SensorFormula.SensorLNR;

                    }

                    if (RawReading == "00" && Option != 0x00 && SensorType == "04")
                    {
                     //   var SensorReading = 0;
                        if (SensorTableArray != null)
                            SensorTableArray[2] = "0 " + Unit;
                        NeedCompare = 1;
                        SensorReading = 0;
                    }
                    else if ((node["READING"].ToString() == "") || (RawReading == "00"))
                    {
                        if (SensorTableArray!=null)
                            SensorTableArray[2] = "Not Present!";
                    }
                    /* 	linear_reading	*/
                    else
                    {
                       float re = float.Parse((AfterFuncSensorReading * float.Parse(SensorReadingScale.ToString())).ToString()) / SensorReadingScale;
                        if (SensorTableArray!=null)
                            SensorTableArray[2] = re + " " + Unit;//SensorReading + " " + Unit;
                        NeedCompare = 1;
                        SensorReading = re;
                    }
                }
                else if (SFormula == "07")
                {
             //       var SFunction = SFuction2(0);

                    if (SensorTableArray != null)
                    {
                        SensorTableArray[8] = SFuction2(AfterFuncSensorUNR);//SensorFormula.SensorUNR;
                        SensorTableArray[7] = SFuction2(AfterFuncSensorUC);// SensorFormula.SensorUC;
                        SensorTableArray[6] = SFuction2(AfterFuncSensorUNC);//SensorFormula.SensorUNC;
                        SensorTableArray[5] = SFuction2(AfterFuncSensorLNC);// SensorFormula.SensorLNC;
                        SensorTableArray[4] = SFuction2(AfterFuncSensorLC);// SensorFormula.SensorLC;
                        SensorTableArray[3] = SFuction2(AfterFuncSensorLNR); //SensorFormula.SensorLNR;

                    }

                    if (RawReading == "00" && Option != 0x00 && SensorType == "04")
                    {
                        SensorReading = 0;
                        if (SensorTableArray != null)
                            SensorTableArray[2] = "0 " + Unit;
                        NeedCompare = 1;
                    }
                    else if ((node["READING"].ToString() == "") || (RawReading == "ff") || (RawReading == "00"))
                    {
                        if (SensorTableArray != null)
                            SensorTableArray[2] = "Not Present!";
                    }
                    /* 	linear_reading	*/
                    else
                    {
                        int re = int.Parse((float.Parse(SensorReadingScale.ToString()) / AfterFuncSensorReading).ToString()) / SensorReadingScale;
                        if (SensorTableArray!=null)
                            SensorTableArray[2] = SensorReading + " " + Unit;
                        NeedCompare = 1;
                        SensorReading = re;
                    }
                }
                else if (SFormula == "08")
                {
                    if (SensorTableArray != null)
                    {
                        SensorTableArray[8] = SFuction3(AfterFuncSensorUNR);//SensorFormula.SensorUNR;
                        SensorTableArray[7] = SFuction3(AfterFuncSensorUC);// SensorFormula.SensorUC;
                        SensorTableArray[6] = SFuction3(AfterFuncSensorUNC);//SensorFormula.SensorUNC;
                        SensorTableArray[5] = SFuction3(AfterFuncSensorLNC);// SensorFormula.SensorLNC;
                        SensorTableArray[4] = SFuction3(AfterFuncSensorLC);// SensorFormula.SensorLC;
                        SensorTableArray[3] = SFuction3(AfterFuncSensorLNR); //SensorFormula.SensorLNR;

                    }
                    //var SFunction = function(val)

                    //{
                    //     return parseInt(parseFloat(Math.pow(val, 2), 10) * SensorReadingScale, 10) / SensorReadingScale;
                    // } 

                    if (RawReading == "00" && Option != 0x00 && SensorType == "04")
                    {
                        SensorReading = 0;
                        if (SensorTableArray != null)
                            SensorTableArray[2] = "0 " + Unit;
                        NeedCompare = 1;
                    }
                    else if ((node["READING"].ToString() == "") || (RawReading == "ff") || (RawReading == "00"))
                    {
                        if (SensorTableArray!=null)
                            SensorTableArray[2] = "Not Present!";
                    }
                    /* 	linear_reading	*/
                    else
                    {
                        int  sr = (int.Parse(Math.Pow(AfterFuncSensorReading, 2).ToString()) * SensorReadingScale) / SensorReadingScale;
                        if (SensorTableArray!= null)
                            SensorTableArray[2] = sr + " " + Unit;
                      SensorReading = sr;
                        NeedCompare = 1;
                    }
                }
                //SensorUNR = SFunction(AfterFuncSensorUNR);
                //SensorUC = SFunction(AfterFuncSensorUC);
                //SensorUNC = SFunction(AfterFuncSensorUNC);
                //SensorLNC = SFunction(AfterFuncSensorLNC);
                //SensorLC = SFunction(AfterFuncSensorLC);
                //SensorLNR = SFunction(AfterFuncSensorLNR);
                //if (SensorTableArray!=null)
                //{
                //    SensorTableArray[8] = SFunction(AfterFuncSensorUNR);//SensorFormula.SensorUNR;
                //    SensorTableArray[7] = SFunction(AfterFuncSensorUC);// SensorFormula.SensorUC;
                //    SensorTableArray[6] = SFunction(AfterFuncSensorUNC);//SensorFormula.SensorUNC;
                //    SensorTableArray[5] = SFunction(AfterFuncSensorLNC);// SensorFormula.SensorLNC;
                //    SensorTableArray[4] = SFunction(AfterFuncSensorLC);// SensorFormula.SensorLC;
                //    SensorTableArray[3] = SFunction(AfterFuncSensorLNR); //SensorFormula.SensorLNR;

                //}

            }
        }
        
        static float SFuction2(float val)
        {
            return (SensorReadingScale / val) / SensorReadingScale;
            //return 0;
        }
        static float SFuction3(float val)
        {
     //       return parseInt(parseFloat(Math.pow(val, 2), 10) * SensorReadingScale, 10) / SensorReadingScale;

            return (float.Parse(Math.Pow(val, 2).ToString()) * float.Parse( SensorReadingScale.ToString())) 
                / float.Parse( SensorReadingScale.ToString());
        }
        static float  SFuction1(float val)
        {
            return val * SensorReadingScale/ SensorReadingScale;
            //return 0;
        }
        static int SensorReadingScale = 1000;
        static float SensorReading = 0;



        static double    SensorFunc(string raw_data, string m,string  b, string rb)
        {
            double  sensor_data;

            int  M_raw, M_data;
            int B_raw, B_data;
            int  Km_raw, Km_data;
            int  Kb_raw, Kb_data;


            /* change sequense of lsb and msb into 10b char */
            M_raw = ((int.Parse(m, NumberStyles.HexNumber) & 0xC0) << 2) + (int.Parse(m, NumberStyles.HexNumber) >> 8);
            B_raw = ((int.Parse(b, NumberStyles.HexNumber) & 0xC0) << 2) + (int.Parse(b, NumberStyles.HexNumber) >> 8);
            if (B_raw!=0)
            {
       //         Console.WriteLine(B_raw + "----------------------------------------------------   ");
            }

            Km_raw = int.Parse(rb, NumberStyles.HexNumber) >> 4;
            Kb_raw = (int.Parse(rb, NumberStyles   .HexNumber) & 0x0F);

            M_data = ToSigned(M_raw, 10);
            B_data = ToSigned(B_raw, 10);
            Km_data = ToSigned(Km_raw, 4);
            Kb_data = ToSigned(Kb_raw, 4);
            try
            {

                //      sensor_data = (M_data * int.Parse(raw_data, NumberStyles.HexNumber) +
                //Convert.ToInt32(
                //B_data * Math.Pow(10, Convert.ToDouble(Kb_data)) *

                //Math.Pow(10, Convert.ToDouble(Km_data)))

                //);


                sensor_data = (double)(M_data * int.Parse(raw_data, NumberStyles.HexNumber) +(double) B_data * Math.Pow(10, (double)Kb_data))
                    
                    
                    
                    
                    * Math.Pow(10, (double)Km_data);
               // sensor_data = Convert.ToInt32(_data);


            }
            catch (Exception)
            {
                sensor_data = 0;
            //    throw;
            }
      

            return sensor_data;

        }
        public static string tenToSixteen(string msg)
        {
            long number = Convert.ToInt64(msg);
            return Convert.ToString(number, 16);
        }
    }
}
