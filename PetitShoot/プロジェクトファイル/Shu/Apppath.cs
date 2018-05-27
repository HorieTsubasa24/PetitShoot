using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shu
{
    public static class AppPath
    {
        public static string path; 

        public static void SetAppPath()
        {
            string p = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string[] ps = p.Split('\\');
            path = "";
            for(int i = 0; i < ps.Length - 1; i++)
            {
                path += ps[i] + "/";
            }
            path = path.Remove(path.Length - 1, 1);
            Console.WriteLine(path);
        }
    }
}