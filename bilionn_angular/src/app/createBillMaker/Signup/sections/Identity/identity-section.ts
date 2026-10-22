import { Component, input } from '@angular/core';
import { FieldTree, FormField } from '@angular/forms/signals';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { IdentityData } from '../../models/identity.model';
import { ValidationMessage } from '../../shared/validation/validation-message';

@Component({
  selector: 'app-identity-section',
  imports: [FormField, MatFormField, MatLabel, MatInput, ValidationMessage],
  templateUrl: './identity-section.html',
})
export class IdentitySection {
  readonly form = input.required<FieldTree<IdentityData>>();
  
}