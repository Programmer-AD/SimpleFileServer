namespace SimpleFileServer.IntegrationTests.Clients;

public class AuthClient(HttpClient httpClient)
{
    public async Task<HttpResponseMessage> GetAuthCookiesAsync()
    {
        HttpResponseMessage result = await httpClient.GetAsync("/api/auth/cookies");
        return result;
    }

    public async Task<HttpResponseMessage> DeleteAuthCookiesAsync()
    {
        HttpResponseMessage result = await httpClient.DeleteAsync("/api/auth/cookies");
        return result;
    }
}
