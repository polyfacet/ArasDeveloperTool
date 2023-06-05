using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client.IOM;
using Innovator.Client.Model;

namespace Hille.Aras.DevTool.Common.Commands.Aras.Action {
    internal class RestoreMethod {

        private readonly Innovator.Client.IOM.Innovator Inn;
        private readonly string _methodName;
        private readonly string _dateInputString = String.Empty;
        private Item CurrentMethod;

        public RestoreMethod(Innovator.Client.IOM.Innovator inn, string methodName) { 
            Inn = inn;
            _methodName = methodName;
        }

        public RestoreMethod(Innovator.Client.IOM.Innovator inn, string methodName, string dateString) : this(inn,methodName)
        {
            _dateInputString = dateString;
        }

        public Item Apply() {
            List<Item> methodGenerations = GetAllMethodGenerationsOrderedByGenDesc();

            if (methodGenerations.Count == 0) return Inn.newError($"No method found with name {_methodName}");
            if (methodGenerations.Count == 1) return Inn.newError($"Only one generation of method with name {_methodName} found.");

            Item restoreToMethod = GetMethodToRestoreTo(methodGenerations);
            if (restoreToMethod.isError()) return restoreToMethod;

            Item updatedMethod = UpdateMethod(restoreToMethod);
            
            if (updatedMethod.isError()) return updatedMethod;
            updatedMethod = Inn.newResult(updatedMethod.getProperty("method_code"));
            return updatedMethod;
        }

        private Item GetMethodToRestoreTo(List<Item> methodGenerations) {
            if (String.IsNullOrEmpty(_dateInputString)) {
                return methodGenerations[1];
            }
            Item methodItem = GetMethodFromDate(methodGenerations, _dateInputString);
            return methodItem;
      
        }

        private List<Item> GetAllMethodGenerationsOrderedByGenDesc() {
            var methodGenerations = new List<Item>();
            // We get 'current' first, to avoid issues with older generation with other (changed) name
            // , and other methods that can have had the same name
            CurrentMethod = Inn.newItem("Method", "get");
            CurrentMethod.setAttribute("select", "config_id");
            CurrentMethod.setProperty("name", _methodName);
            CurrentMethod = CurrentMethod.apply();
            if (CurrentMethod.isError()) return methodGenerations;
            if (CurrentMethod.isCollection()) throw new ApplicationException($"Method name not unique: {_methodName}");

            string configId = CurrentMethod.getProperty("config_id");
            Item methodItems = Inn.newItem("Method", "get");
            methodItems.setAttribute("select", "modified_on, generation, comments, method_code");
            methodItems.setAttribute("orderBy", "generation DESC");

            methodItems.setProperty("config_id", configId);
            methodItems.setProperty("generation", "0");
            methodItems.setPropertyAttribute("generation", "condition", "gt");

            methodItems = methodItems.apply();
            for (int i = 0; i < methodItems.getItemCount(); i++) {
                Item methodGen = methodItems.getItemByIndex(i);
                methodGenerations.Add(methodGen);
            }

            return methodGenerations;
        }

        private Item GetMethodFromDate(List<Item> methodGenerations, string dateInputString) {
            DateTime dateInput = DateTime.Parse(dateInputString, null);
            string errorMessageBase = $"Could not restore method {_methodName} to date: {dateInputString}";
            Item methodGen = Inn.newError($"{errorMessageBase}, because it has not been modified since specfied date.");
            foreach (var method in methodGenerations)
            {
                DateTime methodModificationDate = DateTime.Parse(method.getProperty("modified_on"), null);
                methodGen = method;
                if (methodModificationDate < dateInput) {
                    return methodGen;
                }
                
            }
            methodGen = Inn.newError($"{errorMessageBase}, because specified date is earlier than method creation date.");
            return methodGen;
        }

        private Item UpdateMethod(Item item) {
            string comments = item.getProperty("comments");
            string code = item.getProperty("method_code");

            Item updatedItem = Inn.newItem("Method", "edit");
            updatedItem.setID(CurrentMethod.getID());
            updatedItem.setProperty("comments", comments);
            updatedItem.setProperty("method_code", code);
            updatedItem = updatedItem.apply();

            return updatedItem;
        }
    }
}
