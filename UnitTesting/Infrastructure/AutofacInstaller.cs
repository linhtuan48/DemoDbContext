using Autofac;
using DemoBase.Factory;
using DemoBase.RepositoryTable;
using DemoBase.RepositoryTable.Implementation;

namespace UnitTesting.Infrastructure
{
    public static class AutofacInstaller
    {
        public static ContainerBuilder Register()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<DaoFactory>().As<IDaoFactory>().SingleInstance();
            builder.RegisterType<CornerstoneListsRepository>().As<ICornerstoneListsRepository>().SingleInstance();

            return builder;
        }
    }
}
