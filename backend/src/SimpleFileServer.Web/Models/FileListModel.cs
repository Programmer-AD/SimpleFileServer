namespace SimpleFileServer.Web.Models;

public record class FileListModel(
    List<FileInfoModel> Files
);
