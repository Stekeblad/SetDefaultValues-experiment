namespace SetDefaultValuesTest.Models.Pages
{
	[ContentType(GroupName = "Content", GUID = "3bfa3084-2c48-4718-bccf-526d4f0f20da")]
	public class InitializerPage : PageData
	{
		public virtual string? Heading { get; set; } = "Initialized heading";

		public virtual string? Preamble { get; set; }
	}
}
