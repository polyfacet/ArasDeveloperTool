using System;
using System.Collections.Generic;
using System.Linq;

namespace Hille.Aras.DevTool.Interfaces.Command;
public class CommandUtils {

    /// <summary>
    /// Example: Input "-c" should return the subsequent to the flag -c, else empty string
    /// </summary>
    /// <param name="flag"></param>
    /// <returns>The subsequent parameter to the flag</returns>

    /// <summary>
    /// Example: Input "-c" should return the subsequent to the option -c, else empty string
    /// </summary>
    /// <param name="args"></param>
    /// <param name="findOption"></param>
    /// <param name="value"></param>
    /// <returns>True if option and value is found. Else false</returns>
    public static bool OptionExistWithValue(List<string> args, string findOption, out string value) {
        string option = args.SingleOrDefault(s => s.ToLower().Equals(findOption.ToLower()));
        if (!String.IsNullOrEmpty(option)) {
            int i = args.IndexOf(option) + 1;
            if (args.Count > i) {
                value = args[i];
                return true;
            }
        }
        value = String.Empty;
        return false;
    }

    public static bool HasOption(List<string> args, string option) {
        if (String.IsNullOrEmpty(args.SingleOrDefault(s => s.ToLower().Equals(option.ToLower())))) {
            return false;
        }
        return true;
    }

        public static bool HasOptionStartingWith(List<string> args, string optionStart, out string fullParameter) {
        fullParameter = args.SingleOrDefault(s => s.ToLower().StartsWith(optionStart.ToLower()));
        if (String.IsNullOrEmpty(fullParameter)) {
            return false;
        }
        return true;
    }

        public static bool HasOptionStartingWith(List<string> args, string optionStart) {
        if (String.IsNullOrEmpty(args.SingleOrDefault(s => s.ToLower().StartsWith(optionStart.ToLower())))) {
            return false;
        }
        return true;
    }

}
