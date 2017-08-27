using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsTestToXunitConverter.xUnit.Resources
{
    public class FactTest
    {
        [Fact]
        public void TestMethodA()
        {

        }

        [Fact(DisplayName = "Description")]
        public void DescriptionA()
        {

        }

        [Fact(DisplayName = "Description")]
        public void DescriptionB()
        {

        }

        [Fact(DisplayName = "Description")]
        public void DescriptionC()
        {

        }

        [Fact(Skip = "Reason")]
        public void IgnoreA()
        {

        }

        [Fact(Skip = "Reason")]
        public void IgnoreB()
        {

        }

        [Fact(Skip = "Reason")]
        public void IgnoreC()
        {

        }

        [Fact(DisplayName = "Description", Skip = "Reason")]
        public void TestMethodB()
        {

        }

        [Fact(DisplayName = "Description", Skip = "Reason")]
        public void TestMethodC()
        {

        }

        [ayyyyy]
        public void TestMethodX()
        {

        }
    }
}
