using System.Collections.Generic;

namespace ClassGenerator
{
    public class BizModel
    {
        public string Path {get;set;}
        public string NamespacePrefix {get;set;}
        public List<Property> Properties {get;set;}
        public List<Method> Methods {get;set;}
    }
}