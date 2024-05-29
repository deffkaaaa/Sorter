using System;
using AppKit;
using CoreGraphics;

namespace Sorter
{
	public class DrawingView : NSView
	{
        private int[] array;

        public DrawingView(CGRect frame) : base(frame)
        {
        }

        public void SetArray(int[] array)
        {
            this.array = array;
            NeedsDisplay = true;
        }

        public override void DrawRect(CGRect dirtyRect)
        {
            base.DrawRect(dirtyRect);

            if (array == null) return;

            using (var context = NSGraphicsContext.CurrentContext.CGContext)
            {
                context.ClearRect(dirtyRect);
                for (int i = 0; i < array.Length; i++)
                {
                    context.SetFillColor(NSColor.Blue.CGColor);
                    context.FillRect(new CGRect(i * 10, 0, 9, array[i]));
                }
            }
        }
    }
}

