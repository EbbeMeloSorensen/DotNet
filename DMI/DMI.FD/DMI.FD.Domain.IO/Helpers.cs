using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DMI.FD.Domain.IO
{
    public static class Helpers
    {
        public static IList<Parameter> ReadParametersFromJsonFile(
            string fileName)
        {
            List<Parameter> parameterList;

            using (var streamReader = new StreamReader(fileName))
            {
                var json = streamReader.ReadToEnd();
                parameterList = JsonConvert.DeserializeObject<List<Parameter>>(json);
            }

            return parameterList;
        }
    }
}
