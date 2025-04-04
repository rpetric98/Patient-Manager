﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClassLibrary.Utilities
{
    public class OIBValidator
    {
        //Uzeo sa git  huba
        public static bool IsValidOIB(string oib)
        {
            if (string.IsNullOrEmpty(oib) || !Regex.IsMatch(oib, "^[0-9]{11}$"))
                return false;

            var oibSpan = oib.AsSpan();
            var a = 10;
            for (var i = 0; i < 10; i++)
            {
                a += int.Parse(oibSpan.Slice(i, 1));
                a %= 10;

                if (a == 0)
                    a = 10;

                a *= 2;
                a %= 11;
            }

            var kontrolni = 11 - a;

            if (kontrolni == 10)
                kontrolni = 0;

            return kontrolni == int.Parse(oibSpan.Slice(10, 1));
        }
    }
}

