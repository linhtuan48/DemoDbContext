using System;
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
            //var demoOtherContext = Bind<DemoDbContext>().CornerstoneLists.ToList();
            var contactDBcontext = Bind<ContactDbContext>();
            contactDBcontext.CornerstoneLists.Add(new CornerstoneLists
            {
                Id = Guid.NewGuid(),
                Name = "ContactContext",
                Url = string.Empty
            });
            Commit();
            var demoOtherContext = Bind<DemoDbContext>().CornerstoneLists.Add(new CornerstoneLists
            {
                Id = Guid.NewGuid(),
                Name = "DBcontext",
                Url = string.Empty
            });
            Commit();
            return null;
            // return Table.ToList();
        }
    }
}
