using System;
using System.Linq;
using Autofac.Extras.FakeItEasy;
using DemoBase.RepositoryTable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTesting.Infrastructure;

namespace UnitTesting
{
    [TestClass]
    public class DemoDbcontext
    {
        [TestMethod]
        public void TestContext()
        {
            using (var fake = new AutoFake(false, false, false, null, AutofacInstaller.Register()))
            {
                //var listValueModel = GetListDataInCsv();
                var sawEditorPullService = fake.Resolve<ICornerstoneListsRepository>();
                var result = sawEditorPullService.GetListCornerstoneLists();
                Console.WriteLine("List WorkerId: {0}", string.Join(",", result.Select(x => x.Id)));
            }
        }
    }
}
