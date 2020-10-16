
namespace ArasDevTool.Aras {
    abstract class InnovatorBase {
        public InnovatorBase(Innovator.Client.IOM.Innovator inn) {
            Inn = inn;
        }

        protected Innovator.Client.IOM.Innovator Inn { get;  }
    }
}
