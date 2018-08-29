using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CloudStorageActivity.Utils
{
    public class StringUtils
    {
        public static String[] SplitExpressionToArray(String expression)
        {
            return Regex.Matches(expression, @"[\""].+?[\""]|[^ ]+")
                  .Cast<Match>()
                  .Select(m => m.Value)
                  .ToList().ToArray();
        }
    }
}
