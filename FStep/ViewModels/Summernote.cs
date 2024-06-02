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
			['style', ['style']],
			['font', ['bold', 'underline', 'clear']],
			['color', ['color']],
			['para', ['ul', 'ol', 'paragraph']],
			['table', ['table']],
			['insert', ['link', 'picture', 'video']],
			['height', ['height']],
			['view', ['fullscreen', 'codeview', 'help']]
		]
		";
	}
}
