using Aras.IOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Aras {
    class ArasUtils {

        public static int GetMajorVersion(Innovator inn) {
            string amlQuery = @"<AML>
                <Item action='get' type='Variable' select='value'>
                    <name>VersionMajor</name>
                </Item></AML>";
            Item majorVersionVarItem = inn.applyAML(amlQuery);
            if (majorVersionVarItem.isError()) return 0;

            int.TryParse(majorVersionVarItem.getProperty("value", "0"), out int version);
            return version;
        }
    }
}
