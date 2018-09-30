using System.Collections.Generic;

namespace ClassGenerator
{
    public class Method
    {
        public string Name          {get;set;}
        public string Visibility    {get;set;}
        public string ReturnType    {get;set;}
        public string SpecialAttribute {get;set;}
        public List<Parameter> Parameters {get;set;}
        public string AfterMethod {get;set;}
        public string MethodContent {get;set;}
    }
}