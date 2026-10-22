import { Component, input } from '@angular/core';
import { FieldTree, FormField } from '@angular/forms/signals';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { EmploymentData } from '../../models/employee.model';
import { ValidationMessage } from '../../shared/validation/validation-message';

@Component({
  selector: 'app-employment-section',
  imports: [FormField, MatFormField, MatLabel, MatInput, ValidationMessage],
  templateUrl: './employment-section.html',
})
export class EmploymentSection {
  readonly form = input.required<FieldTree<EmploymentData>>();
}