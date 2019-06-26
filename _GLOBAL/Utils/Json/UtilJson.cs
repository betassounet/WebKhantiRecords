using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _GLOBAL.Utils.Json {

    // Au prealable installer le package NewtonSoft dans la solution..:
    // sous la console :
    // PM>    Install-Package Newtonsoft.json
    
    public class UtilJson {

        public static void Test() {
            dynamic stuff = JObject.Parse("{ 'Name': 'Jon Smith', 'Address': { 'City': 'New York', 'State': 'NY' }, 'Age': 42 }");

            string name = stuff.Name;
            string address = stuff.Address.City;
        }


        public static string ToJson(object o) {
            return JsonConvert.SerializeObject(o);
        }
    
    }
}
