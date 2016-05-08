using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;


namespace markPropertys
{
    //使用反射时，通过该标记判断，程序集是否符合主程序要求的规范，可以被主程序调用
    [AttributeUsage(AttributeTargets.Assembly)]
    public class canUsedBySystem : Attribute
    {
    }

    //标记该类是否用来和主程序交互；
    [AttributeUsage(AttributeTargets.Class)]
    public class AInteraction : Attribute
    {
    }

    //标记该方法是否可以调用
    [AttributeUsage(AttributeTargets.Method)]
    public class AcanRun : Attribute
    {
    }

    //dll中，标记参数是否是需要主程序获得的参数
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class AIsInput : Attribute
    {
    }

    //dll中，标记参数是否是需要主程序获取并输出展示的参数
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class AIsOutput : Attribute
    {
    }

    //dll中,参数的默认值标记
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class ADefaultValues : Attribute
    {
        public ADefaultValues(object obj)
        {
        }
        public ADefaultValues(object[] obj)
        {
        }
    }

    //dll中，对参数标记物理意义，便于理解
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class AMessage : Attribute
    {
        public AMessage(string msg)
        {
        }
    }
}