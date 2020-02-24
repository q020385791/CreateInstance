using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using  DL=CreateInstance.DesignList;

namespace CreateInstance
{

    internal class ServerCenter
    {
        private readonly Form1 form;
        public ServerCenter(Form1 form)
        {
            this.form = form;
        }

        public void Run()
        {
            HttpListener server = new HttpListener();
            try
            {
                //finish
                server = new HttpListener
                {
                    //監聽90port
                    Prefixes = { "http://127.0.0.1:90/" },
                };
                server.Start();
            }
            catch (Exception exx)
            {

                MessageBox.Show(exx.Message);
            }
            while (true)
            {
                var context = server.GetContext();
                HttpListenerRequest request;
                request = context.Request;
                Stream body = request.InputStream;
                Encoding encoding = request.ContentEncoding;

                StreamReader reader = new StreamReader(body, encoding);

                HttpListenerResponse response = context.Response;
                string JsonData = reader.ReadToEnd();
                string mdethod = context.Request.HttpMethod;
                string FunctionName = context.Request.Url.Segments[1].Replace("/", "");
                try
                {
                    //有帶data
                    JObject jsonObj = new JObject();
                    //取得為Get還是Post

                    var method = Type.GetType("CreateInstance.DesignList").GetMethods()
                        .Where(mi => mi.GetCustomAttributes(true)
                        .Any(attr => attr is DL.Mapping
                        && ((DL.Mapping)attr).FunctionName == FunctionName && ((DL.Mapping)attr).Method == mdethod)).First();
                    if (JsonData != "")
                    {

                        jsonObj = JObject.Parse(JsonData);

                    }
                    else
                    {
                        //restful參數 方法後的參數 Ex:OpenCom/0
                        string strParams = context.Request.Url
                                           .Segments
                                           .Skip(2)
                                           .Select(k => k.Replace("/", "")).FirstOrDefault();

                        object PackageNameAndData = new { FunctionName = FunctionName, data = strParams };
                        jsonObj = JObject.FromObject(PackageNameAndData);
                    }


                    string result = "";
                    ThreadPool.QueueUserWorkItem((_) =>
                    {
                        DL FunctionFiles = new DL(form);
                        result = FunctionFiles.InvokeRestful(FunctionName, mdethod, jsonObj);
                        string retstr = JsonConvert.SerializeObject(result);
                        Console.WriteLine("result : " + (string)result);
                        StringBuilder builder = new StringBuilder((string)result);
                        string something = builder.ToString();
                        byte[] buffer = Encoding.UTF8.GetBytes(something);
                        response.ContentLength64 = buffer.Length;
                        response.ContentType = "text/html";
                        response.Headers.Add("Access-Control-Allow-Origin", "*");
                        response.Headers.Add("Access-Control-Allow-Methods", "POST, GET");
                        response.Headers.Add("Access-Control-Allow-Headers", "Origin, Content-Type, X-Auth-Token");
                        response.StatusDescription = "OK";
                        Stream st = response.OutputStream;
                        st.Write(buffer, 0, buffer.Length);
                        context.Response.Close();


                    });
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message);
                    MessageBox.Show(e.Message);
                }



            }


        }
    }
}