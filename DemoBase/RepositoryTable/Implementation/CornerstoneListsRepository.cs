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
        private readonly IDaoFactory _daoFactory;

        public CornerstoneListsRepository(IDaoFactory daoFactory) : base(daoFactory.GetContext<ContactDbContext>())
        {
            _daoFactory = daoFactory;
        }

        public List<CornerstoneLists> GetListCornerstoneLists()
        {
            //var context = _daoFactory.GetContext<>()
            var demoOtherContext = _daoFactory.GetContext<DemoDbContext>();
            
            return Table.ToList();
        }
    }
}
