using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace TankLike.Data.Utils
{
    public static class CSVWriter
    {
        public static void WriteCSV(string[] data, string filePath)
        {
            filePath = Application.dataPath + filePath;

            using (TextWriter tw = new StreamWriter(filePath, false))
            {
                foreach (string line in data)
                {
                    tw.WriteLine(line);
                }
            }
        }
    }
}
