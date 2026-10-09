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
            size: getFileSizeDisplay(file.size),
            createdAt: new Date(file.createdAt + "Z"),

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

        await this.fileService.uploadAsync({
            name: selectedFile.name,
            size: selectedFile.size,
        }, selectedFile);

        this.fileListResource.reload();
    }

    protected async onDownloadFileClick(fileRow: FileRow) {
        // Validate file is still here and is accessible
        const file = await this.fileService.getAsync(fileRow.id);
        if (file === null) {
            // Probably the list is out of data.
            this.fileListResource.reload();
            return;
        }

        const downloadLink = document.createElement("a");
        downloadLink.href = this.fileService.getContentUrl(fileRow.id);
        downloadLink.download = file.name;
        downloadLink.target = "_blank";
        downloadLink.click();
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
    createdAt: Date,
    hasContent: boolean,
}

function getFileSizeDisplay(size: number): string {
    let degree = Math.min(Math.max(Math.floor(Math.log(size) / Math.log(1024)), 0), 3);
    let sizeNumber = (size / (1024 ** degree)).toPrecision(3);

    return `${sizeNumber} ${getFileSizeLiteral(degree)}`;
}

function getFileSizeLiteral(degree: number) {
    switch (degree) {
        case 0: return "B";
        case 1: return "KB";
        case 2: return "MB";
        case 3: return "GB";
    }

    return "?";
}
