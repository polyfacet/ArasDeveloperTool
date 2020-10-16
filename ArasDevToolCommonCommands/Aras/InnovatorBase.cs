
namespace Hille.Aras.DevTool.Common.Commands.Aras {
    public abstract class InnovatorBase {
        public InnovatorBase(Innovator.Client.IOM.Innovator inn) {
            Inn = inn;
        }

        protected Innovator.Client.IOM.Innovator Inn { get;  }
    }
}
