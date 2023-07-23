using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.Web.Mvc;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SetDefaultValuesTest.Business;
using SetDefaultValuesTest.Models.Pages;
using System.Data.Common;

namespace SetDefaultValuesTest.Pages
{
	public class StartPageModel : RazorPageModel<StartPage>
	{
		private readonly IContentRepository _contentRepository;
		private readonly UrlResolver _urlResolver;
        private readonly IConfiguration _configuration;

        public string? Init1Link { get; set; }
		public int Init1Id { get; set; }

		public string? Init2Link { get; set; }
        public int Init2Id { get; set; }

        public string? Default1Link { get; set; }
        public int Default1Id { get; set; }

        public string? Default2Link { get; set; }
        public int Default2Id { get; set; }

        public List<Row> DbRows { get; set; } = new();

		public StartPageModel(IContentRepository contentRepository, UrlResolver urlResolver, IConfiguration configuration)
		{
			_contentRepository = contentRepository;
			_urlResolver = urlResolver;
            _configuration = configuration;
        }

		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPost(string create)
		{
			if (!"doCreate".Equals(create))
				return BadRequest();

			string timeFragment = DateTime.UtcNow.ToString("_hh-mm-ss");

            // Create a InitializerPage and let the heading be set by standard .NET property initialization
            var initPage1 = _contentRepository.GetDefault<InitializerPage>(ContentReference.StartPage);
			initPage1.Name = "initPage1" + timeFragment;
			_contentRepository.Save(initPage1, SaveAction.Publish, AccessLevel.NoAccess);
			Init1Link = _urlResolver.GetUrl(initPage1);
			Init1Id = initPage1.ContentLink.ID;

            // Create a second InitializerPage but assign property values on the instance
            var initPage2 = _contentRepository.GetDefault<InitializerPage>(ContentReference.StartPage);
			initPage2.Name = "initPage2" + timeFragment;
			initPage2.Heading = "Manually set heading";
			initPage2.Preamble = "A preamble";
			_contentRepository.Save(initPage2, SaveAction.Publish, AccessLevel.NoAccess);
			Init2Link = _urlResolver.GetUrl(initPage2);
            Init2Id = initPage2.ContentLink.ID;

            // Create a SetDefaultValuesPage and let Optimizely initialize the properties by calling SetDefaultValues
            var defaultValuePage1 = _contentRepository.GetDefault<SetDefaultValuesPage>(ContentReference.StartPage);
			defaultValuePage1.Name = "defaultValuePage1" + timeFragment;
			_contentRepository.Save(defaultValuePage1, SaveAction.Publish, AccessLevel.NoAccess);
			Default1Link = _urlResolver.GetUrl(defaultValuePage1);
            Default1Id = defaultValuePage1.ContentLink.ID;

            // Create a second SetDefaultValuesPage but assign property values on the instance
            var defaultValuePage2 = _contentRepository.GetDefault<SetDefaultValuesPage>(ContentReference.StartPage);
			defaultValuePage2.Name = "defaultValuePage2" + timeFragment;
			defaultValuePage2.Heading = "Manually set heading";
			defaultValuePage2.SubHeading = "A subheading";
			_contentRepository.Save(defaultValuePage2, SaveAction.Publish, AccessLevel.NoAccess);
			Default2Link = _urlResolver.GetUrl(defaultValuePage2);
            Default2Id = defaultValuePage2.ContentLink.ID;

			// Extract the pages that was just created and their property values using SQL to see what was persisted and not.
			// IT'S BAD TO WRITE SQL CODE TO READ FROM OPTIMIZELY TABLES AND DANGEROUS TO WRITE SQL FOR UPDATING/DELETING!
			// Use the provided API:s and think twice before directly accessing the data.
			string conStr = _configuration.GetConnectionString("EPiServerDB");
            using var sql = SqlClientFactory.Instance.CreateConnection();
            sql.ConnectionString = conStr;
			sql.Open();

            using var command = SqlClientFactory.Instance.CreateCommand();
			command.Connection = sql;
            command.CommandText = @$"
				SELECT c.pkID, pd.Name, cp.LongString
				FROM tblContent AS c
				INNER JOIN tblContentProperty AS cp ON c.pkID = cp.fkContentID
				INNER JOIN tblPropertyDefinition AS pd ON pd.pkID = cp.fkPropertyDefinitionID
				WHERE c.pkID IN ({Init1Id}, {Init2Id}, {Default1Id}, {Default2Id})";

			using DbDataReader reader = await command.ExecuteReaderAsync();
			while (reader.Read())
			{
				DbRows.Add(new Row
				{
					ContentId = reader.GetInt32(0),
					PropertyName = reader.GetString(1),
					PropertyValue = reader.GetString(2)
				});
			}

            return Page();
		}
	}
}
