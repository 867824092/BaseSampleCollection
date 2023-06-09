using AspNetCoreIntegration.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace AspNetCoreIntegration.Binders {
    public class AuthorEntityBinderProvider : IModelBinderProvider {
        public IModelBinder GetBinder(ModelBinderProviderContext context) {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(Author)) {
                return new BinderTypeModelBinder(typeof(AuthorEntityBinder));
            }

            return null;
        }
    }
}
