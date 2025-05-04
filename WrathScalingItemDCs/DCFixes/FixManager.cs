using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WrathHamCore.Utility.Initialization;

namespace WrathScalingItemDCs.DCFixes
{
    public static class FixManager
    {
        public static List<IFix> Fixes =
        [
            new FrostEmbraceFix(),
            new HeartOfIraAreaFix()
        ];


        [InitializeOnBlueprints(10)]
        public static void ApplyAll() => Fixes.ForEach(x => x.Apply());
    }
}
