namespace SetDefaultValuesTest.Models.Pages
{
	[ContentType(GroupName = "Content", GUID = "028a7cf0-5d38-472b-aa23-073276551d3b")]
	public class SetDefaultValuesPage : PageData
	{
		public virtual string? Heading { get; set; }

		public virtual string? SubHeading { get; set; }

		public override void SetDefaultValues(ContentType contentType)
		{
			base.SetDefaultValues(contentType);
			Heading = "Default value heading";
		}
	}
}
