using System;
using System.Collections;
using System.Globalization;
using NUnit.Framework;

namespace NSane.Tests
{
    public class ScanTestCaseFactory
    {
        public static IEnumerable ScanTestCases
        {
            get
            {
                for (int i = 0; i < 24 /* should be 24*/; i += 8)
                {
                    int depth = i == 0 ? 1 : i;

                    foreach (var pat in new[]
                        {
                            "Color pattern",
                            "Grid",
                            "Solid black",
                            "Solid white"
                        })
                    {
                        foreach (var mode in new[] {"Gray", "Color"})
                        {
                            // 1 bpp color, is not allowed
                            if(depth == 1 && 
                                mode.Equals("color", StringComparison.OrdinalIgnoreCase))
                                continue;
                            
                            if (depth == 16)
                            {
                                foreach (var invert in new[] {true, false})
                                {
                                    string file = string.Format(CultureInfo.InvariantCulture,
                                                                "{0}_{1}_{2}_{3}",
                                                                depth.ToString(CultureInfo.InvariantCulture),
                                                                mode.ToLowerInvariant(),
                                                                pat.Replace(' ', '_').ToLowerInvariant(),
                                                                invert ? "1" : "0");

                                    yield return new TestCaseData(depth,
                                                                  mode,
                                                                  pat,
                                                                  invert,
                                                                  file);
                                }
                            }
                            else
                            {
                                string file = string.Format(CultureInfo.InvariantCulture,
                                                            "{0}_{1}_{2}",
                                                            depth.ToString(CultureInfo.InvariantCulture),
                                                            mode.ToLowerInvariant(),
                                                            pat.Replace(' ', '_').ToLowerInvariant());

                                yield return new TestCaseData(depth,
                                                              mode,
                                                              pat,
                                                              false,
                                                              file);
                            }
                        }
                    }
                }
            }
        }  
    }
}
