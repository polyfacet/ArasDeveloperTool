using Innovator.Client.IOM;

namespace Hille.Aras.DevTool.Common.Commands.Aras {
    internal static class ItemExtension {

        public static string TypeAndName(this Item item) {
            return $"{item.getType()} name: {GetName(item)}";
        }

        public static string Name(this Item item) {
            return GetName(item);
        }

        private static string GetName(Item item) {
            string name = item.getProperty("name");
            if (name == null) name = item.getPropertyAttribute("id", "keyed_name", "N/A");
            return name;
        }

    }
}
