namespace SimpleFileServer.Web.Models;

public record class FileListModel(
    FileInfoModel[] Files
);
