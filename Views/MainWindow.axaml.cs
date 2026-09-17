using Avalonia.Controls;
using SIC.ViewModels;
using System.IO;
using System.Linq;
using Avalonia.Input;
 using Avalonia.Interactivity;

namespace SIC.Views;

public partial class MainWindow : Window
{
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();
    public MainWindow()
    {

        InitializeComponent();

        Closed += (_, _) =>
        {
            if(DataContext is MainViewModel vm)
                vm.Logs.Dispose();
        };
        OutputFileNameTextBox.AddHandler(
            TextInputEvent,
            OnOutputFileNameTextInput,
            RoutingStrategies.Tunnel);

        AddHandler(DragDrop.DragOverEvent, OnDragOver);
        AddHandler(DragDrop.DropEvent, OnDrop);

    }
    private void OnDragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.DataTransfer.Contains(DataFormat.File) ? DragDropEffects.Copy : DragDropEffects.None;
    }
    private void OnDrop(object? sender, DragEventArgs e)
    {
        if (DataContext is not MainViewModel vm) return;
        var files = e.DataTransfer.TryGetFiles()?.ToList();
        if (files is { Count: > 0 })
        {
            vm.SetSourceFromPath(files[0].Path.LocalPath);
        }
    }
    private void OnOutputFileNameTextInput(object? sender, TextInputEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Text))
            return;
 
        if (e.Text.Any(c => InvalidFileNameChars.Contains(c)))
        {
            e.Handled = true;
        }
    }


}