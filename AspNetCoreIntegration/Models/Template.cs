using System.Collections;
using System.Dynamic;
using System.Reflection;
using System.Text;

namespace AspNetCoreIntegration.Models; 

 public abstract class RazorEngineTemplateBase 
    {
        private readonly StringBuilder stringBuilder = new StringBuilder();

        private string attributeSuffix = null;

        public dynamic Model { get; set; }

        public void WriteLiteral(string literal = null)
        {
            WriteLiteralAsync(literal).GetAwaiter().GetResult();
        }

        public virtual Task WriteLiteralAsync(string literal = null)
        {
            this.stringBuilder.Append(literal);
            return Task.CompletedTask;
        }

        public void Write(object obj = null)
        {
            WriteAsync(obj).GetAwaiter().GetResult();
        }

        protected virtual Task  WriteAsync(object obj = null)
        {
            this.stringBuilder.Append(obj);
            return Task.CompletedTask;
        }

        public void BeginWriteAttribute(string name, string prefix, int prefixOffset, string suffix, int suffixOffset,
            int attributeValuesCount)
        {
            BeginWriteAttributeAsync(name, prefix, prefixOffset, suffix, suffixOffset, attributeValuesCount).GetAwaiter().GetResult();
        }

        public virtual Task BeginWriteAttributeAsync(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount)
        {
            this.attributeSuffix = suffix;
            this.stringBuilder.Append(prefix);
            return Task.CompletedTask;
        }

        public void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength,
            bool isLiteral)
        {
            WriteAttributeValueAsync(prefix, prefixOffset, value, valueOffset, valueLength, isLiteral).GetAwaiter().GetResult();
        }

        public virtual Task WriteAttributeValueAsync(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral)
        {
            this.stringBuilder.Append(prefix);
            this.stringBuilder.Append(value);
            return Task.CompletedTask;
        }

        public void EndWriteAttribute()
        {
            EndWriteAttributeAsync().GetAwaiter().GetResult();
        }

        public virtual Task EndWriteAttributeAsync()
        {
            this.stringBuilder.Append(this.attributeSuffix);
            this.attributeSuffix = null;
            return Task.CompletedTask;
        }

        public void Execute()
        {
            ExecuteAsync().GetAwaiter().GetResult();
        }

        public virtual Task ExecuteAsync()
        {
            return Task.CompletedTask;
        }

        public virtual string Result()
        {
            return ResultAsync().GetAwaiter().GetResult();
        }

        public virtual Task<string> ResultAsync()
        {
            return Task.FromResult<string>(this.stringBuilder.ToString());
        }
    }
    
public class AnonymousTypeWrapper : DynamicObject
{
    private readonly object model;

    public AnonymousTypeWrapper(object model)
    {
        this.model = model;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
        PropertyInfo propertyInfo = this.model.GetType().GetProperty(binder.Name);

        if (propertyInfo == null)
        {
            result = null;
            return false;
        }

        result = propertyInfo.GetValue(this.model, null);

        if (result == null)
        {
            return true;
        }

        var type = result.GetType();

        if (result.IsAnonymous())
        {
            result = new AnonymousTypeWrapper(result);
        }

        if (result is IDictionary dictionary)
        {
            List<object> keys = new List<object>();

            foreach(object key in dictionary.Keys)
            {
                keys.Add(key);
            }

            foreach(object key in keys)
            {
                if (dictionary[key].IsAnonymous())
                {
                    dictionary[key] = new AnonymousTypeWrapper(dictionary[key]);
                }
            }
        }
        else if (result is IEnumerable enumer && !(result is string))
        {
            result = enumer.Cast<object>()
                .Select(e =>
                {
                    if (e.IsAnonymous())
                    {
                        return new AnonymousTypeWrapper(e);
                    }

                    return e;
                })
                .ToList();
        }


        return true;
    }
}