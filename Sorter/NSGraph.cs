using AppKit;
using CoreGraphics;

namespace Sorter
{
    internal class NSGraph : NSView
    {
        public NSGraph(CGRect frameRect) : base(frameRect)
        {
        }

        public NSGraphData Data { get; internal set; }
    }
}