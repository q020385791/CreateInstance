using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static CreateInstance.DesignList;

namespace CreateInstance
{
    public class DesignList
    {
        private readonly Form1 form;
        public DesignList(Form1 form)
        {
            this.form = form;
        }
        public class Mapping : Attribute
        {
            public string Method;
            public string FunctionName;
            public Mapping(string _Method, string _FunctionName)
            {
                this.Method = _Method;
                this.FunctionName = _FunctionName;
            }

        }


        public string InvokeRestful(string FunctionName, string methodName, JObject data)
        {
            //Type t = Type.GetType("CreateInstance.DesignList");
            var method = Type.GetType("CreateInstance.DesignList").GetMethods()
                        .Where(mi => mi.GetCustomAttributes(true)
                        .Any(attr => attr is Mapping
                        && ((Mapping)attr).FunctionName == FunctionName && ((Mapping)attr).Method == methodName)).First();
            object DymaticClass = new object();
            var ParamterName = method.GetParameters();
            object[] objectlist = new object[ParamterName.Count()];

            var dataCount = data.Count;
            int C = 0;

            foreach (var item in ParamterName)
            {
                Console.WriteLine(item.Name);
                var Parametertype = item.ParameterType.FullName;
                Console.WriteLine(Parametertype.ToString());
                var oType = item.ParameterType;

                if (oType == typeof(string))
                {
                    objectlist[C] = (string)data[item.Name] as string;

                }
                else if (oType == typeof(int))
                {
                    objectlist[C] = (int)data[item.Name];
                }
                else if (oType == typeof(char[]))
                {
                    objectlist[C] = data[item.Name].ToObject<char[]>();
                }
                else if (oType == typeof(object))
                {
                    objectlist[C] = data[item.Name].ToObject<object>();
                }
                else
                {
                    Type ClassType = Type.GetType(item.ParameterType.FullName, true);
                    object[] ovalues = new object[data[item.Name].Count()];
                    List<string> values = new List<string>();
                    int Count = 0;
                    ////////////////////////////
                    var methodinfo = ClassType.GetMethod(item.ParameterType.FullName);
                    ParameterInfo[] paramvalue = new ParameterInfo[] { };
                    foreach (var singlevalue in data[item.Name])
                    {

                        var Valuetype = paramvalue[Count].ParameterType;

                        if (Valuetype == typeof(string))
                        {
                            ovalues[Count] = (string)data[item.Name] as string;
                        }
                        else if (Valuetype == typeof(int))
                        {
                            ovalues[Count] = (int)data[item.Name];
                        }
                        else if (Valuetype == typeof(char[]))
                        {
                            ovalues[Count] = data[item.Name].ToObject<char[]>();
                        }
                        else
                        {
                            ovalues[Count] = (string)singlevalue;
                        }
                       
                        Count++;
                    }

                    objectlist[C] = Activator.CreateInstance(ClassType, ovalues);
                }
                C++;
            }
            return (string)method.Invoke(this, objectlist);
        }

        [Mapping("GET", "StrParamType")]
        public string StrParamType(string param1, string param2)
        {

            return "ok";
        }
        [Mapping("GET", "DifferentParam")]
        public string DifferentParam(string strParam, int intParam)
        {


            return "ok2";
        }



    }
    public class FunctionList
    {
        
    }
}
