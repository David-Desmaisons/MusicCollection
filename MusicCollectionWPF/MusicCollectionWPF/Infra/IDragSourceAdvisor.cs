using System.Windows;


//Original code
//From Pavan Podila & Kevin Hoffman 
//WPF Control Development


namespace MusicCollectionWPF.Infra
{
	public interface IDragSourceAdvisor
	{
		UIElement SourceUI { get; set; }

		DragDropEffects SupportedEffects { get; }

		DataObject GetDataObject(UIElement draggedElt);
        void FinishDrag(DataObject draggedElt, DragDropEffects finalEffects, bool DropOk);
		bool IsDraggable(UIElement dragElt);
		UIElement GetTopContainer();
	}
}