using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEDAlfaiaApp.Core
{
    public static class Helper
    {
        public static int? GetIntFromString(string pString)
        {
            int result = 0;
            if (Int32.TryParse(pString, out result))
				return result;
			return null;
        }
        public static bool? GetBoolFromString(string pString)
        {
            bool result = false;
            if (bool.TryParse(pString, out result))
                return result;
            return null;
        }
    }
}
