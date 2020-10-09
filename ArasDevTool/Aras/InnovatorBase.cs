using Aras.IOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Aras {
    abstract class InnovatorBase {
        public InnovatorBase(Innovator inn) {
            Inn = inn;
        }

        protected Innovator Inn { get;  }
    }
}
