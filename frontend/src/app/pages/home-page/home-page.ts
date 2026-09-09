import { Component, computed, ElementRef, inject, linkedSignal, resource, signal, viewChild } from "@angular/core";
import { DatePipe } from "@angular/common";
import { FileService } from "../../services/file-service";
import { ModalDialogComponent } from "../../components/modal-dialog-component/modal-dialog-component";
import { ShortTextInputComponent } from "../../components";

@Component({
    imports: [DatePipe, ModalDialogComponent, ShortTextInputComponent],
    selector: "app-home-page",
    styleUrl: "./home-page.scss",
    templateUrl: "./home-page.html",
})
export class HomePage {
    private fileService = inject(FileService);

    protected uploadFileInputRef = viewChild.required<ElementRef<HTMLInputElement>>('uploadFileInput');

    protected fileListResource = resource({
        params: () => null,
        loader: () => this.fileService.getAllAsync(),
    });
    protected fileList = computed(() => {
        if (!this.fileListResource.hasValue()) {
            return [];
        }

        const files = this.fileListResource.value().files.map(file => (<FileRow>{
            ...file,
            size: (file.size / 1024).toFixed(2) + " KB",
            createdAt: new Date(file.createdAt + "Z")
        }));
        return files;
    });

    protected renamedFile = signal<FileRow | undefined>(undefined);
    protected renamedFileNewName = linkedSignal(() => this.renamedFile()?.name ?? "");
    protected canSaveRename = computed(() => {
        const newNameMaxLength = 250;
        const validNameRegex = /^(?=\s*\S)[^\\"<>|:*?\\\/\t\n\v\f\r]+$/gm;

        const renamedFileOldName = this.renamedFile()?.name;
        const renamedFileNewName = this.renamedFileNewName().trim();

        return renamedFileNewName !== ""
            && renamedFileNewName !== renamedFileOldName
            && renamedFileNewName.length < newNameMaxLength
            && renamedFileNewName.match(validNameRegex) !== null;
    });

    protected async onUploadFileClick() {
        const selectedFiles = this.uploadFileInputRef().nativeElement.files;
        if (selectedFiles === null) {
            return;
        }

        const selectedFile = selectedFiles.item(0);
        if (selectedFile === null) {
            return;
        }

        await this.fileService.uploadAsync(selectedFile);

        this.fileListResource.reload();
    }

    protected async onDownloadFileClick(fileRow: FileRow) {
        const blob = await this.fileService.getContentAsync(fileRow.id);
        const objectUrl = URL.createObjectURL(blob);

        const downloadLink = document.createElement("a");
        downloadLink.href = objectUrl;
        downloadLink.download = fileRow.name;
        downloadLink.target = "_blank";
        downloadLink.click();

        URL.revokeObjectURL(objectUrl);
    }

    protected async onRenameFileClick(fileRow: FileRow) {
        this.renamedFile.set(fileRow);
    }

    protected async onDeleteFileClick(fileRow: FileRow) {
        if (!confirm(`Are you sure you want to delete file '${fileRow.name}'?`)) {
            return;
        }

        await this.fileService.deleteAsync(fileRow.id);

        this.fileListResource.reload();
    }

    protected onRenameClosingClick() {
        this.renamedFile.set(undefined);
    }

    protected async onRenameSaveClick() {
        const renamedFile = this.renamedFile();
        if (renamedFile === undefined || !this.canSaveRename()) {
            return;
        }

        await this.fileService.renameAsync(renamedFile.id, {
            newName: this.renamedFileNewName()
        });

        this.renamedFile.set(undefined);
        this.fileListResource.reload();
    }
}

interface FileRow {
    id: string,
    name: string,
    size: string,
    createdAt: Date
}
