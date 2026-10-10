using SimpleFileServer.Web.Models;

namespace SimpleFileServer.IntegrationTests.Clients;

public class FileClient(HttpClient httpClient)
{
    public async Task<IdModel> RegisterAsync(string fileName, long fileSize)
    {
        HttpResponseMessage response = await httpClient.PostAsJsonAsync($"/api/files", new FileRegisterRequest(fileName, fileSize));
        response.EnsureSuccessStatusCode();
        IdModel? result = await response.Content.ReadFromJsonAsync<IdModel>();
        return result!;
    }

    public async Task InitContentAsync(Guid id, Stream fileContent)
    {
        using var streamContent = new StreamContent(fileContent);
        HttpResponseMessage response = await httpClient.PostAsync($"/api/files/{id}/content", streamContent);
        response.EnsureSuccessStatusCode();
    }

    public async Task<FileListModel> GetAllAsync()
    {
        FileListModel? response = await httpClient.GetFromJsonAsync<FileListModel>("/api/files");
        return response!;
    }

    public async Task<FileInfoModel?> GetAsync(Guid id)
    {
        FileInfoModel? response = await httpClient.GetFromJsonAsync<FileInfoModel>($"/api/files/{id}");
        return response;
    }

    public async Task<Stream> GetContentAsync(Guid id)
    {
        Stream result = await httpClient.GetStreamAsync($"/api/files/{id}/content");
        return result;
    }

    public async Task RenameAsync(Guid id, string newName)
    {
        HttpResponseMessage response = await httpClient.PatchAsJsonAsync($"/api/files/{id}/rename", new FileRenameRequest(newName));
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(Guid id)
    {
        HttpResponseMessage response = await httpClient.DeleteAsync($"/api/files/{id}");
        response.EnsureSuccessStatusCode();
    }
}
