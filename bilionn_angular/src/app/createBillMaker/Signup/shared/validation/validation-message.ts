import { Component, computed, input } from '@angular/core';
import { FieldTree } from '@angular/forms/signals';
import { MatError } from '@angular/material/form-field';

@Component({
  selector: 'app-validation-message',
  imports: [MatError],
  templateUrl: './validation-message.html'
//   styleUrl: './validation-message.css'
})
export class ValidationMessage<TValue> {

  receivedField = input.required<FieldTree<TValue>>();

  fieldState = computed(() => this.receivedField()());

  isTouchedAndInvalid = computed(() =>
    this.fieldState().touched() &&
    this.fieldState().invalid()
  );

  errors = computed(() => this.fieldState().errors());
}