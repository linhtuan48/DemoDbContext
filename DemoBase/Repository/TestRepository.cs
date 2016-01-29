using DemoBase.Data.Entity;
using DemoBase.DataLayer;
using DemoBase.Table;

namespace DemoBase.Repository
{
    public class TestRepository : EntityRepository<CornerstoneLists>, ITestRepository
    {
        public TestRepository(IDaoFactory context) : base(context.GetContext())
        {

        }
    }
}
