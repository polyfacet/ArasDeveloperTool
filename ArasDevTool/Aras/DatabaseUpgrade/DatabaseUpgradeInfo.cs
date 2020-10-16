﻿using Aras.IOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArasDevTool.Aras.DatabaseUpgrade {
    internal class DatabaseUpgradeInfo : InnovatorBase {

        public DatabaseUpgradeInfo(Innovator inn):base(inn) {}
    
        public List<DatabaseUpgrade> GetDatabaseUpgrades() {
            var databaseUpgrades = new List<DatabaseUpgrade>();

            string aml = @"<AML>
                <Item action='get' type='DatabaseUpgrade' 
                    select='applied_on,name,target_release,is_latest,description' orderBy='applied_on DESC'>
                </Item></AML>";
            string latestUpdatedDate = String.Empty;
            Item dbUpgradeItems = Inn.applyAML(aml);
            int i = 0;
            bool allUpdatesFromLatestDay =false;
            while (!allUpdatesFromLatestDay && (i < dbUpgradeItems.getItemCount())) {
                Item dbUpgrade = dbUpgradeItems.getItemByIndex(i);
                string appliedDate = dbUpgrade.getProperty("applied_on", String.Empty);
                if (appliedDate != String.Empty) appliedDate = appliedDate.Substring(0, 10);
                if (latestUpdatedDate == String.Empty && appliedDate != String.Empty) {
                        latestUpdatedDate = appliedDate.Substring(0, 10);
                }
                
                if (appliedDate == latestUpdatedDate) {
                    var databaseUpgrade = new DatabaseUpgrade
                    {
                        AppliedOn = dbUpgrade.getProperty("applied_on"),
                        Name = dbUpgrade.getProperty("name"),
                        TargetRelease = dbUpgrade.getProperty("target_release"),
                        Description = dbUpgrade.getProperty("description"),
                        IsLatest = (dbUpgrade.getProperty("is_latest") == "1") ? true : false
                    };
                    databaseUpgrades.Add(databaseUpgrade);
                }
                i++;
            }
            return databaseUpgrades;
        }
    }
    
    struct DatabaseUpgrade {
        public string AppliedOn { get; set; }
        public string Name { get; set; }
        public string TargetRelease { get; set; }
        public bool IsLatest { get; set; }
        public string Description { get; set; }

    }
}