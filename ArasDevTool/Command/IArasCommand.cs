namespace ArasDevTool.Command {
    interface IArasCommand : ICommand {

        Innovator.Client.IOM.Innovator Innovator { set; }

    }
}
