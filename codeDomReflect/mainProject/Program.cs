using System.CodeDom.Compiler;
using System.Reflection;
using System.Collections;
using markPropertys;
using dynamic;
using System.Collections.Generic;
using System;
using System.Text;

namespace mainProject
{
    class Program
    {
        //创建一个全局变量，保存通过界面输入的参数，并将该参数传给动态类，完成参数赋值
        static List<programData> inputDataList = new List<programData>();

        static void Main(string[] args)
        {
            Type interactionClass = null;
            string dllName = "personClass.dll";
            string fullInteractClassName = "";


            #region 找到可以系统调用的程序集名称
            //是否可以被系统集成
            Assembly personAsm = Assembly.LoadFrom(dllName);
            Attribute attr = Attribute.GetCustomAttribute(personAsm, typeof(canUsedBySystem));
            if (attr == null)
            {
                return;
            }
            #endregion
            #region 找到可以被交互调用的类，并获取类的全名 namespace.class
            Type[] ts = personAsm.GetTypes();
            foreach (Type t in ts)
            {
                Attribute inputAttr = Attribute.GetCustomAttribute(t, typeof(AInteraction));
                if (inputAttr != null)
                {
                    interactionClass = t;
                    fullInteractClassName = t.FullName;
                }
                else
                    interactionClass = null;
            }
            #endregion
            //创建类的实例，下面获取参数、设置参数、求解计算的部分都共用这个实例
            object obj = Activator.CreateInstance(interactionClass);
            //获取输入参数
            getINParams(obj);

            #region 在主程序中设置参数
            // 可以通过界面进行设置参数
            Console.WriteLine("我的名字:");
            inputDataList[0].参数值 = Console.ReadLine();


            Console.WriteLine("我的年龄：");
            inputDataList[1].参数值 = Console.ReadLine();


            Console.WriteLine("男？True or False：");
            inputDataList[3].参数值 = Console.ReadLine();

            Console.WriteLine("从事工作有哪些？");
            inputDataList[4].参数值 = Console.ReadLine();


            Console.WriteLine("我的资产：");
            inputDataList[2].参数值 = Console.ReadLine();

            #endregion
            // 动态生成包含执行参数设置函数的类
            genDynamicDll(fullInteractClassName, dllName);

            #region 调用动态生成的dll，执行赋值操作；因为这个代码是你写的，所以dll名称及内部信息都知道


            Assembly dynamicDll = Assembly.LoadFrom("dynamic.dll");
            Type dynamicClass = dynamicDll.GetType("dynamicNS.dynamicClass");
            object dynamicObj = Activator.CreateInstance(dynamicClass);


            //调用函数
            MethodInfo setm = dynamicClass.GetMethod("setValues", new Type[] { interactionClass });
            setm.Invoke(dynamicObj, new object[] { obj });
            #endregion
            #region 设置makeSomeMoney函数的参数列表;
            Console.WriteLine("我想赚到：");
            double doller = Convert.ToDouble(Console.ReadLine());
            #endregion


            #region 执行Person类中的函数


            MethodInfo[] methods = interactionClass.GetMethods();
            foreach (MethodInfo m in methods)
            {
                if (m.Name == "makeSomeMoney")      //肯定有这个函数，因为在接口中定义了
                {
                    m.Invoke(obj, new object[] { doller }); // 传入doller参数,执行makeSomeMoney函数  
                    continue;
                }
                if (m.Name == "showMsg")        // 肯定有这个函数，因为在接口中定义了
                {
                    m.Invoke(obj, new object[] { "主函数" });   //执行showMsg函数
                    continue;
                }


                //通过此标志获取所有可以使用的函数，但函数名还得约定好
                Attribute cattr = Attribute.GetCustomAttribute(m, typeof(AcanRun));
                if (cattr != null)
                {
                    m.Invoke(obj, null);
                    continue;
                }
            }
            #endregion

            Console.ReadKey();
        }


        static void getINParams(object t)
        {
            inputDataList.Clear();

            FieldInfo[] fi = t.GetType().GetFields();       //获取所有field
            StringBuilder sb = new StringBuilder();
            foreach (FieldInfo f in fi)
            {

                //根据属性判断，是否是输入参数
                Attribute assAttr = Attribute.GetCustomAttribute(f, typeof(AIsInput));
                if (assAttr != null)
                {
                    string name = f.Name;       //参数名
                    programData data = new programData();
                    data.参数名称 = name;
                    sb.Append(name + "----;type is ");

                    Type ftp = f.FieldType;  //参数类型
                    data.参数类型 = ftp.Name;
                    sb.Append(ftp.Name + "----;default value is ");

                   // f.ge
                    //遍历各个参数中的默认值，目前参数可以设置为object,或者object[]
                    IList<CustomAttributeData> datalist = f.GetCustomAttributesData();
                    if (datalist.Count != 0)
                    {
                        foreach (CustomAttributeData da in datalist)
                        {
                            if (da.Constructor.DeclaringType.Name == "ADefaultValues")
                            {
                                string values = "";
                                Type tt = da.ConstructorArguments[0].ArgumentType;
                                object x = da.ConstructorArguments[0].Value;
                                if (tt.Name == "Object[]")
                                {
                                    System.Collections.ObjectModel.ReadOnlyCollection<System.Reflection.CustomAttributeTypedArgument> sa=(System.Collections.ObjectModel.ReadOnlyCollection<System.Reflection.CustomAttributeTypedArgument>)x;
                                    for (int i = 0; i < sa.Count; i++)
                                    {
                                        x = sa[i].Value;
                                        values += x.ToString() + ",";
                                        sb.Append(x.ToString() + ",");
                                    }
                                }
                                else
                                {// 单值            
                                    values = x.ToString();
                                    sb.Append(x.ToString());
                                }
                                data.参数值 = values;
                            }

                            if (da.Constructor.DeclaringType.Name == "AMessage")
                            {
                                object x = da.ConstructorArguments[0].Value;
                                data.参数描述 = x.ToString();
                            }
                        }
                    }
                    sb.Append("\n");
                    inputDataList.Add(data);
                }
            }
        }

        static void genDynamicDll(string fullClassName,string dllname)
        {
            // 获取插件类的名称
            string filenm = "..\\..\\dynamicCode.cs";
//将代码写到一个文件里。当然也可以直接写到内存里或一个流里面，比如StringWriter里面---  StringWriter sw = new StringWriter();
            
            //将所有参数分解到一个ArrayList里面，传入动态类。（暂且这么用，应该有更好的方式）
            ArrayList arr = new ArrayList();
            foreach (programData data in inputDataList)
            {
                if (data.参数值 == null)
                    continue;
                arr.Add(data.参数名称);
                arr.Add(data.参数类型);
                if (data.参数类型 == "Double")
                    arr.Add(Convert.ToDouble(data.参数值.Trim()));
                else if (data.参数类型 == "Int32")
                    arr.Add(Convert.ToInt32(data.参数值.Trim()));
                else if (data.参数类型 == "Boolean")
                    arr.Add(Convert.ToBoolean(data.参数值.Trim()));
                else if (data.参数类型 == "String")
                    arr.Add(data.参数值);
                else if (data.参数类型 == "Int32[]")
                    arr.Add(data.参数值);
                else if (data.参数类型 == "String[]")
                    arr.Add(data.参数值);
                else
                {
                }
            }
            
            dynamic.dynamic.generateCode(filenm, fullClassName, arr);


            CompilerResults crt = dynamic.dynamic.compileCode(filenm, dllname);


#if DEBUG   //输出一些调试信息
            if (crt.Errors.Count > 0)
            {
                for (int i = 0; i < crt.Output.Count; i++)
                    Console.WriteLine(crt.Output[i]);
                for (int i = 0; i < crt.Errors.Count; i++)
                    Console.WriteLine(i.ToString() + ": " + crt.Errors[i].ToString());
            }
            else
            {
                Console.WriteLine("compile ok");
            }
#endif
        }


    }
}
