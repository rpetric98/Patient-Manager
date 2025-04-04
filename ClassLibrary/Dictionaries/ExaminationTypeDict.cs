using ClassLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Dictionaries
{
    public static class ExaminationTypeDict
    {
        public static readonly Dictionary<ExaminationType, string> Description = new Dictionary<ExaminationType, string>
        {
            { ExaminationType.GP, "Opći pregled" },
            {ExaminationType.KRV, "Krvna slika" },
            {ExaminationType.X_RAY, "Rengen" },
            {ExaminationType.CT, "CT sken" },
            {ExaminationType.MR, "Magnetna rezonanca " },
            {ExaminationType.ULTRA, "Ultrazvuk" },
            {ExaminationType.EKG, "Elektrokardiogram" },
            {ExaminationType.ECHO, "Ehokardiogram" },
            {ExaminationType.EYE, "Pregled oka" },
            {ExaminationType.DERM, "Dermatološki pregled" },
            {ExaminationType.DENTA, "Dentalni pregled" },
            {ExaminationType.MAMMO, "Mamografija" },
            {ExaminationType.NEURO, "Neurološki pregled" },


        };
    }
}
