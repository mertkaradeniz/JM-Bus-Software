using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Lisans;

namespace LisanslamaAlgoritması
{
    class ClsLiCtrl
    {
        [DllImport("JMT.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static bool LiFinishRead();
 
        [DllImport("JMT.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public extern static bool LiDemoStart();

        string LiProSeriNo, LiDateRecKey, LiDate;

        public bool LiDemoCtrl()
        {
            try
            {
                if (LiFinishRead())
                    return true;
                else
                    return false;
            }
            catch { return true; }
        }

        public void LiDemoStarts()
        {
            LiDemoStart();
        }

        string LiSeriNoSplit(string text)
        {
            string[] split = text.Split('-');
            string splittop = "";

            foreach (String s in split)
            {
                splittop += s;
            }
            return splittop;
        }

        public void LiDEMO()
        {
            new LiDataCtrl().LiDateAdd("100");
        }

        public void LisansFinish()
        {
            new LiDataCtrl().LiDateAdd("0");
        }

        public bool LiKeyCompare(string text,string Parametre)
        {
            string LiKey = text.Split('-')[0] + text.Split('-')[1] + text.Split('-')[2] + text.Split('-')[3] + text.Split('-')[4] + text.Split('-')[5];

            LiProSeriNo = LiKey.Split('J')[0];
            LiDateRecKey = LiKey.Split('M')[1];
            string LiDate = (LiKey.Split('J')[1].Substring(0, 3));

            new LiDataCtrl().InputParametre(Parametre);
            new LiDataCtrl().KEYSPLIT();

            if (LiProSeriNo == LiSeriNoSplit(LiVariables.LiProSeriNo) & LiDateRecKey == LiSeriNoSplit(LiVariables.LiDateRecKey))
            { new LiDataCtrl().LiDateAdd(LiDate); return true; }
            else
                return false;
        }
    }
}
