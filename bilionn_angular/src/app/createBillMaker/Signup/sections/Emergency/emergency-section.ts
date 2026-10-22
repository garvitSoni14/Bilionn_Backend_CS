import { Component, input } from '@angular/core';
import { FieldTree, FormField } from '@angular/forms/signals';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { EmergencyData } from '../../models/emergency.model';
import { ValidationMessage } from '../../shared/validation/validation-message';

@Component({
  selector: 'app-emergency-section',
  imports: [FormField, MatFormField, MatLabel, MatInput, ValidationMessage],
  templateUrl: './emergency-section.html',
})
export class EmergencySection {
  readonly form = input.required<FieldTree<EmergencyData>>();
}