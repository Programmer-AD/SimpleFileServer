import { HttpClient } from "@angular/common/http";
import { inject, Service } from "@angular/core";
import { FileListModel, FileRenameRequest, IdModel } from "../models";
import { firstValueFrom } from "rxjs";

@Service()
export class FileService {
    private baseUrl = "/api/files";

    private httpClient = inject(HttpClient);

    public async uploadAsync(fileBlob: Blob): Promise<IdModel> {
        const formData = new FormData();
        formData.append("file", fileBlob);

        const response = await firstValueFrom(this.httpClient.post<IdModel>(this.baseUrl, formData));

        return response;
    }

    public async getAllAsync(): Promise<FileListModel> {
        const response = await firstValueFrom(this.httpClient.get<FileListModel>(this.baseUrl));
        return response;
    }

    public async getContentAsync(id: string): Promise<Blob> {
        const file = await firstValueFrom(this.httpClient.get(`${this.baseUrl}/${id}/content`, {
            responseType: "blob"
        }));

        return file;
    }

    public async renameAsync(id: string, request: FileRenameRequest): Promise<void> {
        await firstValueFrom(this.httpClient.patch(`${this.baseUrl}/${id}/rename`, request));
    }

    public async deleteAsync(id: string): Promise<void> {
        await firstValueFrom(this.httpClient.delete(`${this.baseUrl}/${id}`,));
    }
}
