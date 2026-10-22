import { Component, input } from '@angular/core';
import { FieldTree, FormField } from '@angular/forms/signals';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { PersonalData } from '../../models/personal.model';
import { ValidationMessage } from '../../shared/validation/validation-message';

@Component({
  selector: 'app-personal-section',
  imports: [FormField, MatFormField, MatLabel, MatInput, ValidationMessage],
  templateUrl: './personal-section.html',
})
export class PersonalSection {
  readonly form = input.required<FieldTree<PersonalData>>();
}