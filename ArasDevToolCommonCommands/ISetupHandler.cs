using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hille.Aras.DevTool.Interfaces.Configuration;

namespace Hille.Aras.DevTool.Common.Commands {
    public interface ISetupHandler {

        IArasSetupConfig Setup(string env);
        IArasConnectionConfig SetupConnection(string env);

        IArasSetupConfig GetConfig(string env);
        //void SaveConfig(IArasSetupConfig config);

        IArasConnectionConfig GetArasConnectionConfig(string env);
        //void SaveArasConnectionConfig(IArasConnectionConfig config);

    }
}
