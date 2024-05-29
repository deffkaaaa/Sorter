using Foundation;
using System.CodeDom.Compiler;

namespace Sorter
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSTextFieldCell arrayOutput { get; set; }

		[Outlet]
		AppKit.NSTextField arrayRange { get; set; }

		[Outlet]
		AppKit.NSComboBox arraySize { get; set; }

		[Outlet]
		AppKit.NSImageView sortGraph { get; set; }

		[Outlet]
		AppKit.NSPopUpButton sortMethod { get; set; }

		[Outlet]
		AppKit.NSPopUpButton sortOrder { get; set; }

		[Outlet]
		AppKit.NSTextField timeOutput { get; set; }

		[Action ("generateArray:")]
		partial void GenerateArray (Foundation.NSObject sender);

		[Action ("saveArray:")]
		partial void SaveArray (Foundation.NSObject sender);

		[Action ("sortArray:")]
		partial void SortArray (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (arrayOutput != null) {
				arrayOutput.Dispose ();
				arrayOutput = null;
			}

			if (arrayRange != null) {
				arrayRange.Dispose ();
				arrayRange = null;
			}

			if (arraySize != null) {
				arraySize.Dispose ();
				arraySize = null;
			}

			if (sortGraph != null) {
				sortGraph.Dispose ();
				sortGraph = null;
			}

			if (sortMethod != null) {
				sortMethod.Dispose ();
				sortMethod = null;
			}

			if (sortOrder != null) {
				sortOrder.Dispose ();
				sortOrder = null;
			}

			if (timeOutput != null) {
				timeOutput.Dispose ();
				timeOutput = null;
			}
		}
	}
}
