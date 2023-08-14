using System.Reactive.Disposables;
using Avalonia.Media;
using RazerSdkReader.Structures;
using ReactiveUI;

namespace RazerSdkReader.Avalonia.ViewModels;

public class HeadsetGridWindowViewModel : GridViewerWindowViewModel<ChromaHeadset>
{
    public HeadsetGridWindowViewModel() : base(5, 1, "Headset")
    {
        this.WhenActivated(d =>
        {
            App.Reader.HeadsetUpdated += ReaderOnHeadsetUpdated;
            Disposable.Create(() => App.Reader.HeadsetUpdated -= ReaderOnHeadsetUpdated).DisposeWith(d);
        });
    }

    private void ReaderOnHeadsetUpdated(object? sender, ChromaHeadset e)
    {
        Update(e);
    }

    protected override Color GetColor(in ChromaHeadset data, int index)
    {
        var clr =  data.GetColor(index);
        return Color.FromRgb(clr.R, clr.G, clr.B);
    }
}