using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsTestToXunitConverter.xUnit.Resources
{
    class FactTest
    {
        [TestMethod]
        public void TestMethodA()
        {

        }

		[Description("Description")]
		public void DescriptionA()
        {

        }

		[TestMethod][Description("Description")]
		public void DescriptionB()
        {

        }

		[TestMethod, Description("Description")]
		public void DescriptionC()
        {

        }

		[Ignore("Reason")]
		public void IgnoreA()
        {

        }

		[TestMethod][Ignore("Reason")]
		public void IgnoreB()
        {

        }

		[TestMethod, Ignore("Reason")]
		public void IgnoreC()
        {

        }

		[TestMethod][Description("Description")][Ignore("Reason")]
		public void TestMethodB()
        {

        }

        [TestMethod][Description("Description")][Ignore("Reason")]
        public void TestMethodC()
        {

        }
    }
}
