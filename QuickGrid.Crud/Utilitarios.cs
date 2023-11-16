using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quick_crud
{
    public static class Utilitarios
    {
        public static int GetVersionNET()
        {
            #if NET7_0
                return 7;
            #elif NET8_0
                return 8;
            #endif
        }
    }
}
