import { Component, input, ViewEncapsulation } from '@angular/core';
import { FieldTree, FormField } from '@angular/forms/signals';

@Component({
  selector: 'app-email-section',
  standalone: true,
  encapsulation: ViewEncapsulation.None,
  imports: [FormField],
  template: `
    <label for="email">Email address</label>
    <div class="input-wrap" [class.invalid]="field()().touched() && field()().invalid()">
      <span class="field-icon" aria-hidden="true">@</span>
      <input
        id="email"
        type="email"
        [formField]="field()"
        placeholder="you@example.com"
        autocomplete="username"
      />
    </div>
    @if (field()().touched() && field()().invalid()) {
      @for (error of field()().errors(); track error.kind) {
        <small class="field-error">{{ error.message }}</small>
      }
    }
  `,
})
export class EmailSection {
  readonly field = input.required<FieldTree<string>>();
}
