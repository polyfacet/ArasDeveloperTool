namespace Hille.Aras.DevTool.Interfaces.Command {
    public interface IArasCommand : ICommand {

        Innovator.Client.IOM.Innovator Innovator { set; }

    }
}
