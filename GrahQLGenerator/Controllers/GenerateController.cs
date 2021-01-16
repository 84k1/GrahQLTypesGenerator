using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrahQLGenerator.Controllers
{
    [Route("generate")]
    public class GenerateController : Controller
    {
        [HttpPost("GraphType")]
        public string GrpahType(string inputClass)
        {
           
            string className = "";
            string ctorName = "";
            List<string> propertyList = inputClass.Replace("{ get; set; }", "").Replace("\t", "").Replace("public", "").Replace("{", "").Replace("}", "")
                .Trim().Split("\n").ToList();

            for (int i = 0; i < propertyList.Count; i++)
            {
                propertyList[i] = propertyList[i].Trim();

                if (string.IsNullOrWhiteSpace(propertyList[i]))
                    propertyList.RemoveAt(i);
            }

            var outputString = "";
            List<string> graphTypeProperties = new List<string>();
            foreach (var item in propertyList)
            {
                string type = item.Split(" ")[0];
                string name = item.Split(" ")[1];
                if (type != "class")
                {
                    if (type.Contains("?"))
                    {
                        type = type.Replace("?", "");
                        graphTypeProperties.Add($"Field(x => x.{name}, nullable: true, type: typeof({ToGraphType(type)}));");
                    }
                    else
                        graphTypeProperties.Add($"Field(x => x.{name}, nullable: false, type: typeof({ToGraphType(type)}));");
                }
                else
                {
                    className = $"public class {name}Type : ObjectGraphType<{name}>";
                    ctorName = $"{name}Type";
                }
            }

            outputString = className + " { \n\tpublic " + ctorName + "() { \n";

            foreach (var item in graphTypeProperties)
            {
                outputString += "\t\t"+ item + "\n";
            }

            outputString += " } \n}";

            return outputString;
        }

        private string ToGraphType(string type)
        {
            if (type.Contains("List<"))
            {
                return $"ListGraphType<{ToGraphType(type.Replace("List<", "").Replace(">",""))}>";
            }
            switch (type)
            {
                case "string":
                    return "StringGraphType";
                case "int":
                    return "IntGraphType";
                case "bool":
                    return "BooleanGraphType";
                case "float":
                    return "FloatGraphType";
                case "decimal":
                    return "DecimalGraphType";
                case "DateTime":
                    return "DateTimeGraphType";
                case "double":
                    return "FloatGraphType";
                default:
                    return $"{type}Type";
            }
        }

        [HttpPost("InputGraphType")]
        public string InputGrpahType(string inputClass)
        {

            string className = "";
            string ctorName = "";
            List<string> propertyList = inputClass.Replace("{ get; set; }", "").Replace("\t", "").Replace("public", "").Replace("{", "").Replace("}", "")
                .Trim().Split("\n").ToList();

            for (int i = 0; i < propertyList.Count; i++)
            {
                propertyList[i] = propertyList[i].Trim();

                if (string.IsNullOrWhiteSpace(propertyList[i]))
                    propertyList.RemoveAt(i);
            }

            var outputString = "";
            List<string> graphTypeProperties = new List<string>();
            foreach (var item in propertyList)
            {
                var prop = item.Trim();
                string type = prop.Split(" ")[0];
                string name = prop.Split(" ")[1];
                if (type != "class")
                {
                    if (type.Contains("?"))
                    {
                        type = type.Replace("?", "");
                        graphTypeProperties.Add($"Field(x => x.{name}, nullable: true, type: typeof({ToInputGraphType(type)}));");
                    }
                    else
                        graphTypeProperties.Add($"Field(x => x.{name}, nullable: false, type: typeof({ToInputGraphType(type)}));");
                }
                else
                {
                    className = $"public class {name}InputType : InputObjectGraphType<{name}>";
                    ctorName = $"{name}InputType";
                }
            }

            outputString = className + " { \n\tpublic " + ctorName + "() { \n";

            foreach (var item in graphTypeProperties)
            {
                outputString += "\t\t" + item + "\n";
            }

            outputString += " } \n}";

            return outputString;
        }

        private string ToInputGraphType(string type)
        {
            if (type.Contains("List<"))
            {
                return $"ListGraphType<{ToInputGraphType(type.Replace("List<", "").Replace(">", ""))}>";
            }
            switch (type)
            {
                case "string":
                    return "StringGraphType";
                case "int":
                    return "IntGraphType";
                case "bool":
                    return "BooleanGraphType";
                case "float":
                    return "FloatGraphType";
                case "decimal":
                    return "DecimalGraphType";
                case "DateTime":
                    return "DateTimeGraphType";
                case "double":
                    return "FloatGraphType";
                default:
                    return $"{type}InputType";
            }
        }
    }
}
