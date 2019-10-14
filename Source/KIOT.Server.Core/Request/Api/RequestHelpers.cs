using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;

namespace KIOT.Server.Core.Request.Api
{
    internal static class RequestHelpers
    {
        public static string BuildQueryString(IDictionary<string, string> values)
        {
            if (values == null) { throw new ArgumentNullException($"Parameter: { nameof(values) } cannot be null!"); }

            if (values.Count == 0) { return string.Empty;}

            var stringBuilder = new StringBuilder();

            foreach (var item in values)
            {
                stringBuilder.Append('&');
                stringBuilder.Append(UrlEncoder.Default.Encode(item.Key));
                stringBuilder.Append('=');
                stringBuilder.Append(UrlEncoder.Default.Encode(item.Value));
            }

            return stringBuilder.ToString();
        }
    }
}
