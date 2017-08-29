using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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

        [Fact(Skip = "")]
        public void IgnoreD()
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
        
        public void DoSomethingBad()
        {
            Assert.Throws<ArgumentNullException>(() => { Foo(); });
        }

        public void DoSeveralThingsBad()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Foo();
                Bar();
                Baz();
            });                
        }
    }
}
