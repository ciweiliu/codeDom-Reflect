using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using markPropertys;
using someInterface;


//标记该程序集是否可以被主程序调用
[assembly: canUsedBySystem]
namespace personClass
{
    //标记这个Person类用来与主程序交互，包括用到的一些参数和函数
    [AInteraction]
    public class Person : IdllFunc
    {
        [AIsInput, ADefaultValues("ciwei"), AMessage("人名")]   //标记name 是输入参数，默认值为ciwei,参数说明是人名
        public string name = "";
        [AIsInput, ADefaultValues(1), AMessage("年龄")]
        public int age = 0;
        [AIsInput, ADefaultValues(1e6), AMessage("原有资金")]
        public double money = 0;
        [AIsInput, ADefaultValues(true), AMessage("性别")]
        public bool isMan = true;


        [AIsInput, ADefaultValues(new object[] { "CAE", "system implement", "Program" }), AMessage("从事工作")]
        public string[] career = new string[3];


        //标记haveMoney是一个输出变量，参数说明是现有资金，
        //当前只能获取静态变量，普通变量怎么获取，还没查到。
        [AIsOutput, AMessage("现有资金")]
        public static double haveMoney;




        //实现接口1
        public void makeSomeMoney(double money2)
        {
            haveMoney = money + money2;
        }
        //实现接口2
        public void showMsg(string xx)
        {
            string xb = "男";
            if (isMan == false)
                xb = "女";


            StringBuilder sb = new StringBuilder();
            sb.Append("传入的参数是：" + xx + "\n");
            sb.Append("姓名：" + name + "\n");
            sb.Append("年龄：" + age.ToString() + "\n");
            sb.Append("我的性别：" + xb + "\n");
            sb.Append("从事工作：");
            foreach (string x in career)
                sb.Append(x + ",");
            sb.Append("\n");
            sb.Append("输入之后的资产：" + haveMoney.ToString() + "\n");
            Console.WriteLine(sb.ToString());
        }

        //该插件特有的一些函数，使用标记，标记该函数虽不在接口中，但是也是可以被主程序调用的。
        [AcanRun]
        public void showMyMsg()
        {
            Console.WriteLine("我不是接口的函数，但是我可以被调用，因为主程序约定可以调用无参函数，哇哈哈！");
        }
    }
}
