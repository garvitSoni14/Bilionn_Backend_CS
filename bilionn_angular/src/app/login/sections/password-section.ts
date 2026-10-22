import { Component, input, ViewEncapsulation } from '@angular/core';
import { FieldTree, FormField } from '@angular/forms/signals';

@Component({
  selector: 'app-password-section',
  standalone: true,
  encapsulation: ViewEncapsulation.None,
  imports: [FormField],
  template: `
    <label for="password">Password</label>
    <div class="input-wrap" [class.invalid]="field()().touched() && field()().invalid()">
      <span class="field-icon" aria-hidden="true">⌑</span>
      <input
        id="password"
        type="password"
        [formField]="field()"
        placeholder="Enter your password"
        autocomplete="current-password"
      />
    </div>
    @if (field()().touched() && field()().invalid()) {
      @for (error of field()().errors(); track error.kind) {
        <small class="field-error">{{ error.message }}</small>
      }
    }
  `,
})
export class PasswordSection {
  readonly field = input.required<FieldTree<string>>();
}
