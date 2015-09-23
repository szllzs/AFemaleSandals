using System;
using System.Collections.Generic;
using System.Text;

namespace Moppet_sSandal
{
    class Program
    {
        static void Main(string[] args)
        {
            new UserConfigureInfo().GetSourcesMachines();
        }
    }

    class UserConfigureInfo
    {
        public string DefaultVirtualMachinePath;
        public int CycleIndex;
        public List<string> SourcesMachines;
        public int StartQuantity;
        public int Interval;

        public UserConfigureInfo()
        {
            Console.WriteLine("请按照提示进行输入，输入后请安回车进行下一步操作！！");
           
          
        }

        public List<string> GetSourcesMachines()
        {
            List<string> result=new List<string>();
            do
            {
                Console.WriteLine("请输入一个要复制的虚拟机名字:");
                string str= Console.ReadLine();
                if (!str.Equals("n"))
                {
                    result.Add(str);
                }
                Console.WriteLine("如果需要继续添加虚拟机请输入小写：\"y\"    下一步操作任意键");
            } while ((Console.ReadLine().Equals("y")));
            return result;
        }
    }
}
