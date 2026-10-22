import { Component, input } from '@angular/core';
import { FieldTree, FormField } from '@angular/forms/signals';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { AccountData } from '../../models/account.model';
import { ValidationMessage } from '../../shared/validation/validation-message';


@Component({
  selector: 'app-account-section',
  imports: [FormField, MatFormField, MatLabel, MatInput, ValidationMessage],
  templateUrl: './account-section.html',
})
export class AccountSection {
  readonly form = input.required<FieldTree<AccountData>>();
}
