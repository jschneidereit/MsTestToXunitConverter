using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MsTestToXunitConverter.xUnit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MsTestToXunitConverter.xUnit
{
    public class UsingsReplaceTest
    {
        [Fact]
        public void SimpleUsingsReplaceTest()
        {
            foreach (var pod in ResourceHelper.GetTestPods())
            {
                var actual_comp = pod.ActualRoot as CompilationUnitSyntax;
                var expected_comp = pod.ExpectedRoot as CompilationUnitSyntax;

                var actual = actual_comp.Usings.Select(u => u.ReplaceUsing(Transformer.MSTEST_USING, Transformer.XUNIT_USING).ToEnvNewlineString()).ToList();
                var expected = expected_comp.Usings.Select(u => u.ToEnvNewlineString()).ToList();

                try
                {
                    Assert.Equal(expected, actual);

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
