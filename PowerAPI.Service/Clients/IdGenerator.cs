using DevExpress.DirectX.Common.Direct2D;
using DevExpress.XtraRichEdit.Commands;
using Microsoft.EntityFrameworkCore.Migrations;
using PowerAPI.Data.Models;
using System;
using System.Collections.Generic;
using PowerAPI.Data.IRepository;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerAPI.Service.Clients
{
    public class IdGenerator : IIdGenerator
    {
        public string GenerateId(string companyId, string divisionId, string departmentId, string username)
        {

            var str1 = companyId.Replace("_", "__");
            var str2 = divisionId.Replace("_", "__");
            var str3 = departmentId.Replace("_", "__");
            var str4 = username.Replace("_", "__");

            return string.Join("_", [str1, str2, str3, str4]);

        }
        public string[] SplitId(string id)
        {
            var splitStrings = SplitConcatenatedString(id);

            if (splitStrings.Length != 4)
            {
                throw new ArgumentException("Id Credentials does not contain 4 items.");
            }



            for (int i = 0; i < splitStrings.Length; i++)
            {
                splitStrings[i] = splitStrings[i].Replace("__", "_");
            }
            return splitStrings;
        }

        private string[] SplitConcatenatedString(string input)
        {
            var parts = new List<string>();
            int startIndex = 0;

            while (startIndex < input.Length)
            {
                int endIndex = input.IndexOf('_', startIndex);
                while (endIndex != -1 && endIndex + 1 < input.Length && input[endIndex + 1] == '_')
                {
                    // Skip over escaped underscore
                    endIndex = input.IndexOf('_', endIndex + 2);
                }

                if (endIndex == -1)
                {
                    parts.Add(input.Substring(startIndex));
                    break;
                }
                else
                {
                    parts.Add(input.Substring(startIndex, endIndex - startIndex));
                    startIndex = endIndex + 1;
                }
            }

            return parts.ToArray();
        }
    }
}
