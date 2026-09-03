import { Component, model } from '@angular/core';

@Component({
    selector: 'app-short-text-input-component',
    imports: [],
    templateUrl: './short-text-input-component.html',
    styleUrl: './short-text-input-component.scss',
})
export class ShortTextInputComponent {
    public value = model<string>();

    protected onInput(event: InputEvent) {
        const inputElement = <HTMLInputElement>event.target;
        const value = inputElement.value;
        this.value.set(value);
    }
}
