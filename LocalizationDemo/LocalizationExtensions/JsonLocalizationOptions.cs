﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationExtensions {
    public class JsonLocalizationOptions {
        /// <summary>
        /// The relative path under application root where resource files are located.
        /// </summary>
        public string ResourcesPath { get; set; } = "Resources";

        /// <summary>
        /// ResourcesPathType
        /// </summary>
        public ResourcesPathType ResourcesPathType { get; set; }

        /// <summary>
        /// RootNamespace
        /// use entry assembly name by default
        /// </summary>
        public string RootNamespace { get; set; }
    }

    public enum ResourcesPathType {
        TypeBased = 0,
        CultureBased = 1,
    }
}
