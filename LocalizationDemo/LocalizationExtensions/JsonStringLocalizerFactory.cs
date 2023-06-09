using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationExtensions {
    internal class JsonStringLocalizerFactory : IStringLocalizerFactory {
        private readonly ConcurrentDictionary<string, JsonStringLocalizer> _localizerCache =
            new ConcurrentDictionary<string, JsonStringLocalizer>();

        private readonly ILoggerFactory _loggerFactory;
        private readonly JsonLocalizationOptions _localizationOptions;
        protected ResourceManagerStringLocalizerFactory InnerFactory { get; }
        public JsonStringLocalizerFactory(IOptions<JsonLocalizationOptions> localizationOptions, ILoggerFactory loggerFactory,
            ResourceManagerStringLocalizerFactory innerFactory) {
            _localizationOptions = localizationOptions.Value;
            _loggerFactory = loggerFactory;
            InnerFactory = innerFactory;
        }

        public IStringLocalizer Create(Type resourceSource) {
            if (resourceSource == null) {
                throw new ArgumentNullException(nameof(resourceSource));
            }
            if(resourceSource.Name == "HomeController" || resourceSource.Name == "ValuesController" || resourceSource.Name == "SharedResource") {
                return InnerFactory.Create(resourceSource);
            }
            var resourceName = TrimPrefix(resourceSource.FullName, (_localizationOptions.RootNamespace ?? "LocalizationDemo") + ".");
            return CreateJsonStringLocalizer(resourceName);
        }

        public IStringLocalizer Create(string baseName, string location) {
            if (baseName == null) {
                throw new ArgumentNullException(nameof(baseName));
            }

            if (location == null) {
                throw new ArgumentNullException(nameof(location));
            }

            var resourceName = TrimPrefix(baseName, location + ".");
            return CreateJsonStringLocalizer(resourceName);
        }

        private JsonStringLocalizer CreateJsonStringLocalizer(string resourceName) {
            if (_localizerCache.TryGetValue(resourceName, out var cacheItem)) {
                return cacheItem;
            }
            var logger = _loggerFactory.CreateLogger<JsonStringLocalizer>();
            return _localizerCache.GetOrAdd(resourceName, resName => new JsonStringLocalizer(
                _localizationOptions,
                resName,
                logger));
        }

        private static string TrimPrefix(string name, string prefix) {
            if (name.StartsWith(prefix, StringComparison.Ordinal)) {
                return name.Substring(prefix.Length);
            }

            return name;
        }
    }
}
