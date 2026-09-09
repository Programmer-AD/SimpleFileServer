using SimpleFileServer.Web.Models;

namespace SimpleFileServer.IntegrationTests.Clients;

public class FileClient(HttpClient httpClient)
{
    public async Task<IdModel> UploadAsync(Stream fileContent, string fileName)
    {
        using var formContent = new MultipartFormDataContent();
        formContent.Add(new StreamContent(fileContent), "file", fileName);

        HttpResponseMessage response = await httpClient.PostAsync("/api/files", formContent);
        response.EnsureSuccessStatusCode();

        IdModel? result = await response.Content.ReadFromJsonAsync<IdModel>();
        return result!;
    }

    public async Task<FileListModel> GetAllAsync()
    {
        FileListModel? response = await httpClient.GetFromJsonAsync<FileListModel>("/api/files");
        return response!;
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
