using System.Windows;

//Original code
//From Pavan Podila & Kevin Hoffman 
//WPF Control Development

namespace MusicCollectionWPF.Infra
{
	public interface IDropTargetAdvisor
	{
		UIElement TargetUI { get; set; }

		bool ApplyMouseOffset { get; }
		bool IsValidDataObject(IDataObject obj);
		bool OnDropCompleted(IDataObject obj, Point dropPoint, object TargetOriginalsource);
		UIElement GetVisualFeedback(IDataObject obj);
		UIElement GetTopContainer();
	}
}