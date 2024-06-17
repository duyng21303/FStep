namespace FStep.ViewModels
{
	public class Summernote
	{
		public Summernote(string iDEditor, bool loadLibrary = true)
		{
			IDEditor = iDEditor;
			LoadLibrary = loadLibrary;
		}
		public string IDEditor { get; set; }
		public bool LoadLibrary { get; set; }
		public int Height { get; set; } = 120;
		public string ToolBar { get; set; } = @"
		[
			['font', ['bold', 'italic', 'underline', 'clear']],
			['para', ['ul', 'ol', 'paragraph']],
			['height', ['height']],
			['view', ['fullscreen', 'codeview', 'help']]
		]
		";
	}
}
