using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using DialogHostAvalonia;

namespace GuildedChatExporter.Gui.Framework;

public class DialogManager
{
    private DialogHost? _host;

    public void RegisterHost(DialogHost host) => _host = host;

    public async Task<bool?> ShowDialogAsync(DialogViewModelBase viewModel)
    {
        if (_host is null)
            throw new InvalidOperationException("Dialog host is not registered.");

        void OnCloseRequested(object? sender, DialogCloseRequestedEventArgs args)
        {
            viewModel.CloseRequested -= OnCloseRequested;
            _host.CurrentSession?.Close(args.DialogResult);
        }

        viewModel.CloseRequested += OnCloseRequested;

        var result = await DialogHost.Show(viewModel, _host.Identifier);
        return result as bool?;
    }

    public async Task<string?> PromptSaveFilePathAsync(
        IReadOnlyList<FilePickerFileType> fileTypes,
        string? defaultFileName = null
    )
    {
        if (_host is null)
            return null;

        var topLevel = TopLevel.GetTopLevel(_host);
        if (topLevel is null)
            return null;

        var options = new FilePickerSaveOptions
        {
            Title = "Save File",
            FileTypeChoices = fileTypes,
            SuggestedFileName = defaultFileName,
        };

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(options);
        return file?.Path.LocalPath;
    }

    public async Task<string?> PromptDirectoryPathAsync()
    {
        if (_host is null)
            return null;

        var topLevel = TopLevel.GetTopLevel(_host);
        if (topLevel is null)
            return null;

        var options = new FolderPickerOpenOptions
        {
            Title = "Select Folder",
            AllowMultiple = false,
        };

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(options);
        return folders.Count > 0 ? folders[0].Path.LocalPath : null;
    }
}
