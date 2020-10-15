using Aras.IOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Aras {
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
