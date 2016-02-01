using System.Collections.Generic;
using System.Linq;
using DemoBase.Factory;
using DemoBase.Infrastructure;
using DemoBase.Repository;
using DemoBase.Table;

namespace DemoBase.RepositoryTable.Implementation
{
    public class CornerstoneListsRepository : EntityRepository<CornerstoneLists>, ICornerstoneListsRepository
    {
        private IDaoFactory _daoFactory;

        public CornerstoneListsRepository(IDaoFactory daoFactory)
            : base(daoFactory)
        {
            _daoFactory = daoFactory;
        }

        public List<CornerstoneLists> GetListCornerstoneLists()
        {
            var demoOtherContext = Bind<DemoDbContext>().CornerstoneLists.ToList();
            return demoOtherContext;
            // return Table.ToList();
        }
    }
}
