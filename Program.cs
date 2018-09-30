using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ClassGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var strPath = Path.GetFullPath("~/../");
            bool exists = Directory.Exists(strPath);
            if (exists)
            {
                //6tnjpu
                var jsonBizModel = File.ReadAllText(strPath+"BizModel.json");
                var jsonDataModel = File.ReadAllText(strPath+"DataModel.json");
                var jsonEntities = File.ReadAllText(strPath+"Entities.json");

                JObject _entities = JObject.Parse(jsonEntities);
                JObject _biz = JObject.Parse(jsonBizModel);
                JObject _data = JObject.Parse(jsonDataModel);

                List<JToken> lstJToken = new List<JToken>();
                #region Entities
                lstJToken = _entities["Entity"].Children().ToList();

                List<Entity> lstEntities = new List<Entity>();
                foreach(JToken entity in lstJToken)
                    lstEntities.Add(entity.ToObject<Entity>());
                #endregion

                #region Biz
                BizModel biz = _biz.ToObject<BizModel>();
                if(biz.Properties is null || biz.Methods is null)
                {
                    biz.Properties = new List<Property>();
                    biz.Methods = new List<Method>();
                }
                lstJToken = _biz["Property"].Children().ToList();
                foreach(JToken property in lstJToken)
                    biz.Properties.Add(property.ToObject<Property>());
                    
                lstJToken = _biz["Method"].Children().ToList();
                foreach(JToken method in lstJToken)
                {
                    Method _met = method.ToObject<Method>();
                    if (method["Parameter"] != null)
                    {
                        List<JToken> lstJTokenParmam  = method["Parameter"].Children().ToList();
                        if (lstJTokenParmam.Count > 0)
                        {
                            _met.Parameters = new List<Parameter>();
                            foreach(JToken parameter in lstJTokenParmam)
                                _met.Parameters.Add(parameter.ToObject<Parameter>());
                        }
                    }
                    biz.Methods.Add(_met);
                }
                #endregion

                #region Data
                DataModel data = _data.ToObject<DataModel>();
                if(data.Properties is null || data.Methods is null)
                {
                    data.Properties = new List<Property>();
                    data.Methods = new List<Method>();
                }
                lstJToken = _data["Property"].Children().ToList();
                foreach(JToken property in lstJToken)
                    data.Properties.Add(property.ToObject<Property>());
                    
                lstJToken = _data["Method"].Children().ToList();
                foreach(JToken method in lstJToken)
                {
                    Method _met = method.ToObject<Method>();
                    if (method["Parameter"] != null)
                    {
                        List<JToken> lstJTokenParmam  = method["Parameter"].Children().ToList();
                        if (lstJTokenParmam.Count > 0)
                        {
                            _met.Parameters = new List<Parameter>();
                            foreach(JToken parameter in lstJTokenParmam)
                                _met.Parameters.Add(parameter.ToObject<Parameter>());
                        }
                    }
                    data.Methods.Add(_met);
                }
                #endregion
                generateFiles(lstEntities, data, biz);
            }
            else
                Console.WriteLine("Entities.json was not found!");
        }

        internal static void generateFiles(List<Entity> lstEntities, DataModel data, BizModel biz)
        {
            var strPath     = Path.GetFullPath("~/");
            var strPathBiz  = Path.GetFullPath(strPath+biz.Path);
            var strPathData = Path.GetFullPath(strPath+data.Path);

            if (!Directory.Exists(strPathBiz))
                Directory.CreateDirectory(strPathBiz);
            if (!Directory.Exists(strPathData))
                Directory.CreateDirectory(strPathData);


            foreach (Entity ent in lstEntities)
            {
                var originalName = ent.className;
                if (ent.className[ent.className.Length - 1].ToString().ToUpper().Equals("S"))
                    ent.className = ent.className.Remove(ent.className.Length - 1);
                
                var entDataPath = strPathData+ent.className.Replace("Ent", string.Empty)+".DataAccess.cs";
                var entBizPath = strPathBiz+ent.className.Replace("Ent", string.Empty)+".Biz.cs";

                var bizProperty = "";
                var bizConstructor = "";
                var bizMethods = "";
                 
                if (biz.Properties != null)
                    foreach(Property pro in biz.Properties)
                    {
                        if (pro.Singleton.Equals("1"))
                            bizProperty += String.Format(Constants.DATA_PROPERTY_SINGLETON, pro.Type.Replace("#CLASSOBJECT#", originalName).Replace("#DATALAYER#", ent.className.Replace("Ent", string.Empty)+"DataAccess"), pro.Name.ToLower());

                        bizProperty += String.Format(
                            Constants.DATA_PROPERTY,
                            pro.Visibility,
                            pro.Type.Replace("#CLASSOBJECT#", originalName).Replace("#DATALAYER#", ent.className.Replace("Ent", string.Empty)+"DataAccess"),
                            pro.Name,
                            pro.Initializers.Contains("GET") ? 
                                pro.Singleton.Equals("0") ? Constants.GET_NOT_SINGLETON : String.Format(Constants.GET_SINGLETON, pro.Name.ToLower(), pro.Type.Replace("#CLASSOBJECT#", originalName).Replace("#DATALAYER#", ent.className.Replace("Ent", string.Empty)+"DataAccess")) 
                                : String.Empty,
                            pro.Initializers.Contains("SET") ?
                                pro.Singleton.Equals("0") ? Constants.SET_NOT_SINGLETON : String.Format(Constants.SET_SINGLETON, pro.Name.ToLower())
                                : String.Empty
                        );
                    }
                if (biz.Methods != null)
                    foreach (Method met in biz.Methods)
                    {
                        if (met.Name.Contains("CONSTRUCTOR"))
                        {
                            bizConstructor += String.Format(Constants.CONTRUCTOR_DECLARATION,
                                met.Visibility,
                                ent.className.Replace("Ent", string.Empty)+"Biz",
                                met.Parameters != null && met.Parameters.Count > 0 ? getParameter(met.Parameters) : string.Empty,
                                met.AfterMethod.Equals("NONE") ? String.Empty : met.AfterMethod                                
                                );
                            bizConstructor += String.Format(Constants.CONSTRUCTOR_DESCRIPTION, met.MethodContent.Equals("NONE") ? String.Empty : met.MethodContent.Replace("#CLASSOBJECT#",ent.className.Replace("Ent", string.Empty)));                            
                        }
                        else
                        {
                            bizMethods += String.Format(Constants.DATA_METHOD,
                                met.Visibility,
                                met.SpecialAttribute.Equals("NONE") ? String.Empty : met.SpecialAttribute,
                                met.ReturnType.Equals("NONE") ? String.Empty : met.ReturnType,
                                met.Name,
                                met.Parameters != null && met.Parameters.Count > 0 ? getParameter(met.Parameters) : string.Empty,
                                met.AfterMethod.Equals("NONE") ? String.Empty : met.AfterMethod,
                                met.MethodContent.Equals("") || met.MethodContent.Equals("NONE") ? String.Empty : met.MethodContent.Replace("#CLASSOBJECT#",ent.className.Replace("Ent", string.Empty))
                                );
                        }
                    }
                

                var dataProperty = "";
                var dataConstructor = "";
                var dataMethods = "";

                if (data.Properties != null)
                    foreach(Property pro in data.Properties)
                    {
                        if (pro.Singleton.Equals("1"))
                            dataProperty += String.Format(Constants.DATA_PROPERTY_SINGLETON, pro.Type.Replace("#CLASSOBJECT#", originalName), pro.Name.ToLower());

                        dataProperty += String.Format(
                            Constants.DATA_PROPERTY,
                            pro.Visibility,
                            pro.Type.Replace("#CLASSOBJECT#", originalName),
                            pro.Name,
                            pro.Initializers.Contains("GET") ? 
                                pro.Singleton.Equals("0") ? Constants.GET_NOT_SINGLETON : String.Format(Constants.GET_SINGLETON, pro.Name.ToLower(), originalName) 
                                : String.Empty,
                            pro.Initializers.Contains("SET") ?
                                pro.Singleton.Equals("0") ? Constants.SET_NOT_SINGLETON : String.Format(Constants.SET_SINGLETON, pro.Name.ToLower())
                                : String.Empty
                        );
                    }

                if (data.Methods != null)
                    foreach (Method met in data.Methods)
                    {
                        if (met.Name.Contains("CONSTRUCTOR"))
                        {
                            dataConstructor += String.Format(Constants.CONTRUCTOR_DECLARATION,
                                met.Visibility,
                                ent.className.Replace("Ent", string.Empty)+"DataAccess",
                                met.Parameters != null && met.Parameters.Count > 0 ? getParameter(met.Parameters) : string.Empty,
                                met.AfterMethod.Equals("NONE") ? String.Empty : met.AfterMethod                                
                                );
                            dataConstructor += String.Format(Constants.CONSTRUCTOR_DESCRIPTION, met.MethodContent.Equals("NONE") ? String.Empty : met.MethodContent.Replace("#CLASSOBJECT#",ent.className.Replace("Ent", string.Empty)));                            
                        }
                        else
                        {
                            dataMethods += String.Format(Constants.DATA_METHOD,
                                met.Visibility,
                                met.SpecialAttribute.Equals("NONE") ? String.Empty : met.SpecialAttribute,
                                met.ReturnType.Equals("NONE") ? String.Empty : met.ReturnType,
                                met.Name,
                                met.Parameters != null && met.Parameters.Count > 0 ? getParameter(met.Parameters) : string.Empty,
                                met.AfterMethod.Equals("NONE") ? String.Empty : met.AfterMethod,
                                met.MethodContent.Equals("") || met.MethodContent.Equals("NONE") ? String.Empty : met.MethodContent.Replace("#CLASSOBJECT#",ent.className.Replace("Ent", string.Empty))
                                );
                        }
                    }
                
                try 
                {
                    using (FileStream fs = File.Create(entBizPath))
                    {
                        var bizFile = Constants.BIZMODEL_STANDARD.Replace("#NamespacePrefix#",biz.NamespacePrefix).Replace("#ClassNamespace#",ent.classNamespace).Replace("#CLASSOBJECT#",ent.className.Replace("Ent", string.Empty));
                        bizFile = bizFile.Replace("#PROPERTIES#", bizProperty);
                        bizFile = bizFile.Replace("#CONSTRUCTORS#", bizConstructor);
                        bizFile = bizFile.Replace("#METHODS#", bizMethods);
                        var writer = new StreamWriter(fs);
                        writer.Write(bizFile);
                        writer.Dispose();
                        fs.Dispose();
                    }
                    using (FileStream fs = File.Create(entDataPath))
                    {
                        var dataFile = Constants.DATAMODEL_STANDARD.Replace("#NamespacePrefix#", data.NamespacePrefix).Replace("#ClassNamespace#",ent.classNamespace).Replace("#CLASSOBJECT#",ent.className.Replace("Ent", string.Empty));
                        dataFile = dataFile.Replace("#PROPERTIES#", dataProperty);
                        dataFile = dataFile.Replace("#CONSTRUCTORS#", dataConstructor);
                        dataFile = dataFile.Replace("#METHODS#", dataMethods);
                        var writer = new StreamWriter(fs);
                        writer.Write(dataFile);
                        writer.Dispose();
                        fs.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static string getParameter(List<Parameter> lstParameters) 
        { 
            var returnString = " ";
            for (var i = 0; i < lstParameters.Count; i++)
                returnString += lstParameters[i].Type + " " + lstParameters[i].Name + (lstParameters.Count > 1 && i < lstParameters.Count - 1 ? ", ": " ");
            return returnString;            
        }
    }
}
