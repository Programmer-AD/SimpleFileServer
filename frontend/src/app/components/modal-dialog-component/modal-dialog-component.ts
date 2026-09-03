import { Component, model, output } from '@angular/core';

@Component({
    selector: 'app-modal-dialog-component',
    imports: [],
    templateUrl: './modal-dialog-component.html',
    styleUrl: './modal-dialog-component.scss',
})
export class ModalDialogComponent {
    public closing = output<void>();

    protected onModalBlockerClick(event: Event): void {
        event.stopPropagation();
        this.closing.emit();
    }

    protected onModelContentClick(event: Event): void {
        // Just stop propagation, so it is not closed on content interaction
        event.stopPropagation();
    }
}
