import { Component, model, signal } from '@angular/core';

@Component({
    selector: 'app-password-input-component',
    imports: [],
    templateUrl: './password-input-component.html',
    styleUrl: './password-input-component.scss',
})
export class PasswordInputComponent {
    public value = model<string>();
    public showValue = signal(false);

    protected onInput(event: InputEvent) {
        const inputElement = <HTMLInputElement>event.target;
        const value = inputElement.value;
        this.value.set(value);
    }

    protected onShowButtonClick() {
        this.showValue.update(value => !value);
    }
}
