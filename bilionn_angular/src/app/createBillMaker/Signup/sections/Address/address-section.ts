import { Component, input } from '@angular/core';
import { FieldTree, FormField } from '@angular/forms/signals';
import { MatFormField, MatLabel } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { AddressData } from '../../models/address.model';
import { ValidationMessage } from '../../shared/validation/validation-message';

@Component({
  selector: 'app-address-section',
  imports: [FormField, MatFormField, MatLabel, MatInput, ValidationMessage],
  templateUrl: './address-section.html',
})
export class AddressSection {
  readonly form = input.required<FieldTree<AddressData>>();
}