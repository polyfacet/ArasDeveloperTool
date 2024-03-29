﻿using Innovator.Client.IOM;

namespace Hille.Aras.DevTool.Common.Commands.Aras {
    class ArasUtils {

        public static int GetMajorVersion(Innovator.Client.IOM.Innovator inn) {
            string amlQuery = @"<AML>
                <Item action='get' type='Variable' select='value'>
                    <name>VersionMajor</name>
                </Item></AML>";
            Item majorVersionVarItem = inn.applyAML(amlQuery);
            if (majorVersionVarItem.isError()) return 0;

            if (int.TryParse(majorVersionVarItem.getProperty("value", "0"), out int version))
                return version;   
            return 0;
        }
    }
}
