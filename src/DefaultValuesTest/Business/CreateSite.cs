using EPiServer.DataAccess;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using SetDefaultValuesTest.Models.Pages;

namespace SetDefaultValuesTest.Business
{
    /// <summary>
    /// This Initialization Module will create a Site Definition and start page if there are none set up
    /// </summary>
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class CreateSite : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var siteDefinitionRepository = ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>();
            var contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();

            var sites = siteDefinitionRepository.List();
            if (sites.Any())
                return;

            var startPage = contentRepository.GetDefault<StartPage>(ContentReference.RootPage);
            startPage.Heading = "Start Page";
            startPage.Name = "Start Page";
            contentRepository.Save(startPage, SaveAction.Publish, AccessLevel.NoAccess);

            var siteDef = new SiteDefinition
            {
                Id = Guid.NewGuid(),
                StartPage = startPage.ContentLink,
                Name = "Site",
                SiteUrl = new Uri("https://localhost:5000/"),
                Hosts = new List<HostDefinition>()
                {
                    new HostDefinition
                    {
                        Name = "localhost:5000",
                        Type = HostDefinitionType.Primary
                    }
                }
            };

            siteDefinitionRepository.Save(siteDef);
        }

        public void Uninitialize(InitializationEngine context)
        {
            return;
        }
    }
}
