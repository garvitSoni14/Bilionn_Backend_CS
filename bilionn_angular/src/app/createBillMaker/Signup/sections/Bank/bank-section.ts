import { Component, input } from '@angular/core';
import { FieldTree, FormField } from '@angular/forms/signals';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { BankData } from '../../models/bank.model';
import { ValidationMessage } from '../../shared/validation/validation-message';

@Component({
  selector: 'app-bank-section',
  imports: [FormField, MatFormField, MatLabel, MatInput, ValidationMessage],
  templateUrl: './bank-section.html',
})
export class BankSection {
  readonly form = input.required<FieldTree<BankData>>();
}