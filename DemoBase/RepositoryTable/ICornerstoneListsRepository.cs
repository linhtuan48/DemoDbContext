using System.Collections.Generic;
using DemoBase.Repository;
using DemoBase.Table;

namespace DemoBase.RepositoryTable
{
    public interface ICornerstoneListsRepository : IEntityRepository<CornerstoneLists>
    {
        List<CornerstoneLists> GetListCornerstoneLists();
    }
}
