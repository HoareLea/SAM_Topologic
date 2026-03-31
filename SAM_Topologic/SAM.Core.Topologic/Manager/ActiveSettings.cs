// SPDX-License-Identifier: LGPL-3.0-or-later
// Copyright (c) 2020–2026 Michal Dengusiak & Jakub Ziolkowski and contributors

using System.Reflection;

namespace SAM.Core.Revit
{
    public static partial class ActiveSetting
    {
        public static class Name
        {
            public const string ParameterName_Topology = "ParameterName_Topology";
        }

        private static Setting setting = null;

        private static readonly object settingLock = new object();

        private static Setting Load()
        {
            Setting setting = ActiveManager.GetSetting(Assembly.GetExecutingAssembly());
            if (setting == null)
            {
                setting = GetDefault();
            }

            return setting;
        }

        public static Setting Setting
        {
            get
            {
                lock (settingLock)
                {
                    if (setting == null)
                    {
                        setting = Load();
                    }
                }

                return setting;
            }
        }

        public static Setting GetDefault()
        {
            Setting setting = new Setting(Assembly.GetExecutingAssembly());

            setting.Add(Name.ParameterName_Topology, "Topology");

            return setting;
        }
    }
}