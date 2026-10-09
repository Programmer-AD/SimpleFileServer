import { HttpClient } from "@angular/common/http";
import { inject, Service } from "@angular/core";
import { FileInfoModel, FileListModel, FileRegisterRequest, FileRenameRequest, IdModel } from "../models";
import { catchError, firstValueFrom, of } from "rxjs";

@Service()
export class FileService {
    private baseUrl = "/api/files";

    private httpClient = inject(HttpClient);

    public async uploadAsync(registerRequest: FileRegisterRequest, fileContent: Blob): Promise<IdModel> {

        const registerResponse = await firstValueFrom(this.httpClient.post<IdModel>(this.baseUrl, registerRequest));

        await firstValueFrom(this.httpClient.post(`${this.baseUrl}/${registerResponse.id}/content`, fileContent));

        return registerResponse;
    }

    public async getAllAsync(): Promise<FileListModel> {
        const response = await firstValueFrom(this.httpClient.get<FileListModel>(this.baseUrl));
        return response;
    }

    public async getAsync(id: string): Promise<FileInfoModel | null> {
        const response = await firstValueFrom(this.httpClient.get<FileInfoModel>(`${this.baseUrl}/${id}`)
            .pipe(catchError(error => {
                if (error.status === 404) {
                    return of(null);
                } else {
                    throw error;
                }
            })));

        return response;
    }

    public getContentUrl(id: string): string {
        return `${this.baseUrl}/${id}/content`;
    }

    public async renameAsync(id: string, request: FileRenameRequest): Promise<void> {
        await firstValueFrom(this.httpClient.patch(`${this.baseUrl}/${id}/rename`, request));
    }

    public async deleteAsync(id: string): Promise<void> {
        await firstValueFrom(this.httpClient.delete(`${this.baseUrl}/${id}`,));
    }
}
