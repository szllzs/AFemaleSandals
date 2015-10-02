using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading;

namespace MoppetSandal
{
    enum VirtualBoxConmand
    {

    }
    class Program
    {
        static string defaultVirtualMachinePath;
        static int cycleIndex;
        static List<string> sourcesMachines;
        static int startQuantity;
        static int interval;
        private static string ToolPath = Directory.GetCurrentDirectory();   //@"d:\MoppetSandal";
        private static string ConmandFile = "VirtualBoxConmand.bat"; //ToolPath+ @"\VirtualBoxConmand.bat";
        static void Main(string[] args)
        {
           // Console.WriteLine("请输入你虚拟机的位置如：f:\\system(英文下输入)");
           // string virtualPath=  Console.ReadLine();
            sourcesMachines = GetVirtralSystemNames(@"f:\myxp");
            ExecuteDelete();
            ExecuteStart();
            for (int i = 0; i < 3; i++)
            {
                ExecuteCopy();
            }
        }

        private static void ExecuteCopy()
        {
            List<string> needCopyList = GetNeedCopyNames(sourcesMachines);
            for (int i = 0; i < needCopyList.Count; i++)
            {
                int j = i * 2;
                List<string> tmpCopyList = new List<string>();
                if (j + 1 <= needCopyList.Count)
                {
                    if (j + 1 != needCopyList.Count)
                    {
                        tmpCopyList.Add(needCopyList[j]);
                        tmpCopyList.Add(needCopyList[j + 1]);
                    }
                    else
                    {
                        tmpCopyList.Add(needCopyList[j]);
                    }
                    List<string> theNewNames=new List<string>();
                    WriteCopyConmand(tmpCopyList,ref  theNewNames);
                    ExecuteConmand();
                    Thread.Sleep(600000);
                    WritePowerOffConmand(theNewNames);
                    ExecuteConmand();
                }

            }
        }

        private static void ExecuteStart()
        {
            List<string> needStartsList = GetNeedStartNames(sourcesMachines);
            for (int i = 0; i < needStartsList.Count; i++)
            {
                int j = i * 2;
                List<string> tmpStartsList = new List<string>();
                if (j + 1 <= needStartsList.Count)
                {
                    if (j + 1 != needStartsList.Count)
                    {
                        tmpStartsList.Add(needStartsList[j]);
                        tmpStartsList.Add(needStartsList[j + 1]);
                    }
                    else
                    {
                        tmpStartsList.Add(needStartsList[j]);
                    }
                    List<string> theNewNames = new List<string>();
                    WriteStartConmand(tmpStartsList);
                    ExecuteConmand();
                   Thread.Sleep(600000);
                    WritePowerOffConmand(tmpStartsList);
                    ExecuteConmand();
                }
            }
        }
       
        private static void ExecuteDelete()
        {
            List<string> needDeleteList = GetNeedDeleteNames(sourcesMachines);
            for (int i = 0; i < needDeleteList.Count; i++)
            {
                int j = i * 2;
                List<string> tmpNeedDelete = new List<string>();
                if (j + 1 <= needDeleteList.Count)
                {
                    if (j + 1 != needDeleteList.Count)
                    {
                        tmpNeedDelete.Add(needDeleteList[j]);
                        tmpNeedDelete.Add(needDeleteList[j + 1]);
                    }
                    else
                    {
                        tmpNeedDelete.Add(needDeleteList[j]);
                    }
                    WriteStartConmand(tmpNeedDelete);
                    ExecuteConmand();
                    Thread.Sleep(600000);
                    WritePowerOffConmand(tmpNeedDelete);
                    ExecuteConmand();
                    WriteDeleteConmand(tmpNeedDelete);
                    ExecuteConmand();
                }
            }
        }

        public static bool ExecuteConmand()
        {
            bool result = false;
            try
            {
                Process proc = new Process();
                proc.StartInfo.FileName = ConmandFile;
                proc.StartInfo.CreateNoWindow = false;
                proc.Start();
                proc.WaitForExit();
                result = true;
                proc.Close();
                proc.Dispose();
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public static void WriteCopyConmand(string copyName)
        {
           
            using (StreamWriter sw = new StreamWriter(ConmandFile, false, Encoding.UTF8))
            {
                sw.WriteLine(string.Format("cd {0}", ToolPath));
                string dateTime = DateTime.Now.ToString("yyyyMMdd");
                string name = copyName + "copy" + new Random().Next(0, 1000) + "_" + dateTime;
                sw.WriteLine(string.Format("VBoxManage clonevm {0} --name {1} --register --mode all", copyName, name));
                sw.WriteLine(string.Format("vboxmanage startvm  {0}", name));
            }
        }
        public static void WriteCopyConmand(List<string> copyNames,ref List<string> theNewNames)
        {
            
            using (StreamWriter sw = new StreamWriter(ConmandFile, false, Encoding.UTF8))
            {
                sw.WriteLine(string.Format("cd {0}", ToolPath));
                string dateTime = DateTime.Now.ToString("yyyyMMdd");
                List<string> names = new List<string>();
                theNewNames = names;
                names.Clear();
                if (copyNames.Count > 0)
                {
                    foreach (string copyName in copyNames)
                    {
                        string name = copyName + "copy" + new Random().Next(0, 100000) + "_" + dateTime;
                        names.Add(name);
                        sw.WriteLine(string.Format("VBoxManage clonevm {0} --name {1} --register --mode all", copyName, name));
                    }
                    for (int i = 0; i < copyNames.Count; i++)
                    {
                        sw.WriteLine(string.Format("vboxmanage startvm  {0}", names[i]));
                    }

                }

            }
        }

        public static void WriteDeleteConmand(List<string> deleteNames)
        {
           
            using (StreamWriter sw = new StreamWriter(ConmandFile, false, Encoding.UTF8))
            {
                sw.WriteLine(string.Format("cd {0}", ToolPath));
                foreach (string deleteName in deleteNames)
                {
                   // sw.WriteLine(string.Format("vboxmanage controlvm {0} poweroff", deleteName));
                    sw.WriteLine(string.Format("vboxmanage unregistervm {0} --delete", deleteName));
                }
            }
        }
       
        public static void WritePowerOffConmand(List<string> deleteNames)
        {
           
            using (StreamWriter sw = new StreamWriter(ConmandFile, false, Encoding.UTF8))
            {
                sw.WriteLine(string.Format("cd {0}", ToolPath));
                foreach (string deleteName in deleteNames)
                {
                    sw.WriteLine(string.Format("vboxmanage controlvm {0} poweroff", deleteName));
                }
            }
        }


        public static void WriteStartConmand(List<string> startNames)
        {
           
            using (StreamWriter sw = new StreamWriter(ConmandFile, false, Encoding.UTF8))
            {
                sw.WriteLine(string.Format("cd {0}", ToolPath));
                foreach (string startName in startNames)
                {
                    sw.WriteLine(string.Format("vboxmanage startvm  {0}", startName));
                   
                }
            }
        }
        public static void WriteStartConmand(string startName)
        {
           
            using (StreamWriter sw = new StreamWriter(ConmandFile, false, Encoding.UTF8))
            {
                sw.WriteLine(string.Format("cd {0}", ToolPath));
                sw.WriteLine(string.Format("vboxmanage startvm  {0}", startName));
            }
        }

        public static List<string> GetVirtralSystemNames(string path)
        {
            List<string> result = new List<string>();
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            try
            {
                var directories = directoryInfo.GetDirectories();
                foreach (DirectoryInfo directory in directories)
                {
                    result.Add(directory.Name);
                }
            }
            catch (Exception)
            {
                result = null;
            }


            return result;
        }

        private static List<string> GetNeedCopyNames(List<string> names)
        {
            List<string> result = new List<string>();

            for (int i = 0; i < names.Count; i++)
            {
                if (!names[i].Contains("_"))
                {
                    result.Add(names[i]);
                }

            }
            return result;
        }

        private static List<string> GetNeedDeleteNames(List<string> names)
        {
            List<string> result = new List<string>();
            List<int> intResult = new List<int>();
            foreach (string name in names)
            {
                if (name.Contains("_"))
                {
                    string str = (name.Split('_'))[1];  //后半部分
                    intResult.Add(Int32.Parse(str));
                }
            }
            if (intResult.Count > 0)
            {
                intResult = intResult.Distinct().ToList();
                intResult.Sort();
            }
            if (intResult.Count >= 3)
            {
                foreach (string name in names)
                {
                    if (name.Contains(intResult.Min().ToString()))
                    {
                        result.Add(name);
                    }
                }
            }

            return result;
        }

        private static List<string> GetNeedStartNames(List<string> names)
        {
            List<string> result = new List<string>();
            List<int> intResult = new List<int>();
            foreach (string name in names)
            {
                if (name.Contains("_"))
                {
                    string str = (name.Split('_'))[1];  //后半部分
                    intResult.Add(Int32.Parse(str));
                }
            }
            if (intResult.Count > 0)
            {
                intResult = intResult.Distinct().ToList();
                intResult.Sort();
            }
            if (intResult.Count >= 3)
            {
                intResult.RemoveAt(0);
            }
            foreach (var name in names)
            {
                foreach (var i in intResult)
                {
                    if (name.Contains(i.ToString()))
                    {
                        result.Add(name);
                    }
                }
            }
            return result;
        }
    }

    public class UserConfigureInfo
    {
        private List<string> virtrualBaseName = new List<string>();
        public static string DefaultVirtualMachinePath;
        public static int CycleIndex;
        public static List<string> SourcesMachines;
        public static int StartQuantity;
        public static int Interval;

        public UserConfigureInfo()
        {
            SourcesMachines = this.GetSourcesMachines();
            Console.WriteLine("你要复制的虚拟机有：...........................................");
            for (int i = 0; i < SourcesMachines.Count; i++)
            {
                Console.WriteLine(string.Format("第{0}个虚拟机为:{1}", i, SourcesMachines[i]));
            }
            DefaultVirtualMachinePath = this.GetDefaultVirtualMachinePath();

            Console.WriteLine(DefaultVirtualMachinePath + "目录下有文件夹：.......................");
            DirectoryInfo directoryInfo = new DirectoryInfo(DefaultVirtualMachinePath);
            var directoryInfos = directoryInfo.GetDirectories();
            for (int i = 0; i < directoryInfos.Length; i++)
            {
                Console.WriteLine(string.Format("第{0}个文件为:{1}", i, directoryInfos[i].Name));
            }

            StartQuantity = this.GetStartQuantity();
            Console.WriteLine(string.Format("你的电脑将同时启动{0}个：.......................", StartQuantity));

            CycleIndex = this.GetCycleIndex();
            Console.WriteLine(string.Format("一共循环{0}个：.......................", CycleIndex));
            Console.WriteLine(string.Format("你将会创建启动或者删除{0}个虚拟机：.......................", CycleIndex * StartQuantity * SourcesMachines.Count));
            Console.WriteLine(string.Format("大约使用{0}G硬盘空间：.......................", CycleIndex * StartQuantity * SourcesMachines.Count * 3 * 2));
            Interval = this.GetInterval();
            Console.WriteLine(string.Format("此次过程将会一共耗时{0}分钟多：.......................", CycleIndex * SourcesMachines.Count * 3 * Interval));
            Console.WriteLine(string.Format("上面的数字为估算，请以实际过程为准.............."));

        }
        private List<string> GetSourcesMachines()
        {
            List<string> result = new List<string>();
            do
            {
                Console.WriteLine("请输入一个要复制的虚拟机名字:");
                string str = Console.ReadLine();
                if (!str.Equals("n"))
                {
                    result.Add(str);
                    virtrualBaseName.Add(str);
                }
                Console.WriteLine("如果需要继续添加虚拟机请输入小写：\"y\"。下一步操作任意键");
            } while ((Console.ReadLine().Equals("y")));
            return result;
        }

        private int GetStartQuantity()
        {
            int reslut = 0;
            Console.WriteLine("请输入同时启动虚拟机的个数（根据自己电脑性能决定一般填2-3个）:");
            string str = Console.ReadLine();
            if (Int32.Parse(str) >= 5 || Int32.Parse(str) <= 0)
            {
                do
                {
                    Console.WriteLine("输入1-4之间请从新输入！");
                    str = Console.ReadLine();
                } while (Int32.Parse(str) >= 5 || Int32.Parse(str) <= 0);
            }
            reslut = Int32.Parse(str);
            return reslut;
        }

        private int GetCycleIndex()
        {
            int reslut = 0;
            Console.WriteLine("请输入循环的次数（总数=循环次数×同时启动数）:");
            string str = Console.ReadLine();
            if (Int32.Parse(str) >= 6 || Int32.Parse(str) <= 0)
            {
                do
                {
                    Console.WriteLine("输入1-5之间，请从新输入！");
                    str = Console.ReadLine();
                } while (Int32.Parse(str) >= 6 || Int32.Parse(str) <= 0);
            }
            reslut = Int32.Parse(str);
            return reslut;
        }
        private int GetInterval()
        {
            int reslut = 0;
            Console.WriteLine("请输虚拟机开机等待时间（单位分钟，一般5分钟以上）:");
            string str = Console.ReadLine();
            if (Int32.Parse(str) >= 21 || Int32.Parse(str) <= 2)
            {
                do
                {
                    Console.WriteLine("输入3-20之间，请从新输入！");
                    str = Console.ReadLine();
                } while (Int32.Parse(str) >= 21 || Int32.Parse(str) <= 2);
            }
            reslut = Int32.Parse(str);
            return reslut;
        }

        private string GetDefaultVirtualMachinePath()
        {
            String result = null;
            Console.WriteLine("请输VirtualBox存放虚拟机的位置（注意符号的中英文输入）:");
            result = Console.ReadLine();
            if (!IsValidate(@result))
            {
                do
                {
                    Console.WriteLine("输入有吴！请从新输VirtualBox存放虚拟机的位置（注意符号的中英文输入）:");
                    result = Console.ReadLine();

                } while (!IsValidate(@result));
            }

            return result;
        }

        private bool IsValidate(string strPath)
        {
            bool result = false;
            DirectoryInfo directoryInfo = new DirectoryInfo(strPath);
            try
            {
                var directories = directoryInfo.GetDirectories();
                foreach (DirectoryInfo directory in directories)
                {
                    int i = 0;
                    if (directory.Name.Contains(virtrualBaseName[i]))
                    {
                        result = true;
                        break;
                    }
                    else
                    {
                        result = false;
                    }
                    i++;
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

    }


}
