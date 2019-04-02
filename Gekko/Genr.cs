using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Gekko.Parser;
namespace Gekko
{
    public class TranslatedCode
    {
        public static GekkoTime globalGekkoTimeIterator = GekkoTime.tNull;
        public static int labelCounter;
        public static void C0(GekkoSmpl smpl, P p)
        {
            //[[commandStart]]0
            p.SetText(@"¤1"); O.InitSmpl(smpl, p);

            O.Assignment o0 = new O.Assignment();


            Action assign_21 = () =>
            {
                O.AdjustT0(smpl, -2);
                IVariable ivTmpvar19 = null; // Functions.slet(smpl, new ScalarDate(smpl.t1), new ScalarDate(smpl.t2), i20);
                O.AdjustT0(smpl, 2);
                O.Lookup(smpl, null, null, "%y", null, ivTmpvar19, new LookupSettings(O.ELookupType.LeftHandSide), EVariableType.Var, o0)
                ;
            };
            Func<bool> check_21 = () =>
            {
                O.AdjustT0(smpl, -2);
                IVariable ivTmpvar19 = null; // Functions.slet(smpl, new ScalarDate(smpl.t1), new ScalarDate(smpl.t2), i20);
                O.AdjustT0(smpl, 2);
                if (ivTmpvar19.Type() != EVariableType.Series) return false;
                O.Dynamic1(smpl);
                O.Lookup(smpl, null, null, "%y", null, ivTmpvar19, new LookupSettings(O.ELookupType.LeftHandSide), EVariableType.Var, o0)
                ;
                return O.Dynamic2(smpl);
            };
            O.RunAssigmentMaybeDynamic(smpl, assign_21, check_21, o0);

            //[[commandEnd]]0
        }


        public static readonly ScalarVal i20 = new ScalarVal(3d);

        public static void CodeLines(P p)
        {
            GekkoSmpl smpl = new GekkoSmpl(); O.InitSmpl(smpl, p);

            C0(smpl, p);



        }
    }
}
