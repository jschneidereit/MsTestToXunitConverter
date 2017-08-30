using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MsTestToXunitConverter.xUnit
{
    public class MstestAssertToXunitTests
    {
        [Fact]
        public void ConvertsAssertAreEqual()
        {
            var resource = ResourceHelper.GetTestInvocation("TestAreEqual");

            var actual = resource.Item1.RewriteAreEqual().ToString();
            var expected = resource.Item2.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConvertsAssertAreNotEqual()
        {
            var resource = ResourceHelper.GetTestInvocation("TestAreNotEqual");

            var actual = resource.Item1.RewriteAreNotEqual().ToString();
            var expected = resource.Item2.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConvertAssertAreSame()
        {
            var resource = ResourceHelper.GetTestInvocation("TestContains");

            var actual = resource.Item1.RewriteAreSame().ToString();
            var expected = resource.Item2.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConvertsAssertContains()
        {
            var resource = ResourceHelper.GetTestInvocation("TestContains");

            var actual = resource.Item1.RewriteContains().ToString();
            var expected = resource.Item2.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConvertsAssertDoesNotContain() //wat
        {
            //var resource = ResourceHelper.GetTestInvocation("TestDoesNotContain");

            //var actual = resource.Item1.RewriteAreEqual().ToString();
            //var expected = resource.Item2.ToString();

            //Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConvertsAssertInconclusive()
        {
            var resource = ResourceHelper.GetTestInvocation("TestInconculsive");

            var actual = resource.Item1.RewriteInconclusive().ToString();
            var expected = resource.Item2.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConvertsAssertIsFalse()
        {
            var resource = ResourceHelper.GetTestInvocation("TestIsFalse");

            var actual = resource.Item1.RewriteIsFalse().ToString();
            var expected = resource.Item2.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConvertsAssertIsInstanceOfType()
        {
            var resource = ResourceHelper.GetTestInvocation("TestIsInstanceOfType");

            var actual = resource.Item1.RewriteIsInstanceOfType().ToString();
            var expected = resource.Item2.ToString();

            Assert.Equal(expected, actual);
        }

        ////
        [Fact]
        public void CovnertAssertIsNotInstanceOfType()
        {
            var resource = ResourceHelper.GetTestInvocation("TestIsNotInstanceOfType");

            var actual = resource.Item1.RewriteIsNotInstanceOfType().ToString();
            var expected = resource.Item2.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConvertsAssertIsNotNull()
        {
            var resource = ResourceHelper.GetTestInvocation("TestIsNotNull");

            var actual = resource.Item1.RewriteIsNotNull().ToString();
            var expected = resource.Item2.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConvertsAssertIsNull()
        {
            var resource = ResourceHelper.GetTestInvocation("TestIsNull");

            var actual = resource.Item1.RewriteIsNull().ToString();
            var expected = resource.Item2.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConvertsAssertIsTrue()
        {
            var resource = ResourceHelper.GetTestInvocation("TestIsTrue");

            var actual = resource.Item1.RewriteIsTrue().ToString();
            var expected = resource.Item2.ToString();

            Assert.Equal(expected, actual);
        }
    }
}
