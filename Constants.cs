using System;

namespace ClassGenerator
{
    public static class Constants
    {
        public const string DATAMODEL_STANDARD = "using System.Linq;\r\nusing System.Collections.Generic;\r\nusing Microsoft.EntityFrameworkCore;\r\nusing Factory.Interfaces;\r\nusing Factory.Models;\r\nusing Factory.Exceptions;\r\nusing System;\r\n\r\nnamespace #NamespacePrefix#.#ClassNamespace#\r\n{\r\n\tpublic class #CLASSOBJECT#DataAccess : BaseDataAccess\r\n\t{\r\n\t\t#region Properties\r\n\t\t#PROPERTIES#\r\n\t\t#endregion\r\n\t\t\r\n\t\t#region Constructors\r\n\t\t#CONSTRUCTORS#\r\n\t\t#endregion\r\n\t\t\r\n\t\t#region Generated Methods\r\n\t\t#METHODS#\r\n\t\t#endregion\r\n\t}\r\n}";
        public const string BIZMODEL_STANDARD = "using System;\r\nusing System.Collections.Generic;\r\nusing Data;\r\nusing Data.Common;\r\nusing Factory;\r\nusing Factory.Exceptions;\r\nusing Factory.Interfaces;\r\nusing Factory.Models;\r\n\r\nnamespace #NamespacePrefix#.#ClassNamespace#\r\n{\r\n\tpublic class #CLASSOBJECT#Biz : BaseBiz\r\n\t{\r\n\t\t#region Properties\r\n\t\t#PROPERTIES#\r\n\t\t#endregion\r\n\t\t#region Constructors\r\n\t\t#CONSTRUCTORS#\r\n\t\t#endregion\r\n\t\t#region Generated Methods\r\n\t\t#METHODS#\r\n\t\t#endregion\r\n\t}\r\n}";

        public const string DATA_PROPERTY_SINGLETON = "\r\n\t\tprivate {0} _{1};";
        public const string DATA_PROPERTY = "\r\n\t\t{0} {1} {2}\r\n\t\t{{\r\n\t{3}\r\n\t{4}\r\n\t\t}}";
        public const string GET_SINGLETON = "\t\tget {{ if (_{0} == null) _{0} = new {1}(); return _{0}; }}";
        public const string GET_NOT_SINGLETON = "\t\tget;";
        public const string SET_NOT_SINGLETON = "\t\tset;";
        public const string SET_SINGLETON = "\t\tset {{ _{0} = value; }}";

        public const string DATA_METHOD = "\r\n\t\t{0} {1} {2} {3} ({4}) {5}\r\n\t\t{6}\r\n";
        public const string CONTRUCTOR_DECLARATION = "\r\n\t\t{0} {1} ({2}) {3}";
        public const string CONSTRUCTOR_DESCRIPTION = "\r\n\t\t{0}";
    }
}